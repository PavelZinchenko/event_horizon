////////////////////////////////////////////////////////////////////////////////
//
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets)
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

#import "ISN_NativeCore.h"


#define kInAppPurchaseManagerProductsFetchedNotification @"kInAppPurchaseManagerProductsFetchedNotification"

NSString* const UNITY_SPLITTER = @"|";
NSString* const UNITY_SPLITTER2 = @"|%|";
NSString* const UNITY_EOF = @"endofline";
NSString* const ARRAY_SPLITTER = @"%%%";

const char* UNITY_PAYMENT_LISTENER = "SA.IOSNative.StoreKit.PaymentManager";


static bool MANUAL_TRANSACTIONS_HANDLING = false;
static bool PROMOTED_PURCHASES_ALLOWED = true;
static NSMutableArray* completedTransactions = [[NSMutableArray alloc] init];



@interface SKProduct (LocalizedPrice)
@property (nonatomic, readonly) NSString *localizedPrice;
@end


@interface TransactionServer : NSObject <SKPaymentTransactionObserver>
-(void) verifyLastPurchase:(NSString *) verificationURL;
@end

#if !TARGET_OS_TV

@interface StoreProductView : NSObject<SKStoreProductViewControllerDelegate>
@property (strong)  NSNumber *vid;
@property (strong)  SKStoreProductViewController *storeViewController;

- (void) CreateView:(int) viewId products: (NSArray *) products;
- (void) Show;
@end

#endif


@interface ISN_Security : NSObject <SKRequestDelegate>

+ (id) sharedInstance;

-(void) RetrieveLocalReceipt;
-(void) ReceiptRefreshRequest;


@end



@interface InAppPurchaseManager : NSObject <SKProductsRequestDelegate, SKRequestDelegate> {
    
}

@property (nonatomic, strong) NSMutableArray* productIdentifiers;
@property (nonatomic, strong) NSMutableDictionary* products;
@property (nonatomic, strong) TransactionServer* storeServer;
@property (nonatomic, strong) SKProductsRequest* productRequest;


+ (InAppPurchaseManager *) instance;

- (void) loadStore;
- (void) restorePurchases;
- (void) addProductId:(NSString *) productId;
- (void) buyProduct:(NSString * )productId;
- (void) finishTransaction:(NSString * )productId;

- (void) ShowProductView:(int)viewId;
- (void) CreateProductView:(int) viewId products: (NSArray *) products;


-(void) verifyLastPurchase:(NSString *) verificationURL;

@end



@implementation SKProduct (LocalizedPrice)

- (NSString *)localizedPrice
{
    NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
    [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
    [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
    [numberFormatter setLocale:self.priceLocale];
    NSString *formattedString = [numberFormatter stringFromNumber:self.price];
    
    
#if UNITY_VERSION < 500
    [numberFormatter release];
#endif
    
    
    return formattedString;
}

@end


//--------------------------------------
//  InApp Purchase Manager
//--------------------------------------


@implementation InAppPurchaseManager

static InAppPurchaseManager * _instance;

static NSMutableDictionary* _views;

+ (InAppPurchaseManager *) instance {
    
    if (_instance == nil){
        _instance = [[InAppPurchaseManager alloc] init];
    }
    
    return _instance;
}

-(id) init {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"init"];
    if(self = [super init]){
        _views = [[NSMutableDictionary alloc] init];
        
        
        [self setProductIdentifiers:[[NSMutableArray alloc] init]];
        [self setProducts:[[NSMutableDictionary alloc] init]];
        [self setStoreServer:[[TransactionServer alloc] init]];
        
        
        if([SKPaymentQueue canMakePayments]) {
            [[SKPaymentQueue defaultQueue] addTransactionObserver:[self storeServer]];
        }
        
        
    }
    return self;
}

-(void) dealloc {
    
    if([SKPaymentQueue canMakePayments]) {
        [[SKPaymentQueue defaultQueue] removeTransactionObserver:[self storeServer]];
    }
}


//--------------------------------------
//  Initialisation
//--------------------------------------

- (void)loadStore {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"loadStore...."];
    SKProductsRequest *request= [[SKProductsRequest alloc] initWithProductIdentifiers:[NSSet setWithArray:[self productIdentifiers]]];
    
    [self setProductRequest:request];
    [self productRequest].delegate = self;
    [[self productRequest] start];
    
    request.delegate = self;
    [request start];
    
}



-(void) addProductId:(NSString *)productId {
    [[self productIdentifiers] addObject:productId];
}


- (void)request:(SKRequest *)request didFailWithError:(NSError *)error {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"productsRequest....failed: %@", error.description];
    NSString *code = [NSString stringWithFormat: @"%d", (int)error.code];
    
    NSMutableString * data = [[NSMutableString alloc] init];
    [data appendString: code ];
    [data appendString:@"|"];
    
    NSString *descr = @"no_descr";
    if(error.description != nil) {
        descr = error.description;
    }
    
    [data appendString:descr];
    
    
    UnitySendMessage(UNITY_PAYMENT_LISTENER, "OnStoreKitInitFailed", [ISN_DataConvertor NSStringToChar:data]);
}


- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"productsRequest...."];
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Total loaded products: %i", [response.products count]];
    
    
    NSMutableString * data = [[NSMutableString alloc] init];
    BOOL first = YES;
    
    
    for (SKProduct *product in response.products) {
        
        [[self products] setObject:product forKey:product.productIdentifier];
        
        if(!first) {
            [data appendString:UNITY_SPLITTER];
        }
        
        
        first = NO;
        
        
        [data appendString:product.productIdentifier];
        [data appendString:UNITY_SPLITTER];
        
        if( product.localizedTitle != NULL ) {
            [data appendString:product.localizedTitle];
        } else {
            [data appendString:@"null"];
        }
        [data appendString:UNITY_SPLITTER];
        
        
        
        if( product.localizedDescription != NULL ) {
            [data appendString:product.localizedDescription];
        } else {
            [data appendString:@"null"];
        }
        [data appendString:UNITY_SPLITTER];
        
        
        
        if( product.localizedPrice != NULL ) {
            [data appendString:product.localizedPrice];
        } else {
            [data appendString:@"null"];
        }
        [data appendString:UNITY_SPLITTER];
        
        
        
        [data appendString:[product.price stringValue]];
        [data appendString:UNITY_SPLITTER];
        
        
        
        NSLocale *productLocale = product.priceLocale;
        
        //  symbol and currency code
        NSString *localCurrencySymbol = [productLocale objectForKey:NSLocaleCurrencySymbol];
        NSString *currencyCode = [productLocale objectForKey:NSLocaleCurrencyCode];
        
        
        
        [data appendString:currencyCode];
        [data appendString:UNITY_SPLITTER];
        
        [data appendString:localCurrencySymbol];
        
        
        
        
    }
    
    for (NSString *invalidProductId in response.invalidProductIdentifiers) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Invalid product id: %@" , invalidProductId];
    }
    
    
    UnitySendMessage(UNITY_PAYMENT_LISTENER, "onStoreDataReceived", [ISN_DataConvertor NSStringToChar:data]);
    [[NSNotificationCenter defaultCenter] postNotificationName:kInAppPurchaseManagerProductsFetchedNotification object:self userInfo:nil];
    
}

//--------------------------------------
//  Public Methods
//--------------------------------------


-(void) restorePurchases {
    // [[SKPaymentQueue defaultQueue] addTransactionObserver:_storeServer];
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

-(void) buyProduct:(NSString *)productId {
    
    
    SKProduct* selectedProduct = [[self products] objectForKey: productId];
    if(selectedProduct != NULL) {
        SKPayment *payment = [SKPayment paymentWithProduct:selectedProduct];
        [[SKPaymentQueue defaultQueue] addPayment:payment];
    } else {
        NSMutableString * data = [[NSMutableString alloc] init];
        
        [data appendString:productId];
        [data appendString:UNITY_SPLITTER2];
        [data appendString:@"4"];
        [data appendString:UNITY_SPLITTER];
        [data appendString:@"Product Not Available"];
        
        UnitySendMessage(UNITY_PAYMENT_LISTENER, "onTransactionFailed", [ISN_DataConvertor NSStringToChar:data]);
    }
}

-(void) finishTransaction:(NSString *)productId {
    for (SKPaymentTransaction *transaction in completedTransactions) {
        if([transaction.payment.productIdentifier isEqualToString:productId] ) {
            [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Transaction finished for prodcut: %@", transaction.payment.productIdentifier];
        }
    }
    
    NSMutableArray* newTransactionsList = [[NSMutableArray alloc] init];
    for (SKPaymentTransaction *transaction in completedTransactions) {
        if(![transaction.payment.productIdentifier isEqualToString:productId] ) {
            [newTransactionsList addObject:transaction];
        }
    }
    
    completedTransactions = newTransactionsList;
    
    
}

-(void) verifyLastPurchase:(NSString *) verificationURL {
    [[self storeServer] verifyLastPurchase:verificationURL];
}


//--------------------------------------
//  Prodcut View
//--------------------------------------


- (void) CreateProductView:(int)viewId products:(NSArray *)products {
    
#if !TARGET_OS_TV
    StoreProductView* v = [[StoreProductView alloc] init];
    [v CreateView:viewId products:products];
    
    [_views setObject:v forKey:[NSNumber numberWithInt:viewId]];
#endif
}

-(void) ShowProductView:(int)viewId {
#if !TARGET_OS_TV
    StoreProductView *v = [_views objectForKey:[NSNumber numberWithInt:viewId]];
    if(v != nil) {
        [v Show];
    }
#endif
}

@end


#if !TARGET_OS_TV


@implementation StoreProductView

- (void) CreateView:(int)viewId products:(NSArray *)products {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"CreateView"];
    
    NSNumber *n = [NSNumber numberWithInt:viewId];
    [self setVid:n];
    
    [self setStoreViewController:[[SKStoreProductViewController alloc] init]];
    
    
    NSMutableDictionary *parameters = [[NSMutableDictionary alloc] init];
    
    
    for (NSString* p in products) {
        NSInteger intVal = [p intValue];
        [parameters setObject:[NSNumber numberWithInt: (int) intVal] forKey:SKStoreProductParameterITunesItemIdentifier];
    }
    
    [self storeViewController].delegate = self;
    
    [[self storeViewController] loadProductWithParameters:parameters completionBlock:^(BOOL result, NSError *error) {
        if (result) {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ok"];
            UnitySendMessage(UNITY_PAYMENT_LISTENER, "OnProductViewLoaded", [[[self vid] stringValue] UTF8String]);
        } else {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"no"];
            UnitySendMessage(UNITY_PAYMENT_LISTENER, "OnProductViewLoadedFailed", [[[self vid] stringValue] UTF8String]);
        }
    }];
    
    
    
}

-(void) Show {
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:[self storeViewController]  animated:YES completion:nil];
    
    
}

-(void)productViewControllerDidFinish:(SKStoreProductViewController *)viewController {
    [viewController dismissViewControllerAnimated:YES completion:nil];
    UnitySendMessage(UNITY_PAYMENT_LISTENER, "OnProductViewDismissed", [[[self vid] stringValue] UTF8String]);
}


@end


#endif



//--------------------------------------
//  Transaction Server
//--------------------------------------


@implementation TransactionServer


NSString* lastTransactionReceipt = @"";


- (BOOL)paymentQueue:(SKPaymentQueue *)queue shouldAddStorePayment:(SKPayment *)payment forProduct:(SKProduct *)product {
    
    if(PROMOTED_PURCHASES_ALLOWED) {
        NSMutableString * data = [[NSMutableString alloc] init];
        [data appendString:product.productIdentifier];
        UnitySendMessage(UNITY_PAYMENT_LISTENER, "onProductPurchasedExternally", [ISN_DataConvertor NSStringToChar:data]);
    }
    
    return PROMOTED_PURCHASES_ALLOWED;
}


- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions {
    
    
    for (SKPaymentTransaction *transaction in transactions) {
        
        switch (transaction.transactionState) {
            case SKPaymentTransactionStatePurchased:
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                [self restoreTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed:
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateDeferred:
                [self reportDeferredState:transaction];
                break;
            default:
                break;
        }
    }
}

//--------------------------------------
//  Transaction Porcesing
//--------------------------------------


- (void)completeTransaction:(SKPaymentTransaction *)transaction {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: completeTransaction..."];
    
    if(MANUAL_TRANSACTIONS_HANDLING) {
        [completedTransactions addObject:transaction];
    } else {
        [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    }
    
    [self provideContent:transaction isRestored:false];
}

- (void)restoreTransaction:(SKPaymentTransaction *)transaction {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: restoreTransaction..."];
    
    [self provideContent:transaction isRestored:true];
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}

- (void)failedTransaction:(SKPaymentTransaction *)transaction {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Transaction Failed with code : %li", (long)transaction.error.code];
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Transaction error: %@", transaction.error.description];
    
    
    NSString *serializedError = [ISN_DataConvertor serializeErrorToNSString:transaction.error];
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    [data appendString:transaction.payment.productIdentifier];
    [data appendString:UNITY_SPLITTER2];
    [data appendString:serializedError];
    
    UnitySendMessage(UNITY_PAYMENT_LISTENER, "onTransactionFailed", [ISN_DataConvertor NSStringToChar:data]);
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}

- (void)reportDeferredState:(SKPaymentTransaction *)transaction {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Transaction  Deferred for: %@", transaction.payment.productIdentifier];
    
    UnitySendMessage(UNITY_PAYMENT_LISTENER, "onProductStateDeferred", [ISN_DataConvertor NSStringToChar:transaction.payment.productIdentifier]);
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}

//--------------------------------------
//  Public Methods
//--------------------------------------


-(void) verifyLastPurchase:(NSString *)verificationURL {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: url: %@",verificationURL];
    
    
    NSURL *url = [NSURL URLWithString:verificationURL];
    NSMutableURLRequest *theRequest = [NSMutableURLRequest requestWithURL:url];
    
    
    
    NSString *json = [NSString stringWithFormat:@"{\"receipt-data\":\"%@\"}", lastTransactionReceipt];
    
    [theRequest setHTTPBody:[json dataUsingEncoding:NSUTF8StringEncoding]];
    [theRequest setHTTPMethod:@"POST"];
    [theRequest setValue:@"application/x-www-form-urlencoded" forHTTPHeaderField:@"Content-Type"];
    NSString *length = [NSString stringWithFormat:@"%lu", (unsigned long)[json length]];
    [theRequest setValue:length forHTTPHeaderField:@"Content-Length"];
    NSHTTPURLResponse* urlResponse = nil;
    NSError *error = [[NSError alloc] init];
    NSData *responseData = [NSURLConnection sendSynchronousRequest:theRequest
                                                 returningResponse:&urlResponse
                                                             error:&error];
    NSString *responseString = [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
    
    //  NSLog(@"resp: %@",responseString);
    
    NSError *e = nil;
    
    NSDictionary *dic =
    [NSJSONSerialization JSONObjectWithData: [responseString dataUsingEncoding:NSUTF8StringEncoding]
                                    options: NSJSONReadingMutableContainers
                                      error: &e];
    
    NSString *statusCode = [NSString stringWithFormat:@"%d", [[dic objectForKey:@"status"] intValue]];
    
    
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    [data appendString:statusCode];
    [data appendString:UNITY_SPLITTER];
    [data appendString: responseString];
    [data appendString:UNITY_SPLITTER];
    [data appendString: lastTransactionReceipt];
    
    UnitySendMessage(UNITY_PAYMENT_LISTENER, "onVerificationResult", [ISN_DataConvertor NSStringToChar:data]);
    
}


//--------------------------------------
//  Private  Methods
//--------------------------------------



- (void)provideContent:(SKPaymentTransaction *)transaction  isRestored:(BOOL)isRestored{
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: provideContent for: %@", transaction.payment.productIdentifier];
    
    lastTransactionReceipt =  [transaction.transactionReceipt AsBase64String];
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    [data appendString:transaction.payment.productIdentifier];
    
    [data appendString: UNITY_SPLITTER];
    if(isRestored) {
        [data appendString:@"0"];
    } else {
        [data appendString:@"1"];
    }
    
    
    [data appendString: UNITY_SPLITTER];
    
    if(transaction.payment.applicationUsername ==  nil) {
        [data appendString:@""];
    } else {
        [data appendString:transaction.payment.applicationUsername];
    }
    
    
    
    [data appendString: UNITY_SPLITTER];
    [data appendString: lastTransactionReceipt];
    
    [data appendString: UNITY_SPLITTER];
    [data appendString: transaction.transactionIdentifier];
    
    
    UnitySendMessage(UNITY_PAYMENT_LISTENER, "onProductBought", [ISN_DataConvertor NSStringToChar:data]);
}


//--------------------------------------
//  Payment Queue Delegate
//--------------------------------------


- (void)paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: paymentQueue error: %@",error];
    
    UnitySendMessage(UNITY_PAYMENT_LISTENER, "onRestoreTransactionFailed", [ISN_DataConvertor serializeError:error]);
}

- (void) paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: received restored transactions: %lu", (unsigned long)queue.transactions.count];
    
    if (queue.transactions.count == 0) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: No purchases to restore, fail event sent"];
        
        NSMutableString * data = [[NSMutableString alloc] init];
        
        [data appendString: @"6"];
        [data appendString:UNITY_SPLITTER];
        [data appendString:@"No purchases to restore"];
        
        UnitySendMessage(UNITY_PAYMENT_LISTENER, "onRestoreTransactionFailed", [ISN_DataConvertor NSStringToChar:data]);
        return;
    }
    
    for (SKPaymentTransaction *transaction in queue.transactions) {
        NSString *productID = transaction.payment.productIdentifier;
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: restored: %@",productID];
    }
    
    UnitySendMessage(UNITY_PAYMENT_LISTENER, "onRestoreTransactionComplete", "");
}


@end


//--------------------------------------
//  Payment Security
//--------------------------------------


@implementation ISN_Security

static ISN_Security * security_sharedInstance;


+ (id)sharedInstance {
    
    if (security_sharedInstance == nil)  {
        security_sharedInstance = [[self alloc] init];
    }
    
    return security_sharedInstance;
}


- (void) RetrieveLocalReceipt {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"RetrieveLocalRecipe"];
    
    NSString *encodedString = @"";
    NSBundle *mainBundle = [NSBundle mainBundle];
    NSURL *receiptURL = [mainBundle appStoreReceiptURL];
    NSError *receiptError;
    BOOL isPresent = [receiptURL checkResourceIsReachableAndReturnError:&receiptError];
    if (isPresent) {
        NSData *receiptData = [NSData dataWithContentsOfURL:receiptURL];
        encodedString = [receiptData base64Encoding];
    }
    
    UnitySendMessage("ISN_Security", "Event_ReceiptLoaded", [ISN_DataConvertor NSStringToChar:encodedString]);
    
}

-(void) ReceiptRefreshRequest {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"SKReceiptRefreshRequest"];
    SKReceiptRefreshRequest *request = [[SKReceiptRefreshRequest alloc] init];
    [request setDelegate:self];
    [request start];
}


//--------------------------------------
//  SKRequest Delegate
//--------------------------------------


- (void)request:(SKRequest *)request didFailWithError:(NSError *)error {
    UnitySendMessage("ISN_Security", "Event_ReceiptRefreshRequestReceived", [ISN_DataConvertor NSStringToChar:@"0"]);
}


- (void)requestDidFinish:(SKRequest *)request {
    UnitySendMessage("ISN_Security", "Event_ReceiptRefreshRequestReceived", [ISN_DataConvertor NSStringToChar:@"1"]);
}


@end




extern "C" {
    
    
    //--------------------------------------
    //  SECURITY
    //--------------------------------------
    
    void _ISN_RetrieveLocalReceipt ()  {
        [[ISN_Security sharedInstance] RetrieveLocalReceipt];
    }
    
    void _ISN_ReceiptRefreshRequest ()  {
        [[ISN_Security sharedInstance] ReceiptRefreshRequest];
    }
    
    void _ISN_VerifyLastPurchase(char* url) {
        [[InAppPurchaseManager instance] verifyLastPurchase:[ISN_DataConvertor charToNSString:url]];
    }
    
    
    //--------------------------------------
    //  MARKET
    //--------------------------------------
    
    void _ISN_LoadStore(char * productsId) {
        
        NSString* str = [ISN_DataConvertor charToNSString:productsId];
        NSArray *items = [str componentsSeparatedByString:@","];
        
        for(NSString* s in items) {
            [[InAppPurchaseManager instance] addProductId:s];
        }
        
        [[InAppPurchaseManager instance] loadStore];
    }
    
    void _ISN_EnableManulaTransactionsMode() {
        MANUAL_TRANSACTIONS_HANDLING = true;
    }
    
    void _ISN_DisablePromotedPurchases () {
        PROMOTED_PURCHASES_ALLOWED = false;
    }
    
    void _ISN_FinishTransaction(char* productIdentifier) {
        NSString *productId = [ISN_DataConvertor charToNSString:productIdentifier];
        [[InAppPurchaseManager instance] finishTransaction:productId];
    }
    
    void _ISN_BuyProduct(char * productId) {
        [[InAppPurchaseManager instance] buyProduct:[ISN_DataConvertor charToNSString:productId]];
    }
    
    void _ISN_RestorePurchases() {
        [[InAppPurchaseManager instance] restorePurchases];
    }
    
    
    bool _ISN_InAppSettingState() {
        return [SKPaymentQueue canMakePayments];
    }
    
    
    //--------------------------------------
    //  PRODUCT VIEW
    //--------------------------------------
    
    void _ISN_CreateProductView(int viewId, char * productsId ) {
        NSString* str = [ISN_DataConvertor charToNSString:productsId];
        NSArray *items = [str componentsSeparatedByString:@","];
        
        [[InAppPurchaseManager instance] CreateProductView: viewId products:items];
    }
    
    void _ISN_ShowProductView(int viewId) {
        [[InAppPurchaseManager instance] ShowProductView:viewId];
    }
    
    
    //--------------------------------------
    //  SKStoreReviewController
    //--------------------------------------
    
    bool _ISN_StoreReviewControllerAvaliable() {
        if([SKStoreReviewController class]){
            return true;
        } else {
            return false;
        }
    }
    
    void _ISN_StoreRrequestReview() {
        
        if(_ISN_StoreReviewControllerAvaliable()) {
            [SKStoreReviewController requestReview] ;
        }
    }
    
    
    
    //--------------------------------------
    //  SKCloudServiceController
    //--------------------------------------
    
    int ISN_SKCloudService_AuthorizationStatus() {
        /*
         
         SKCloudServiceAuthorizationStatus status = [SKCloudServiceController authorizationStatus];
         
         switch (status) {
         case SKCloudServiceAuthorizationStatusNotDetermined:
         return 0;
         break;
         
         case SKCloudServiceAuthorizationStatusDenied:
         return 1;
         break;
         
         case SKCloudServiceAuthorizationStatusRestricted:
         return 2;
         break;
         
         case SKCloudServiceAuthorizationStatusAuthorized:
         return 3;
         break;
         
         default:
         return 0;
         break;
         }
         */
        return 0;
    }
    
    void ISN_SKCloudService_RequestAuthorization() {
        /*
         [SKCloudServiceController requestAuthorization:^(SKCloudServiceAuthorizationStatus status) {
         switch (status) {
         
         case SKCloudServiceAuthorizationStatusDenied:
         UnitySendMessage("SK_CloudService", "Event_AuthorizationFinished", [ISN_DataConvertor NSStringToChar:@"1"]);;
         break;
         
         case SKCloudServiceAuthorizationStatusRestricted:
         UnitySendMessage("SK_CloudService", "Event_AuthorizationFinished", [ISN_DataConvertor NSStringToChar:@"2"]);
         break;
         
         case SKCloudServiceAuthorizationStatusAuthorized:
         UnitySendMessage("SK_CloudService", "Event_AuthorizationFinished", [ISN_DataConvertor NSStringToChar:@"3"]);
         break;
         
         default:
         UnitySendMessage("SK_CloudService", "Event_AuthorizationFinished", [ISN_DataConvertor NSStringToChar:@"0"]);
         break;
         }
         
         }];
         */
    }
    
    void ISN_SKCloudService_RequestCapabilities() {
        /*
         SKCloudServiceController * controller = [[SKCloudServiceController alloc] init];
         [controller requestCapabilitiesWithCompletionHandler:^(SKCloudServiceCapability capabilities, NSError * _Nullable error) {
         
         if(error == nil) {
         
         NSMutableString * data = [[NSMutableString alloc] init];
         [data appendString:[NSString stringWithFormat: @"%lu", (unsigned long)capabilities]];
         UnitySendMessage("SK_CloudService", "Event_RequestCapabilitieSsuccess",  [ISN_DataConvertor NSStringToChar:data] );
         
         } else {
         UnitySendMessage("SK_CloudService", "Event_RequestCapabilitiesFailed",  [ISN_DataConvertor serializeError:error] );
         }
         
         }];
         */
    }
    
    
    void ISN_SKCloudService_RequestStorefrontIdentifier() {
        /*
         SKCloudServiceController * controller = [[SKCloudServiceController alloc] init];
         [controller requestStorefrontIdentifierWithCompletionHandler:^(NSString * _Nullable storefrontIdentifier, NSError * _Nullable error) {
         
         if(error == nil) {
         
         NSString* Identifier = @"";
         if(storefrontIdentifier != nil) {
         Identifier = storefrontIdentifier;
         }
         UnitySendMessage("SK_CloudService", "Event_RequestStorefrontIdentifierSsuccess",  [ISN_DataConvertor NSStringToChar:Identifier] );
         } else {
         UnitySendMessage("SK_CloudService", "Event_RequestStorefrontIdentifierFailed",  [ISN_DataConvertor serializeError:error] );
         }
         
         
         }];
         */
        
    }
    
    
}