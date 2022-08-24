////////////////////////////////////////////////////////////////////////////////
//
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets)
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#import <Foundation/Foundation.h>

#import "ISN_NativeCore.h"
#import "AppDelegateListener.h"
#if UNITY_VERSION < 450
#include "iPhone_View.h"§§
#endif



NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_EOF = @"endofline";
NSString * const ARRAY_SPLITTER = @"%%%";


@implementation NSData (Base64)

+ (NSData *)InitFromBase64String:(NSString *)aString {
    return [[NSData alloc] initWithBase64Encoding:aString];
}

- (NSString *)AsBase64String {
    return [self base64EncodedStringWithOptions:0];
}

@end


@implementation NSDictionary (JSON)

- (NSString *)AsJSONString {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:self options:0  error:&error];
    
    if (!jsonData) {
        return @"{}";
    } else {
        return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
}

@end


@implementation ISN_DataConvertor


+(NSString *) charToNSString:(char *)value {
    if (value != NULL) {
        return [NSString stringWithUTF8String: value];
    } else {
        return [NSString stringWithUTF8String: ""];
    }
}

+(const char *)NSIntToChar:(NSInteger)value {
    NSString *tmp = [NSString stringWithFormat:@"%ld", (long)value];
    return [tmp UTF8String];
}

+ (const char *) NSStringToChar:(NSString *)value {
    return [value UTF8String];
}

+ (NSArray *)charToNSArray:(char *)value {
    NSString* strValue = [ISN_DataConvertor charToNSString:value];
    
    NSArray *array;
    if([strValue length] == 0) {
        array = [[NSArray alloc] init];
    } else {
        array = [strValue componentsSeparatedByString:ARRAY_SPLITTER];
    }
    
    return array;
}

+ (const char *) NSStringsArrayToChar:(NSArray *) array {
    return [ISN_DataConvertor NSStringToChar:[ISN_DataConvertor serializeNSStringsArray:array]];
}

+ (NSString *) serializeNSStringsArray:(NSArray *) array {
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    
    for(NSString* str in array) {
        [data appendString:str];
        [data appendString: ARRAY_SPLITTER];
    }
    
    [data appendString: UNITY_EOF];
    
    NSString *str = [data copy];
#if UNITY_VERSION < 500
    [str autorelease];
#endif
    
    return str;
}


+ (NSMutableString *)serializeErrorToNSString:(NSError *)error {
    NSString* description = @"";
    if(error.description != nil) {
        description = error.description;
    }
    
    return  [self serializeErrorWithDataToNSString:description code: (int) error.code];
}

+ (NSMutableString *)serializeErrorWithDataToNSString:(NSString *)description code:(int)code {
    NSMutableString * data = [[NSMutableString alloc] init];
    
    [data appendFormat:@"%i", code];
    [data appendString: UNITY_SPLITTER];
    [data appendString: description];
    
    return  data;
}



+ (const char *) serializeErrorWithData:(NSString *)description code: (int) code {
    NSString *str = [ISN_DataConvertor serializeErrorWithDataToNSString:description code:code];
    return [ISN_DataConvertor NSStringToChar:str];
}

+ (const char *) serializeError:(NSError *)error  {
    NSString *str = [ISN_DataConvertor serializeErrorToNSString:error];
    return [ISN_DataConvertor NSStringToChar:str];
}

@end




@implementation ISN_NativeUtility

static bool logState = true;
static ISN_NativeUtility * na_sharedInstance;
static NSString* templateReviewURLIOS7  = @"itms-apps://itunes.apple.com/app/idAPP_ID";
NSString *templateReviewURL = @"itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=APP_ID";

+ (id)sharedInstance {
    
    if (na_sharedInstance == nil)  {
        na_sharedInstance = [[self alloc] init];
    }
    
    return na_sharedInstance;
}

+ (BOOL) IsIPad {
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
        return true;
    } else {
        return false;
    }
}

+ (int) majorIOSVersion {
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    return [[vComp objectAtIndex:0] intValue];
}

-(void) redirectToRatingPage:(NSString *)appId {
#if TARGET_IPHONE_SIMULATOR
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"NOTE: iTunes App Store is not supported on the iOS simulator. Unable to open App Store page."];
#else
    
    
    NSString *reviewURL;
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    
    
    if ([[vComp objectAtIndex:0] intValue] >= 7) {
        reviewURL = [templateReviewURLIOS7 stringByReplacingOccurrencesOfString:@"APP_ID" withString:[NSString stringWithFormat:@"%@", appId]];
    }  else {
        reviewURL = [templateReviewURL stringByReplacingOccurrencesOfString:@"APP_ID" withString:[NSString stringWithFormat:@"%@", appId]];
    }
    
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"redirecting to iTunes page, iOS version: %i", [[vComp objectAtIndex:0] intValue]];
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"redirect URL: %@", reviewURL];
    
    
    
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:reviewURL]];
#endif
}

-(void) ISN_SetLogState:(BOOL)state {
    logState = state;
}

-(void) ISN_NativeLog:(NSString *)msg, ... {
    if(logState) {
        va_list argumentList;
        va_start(argumentList, msg);
        
        NSString *message = [[NSString alloc] initWithFormat:msg arguments:argumentList];
        
        // clean up
        va_end(argumentList);
        
        NSLog(@"ISN_NativeLog: %@", message);
    }
}

-(void) setApplicationBagesNumber:(int) count {
#if !TARGET_OS_TV
    [UIApplication sharedApplication].applicationIconBadgeNumber = count;
#endif
}

- (void) ShowSpinner {
    
#if !TARGET_OS_TV
    [[UIApplication sharedApplication] beginIgnoringInteractionEvents];
    
    if([self spinner] != nil) {
        return;
    }
    
    UIViewController *vc =  UnityGetGLViewController();
    
    
    [self setSpinner:[[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhiteLarge]];
    
    
    
    [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
    
    
    
    
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"iOS 8 detected"];
        [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.width, vc.view.frame.size.height)];
    } else {
        
        if([[UIDevice currentDevice] orientation] == UIDeviceOrientationPortrait || [[UIDevice currentDevice] orientation] == UIDeviceOrientationPortraitUpsideDown) {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"portrait detected"];
            [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.width, vc.view.frame.size.height)];
            
        } else {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"landscape detected"];
            [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.height, vc.view.frame.size.width)];
        }
        
    }
    
    
    [self spinner].opaque = NO;
    [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.0f];
    
    
    [UIView animateWithDuration:0.8 animations:^{
        [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.5f];
    }];
    
    
    
    
    [vc.view addSubview:[self spinner]];
    [[self spinner] startAnimating];
    
    //  [[self spinner] retain];
    
#endif
    
}



- (void) HideSpinner {
    
    [[UIApplication sharedApplication] endIgnoringInteractionEvents];
    
    if([self spinner] != nil) {
        //  [[UIApplication sharedApplication] endIgnoringInteractionEvents];
        
        [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.5f];
        [UIView animateWithDuration:0.8 animations:^{
            [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.0f];
            
        } completion:^(BOOL finished) {
            [[self spinner] removeFromSuperview];
#if UNITY_VERSION < 500
            [[self spinner] release];
#endif
            
            [self setSpinner:nil];
        }];
        
        
    }
    
}

- (CGFloat) GetW {
    
    UIViewController *vc =  UnityGetGLViewController();
    
    bool IsLandscape = true;
    
#if !TARGET_OS_TV
    
    UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
    if(orientation == UIInterfaceOrientationLandscapeLeft || orientation == UIInterfaceOrientationLandscapeRight) {
        IsLandscape = true;
    } else {
        IsLandscape = false;
    }
#endif
    
    CGFloat w;
    if(IsLandscape) {
        w = vc.view.frame.size.height;
    } else {
        w = vc.view.frame.size.width;
    }
    
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        w = vc.view.frame.size.width;
    }
    
    
    return w;
}

#if !TARGET_OS_TV

- (void)DP_changeDate:(UIDatePicker *)sender {
    
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
#if UNITY_VERSION < 500
    [dateFormatter autorelease];
#endif
    
    [dateFormatter setDateFormat: @"yyyy-MM-dd HH:mm:ss"];
    NSString *dateString = [dateFormatter stringFromDate:sender.date];
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"DateChangedEvent: %@", dateString];
    
    UnitySendMessage("SA.IOSNative.UIKit.NativeReceiver", "DateTimePickerDateChanged", [ISN_DataConvertor NSStringToChar:dateString]);
}

-(void) DP_PickerClosed:(UIDatePicker *)sender {
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
#if UNITY_VERSION < 500
    [dateFormatter autorelease];
#endif
    [dateFormatter setDateFormat: @"yyyy-MM-dd HH:mm:ss"];
    NSString *dateString = [dateFormatter stringFromDate:sender.date];
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"DateChangedEvent: %@", dateString];
    
    UnitySendMessage("SA.IOSNative.UIKit.NativeReceiver", "DateTimePickerClosed", [ISN_DataConvertor NSStringToChar:dateString]);
    
}


UIDatePicker *datePicker;

- (void) DP_show:(int)mode date: (NSDate*) date {
    UIViewController *vc =  UnityGetGLViewController();
    
    CGRect toolbarTargetFrame = CGRectMake(0, vc.view.bounds.size.height-216-44, [self GetW], 44);
    CGRect datePickerTargetFrame = CGRectMake(0, vc.view.bounds.size.height-216, [self GetW], 216);
    CGRect darkViewTargetFrame = CGRectMake(0, vc.view.bounds.size.height-216-44, [self GetW], 260);
    
    UIView *darkView = [[UIView alloc] initWithFrame:CGRectMake(0, vc.view.bounds.size.height, [self GetW], 260)];
    darkView.alpha = 1;
    darkView.backgroundColor = [UIColor whiteColor];
    darkView.tag = 9;
    
    UITapGestureRecognizer *tapGesture = [[UITapGestureRecognizer alloc] initWithTarget:self action:@selector(DP_dismissDatePicker:)];
    [darkView addGestureRecognizer:tapGesture];
    [vc.view addSubview:darkView];
    
    
    datePicker = [[UIDatePicker alloc] initWithFrame:CGRectMake(0, vc.view.bounds.size.height+44, [self GetW], 216)];
    datePicker.tag = 10;
    if(date != nil) {
        [datePicker setDate:date];
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"DateChangedEventManually date: %@", date];
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"DateChangedEventManually datePicker: %@", [datePicker date]];
    }
    
#if UNITY_VERSION < 500
    [darkView autorelease];
    [tapGesture autorelease];
    [datePicker autorelease];
#endif
    
    
    [datePicker addTarget:self action:@selector(DP_changeDate:) forControlEvents:UIControlEventValueChanged];
    switch (mode) {
        case 1:
            datePicker.datePickerMode = UIDatePickerModeTime;
            break;
            
        case 2:
            datePicker.datePickerMode = UIDatePickerModeDate;
            break;
            
        case 3:
            datePicker.datePickerMode = UIDatePickerModeDateAndTime;
            break;
            
        case 4:
            datePicker.datePickerMode = UIDatePickerModeCountDownTimer;
            break;
            
        default:
            break;
    }
    
    // NSLog(@"dtp mode: %ld", (long)datePicker.datePickerMode);
    
    
    [vc.view addSubview:datePicker];
    
    UIToolbar *toolBar = [[UIToolbar alloc] initWithFrame:CGRectMake(0, vc.view.bounds.size.height, [self GetW], 44)];
    
    toolBar.tag = 11;
    toolBar.barStyle = UIBarStyleDefault;
    UIBarButtonItem *spacer = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemFlexibleSpace target:nil action:nil];
    UIBarButtonItem *doneButton = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemDone target:self action:@selector(DP_dismissDatePicker:)];
    
#if UNITY_VERSION < 500
    [toolBar autorelease];
    [spacer autorelease];
    [doneButton autorelease];
#endif
    
    [toolBar setItems:[NSArray arrayWithObjects:spacer, doneButton, nil]];
    [vc.view addSubview:toolBar];
    
    [UIView beginAnimations:@"MoveIn" context:nil];
    toolBar.frame = toolbarTargetFrame;
    datePicker.frame = datePickerTargetFrame;
    darkView.frame = darkViewTargetFrame;
    
    [UIView commitAnimations];
    
}

- (void)DP_removeViews:(id)object {
    UIViewController *vc =  UnityGetGLViewController();
    
    [[vc.view viewWithTag:9] removeFromSuperview];
    [[vc.view viewWithTag:10] removeFromSuperview];
    [[vc.view viewWithTag:11] removeFromSuperview];
}

- (void)DP_dismissDatePicker:(id)sender {
    UIViewController *vc =  UnityGetGLViewController();
    
    [self DP_PickerClosed:datePicker];
    
    CGRect toolbarTargetFrame = CGRectMake(0, vc.view.bounds.size.height, [self GetW], 44);
    CGRect datePickerTargetFrame = CGRectMake(0, vc.view.bounds.size.height+44, [self GetW], 216);
    CGRect darkViewTargetFrame = CGRectMake(0, vc.view.bounds.size.height, [self GetW], 260);
    
    
    [UIView beginAnimations:@"MoveOut" context:nil];
    [vc.view viewWithTag:9].frame = darkViewTargetFrame;
    [vc.view viewWithTag:10].frame = datePickerTargetFrame;
    [vc.view viewWithTag:11].frame = toolbarTargetFrame;
    [UIView setAnimationDelegate:self];
    [UIView setAnimationDidStopSelector:@selector(DP_removeViews:)];
    [UIView commitAnimations];
}

#endif



- (void) GetLocale {
    
    NSUserDefaults* userDefaults = [NSUserDefaults standardUserDefaults];
    NSArray* arrayLanguages = [userDefaults objectForKey:@"AppleLanguages"];
    NSString* currentLanguage = [arrayLanguages firstObject];
    
    NSLocale *countryLocale = [NSLocale currentLocale];
    NSString *countryCode = [countryLocale objectForKey:NSLocaleCountryCode];
    NSString *country = [countryLocale displayNameForKey:NSLocaleCountryCode value:countryCode];
    
    NSString *languageID = [[NSBundle mainBundle] preferredLocalizations].firstObject;
    NSLocale *locale = [NSLocale localeWithLocaleIdentifier:languageID];
    NSString *languageCode = [locale displayNameForKey:NSLocaleIdentifier value:languageID];
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    [data appendString:countryCode];
    [data appendString: UNITY_SPLITTER];
    
    [data appendString:country];
    [data appendString: UNITY_SPLITTER];
    
    [data appendString:currentLanguage];
    [data appendString: UNITY_SPLITTER];
    
    [data appendString:languageCode];
    
    
    NSString *str = [data copy];
    
    UnitySendMessage("IOSNativeUtility", "OnLocaleLoadedHandler", [ISN_DataConvertor NSStringToChar:str]);
    
}

- (void)pickDate:(int)startYear {
#if !TARGET_OS_TV
    
    UIViewController *vc =  UnityGetGLViewController();
    UINavigationController *ctrl = [CalendarPickerController defaultPicker];
    
    [vc presentViewController:ctrl animated:YES completion:nil];
#endif
}

@end








@implementation CloudManager
static CloudManager * cm_sharedInstance;


+ (id)sharedInstance {
    
    if (cm_sharedInstance == nil)  {
        cm_sharedInstance = [[self alloc] init];
        [cm_sharedInstance initialize];
    }
    
    return cm_sharedInstance;
}


-(void) initialize {
    
    /*
     
     [[NSNotificationCenter defaultCenter]
     addObserver: self
     selector: @selector (iCloudAccountAvailabilityChanged:)
     name: NSUbiquityIdentityDidChangeNotification
     object: nil];
     
     */
    
    
    NSFileManager*  fileManager = [NSFileManager defaultManager];
    id currentToken = [fileManager ubiquityIdentityToken];
    bool isSignedIntoICloud = (currentToken!=nil);
    
    if(isSignedIntoICloud) {
        NSUbiquitousKeyValueStore *store = [NSUbiquitousKeyValueStore defaultStore];
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(storeDidChange:)
                                                     name:NSUbiquitousKeyValueStoreDidChangeExternallyNotification
                                                   object:store];
        [store synchronize];
    }
    
    
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"iCloud Initialize"];
    
}

-(void)setString:(NSString *)val key:(NSString *)key {
    NSUbiquitousKeyValueStore *store = [NSUbiquitousKeyValueStore defaultStore];
    [store setString:val forKey:key];
    
    [store synchronize];
}

-(void) setData:(NSData *)val key:(NSString *)key {
    NSUbiquitousKeyValueStore *store = [NSUbiquitousKeyValueStore defaultStore];
    [store setData:val forKey:key];
    
    [store synchronize];
}

-(void) setDouble:(double)val key:(NSString *)key {
    NSUbiquitousKeyValueStore *store = [NSUbiquitousKeyValueStore defaultStore];
    [store setDouble:val forKey:key];
    
    [store synchronize];
    
}


-(void) requestDataForKey:(NSString *)key {
    NSUbiquitousKeyValueStore *store = [NSUbiquitousKeyValueStore defaultStore];
    
    id data = [store objectForKey:key];
    
    
    
    NSMutableString * array = [[NSMutableString alloc] init];
    [array appendString:key];
    [array appendString:UNITY_SPLITTER];
    
    
    NSString* stringData;
    
    if(data != nil) {
        if([data isKindOfClass:[NSString class]]) {
            stringData = (NSString*) data;
        }
        
        if([data isKindOfClass:[NSData class]]) {
            NSData *b = (NSData*) data;
            stringData = [b base64Encoding];
        }
        
        if([data isKindOfClass:[NSNumber class]]) {
            NSNumber* n = (NSNumber*) data;
            stringData = [n stringValue];
        }
        
    } else {
        stringData = @"null";
    }
    
    
    [array appendString:stringData];
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"data: %@", stringData];
    
    
    NSString *package = [array copy];
#if UNITY_VERSION < 500
    [package autorelease];
#endif
    
    if(data == nil) {
        UnitySendMessage("iCloudManager", "OnCloudDataEmpty", [ISN_DataConvertor NSStringToChar:package]);
    } else {
        UnitySendMessage("iCloudManager", "OnCloudData", [ISN_DataConvertor NSStringToChar:package]);
        
    }
    
    
    
}



- (void)storeDidChange:(NSNotification *)notification {
    NSDictionary* userInfo = [notification userInfo];
    NSNumber* reasonForChange = [userInfo objectForKey:NSUbiquitousKeyValueStoreChangeReasonKey];
    NSInteger reason = -1;
    
    // If a reason could not be determined, do not update anything.
    if (!reasonForChange)
        return;
    
    
    NSMutableString * array = [[NSMutableString alloc] init];
    
    
    
    // Update only for changes from the server.
    reason = [reasonForChange integerValue];
    if ((reason == NSUbiquitousKeyValueStoreServerChange) || (reason == NSUbiquitousKeyValueStoreInitialSyncChange)) {
        
        NSArray* changedKeys = [userInfo objectForKey:NSUbiquitousKeyValueStoreChangedKeysKey];
        NSUbiquitousKeyValueStore* store = [NSUbiquitousKeyValueStore defaultStore];
        
        for (NSString* key in changedKeys) {
            id value = [store objectForKey:key];
            
            [array appendString:key];
            [array appendString:UNITY_SPLITTER];
            
            NSString* stringData;
            
            if(value != nil) {
                if([value isKindOfClass:[NSString class]]) {
                    stringData = (NSString*) value;
                }
                
                if([value isKindOfClass:[NSData class]]) {
                    
                    NSData *b = (NSData*) value;
                    
                    NSMutableString *str = [[NSMutableString alloc] init];
                    const char *db = (const char *) [b bytes];
                    for (int i = 0; i < [b length]; i++) {
                        if(i != 0) {
                            [str appendFormat:@","];
                        }
                        
                        [str appendFormat:@"%i", (unsigned char)db[i]];
                    }
                    
                    stringData = str;
                    
                }
                
                if([value isKindOfClass:[NSNumber class]]) {
                    NSNumber* n = (NSNumber*) value;
                    stringData = [n stringValue];
                }
                
            } else {
                stringData = @"null";
            }
            
            
            [array appendString:stringData];
            [array appendString:UNITY_SPLITTER];
        }
        
        [array appendString:UNITY_EOF];
        
    }
    
    UnitySendMessage("iCloudManager", "OnCloudDataChanged", [ISN_DataConvertor NSStringToChar:array]);
}

-(void) iCloudAccountAvailabilityChanged {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"iCloudAccountAvailabilityChanged:"];
}

@end




@implementation ISN_NativePopUpsManager

static UIAlertController* _currentAlert =  nil;


static ISN_NativePopUpsManager *_sharedInstance;

+ (id)sharedInstance {
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}



+(void) dismissCurrentAlert {
    if(_currentAlert != nil) {
        [_currentAlert dismissViewControllerAnimated:true completion:^{
            UnitySendMessage("IOSPopUp", "onPopUpCallBack", [ISN_DataConvertor NSStringToChar:@"0"]);
            UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack", [ISN_DataConvertor NSStringToChar:@"0"]);
        }];
        
        
#if UNITY_VERSION < 500
        //  [_currentAlert release];
#endif
        _currentAlert = nil;
    }
}

+(void) showRateUsPopUp: (NSString *) title message: (NSString*) msg b1: (NSString*) b1 b2: (NSString*) b2 b3: (NSString*) b3 {
    
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:title  message:msg  preferredStyle:UIAlertControllerStyleAlert];
    
    
    
    UIAlertAction* rateAction = [UIAlertAction actionWithTitle:b1 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack", [ISN_DataConvertor NSStringToChar:@"0"]);
        _currentAlert = nil;
    }];
    
    
    UIAlertAction* laterAction = [UIAlertAction actionWithTitle:b2 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack", [ISN_DataConvertor NSStringToChar:@"1"]);
        _currentAlert = nil;
    }];
    
    
    UIAlertAction* declineAction = [UIAlertAction actionWithTitle:b3 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack", [ISN_DataConvertor NSStringToChar:@"2"]);
        _currentAlert = nil;
    }];
    
    
    [alert addAction:rateAction];
    [alert addAction:laterAction];
    [alert addAction:declineAction];
    
    _currentAlert = alert;
    
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:alert animated:YES completion:nil];
    
    
    
}




+ (void) showDialog: (NSString *) title message: (NSString*) msg yesTitle:(NSString*) b1 noTitle: (NSString*) b2{
    
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:title  message:msg  preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction* okAction = [UIAlertAction actionWithTitle:b1 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSPopUp", "onPopUpCallBack", [ISN_DataConvertor NSStringToChar:@"0"]);
        _currentAlert = nil;
    }];
    
    
    UIAlertAction* yesAction = [UIAlertAction actionWithTitle:b2 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSPopUp", "onPopUpCallBack", [ISN_DataConvertor NSStringToChar:@"1"]);
        _currentAlert = nil;
    }];
    
    [alert addAction:yesAction];
    [alert addAction:okAction];
    
    _currentAlert = alert;
    
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:alert animated:YES completion:nil];
    
}


+(void) showMessage: (NSString *) title message: (NSString*) msg okTitle:(NSString*) b1 {
    
    
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:title  message:msg  preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction* defaultAction = [UIAlertAction actionWithTitle:b1 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSPopUp", "onPopUpCallBack", [ISN_DataConvertor NSStringToChar:@"0"]);
        _currentAlert = nil;
    }];
    
    
    [alert addAction:defaultAction];
    _currentAlert = alert;
    
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:alert animated:YES completion:nil];
    
}

//--------------------------------------
//  IOS 6,7 implementation
//--------------------------------------

#if !TARGET_OS_TV

static UIAlertView* _currentAllert =  nil;

+ (void) unregisterAllertView_old {
    if(_currentAllert != nil) {
#if UNITY_VERSION < 500
        [_currentAlert release];
#endif
        _currentAllert = nil;
    }
}

+(void) dismissCurrentAlert_old {
    if(_currentAllert != nil) {
        [_currentAllert dismissWithClickedButtonIndex:0 animated:YES];
#if UNITY_VERSION < 500
        [_currentAlert release];
#endif
        _currentAllert = nil;
    }
}

+(void) showRateUsPopUp_old: (NSString *) title message: (NSString*) msg b1: (NSString*) b1 b2: (NSString*) b2 b3: (NSString*) b3 {
    
    UIAlertView *alert = [[UIAlertView alloc] init];
    [alert setTitle:title];
    [alert setMessage:msg];
    [alert setDelegate: [ISN_NativePopUpsManager sharedInstance]];
    
    [alert addButtonWithTitle:b1];
    [alert addButtonWithTitle:b2];
    [alert addButtonWithTitle:b3];
    
    [alert show];
    
    _currentAllert = alert;
    
}




+ (void) showDialog_old: (NSString *) title message: (NSString*) msg yesTitle:(NSString*) b1 noTitle: (NSString*) b2{
    
    UIAlertView *alert = [[UIAlertView alloc] init];
    [alert setTitle:title];
    [alert setMessage:msg];
    [alert setDelegate: [ISN_NativePopUpsManager sharedInstance]];
    [alert addButtonWithTitle:b1];
    [alert addButtonWithTitle:b2];
    [alert show];
    
    _currentAllert = alert;
    
}


+(void) showMessage_old: (NSString *) title message: (NSString*) msg okTitle:(NSString*) b1 {
    
    UIAlertView *alert = [[UIAlertView alloc] init];
    [alert setTitle:title];
    [alert setMessage:msg];
    [alert setDelegate: [ISN_NativePopUpsManager sharedInstance]];
    [alert addButtonWithTitle:b1];
    [alert show];
    
    _currentAllert = alert;
}





- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex {
    [ISN_NativePopUpsManager unregisterAllertView_old];
    UnitySendMessage("IOSPopUp", "onPopUpCallBack",  [ISN_DataConvertor NSIntToChar:buttonIndex]);
    UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack",  [ISN_DataConvertor NSIntToChar:buttonIndex]);
}
#endif

@end












@implementation IOSNativeNotificationCenter


static IOSNativeNotificationCenter *sharedHelper = nil;

+ (IOSNativeNotificationCenter *) sharedInstance {
    if (!sharedHelper) {
        sharedHelper = [[IOSNativeNotificationCenter alloc] init];
        
        
    }
    return sharedHelper;
}

- (id)init {
    if ((self = [super init])) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Subscibing..."];
        NSNotificationCenter *notificationCenter = [NSNotificationCenter defaultCenter];
        [notificationCenter addObserver: self
                               selector: @selector (handle_NotificationEvent:)
                                   name: kUnityDidReceiveLocalNotification
                                 object: nil];
        
        
    }
    
    return self;
}




#pragma mark Music notification handlers



#if !TARGET_OS_TV


- (void) handle_NotificationEvent: (NSNotification *) receivedNotification {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: handle_NotificationEvent"];
    UILocalNotification* notification = (UILocalNotification*) receivedNotification.userInfo;
    
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    if(notification.alertBody != nil) {
        [data appendString:notification.alertBody];
    } else {
        [data appendString:@""];
    }
    
    [data appendString:@"|"];
    
    
    NSString* AlarmKey =[notification.userInfo objectForKey:@"AlarmKey"];
    if(AlarmKey != nil) {
        [data appendString:AlarmKey];
    } else {
        [data appendString:@""];
    }
    [data appendString:@"|"];
    
    
    NSString* dataKey =[notification.userInfo objectForKey:@"data"];
    if(dataKey != nil) {
        [data appendString:dataKey];
    } else {
        [data appendString:@""];
    }
    [data appendString:@"|"];
    
    
    [data appendString: [NSString stringWithFormat:@"%ld", (long)notification.applicationIconBadgeNumber]];
    
    
    NSString *str = [data copy];
    
#if UNITY_VERSION < 500
    [str autorelease];
#endif
    
    
    UnitySendMessage("ISN_LocalNotificationsController", "OnLocalNotificationReceived_Event", [ISN_DataConvertor NSStringToChar:str]);
    
}

#endif

- (void) RegisterForNotifications {
#if !TARGET_OS_TV
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        [[UIApplication sharedApplication] registerUserNotificationSettings:[UIUserNotificationSettings settingsForTypes:UIUserNotificationTypeAlert|UIUserNotificationTypeBadge|UIUserNotificationTypeSound categories:nil]];
        
    }
#endif
}


-(void) scheduleNotification:(int)time message:(NSString *)message sound:(bool *)sound alarmID:(NSString *)alarmID badges:(int)badges notificationData:(NSString *)notificationData notificationSoundName:(NSString *)notificationSoundName{
    
#if !TARGET_OS_TV
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        UIUserNotificationSettings* NotificationSettings = [[UIApplication sharedApplication] currentUserNotificationSettings];
        
        if((NotificationSettings.types & UIUserNotificationTypeAlert) == 0) {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: user disabled local notification for this app, sending fail event."];
            
            NSMutableString * data = [[NSMutableString alloc] init];
            [data appendString: @"0" ];
            [data appendString:@"|"];
            [data appendString:  [NSString stringWithFormat:@"%u",[[UIApplication sharedApplication] currentUserNotificationSettings].types]];
            
            UnitySendMessage("ISN_LocalNotificationsController", "OnNotificationScheduleResultAction", [ISN_DataConvertor NSStringToChar:data]);
            
            [self RegisterForNotifications];
            return;
        }
        
        if((NotificationSettings.types & UIUserNotificationTypeBadge) == 0) {
            
            if(badges > 0) {
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: no badges allowed for this user. Notification badge disabled."];
                badges = 0;
            }
            
            
        }
        
        if((NotificationSettings.types & UIUserNotificationTypeSound) == 0) {
            if(sound) {
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: no sound allowed for this user. Notification sound disabled."];
#if UNITY_VERSION < 500
                sound = false;
#endif
            }
            
        }
    }
    
    
    UILocalNotification* localNotification = [[UILocalNotification alloc] init];
    localNotification.fireDate = [NSDate dateWithTimeIntervalSinceNow:time];
    localNotification.alertBody = message;
    localNotification.timeZone = [NSTimeZone defaultTimeZone];
    
    
    if (badges > 0)
        localNotification.applicationIconBadgeNumber = badges;
    
    if(sound) {
        if([notificationSoundName isEqual:@""]) {
            localNotification.soundName = UILocalNotificationDefaultSoundName;
        } else {
            localNotification.soundName = notificationSoundName;
        }
    }
    
    
    
    NSMutableDictionary *userInfo = [NSMutableDictionary dictionary];
    [userInfo setObject:alarmID forKey:@"AlarmKey"];
    [userInfo setObject:notificationData forKey:@"data"];
    
    // Set some extra info to your alarm
    localNotification.userInfo = userInfo;
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: scheduleNotification AlarmKey: %@", alarmID];
    
    [[UIApplication sharedApplication] scheduleLocalNotification:localNotification];
    
    
    NSMutableString * data = [[NSMutableString alloc] init];
    [data appendString: @"1" ];
    [data appendString:@"|"];
    
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        [data appendString:  [NSString stringWithFormat:@"%u",[[UIApplication sharedApplication] currentUserNotificationSettings].types]];
    } else {
        [data appendString:@"7"];
    }
    
    
    UnitySendMessage("ISN_LocalNotificationsController", "OnNotificationScheduleResultAction", [ISN_DataConvertor NSStringToChar:data]);
    
#endif
}

#if !TARGET_OS_TV

- (UILocalNotification *)existingNotificationWithAlarmID:(NSString *)alarmID {
    for (UILocalNotification *notification in [[UIApplication sharedApplication] scheduledLocalNotifications]) {
        if ([[notification.userInfo objectForKey:@"AlarmKey"] isEqualToString:alarmID]) {
            return notification;
        }
    }
    
    return nil;
}

#endif



- (void)cleanUpLocalNotificationWithAlarmID:(NSString *)alarmID {
#if !TARGET_OS_TV
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"cleanUpLocalNotificationWithAlarmID AlarmKey: %@", alarmID];
    
    UILocalNotification *notification = [self existingNotificationWithAlarmID:alarmID];
    if (notification) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"notification canceled"];
        [[UIApplication sharedApplication] cancelLocalNotification:notification];
    }
    
#endif
}






- (void) cancelNotifications {
#if !TARGET_OS_TV
    [[UIApplication sharedApplication] cancelAllLocalNotifications];
#endif
}

- (void) applicationIconBadgeNumber:(int) badges {
#if !TARGET_OS_TV
    [UIApplication sharedApplication].applicationIconBadgeNumber = badges;
#endif
}


@end

//================
// Calendar
//================

@implementation UIView (ZolaZoomSnapshot)

- (UIImage *)zo_snapshot {
    UIGraphicsBeginImageContextWithOptions(self.bounds.size, self.opaque, [UIScreen mainScreen].scale);
    CGContextRef context = UIGraphicsGetCurrentContext();
    [self.layer renderInContext:context];
    UIImage *snapshot = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return snapshot;
}

@end

@implementation MonthLayerCollectionViewCell

- (id)initWithFrame:(CGRect)aRect {
    self = [super initWithFrame:(CGRect)aRect];
    if (self) {
        self.opaque = false;
        self.contentView.opaque = false;
        self.autoresizesSubviews = false;
        self.contentView.translatesAutoresizingMaskIntoConstraints = false;
        self.contentView.clearsContextBeforeDrawing = true;
        
        self.monthTitle = [[CATextLayer alloc] init];
        //        [self.monthTitle setFont:@"Helvetica-Light"];
        [self.monthTitle setFontSize:17];
        [self.monthTitle setAlignmentMode:kCAAlignmentLeft];
        [self.monthTitle setForegroundColor:[[UIColor redColor] CGColor]];
        [self.monthTitle setBackgroundColor:[[UIColor clearColor] CGColor]];
        self.monthTitle.contentsScale = [UIScreen mainScreen].scale;
        [self.contentView.layer addSublayer:self.monthTitle];
        
        self.startWeekDay = -1;
        self.numberOfDays = 0;
    }
    return self;
    
}

- (void)setStartDay:(int)startDay {
    if (_startWeekDay != startDay) {
        
        for (int i = startDay; i < startDay + 32; i++) {
            int number = i - startDay;
            
            if (number >= 1) {
                CATextLayer *label = [[CATextLayer alloc] init];
                [self.contentView.layer addSublayer:label];
                label.string = [NSString stringWithFormat:@"%d", number];
                //                [label setFont:@"Helvetica-Light"];
                [label setFontSize:10];
                [label setAlignmentMode:kCAAlignmentCenter];
                [label setForegroundColor:[[UIColor blackColor] CGColor]];
                label.contentsScale = [UIScreen mainScreen].scale;
            } else {
                continue;
            }
        }
        
        self.startWeekDay = startDay;
    }
}

- (void)setNumberOfDays:(int)numberOfDays {
    
    if (numberOfDays != _numberOfDays) {
        
        NSArray *labels = self.contentView.layer.sublayers;
        
        for (int i = 28; i < labels.count; i++ ) {
            if (i <= numberOfDays) {
                ((CATextLayer *)labels[i]).string = [NSString stringWithFormat:@"%d", i];
            } else {
                ((CATextLayer *)labels[i]).string = nil;
            }
            
        }
        _numberOfDays = numberOfDays;
    }
}

- (void)layoutSubviews {
    if (_startWeekDay == -1 || _numberOfDays == 0) {
        return;
    }
    CGSize frameSize = self.frame.size;
    
    CGFloat itemWidth = ceil(frameSize.width / 7);
    CGFloat itemHeight = ceil(1.2 * itemWidth);
    
    CGFloat monthNameHeight = 24;
    
    [self.monthTitle setFrame:CGRectMake(0, 0, frameSize.width, monthNameHeight)];
    
    CGRect frame = CGRectMake(0, 0, itemWidth, itemHeight);
    NSArray *labels = self.contentView.layer.sublayers;
    int i = 1;
    for (int week = 0; week <= 5; week++) {
        for (int day = 0; day <= 6; day++) {
            
            if (i >= labels.count || i > _numberOfDays) {
                return;
            }
            
            if (week == 0 && _startWeekDay > day) {
                continue;
            }
            
            CATextLayer *label = labels[i];
            frame.origin = CGPointMake(day * itemWidth, week * itemHeight + monthNameHeight);
            [label setFrame:frame];
            i++;
        }
    }
    
}

@end

#if !TARGET_OS_TV

@implementation CalendarPickerController

+ (instancetype)defaultPicker
{
    static CalendarPickerController *defaultPicker;
    static dispatch_once_t onceToken;
    
    dispatch_once(&onceToken, ^{
        defaultPicker = [[super alloc]initWithRootViewController:[YearViewController defaultController]];
        defaultPicker.monthNames = @[@"JAN", @"FEB", @"MAR", @"APR", @"MAY", @"JUN", @"JUL", @"AUG", @"SEP", @"OCT", @"NOV", @"DEC"];
        defaultPicker.dayNumberStrings = @[@"1", @"2", @"3", @"4", @"5", @"6", @"7", @"8", @"9", @"10",
                                           @"11", @"12", @"13", @"14", @"15", @"16", @"17", @"18", @"19", @"20",
                                           @"21", @"22", @"23", @"24", @"25", @"26", @"27", @"28", @"29", @"30", @"31"];
        
        defaultPicker.startDayIsSunday = false;
    });
    
    return defaultPicker;
}

+ (instancetype)initWithStartYear:(NSInteger)calendarStartYear endYear:(NSInteger)calendarEndYear withStartDayIsSunday:(BOOL)startDayIsSunday
{
    CalendarPickerController *picker;
    
    
    picker = [[super alloc]initWithRootViewController:[YearViewController initWithStartYear:calendarStartYear endYear:calendarEndYear withStartDayIsSunday:startDayIsSunday]];
    picker.monthNames = @[@"JAN", @"FEB", @"MAR", @"APR", @"MAY", @"JUN", @"JUL", @"AUG", @"SEP", @"OCT", @"NOV", @"DEC"];
    picker.dayNumberStrings = @[@"1", @"2", @"3", @"4", @"5", @"6", @"7", @"8", @"9", @"10",
                                @"11", @"12", @"13", @"14", @"15", @"16", @"17", @"18", @"19", @"20",
                                @"21", @"22", @"23", @"24", @"25", @"26", @"27", @"28", @"29", @"30", @"31"];
    picker.startDayIsSunday = startDayIsSunday;
    
    return picker;
}


- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    [self.view setBackgroundColor:[UIColor whiteColor]];
    
    [self setupToolbar];
    [self.navigationBar setTranslucent:true];
    [self setAutomaticallyAdjustsScrollViewInsets:false];
    [self.navigationBar setTintColor:[UIColor colorWithRed:1 green:59/255.f blue:49/255.f alpha:1]];
    
    
}

-(UIImageView * _Nullable )findShadowImageUnder:(UIView*)view {
    if ([view isKindOfClass:UIImageView.class] && view.bounds.size.height <= 1) {
        return (UIImageView*) view;
    }
    
    for (UIView *subview in view.subviews) {
        UIImageView *imageView = [self findShadowImageUnder:subview];
        if (imageView != nil) {
            return imageView;
        }
    }
    
    return nil;
}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    
    if (self.viewControllers.count > 1) {
        [self popToRootViewControllerAnimated:false];
    }
    
    UIView *shadowImageView = [self findShadowImageUnder:self.navigationBar];
    
    if (shadowImageView) {
        shadowImageView.hidden = true;
    }
}

- (void)setupToolbar {
    [self setToolbarHidden:true];
    
    UIBarButtonItem *flexibleItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemFlexibleSpace target:self action:nil];
    UIBarButtonItem *item1, *item2, *item3, *item4, *item5, *item6, *item7;
    if (!self.startDayIsSunday) {
        item1 = [[UIBarButtonItem alloc] initWithTitle:@"M" style:UIBarButtonItemStylePlain target:nil action:nil];
        item2 = [[UIBarButtonItem alloc] initWithTitle:@"T" style:UIBarButtonItemStylePlain target:nil action:nil];
        item3 = [[UIBarButtonItem alloc] initWithTitle:@"W" style:UIBarButtonItemStylePlain target:nil action:nil];
        item4 = [[UIBarButtonItem alloc] initWithTitle:@"T" style:UIBarButtonItemStylePlain target:nil action:nil];
        item5 = [[UIBarButtonItem alloc] initWithTitle:@"F" style:UIBarButtonItemStylePlain target:nil action:nil];
        item6 = [[UIBarButtonItem alloc] initWithTitle:@"S" style:UIBarButtonItemStylePlain target:nil action:nil];
        item7 = [[UIBarButtonItem alloc] initWithTitle:@"S" style:UIBarButtonItemStylePlain target:nil action:nil];
    } else {
        item1 = [[UIBarButtonItem alloc] initWithTitle:@"S" style:UIBarButtonItemStylePlain target:nil action:nil];
        item2 = [[UIBarButtonItem alloc] initWithTitle:@"M" style:UIBarButtonItemStylePlain target:nil action:nil];
        item3 = [[UIBarButtonItem alloc] initWithTitle:@"T" style:UIBarButtonItemStylePlain target:nil action:nil];
        item4 = [[UIBarButtonItem alloc] initWithTitle:@"W" style:UIBarButtonItemStylePlain target:nil action:nil];
        item5 = [[UIBarButtonItem alloc] initWithTitle:@"T" style:UIBarButtonItemStylePlain target:nil action:nil];
        item6 = [[UIBarButtonItem alloc] initWithTitle:@"F" style:UIBarButtonItemStylePlain target:nil action:nil];
        item7 = [[UIBarButtonItem alloc] initWithTitle:@"S" style:UIBarButtonItemStylePlain target:nil action:nil];
    }
    
    
    item1.tintColor = UIColor.blackColor;
    item2.tintColor = UIColor.blackColor;
    item3.tintColor = UIColor.blackColor;
    item4.tintColor = UIColor.blackColor;
    item5.tintColor = UIColor.blackColor;
    item6.tintColor = UIColor.blackColor;
    item7.tintColor = UIColor.blackColor;
    
    UIFont *font = [UIFont systemFontOfSize:10];
    NSDictionary *attributes = @{NSFontAttributeName: font};
    
    [item1 setTitleTextAttributes:attributes forState:UIControlStateNormal];
    [item2 setTitleTextAttributes:attributes forState:UIControlStateNormal];
    [item3 setTitleTextAttributes:attributes forState:UIControlStateNormal];
    [item4 setTitleTextAttributes:attributes forState:UIControlStateNormal];
    [item5 setTitleTextAttributes:attributes forState:UIControlStateNormal];
    [item6 setTitleTextAttributes:attributes forState:UIControlStateNormal];
    [item7 setTitleTextAttributes:attributes forState:UIControlStateNormal];
    
    NSArray *items = [NSArray arrayWithObjects:item1, flexibleItem, item2, flexibleItem, item3, flexibleItem, item4, flexibleItem, item5, flexibleItem, item6, flexibleItem, item7, nil];
    
    
    _topToolbar = [[UIToolbar alloc]initWithFrame:CGRectMake(0, 0, self.view.frame.size.width, 22)];
    [_topToolbar setItems:items];
    [_topToolbar setTranslatesAutoresizingMaskIntoConstraints:false];
    [self.view addSubview:_topToolbar];
    [_topToolbar.topAnchor constraintEqualToAnchor:self.navigationBar.bottomAnchor constant:-1].active = true;
    [_topToolbar.leftAnchor constraintEqualToAnchor:self.navigationBar.leftAnchor].active = true;
    [_topToolbar.widthAnchor constraintEqualToAnchor:self.view.widthAnchor].active = true;
    [_topToolbar.heightAnchor constraintEqualToConstant:22].active = true;
    [_topToolbar setClipsToBounds:true];
    _topToolbar.layer.borderWidth = 0;
    _topToolbar.layer.borderColor = [[UIColor clearColor] CGColor];
}

@end

@implementation MonthViewController


+ (instancetype)defaultControllerWithYear:(NSInteger)year andMonth:(NSInteger)month withStartDayIsSunday:(BOOL)startDayIsSunday{
    MonthViewController *defaultController;
    UICollectionViewFlowLayout *collectionViewLayout;
    collectionViewLayout = [UICollectionViewFlowLayout new];
    [collectionViewLayout setScrollDirection:UICollectionViewScrollDirectionVertical];
    
    defaultController = [[super alloc] initWithCollectionViewLayout:collectionViewLayout];
    defaultController.year = year;
    defaultController.currentMonth = month;
    defaultController.startDayIsSunday = false;
    defaultController.cellDaysDataByIndexPath = [NSMutableDictionary new];
    
    defaultController.monthNames = [CalendarPickerController defaultPicker].monthNames;
    defaultController.dayNumberStrings = [CalendarPickerController defaultPicker].dayNumberStrings;
    defaultController.startDayIsSunday = startDayIsSunday;
    
    return defaultController;
}

static NSString * const startWeekDayKey = @"startWeekDay";
static NSString * const numberOfDaysKey = @"numberOfDays";
static NSString * const reuseIdentifier = @"DayInYearViewCell";
static NSString * const headerReuseIdentifier = @"MonthSectionHeaderReusableView";

- (void)viewDidLoad {
    [super viewDidLoad];
    if (!_startDayIsSunday) {
        _holidays = @[@5, @6, @12, @13, @19, @20, @26, @27, @33, @34];
    } else {
        _holidays = @[@0, @6, @7, @13, @14, @20, @21, @27, @28, @34, @35];
    }
    [self.collectionView setContentInset:UIEdgeInsetsMake(self.topLayoutGuide.length, 0, 0, 0)];
    
    [self.collectionView registerClass:[DayInYearViewCell class] forCellWithReuseIdentifier:reuseIdentifier];
    [self.collectionView registerClass:[MonthSectionHeaderReusableView class] forSupplementaryViewOfKind:UICollectionElementKindSectionHeader withReuseIdentifier:headerReuseIdentifier];
    
    self.collectionView.opaque = true;
    self.collectionView.backgroundColor = [UIColor whiteColor];
    self.collectionView.showsVerticalScrollIndicator = false;
    
    UICollectionViewFlowLayout *layout = (UICollectionViewFlowLayout *)self.collectionViewLayout;
    [layout setSectionInset:UIEdgeInsetsMake(10, 0, 10, 0)];
    
    self.itemSize = [self itemSizeForSize:self.collectionView.bounds.size];
    
    NSIndexPath *currentMonthIndexPath = [NSIndexPath indexPathForItem:15 inSection:_currentMonth - 1];
    [self.collectionView scrollToItemAtIndexPath:currentMonthIndexPath atScrollPosition:UICollectionViewScrollPositionCenteredVertically animated:false];
    
}

- (CGSize)itemSizeForSize:(CGSize)size {
    CGFloat itemSide = (size.width) / 7.01;
    if (size.width < size.height) {
        return CGSizeMake(itemSide, itemSide);
    }
    return CGSizeMake(itemSide, itemSide / 2);
}

- (BOOL)shouldInvalidateLayoutForBoundsChange:(CGRect)newBounds {
    return YES;
}

- (void)viewWillTransitionToSize:(CGSize)size withTransitionCoordinator:(id<UIViewControllerTransitionCoordinator>)coordinator {
    [super viewWillTransitionToSize:size
          withTransitionCoordinator:coordinator];
    
    [self.collectionView setContentInset:UIEdgeInsetsMake(self.topLayoutGuide.length, 0, 0, 0)];
    
    self.itemSize = [self itemSizeForSize:size];
    
    [coordinator animateAlongsideTransition:^(id<UIViewControllerTransitionCoordinatorContext> context) {
        [self.collectionView.collectionViewLayout invalidateLayout];
    } completion:^(id<UIViewControllerTransitionCoordinatorContext> context) { }];
}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    
    [self.view layoutIfNeeded];
    
    NSIndexPath *currentMonthIndexPath = [NSIndexPath indexPathForItem:15 inSection:_currentMonth - 1];
    [self.collectionView scrollToItemAtIndexPath:currentMonthIndexPath atScrollPosition:UICollectionViewScrollPositionCenteredVertically animated:false];
    
    ((CalendarPickerController*) self.navigationController).topToolbar.hidden = false;
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark <UICollectionViewDataSource>

- (NSInteger)numberOfSectionsInCollectionView:(UICollectionView *)collectionView {
    static NSDateFormatter *dateFormat;
    static dispatch_once_t onceToken;
    static NSString * const stringDateFormat = @"yyyy-MM-dd";
    static NSString * const dateStrFormat = @"%ld-%d-01";
    
    dispatch_once(&onceToken, ^{
        dateFormat = [[NSDateFormatter alloc] init];
        [dateFormat setDateFormat:stringDateFormat];
        [dateFormat setTimeZone:[NSTimeZone timeZoneWithAbbreviation:@"GMT"]];
    });
    
    for (int i = 0; i < 12; i++) {
        NSString *dateStr = [NSString stringWithFormat:dateStrFormat, (long)_year, i + 1];
        NSDate *currentCellDate = [dateFormat dateFromString:dateStr];
        NSRange range = [_calendar rangeOfUnit:NSCalendarUnitDay inUnit:NSCalendarUnitMonth forDate:currentCellDate];
        NSDateComponents *comps = [_calendar components:NSCalendarUnitWeekday fromDate:currentCellDate];
        
        
        int startWeekDay = [comps weekday] - 1;
        
        if (!self.startDayIsSunday) { // TODO: setting start of week day for calendar
            startWeekDay -= 1;
            if (startWeekDay < 0) {
                startWeekDay = 6;
            }
        }
        
        NSDictionary *cellData = @{numberOfDaysKey : [NSNumber numberWithUnsignedInteger:range.length], startWeekDayKey : [NSNumber numberWithInt:startWeekDay]};
        [_cellDaysDataByIndexPath setValue:cellData forKey:[NSString stringWithFormat:@"%d", i]];
        
    }
    return 12;
}


- (NSInteger)collectionView:(UICollectionView *)collectionView numberOfItemsInSection:(NSInteger)section {
    
    NSDictionary *cellData = _cellDaysDataByIndexPath[[NSString stringWithFormat:@"%lu", (unsigned long)section]];
    if (cellData != nil) {
        int startWeekDay = [[cellData valueForKey:startWeekDayKey] integerValue];
        int numberOfDays = [[cellData valueForKey:numberOfDaysKey] integerValue];
        return startWeekDay + numberOfDays;
    }
    return 37;
}

- (UICollectionViewCell *)collectionView:(UICollectionView *)collectionView cellForItemAtIndexPath:(NSIndexPath *)indexPath {
    DayInYearViewCell *cell = [collectionView dequeueReusableCellWithReuseIdentifier:reuseIdentifier forIndexPath:indexPath];
    // Configure the cell
    
    NSDictionary *cellData = _cellDaysDataByIndexPath[[NSString stringWithFormat:@"%lu", (unsigned long)indexPath.section]];
    
    
    if (cellData) {
        int startWeekDay = (int)[[cellData valueForKey:startWeekDayKey] integerValue];
        int numberOfDays = (int)[[cellData valueForKey:numberOfDaysKey] integerValue];
        
        int number = (int)indexPath.item - startWeekDay + 1;
        
        if (number >= 1 && number <= numberOfDays) {
            cell.dayNumber.string = [NSString stringWithFormat:@"%d", number];
            NSNumber *position;
            
            position = [NSNumber numberWithInteger:startWeekDay + number];
            if ([_holidays containsObject:position]) {
                cell.dayNumber.foregroundColor = UIColor.lightGrayColor.CGColor;
            } else {
                cell.dayNumber.foregroundColor = UIColor.blackColor.CGColor;
            }
            cell.topLine.hidden = false;
        } else {
            cell.topLine.hidden = true;
        }
    }
    return cell;
}



-(UICollectionReusableView *)collectionView:(UICollectionView *)collectionView viewForSupplementaryElementOfKind:(nonnull NSString *)kind atIndexPath:(nonnull NSIndexPath *)indexPath {
    
    MonthSectionHeaderReusableView *header;
    
    if (kind == UICollectionElementKindSectionHeader) {
        
        header = [collectionView dequeueReusableSupplementaryViewOfKind:kind
                                                    withReuseIdentifier:headerReuseIdentifier
                                                           forIndexPath:indexPath];
        
        NSDictionary *cellData = _cellDaysDataByIndexPath[[NSString stringWithFormat:@"%lu", (unsigned long)indexPath.section]];
        
        if (cellData != nil) {
            CGFloat width = self.itemSize.width;
            int startWeekDay = [[cellData valueForKey:startWeekDayKey] integerValue];
            header.offset = startWeekDay * width;
            header.width = width;
        }
        
        
        header.monthTitle.string = [NSString stringWithFormat:@"%@", _monthNames[indexPath.section]];
        
    }
    
    
    return header;
}

- (CGSize)collectionView:(UICollectionView *)collectionView layout:(nonnull UICollectionViewLayout *)collectionViewLayout referenceSizeForHeaderInSection:(NSInteger)section {
    return CGSizeMake(self.view.bounds.size.width, 44);
    
}

- (CGSize)collectionView:(UICollectionView *)collectionView layout:(UICollectionViewLayout *)collectionViewLayout
  sizeForItemAtIndexPath:(NSIndexPath *)atIndexPath {
    return self.itemSize;
}

- (CGFloat)collectionView:(UICollectionView *)collectionView layout:(UICollectionViewLayout*)collectionViewLayout minimumInteritemSpacingForSectionAtIndex:(NSInteger)section
{
    return 0;
}

#pragma mark <UICollectionViewDelegate>

- (void)collectionView:(UICollectionView *)collectionView didSelectItemAtIndexPath:(NSIndexPath *)indexPath {
    
    NSDateFormatter *dateFormat = [[NSDateFormatter alloc] init];
    [dateFormat setDateFormat:@"yyyy-MM-dd"];
    [dateFormat setTimeZone:[NSTimeZone timeZoneWithAbbreviation:@"GMT"]];
    
    DayInYearViewCell *cell = (DayInYearViewCell *)[collectionView cellForItemAtIndexPath:indexPath];
    
    CATextLayer *label = cell.dayNumber;
    label.cornerRadius = label.bounds.size.width / 2;
    [label setForegroundColor:[UIColor whiteColor].CGColor];
    
    
    [cell.dayNumber setBackgroundColor:[UIColor redColor].CGColor];
    
    NSString *dateStr = [NSString stringWithFormat:@"%ld-%d-%@", (long)_year, indexPath.section + 1, label.string];
    
    UnitySendMessage("SA.IOSNative.UIKit.NativeReceiver", "CalendarPickerClosed", [ISN_DataConvertor NSStringToChar:dateStr]);
    
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.2 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
        [self.navigationController dismissViewControllerAnimated:true completion:nil];
    });
    
    
}

@end

@implementation YearViewController

+ (instancetype) initWithStartYear:(NSInteger)calendarStartYear endYear:(NSInteger)calendarEndYear withStartDayIsSunday:(BOOL)startDayIsSunday{
    UICollectionViewFlowLayout *collectionViewLayout;
    
    collectionViewLayout = [UICollectionViewFlowLayout new];
    [collectionViewLayout setScrollDirection:UICollectionViewScrollDirectionVertical];
    
    YearViewController *ctrl = [[YearViewController alloc]initWithCollectionViewLayout:collectionViewLayout];
    ctrl.startYear = calendarStartYear;
    ctrl.endYear = calendarEndYear;
    ctrl.currentDate = [NSDate date];
    ctrl.calendar = [[NSCalendar alloc]initWithCalendarIdentifier:NSCalendarIdentifierGregorian];
    NSDateComponents *currentDateComponents = [[NSCalendar currentCalendar] components:NSCalendarUnitYear fromDate:ctrl.currentDate];
    ctrl.currentYear = [currentDateComponents year];
    ctrl.startDayIsSunday = startDayIsSunday;
    
    return ctrl;
}

+ (instancetype)defaultController
{
    YearViewController *defaultController;
    defaultController = [self initWithStartYear:1989 endYear:2089 withStartDayIsSunday:true];
    
    return defaultController;
}

static NSString * const DayInYearViewCellKey = @"DayInYearViewCell";
static NSString * const MonthInYearViewCellKey = @"MonthInYearViewCell";
static NSString * const MonthInYearViewCellKey0 = @"MonthInYearViewCell0";
static NSString * const MonthInYearViewCellKey1 = @"MonthInYearViewCell1";
static NSString * const MonthInYearViewCellKey2 = @"MonthInYearViewCell2";
static NSString * const MonthInYearViewCellKey3 = @"MonthInYearViewCell3";
static NSString * const MonthInYearViewCellKey4 = @"MonthInYearViewCell4";
static NSString * const MonthInYearViewCellKey5 = @"MonthInYearViewCell5";
static NSString * const MonthInYearViewCellKey6 = @"MonthInYearViewCell6";

- (void)viewDidLoad {
    [super viewDidLoad];
    _cellDaysDataByIndexPath = [NSMutableDictionary new];
    
    _monthNames = [CalendarPickerController defaultPicker].monthNames;
    _dayNumberStrings = [CalendarPickerController defaultPicker].dayNumberStrings;
    
    // Register cell classes
    [self.collectionView registerClass:[MonthLayerCollectionViewCell class] forCellWithReuseIdentifier:MonthInYearViewCellKey0];
    [self.collectionView registerClass:[MonthLayerCollectionViewCell class] forCellWithReuseIdentifier:MonthInYearViewCellKey1];
    [self.collectionView registerClass:[MonthLayerCollectionViewCell class] forCellWithReuseIdentifier:MonthInYearViewCellKey2];
    [self.collectionView registerClass:[MonthLayerCollectionViewCell class] forCellWithReuseIdentifier:MonthInYearViewCellKey3];
    [self.collectionView registerClass:[MonthLayerCollectionViewCell class] forCellWithReuseIdentifier:MonthInYearViewCellKey4];
    [self.collectionView registerClass:[MonthLayerCollectionViewCell class] forCellWithReuseIdentifier:MonthInYearViewCellKey5];
    [self.collectionView registerClass:[MonthLayerCollectionViewCell class] forCellWithReuseIdentifier:MonthInYearViewCellKey6];
    
    [self.collectionView registerClass:[YearSectionHeaderReusableView class]forSupplementaryViewOfKind:UICollectionElementKindSectionHeader withReuseIdentifier:headerReuseIdentifier];
    
    self.collectionView.opaque = true;
    self.collectionView.backgroundColor = [UIColor whiteColor];
    self.collectionView.showsVerticalScrollIndicator = false;
    
    self.itemSize = [self itemSizeForBounds:self.collectionView.bounds.size];
    [self setBarButtonItems];
    
    self.navigationController.delegate = self;
}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [self.navigationItem setTitle:nil];
    
    NSInteger currentYearSection = _currentYear - _startYear;
    ((CalendarPickerController*) self.navigationController).topToolbar.hidden = true;
    [self.collectionView scrollToItemAtIndexPath:[NSIndexPath indexPathForItem:5 inSection:currentYearSection] atScrollPosition:UICollectionViewScrollPositionCenteredVertically animated:false];
}

- (CGSize)itemSizeForBounds:(CGSize)bounds {
    
    int numberOfColumns;
    if (bounds.width > bounds.height) {
        numberOfColumns = 6;
    } else {
        numberOfColumns = 3;
    }
    
    CGFloat minimumInteritemSpacing = ((UICollectionViewFlowLayout *) self.collectionView.collectionViewLayout).minimumInteritemSpacing;
    CGFloat itemSide = ceil((bounds.width - 40) / numberOfColumns - minimumInteritemSpacing);
    return CGSizeMake(itemSide, itemSide + numberOfColumns * 8);
}

- (BOOL)shouldInvalidateLayoutForBoundsChange:(CGRect)newBounds {
    return YES;
}

- (void)viewWillTransitionToSize:(CGSize)size withTransitionCoordinator:(id<UIViewControllerTransitionCoordinator>)coordinator {
    [super viewWillTransitionToSize:size
          withTransitionCoordinator:coordinator];
    
    self.itemSize = [self itemSizeForBounds:size];
    
    [coordinator animateAlongsideTransition:^(id<UIViewControllerTransitionCoordinatorContext> context) {
        [self.collectionView.collectionViewLayout invalidateLayout];
    } completion:^(id<UIViewControllerTransitionCoordinatorContext> context) { }];
}

- (void)setBarButtonItems {
    UIBarButtonItem *leftBarButtonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemCancel target:self action:@selector(cancel:)];
    
    self.navigationItem.leftBarButtonItem = leftBarButtonItem;
}

- (void)cancel:(UIBarButtonItem*)sender {
    [[self presentingViewController]dismissViewControllerAnimated:true completion:nil];
}

#pragma mark <UICollectionViewDataSource>

- (NSInteger)numberOfSectionsInCollectionView:(UICollectionView *)collectionView {
    NSInteger numberOfSections = _endYear - _startYear + 1;
    
    static NSDateFormatter *dateFormat;
    static NSDate *referenceDate;
    static dispatch_once_t onceToken;
    static NSString * const stringDateFormat = @"yyyy-MM-dd";
    static NSString * const dateStrFormat = @"%d-%d-01";
    
    dispatch_once(&onceToken, ^{
        referenceDate = [NSDate dateWithTimeIntervalSinceReferenceDate:0];
        dateFormat = [[NSDateFormatter alloc] init];
        [dateFormat setDateFormat:stringDateFormat];
        [dateFormat setTimeZone:[NSTimeZone timeZoneWithAbbreviation:@"GMT"]];
    });
    for (NSInteger section = 0; section < numberOfSections; section++) {
        for (int i = 0; i < 12; i++) {
            NSInteger currentCellYear = _startYear + section;
            NSString *dateStr = [NSString stringWithFormat:dateStrFormat, currentCellYear, i + 1];
            NSDate *currentCellDate = [dateFormat dateFromString:dateStr];
            NSRange range = [_calendar rangeOfUnit:NSCalendarUnitDay inUnit:NSCalendarUnitMonth forDate:currentCellDate];
            NSDateComponents *comps = [_calendar components:NSWeekdayCalendarUnit fromDate:currentCellDate];
            
           // NSIndexPath *indexPathForDictionary = [NSIndexPath indexPathForItem:i inSection:section];
            int startWeekDay = [comps weekday] - 1;
            
            if (!self.startDayIsSunday) { // TODO: setting start of week day for calendar
                startWeekDay -= 1;
                if (startWeekDay < 0) {
                    startWeekDay = 6;
                }
            }
            //
            NSArray *cellData = @[[NSNumber numberWithUnsignedInteger:range.length] , [NSNumber numberWithInt:startWeekDay]];
            [_cellDaysDataByIndexPath setValue:cellData forKey:[NSString stringWithFormat:@"%lu-%lu", (unsigned long)i, (unsigned long)section]];
            
        }
    }
    
    
    return numberOfSections;
}


- (NSInteger)collectionView:(UICollectionView *)collectionView numberOfItemsInSection:(NSInteger)section {
    return 12;
    
}

- (UICollectionViewCell *)collectionView:(UICollectionView *)collectionView cellForItemAtIndexPath:(NSIndexPath *)indexPath {
    NSArray *cellData = _cellDaysDataByIndexPath[[NSString stringWithFormat:@"%lu-%lu", (unsigned long)indexPath.item, (unsigned long)indexPath.section]];
    int startWeekDay = [[cellData objectAtIndex:1] integerValue];
    int numberOfDays = [[cellData objectAtIndex:0] integerValue];
    NSString *identifier = [MonthInYearViewCellKey stringByAppendingFormat:@"%d", startWeekDay];
    
    MonthLayerCollectionViewCell *cell = [collectionView dequeueReusableCellWithReuseIdentifier:identifier forIndexPath:indexPath];
    [cell setStartDay:startWeekDay];
    [cell setNumberOfDays:numberOfDays];
    cell.monthTitle.string = _monthNames[indexPath.item];
    
    return cell;
    
    
}

- (UIEdgeInsets)collectionView:(UICollectionView *)collectionView layout:(nonnull UICollectionViewLayout *)collectionViewLayout insetForSectionAtIndex:(NSInteger)section {
    static UIEdgeInsets insets;
    static dispatch_once_t onceToken;
    
    dispatch_once(&onceToken, ^{
        CGFloat minimumLineSpacing = [(UICollectionViewFlowLayout *)collectionViewLayout minimumLineSpacing];
        insets = UIEdgeInsetsMake(minimumLineSpacing, 20, minimumLineSpacing, 20);
    });
    return insets;
    
}

-(UICollectionReusableView *)collectionView:(UICollectionView *)collectionView viewForSupplementaryElementOfKind:(nonnull NSString *)kind atIndexPath:(nonnull NSIndexPath *)indexPath {
    
    YearSectionHeaderReusableView *header;
    
    if (kind == UICollectionElementKindSectionHeader) {
        
        header = [collectionView dequeueReusableSupplementaryViewOfKind:kind
                                                    withReuseIdentifier:headerReuseIdentifier
                                                           forIndexPath:indexPath];
        
        NSInteger currentSectionYear = _startYear + indexPath.section;
        header.yearTitle.string = [NSString stringWithFormat:@"%ld", (long) currentSectionYear];
    }
    
    
    return header;
}

-(CGSize)collectionView:(UICollectionView *)collectionView layout:(nonnull UICollectionViewLayout *)collectionViewLayout referenceSizeForHeaderInSection:(NSInteger)section {
    static CGSize headerSize;
    static dispatch_once_t onceToken;
    
    dispatch_once(&onceToken, ^{
        headerSize = CGSizeMake(ceil(self.view.frame.size.width), 44);
    });
    
    return headerSize;
    
}

- (CGSize)collectionView:(UICollectionView *)collectionView layout:(UICollectionViewLayout *)collectionViewLayout
  sizeForItemAtIndexPath:(NSIndexPath *)atIndexPath {
    return _itemSize;
}

#pragma mark <UICollectionViewDelegate>

- (void)collectionView:(UICollectionView *)collectionView didSelectItemAtIndexPath:(NSIndexPath *)indexPath {
    
    NSInteger currentSectionYear = _startYear + indexPath.section;
    [self.navigationItem setTitle:[NSString stringWithFormat:@"%d", currentSectionYear]];
    
    self.viewForZooming = [collectionView cellForItemAtIndexPath:indexPath];
    
    MonthViewController *monthViewController = [MonthViewController defaultControllerWithYear:currentSectionYear andMonth:indexPath.item + 1 withStartDayIsSunday:self.startDayIsSunday];
    monthViewController.calendar = self.calendar;
    [monthViewController.collectionView scrollToItemAtIndexPath:[NSIndexPath indexPathForItem:0 inSection:2] atScrollPosition:UICollectionViewScrollPositionBottom animated:false];
    
    
    
    [self.navigationController pushViewController:monthViewController animated:true];
}

- (id <UIViewControllerAnimatedTransitioning>)navigationController:(UINavigationController *)navigationController
                                   animationControllerForOperation:(UINavigationControllerOperation)operation
                                                fromViewController:(UIViewController *)fromVC
                                                  toViewController:(UIViewController *)toVC {
    
    // Determine if we're presenting or dismissing
    ZOTransitionType type = (fromVC == self) ? ZOTransitionTypePresenting : ZOTransitionTypeDismissing;
    
    // Create a transition instance with the selected cell's imageView as the target view
    ZOZolaZoomTransition *zoomTransition = [ZOZolaZoomTransition transitionFromView:self.viewForZooming
                                                                               type:type
                                                                           duration:0.3
                                                                           delegate:self];
    
    return zoomTransition;
}

- (CGRect)zolaZoomTransition:(ZOZolaZoomTransition *)zoomTransition
        startingFrameForView:(UIView *)targetView
              relativeToView:(UIView *)relativeView
          fromViewController:(UIViewController *)fromViewController
            toViewController:(UIViewController *)toViewController {
    
    if (fromViewController == self) {
        // We're pushing to the detail controller
        CGRect newFrame = [self.viewForZooming convertRect:self.viewForZooming.bounds toView:relativeView];
        return newFrame;
    } else if ([fromViewController isKindOfClass:[MonthViewController class]]) {
        // We're popping back to this master controller
        MonthViewController *detailController = (MonthViewController *)fromViewController;
        CGRect newFrame = [detailController.view convertRect:detailController.view.bounds toView:relativeView];
        return newFrame;
    }
    
    return CGRectZero;
}

- (CGRect)zolaZoomTransition:(ZOZolaZoomTransition *)zoomTransition
       finishingFrameForView:(UIView *)targetView
              relativeToView:(UIView *)relativeView
          fromViewController:(UIViewController *)fromViewComtroller
            toViewController:(UIViewController *)toViewController {
    
    if (fromViewComtroller == self) {
        // We're pushing to the detail controller
        MonthViewController *detailController = (MonthViewController *)toViewController;
        
        CGFloat targetWidth =  detailController.view.bounds.size.width;
        CGFloat targetHeight = (targetWidth / self.viewForZooming.bounds.size.width) * self.viewForZooming.bounds.size.height;
        CGFloat targetY = (detailController.view.bounds.size.height - targetHeight) / 2;
        
        CGRect newFrame = CGRectMake(0, targetY, targetWidth, targetHeight);// = [detailController.view convertRect:detailController.view.bounds toView:relativeView];
        return newFrame;
    } else if ([fromViewComtroller isKindOfClass:[MonthViewController class]]) {
        // We're popping back to this master controller.
        CGRect newFrame = [self.viewForZooming convertRect:self.viewForZooming.bounds toView:relativeView];
        return newFrame;
    }
    
    return CGRectZero;
}

@end

@implementation YearSectionHeaderReusableView
- (instancetype)initWithFrame:(CGRect)frame {
    self = [super initWithFrame:frame];
    if (self) {
        [self setOpaque:true];
        [self setBackgroundColor:[UIColor whiteColor]];
        
        _yearTitle = [[CATextLayer alloc] init];
        
        CGFontRef cgFont = CGFontCreateWithFontName((CFStringRef)[UIFont systemFontOfSize:22 weight:UIFontWeightThin].fontName);
        [_yearTitle setFont:cgFont];
        
        [_yearTitle setFontSize:29];
        [_yearTitle setAlignmentMode:kCAAlignmentLeft];
        [_yearTitle setForegroundColor:[[UIColor blackColor] CGColor]];
        _yearTitle.contentsScale = [UIScreen mainScreen].scale;
        
        [self.layer addSublayer:_yearTitle];
        _bottomLine = [[CALayer alloc]init];
        [_bottomLine setBackgroundColor:[[UIColor groupTableViewBackgroundColor] CGColor]];
        [self.layer addSublayer:_bottomLine];
        
    }
    return self;
}

- (void)layoutSubviews {
    CGSize size = self.bounds.size;
    _yearTitle.frame = CGRectMake(20, 7, size.width - 40, size.height - 10);
    _bottomLine.frame = CGRectMake(20, size.height - 1, size.width - 40, 1);
}

@end

@implementation DayInYearViewCell

- (instancetype)initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
    if (self) {
        self.opaque = true;
        self.contentView.opaque = true;
        self.autoresizesSubviews = false;
        self.contentView.translatesAutoresizingMaskIntoConstraints = false;
        self.contentView.clearsContextBeforeDrawing = false;
        
        self.topLine = [[CALayer alloc]init];
        [self.contentView.layer addSublayer:self.topLine];
        
        [self.topLine setBackgroundColor:[[UIColor groupTableViewBackgroundColor] CGColor]];
        
        self.dayNumber = [[LCTextLayer alloc] init];
        [self.contentView.layer addSublayer:self.dayNumber];
        CGFontRef cgFont = CGFontCreateWithFontName((CFStringRef)[UIFont systemFontOfSize:17 weight:UIFontWeightRegular].fontName);
        [self.dayNumber setFont:cgFont];
        [self.dayNumber setFontSize:17];
        [self.dayNumber setAlignmentMode:kCAAlignmentCenter];
        
        [self.dayNumber setForegroundColor:[[UIColor blackColor] CGColor]];
        self.dayNumber.contentsScale = [UIScreen mainScreen].scale;
    }
    return self;
}

- (void)layoutSubviews {
    CGSize size = self.bounds.size;
    
    [self.dayNumber setFrame:CGRectMake(size.width / 4, size.height / 3, size.width / 2, size.height / 2)];
    self.topLine.frame = CGRectMake(0, 0, size.width, 1);
}

- (void)prepareForReuse {
    self.dayNumber.string = nil;
    [self.contentView setBackgroundColor:[UIColor whiteColor]];
}

@end

@implementation MonthSectionHeaderReusableView


- (id)initWithFrame:(CGRect)aRect {
    self = [super initWithFrame:(CGRect)aRect];
    if (self) {
        self.opaque = true;
        self.autoresizesSubviews = false;
        self.translatesAutoresizingMaskIntoConstraints = false;
        self.clearsContextBeforeDrawing = false;
        
        self.monthTitle = [[CATextLayer alloc] init];
        //        [self.monthTitle setFont:@"Helvetica-Light"];
        [self.monthTitle setFontSize:17];
        [self.monthTitle setAlignmentMode:kCAAlignmentCenter];
        [self.monthTitle setForegroundColor:[[UIColor redColor] CGColor]];
        [self.monthTitle setBackgroundColor:[[UIColor whiteColor] CGColor]];
        self.monthTitle.contentsScale = [UIScreen mainScreen].scale;
        [self.layer addSublayer:self.monthTitle];
    }
    return self;
}

- (void)layoutSubviews {
    CGSize size = self.bounds.size;
    _monthTitle.frame = CGRectMake(_offset, 20,  _width, size.height - 20);
}

@end

@implementation ZOZolaZoomTransition

#pragma mark - Constructors

- (instancetype)initWithTargetView:(UIView *)targetView
                              type:(ZOTransitionType)type
                          duration:(NSTimeInterval)duration
                          delegate:(id<ZOZolaZoomTransitionDelegate>)delegate {
    
    self = [super init];
    if (self) {
        self.targetView = targetView;
        self.type = type;
        self.duration = duration;
        self.delegate = delegate;
        self.fadeColor = [UIColor whiteColor];
    }
    return self;
}

+ (instancetype)transitionFromView:(UIView *)targetView
                              type:(ZOTransitionType)type
                          duration:(NSTimeInterval)duration
                          delegate:(id<ZOZolaZoomTransitionDelegate>)delegate {
    
    return [[[self class] alloc] initWithTargetView:targetView
                                               type:type
                                           duration:duration
                                           delegate:delegate];
}

- (instancetype)init NS_UNAVAILABLE {
    return nil;
}

#pragma mark - UIViewControllerAnimatedTransitioning Methods

- (void)animateTransition:(id <UIViewControllerContextTransitioning>)transitionContext {
    
#if !defined(ZO_APP_EXTENSIONS)
    [[UIApplication sharedApplication] beginIgnoringInteractionEvents];
#endif
    
    UIView *containerView = [transitionContext containerView];
    UIViewController *fromViewController = [transitionContext viewControllerForKey:UITransitionContextFromViewControllerKey];
    UIViewController *toViewController = [transitionContext viewControllerForKey:UITransitionContextToViewControllerKey];
    
    // iOS7 and iOS8+ have different ways of obtaining the view from the view controller.
    // Here we're taking care of that inconsistency upfront, so we don't have to deal with
    // it later.
    UIView *fromControllerView = nil;
    UIView *toControllerView = nil;
    if ([transitionContext respondsToSelector:@selector(viewForKey:)]) {
        // iOS8+
        fromControllerView = [transitionContext viewForKey:UITransitionContextFromViewKey];
        toControllerView = [transitionContext viewForKey:UITransitionContextToViewKey];
    } else {
        // iOS7
        fromControllerView = fromViewController.view;
        toControllerView = toViewController.view;
    }
    
    // Setup a background view to prevent content from peeking through while our
    // animation is in progress
    UIView *backgroundView = [[UIView alloc] initWithFrame:containerView.bounds];
    backgroundView.backgroundColor = _fadeColor;
    //    [containerView addSubview:backgroundView];
    
    if (_type == ZOTransitionTypePresenting) {
        // Make sure the "to view" has been laid out if we're presenting. This needs
        // to be done before we ask the delegate for frames.
        [toControllerView setNeedsLayout];
        [toControllerView layoutIfNeeded];
    }
    
    // Ask the delegate for the target view's starting frame
    CGRect startFrame = [_delegate zolaZoomTransition:self
                                 startingFrameForView:_targetView
                                       relativeToView:fromControllerView
                                   fromViewController:fromViewController
                                     toViewController:toViewController];
    
    // Ask the delegate for the target view's finishing frame
    CGRect finishFrame = [_delegate zolaZoomTransition:self
                                 finishingFrameForView:_targetView
                                        relativeToView:toControllerView
                                    fromViewController:fromViewController
                                      toViewController:toViewController];
    
    if (_type == ZOTransitionTypePresenting) {
        // The "from" snapshot
#if TARGET_IPHONE_SIMULATOR
        UIView *fromControllerSnapshot = [[UIImageView alloc] initWithImage:[fromControllerView zo_snapshot]];
#else
        UIView *fromControllerSnapshot = [fromControllerView snapshotViewAfterScreenUpdates:NO];
#endif
        
        // The fade view will sit between the "from" snapshot and the target snapshot.
        // This is what is used to create the fade effect.
        UIView *fadeView = [[UIView alloc] initWithFrame:containerView.bounds];
        fadeView.backgroundColor = _fadeColor;
        fadeView.alpha = 0.0;
        
        // The star of the show
#if TARGET_IPHONE_SIMULATOR
        UIView *targetSnapshot = [[UIImageView alloc] initWithImage:[_targetView zo_snapshot]];
#else
        UIView *targetSnapshot = [_targetView snapshotViewAfterScreenUpdates:NO];
#endif
        targetSnapshot.frame = startFrame;
        
        // Check if the delegate provides any supplementary views
        NSArray *supplementaryViews = nil;
        if ([_delegate respondsToSelector:@selector(supplementaryViewsForZolaZoomTransition:)]) {
            NSAssert([_delegate respondsToSelector:@selector(zolaZoomTransition:frameForSupplementaryView:relativeToView:)], @"supplementaryViewsForZolaZoomTransition: requires zolaZoomTransition:frameForSupplementaryView:relativeToView: to be implemented by the delegate. Implement zolaZoomTransition:frameForSupplementaryView:relativeToView: and try again.");
            supplementaryViews = [_delegate supplementaryViewsForZolaZoomTransition:self];
        }
        
        // All supplementary snapshots are added to a container, and then the same transform
        // that we're going to apply to the "from" controller snapshot will be applied to the
        // supplementary container
        UIView *supplementaryContainer = [[UIView alloc] initWithFrame:containerView.bounds];
        supplementaryContainer.backgroundColor = [UIColor clearColor];
        for (UIView *supplementaryView in supplementaryViews) {
#if TARGET_IPHONE_SIMULATOR
            UIView *supplementarySnapshot = [[UIImageView alloc] initWithImage:[supplementaryView zo_snapshot]];
#else
            UIView *supplementarySnapshot = [supplementaryView snapshotViewAfterScreenUpdates:YES];
#endif
            
            supplementarySnapshot.frame = [_delegate zolaZoomTransition:self
                                              frameForSupplementaryView:supplementaryView
                                                         relativeToView:fromControllerView];
            
            [supplementaryContainer addSubview:supplementarySnapshot];
        }
        
        // Assemble the hierarchy in the container
        [containerView addSubview:fromControllerSnapshot];
        //        [containerView addSubview:fadeView];
        [containerView addSubview:targetSnapshot];
        [containerView addSubview:supplementaryContainer];
        
        // Determine how much we need to scale
        CGFloat scaleFactor = finishFrame.size.width / startFrame.size.width;
        
        // Calculate the ending origin point for the "from" snapshot taking into account the scale transformation
        CGPoint endPoint = CGPointMake((-startFrame.origin.x * scaleFactor) + finishFrame.origin.x, (-startFrame.origin.y * scaleFactor) + finishFrame.origin.y);
        
        // Animate presentation
        [UIView animateWithDuration:[self transitionDuration:transitionContext]
                              delay:0.0
                            options:UIViewAnimationOptionCurveEaseInOut
                         animations:^{
                             // Transform and move the "from" snapshot
                             fromControllerSnapshot.transform = CGAffineTransformMakeScale(scaleFactor, scaleFactor);
                             if (!isnan(endPoint.x) && !isnan(endPoint.y)) {
                                 fromControllerSnapshot.frame = CGRectMake(endPoint.x, endPoint.y, fromControllerSnapshot.frame.size.width, fromControllerSnapshot.frame.size.height);
                                 
                                 // Transform and move the supplementary container with the "from" snapshot
                                 supplementaryContainer.transform = fromControllerSnapshot.transform;
                                 supplementaryContainer.frame = fromControllerSnapshot.frame;
                                 
                                 // Fade
                                 //                                 fadeView.alpha = 1.0;
                                 supplementaryContainer.alpha = 0.0;
                                 
                                 // Move our target snapshot into position
                                 targetSnapshot.frame = finishFrame;
                             }
                             supplementaryContainer.alpha = 0.0;
                         } completion:^(BOOL finished) {
                             // Add "to" controller view
                             [containerView addSubview:toControllerView];
                             
                             // Cleanup our animation views
                             [backgroundView removeFromSuperview];
                             [fromControllerSnapshot removeFromSuperview];
                             [fadeView removeFromSuperview];
                             [targetSnapshot removeFromSuperview];
                             
#if !defined(ZO_APP_EXTENSIONS)
                             [[UIApplication sharedApplication] endIgnoringInteractionEvents];
#endif
                             
                             [transitionContext completeTransition:finished];
                         }];
    } else {
        // Since the "to" controller isn't currently part of the view hierarchy, we need to use the
        // old snapshot API
        UIImageView *toControllerSnapshot = [[UIImageView alloc] initWithImage:[toControllerView zo_snapshot]];
        
        // Used to perform the fade, just like when presenting
        UIView *fadeView = [[UIView alloc] initWithFrame:containerView.bounds];
        fadeView.backgroundColor = _fadeColor;
        fadeView.alpha = 1.0;
        
        // The star of the show again, this time with the old snapshot API
        UIImageView *targetSnapshot = [[UIImageView alloc] initWithImage:[_targetView zo_snapshot]];
        targetSnapshot.frame = startFrame;
        
        // Check if the delegate provides any supplementary views
        NSArray *supplementaryViews = nil;
        if ([_delegate respondsToSelector:@selector(supplementaryViewsForZolaZoomTransition:)]) {
            NSAssert([_delegate respondsToSelector:@selector(zolaZoomTransition:frameForSupplementaryView:relativeToView:)], @"supplementaryViewsForZolaZoomTransition: requires zolaZoomTransition:frameForSupplementaryView:relativeToView: to be implemented by the delegate. Implement zolaZoomTransition:frameForSupplementaryView:relativeToView: and try again.");
            supplementaryViews = [_delegate supplementaryViewsForZolaZoomTransition:self];
        }
        
        // Same as for presentation, except this time with the old snapshot API
        UIView *supplementaryContainer = [[UIView alloc] initWithFrame:containerView.bounds];
        supplementaryContainer.backgroundColor = [UIColor clearColor];
        for (UIView *supplementaryView in supplementaryViews) {
            UIImageView *supplementarySnapshot = [[UIImageView alloc] initWithImage:[supplementaryView zo_snapshot]];
            
            supplementarySnapshot.frame = [_delegate zolaZoomTransition:self
                                              frameForSupplementaryView:supplementaryView
                                                         relativeToView:toControllerView];
            
            [supplementaryContainer addSubview:supplementarySnapshot];
        }
        
        // We're switching the values such that the scale factor returns the same result
        // as when we were presenting
        CGFloat scaleFactor = startFrame.size.width / finishFrame.size.width;
        
        // This is also the same equation used when presenting and will result in the same point,
        // except this time it's the start point for the animation
        CGPoint startPoint = CGPointMake((-finishFrame.origin.x * scaleFactor) + startFrame.origin.x, (-finishFrame.origin.y * scaleFactor) + startFrame.origin.y);
        
        // Apply the transformation and set the origin before the animation begins
        toControllerSnapshot.transform = CGAffineTransformMakeScale(scaleFactor, scaleFactor);
        if (!isnan(startPoint.x) && !isnan(startPoint.y)) {
            toControllerSnapshot.frame = CGRectMake(startPoint.x, startPoint.y, toControllerSnapshot.frame.size.width, toControllerSnapshot.frame.size.height);
        }
        
        // Apply the same transform and starting position to the supplementary container
        //        supplementaryContainer.transform = toControllerSnapshot.transform;
        //        supplementaryContainer.frame = toControllerSnapshot.frame;
        //        supplementaryContainer.alpha = 0.0;
        //
        // Assemble the view hierarchy in the container
        [containerView addSubview:toControllerSnapshot];
        //        [containerView addSubview:fadeView];
        [containerView addSubview:targetSnapshot];
        //        [containerView addSubview:supplementaryContainer];
        
        // Animate dismissal
        [UIView animateWithDuration:[self transitionDuration:transitionContext]
                              delay:0.0
                            options:UIViewAnimationOptionCurveEaseInOut
                         animations:^{
                             // Put the "to" snapshot back to it's original state
                             toControllerSnapshot.transform = CGAffineTransformIdentity;
                             toControllerSnapshot.frame = toControllerView.frame;
                             
                             //                             // Transform and move the supplementary container with the "to" snapshot
                             //                             supplementaryContainer.transform = toControllerSnapshot.transform;
                             //                             supplementaryContainer.frame = toControllerSnapshot.frame;
                             //
                             //                             // Fade
                             ////                             fadeView.alpha = 0.0;
                             //                             supplementaryContainer.alpha = 1.0;
                             
                             // Move the target snapshot into place
                             targetSnapshot.frame = finishFrame;
                         } completion:^(BOOL finished) {
                             // Add "to" controller view
                             [containerView addSubview:toControllerView];
                             
                             // Cleanup our animation views
                             [backgroundView removeFromSuperview];
                             [toControllerSnapshot removeFromSuperview];
                             //                             [fadeView removeFromSuperview];
                             [targetSnapshot removeFromSuperview];
                             
#if !defined(ZO_APP_EXTENSIONS)
                             [[UIApplication sharedApplication] endIgnoringInteractionEvents];
#endif
                             
                             [transitionContext completeTransition:finished];
                         }];
    }
}

- (NSTimeInterval)transitionDuration:(id <UIViewControllerContextTransitioning>)transitionContext {
    return _duration;
}

@end

@implementation LCTextLayer
- (void)drawInContext:(CGContextRef)ctx {
    CGFloat height, fontSize, yDiff;
    
    height = self.bounds.size.height;
    fontSize = self.fontSize;
    yDiff = (height-fontSize)/2 - fontSize/10;
    
    CGContextSaveGState(ctx);
    CGContextTranslateCTM(ctx, 0.0, yDiff);
    [super drawInContext:ctx];
    CGContextRestoreGState(ctx);
}
@end

#endif


extern "C" {
    
    
    //--------------------------------------
    //  Date Time Picker
    //--------------------------------------
    
    void _ISN_ShowDP(int mode) {
#if !TARGET_OS_TV
        [[ISN_NativeUtility sharedInstance] DP_show:mode date:nil];
#endif
    }
    
    void _ISN_ShowDPWithTime(int mode, double seconds) {
#if !TARGET_OS_TV
        NSTimeInterval _interval = seconds;
        NSDate *date = [NSDate dateWithTimeIntervalSince1970:_interval];
        [[ISN_NativeUtility sharedInstance] DP_show:mode date:date];
#endif
    }
    
    //--------------------------------------
    //  IOS Native Utility
    //--------------------------------------

    void _ISN_CopyToClipboard(char *text) {
        
        NSString *textToCopy = [ISN_DataConvertor charToNSString:text];
        UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
        pasteboard.string = textToCopy;
    }
    
    void _ISN_SetApplicationBagesNumber(int count) {
        [[ISN_NativeUtility sharedInstance] setApplicationBagesNumber:count];
    }
    
    
    void _ISN_RedirectToAppStoreRatingPage(char* appId) {
        [[ISN_NativeUtility sharedInstance] redirectToRatingPage: [ISN_DataConvertor charToNSString:appId ]];
    }
    
    
    void _ISN_ShowPreloader() {
        [[ISN_NativeUtility sharedInstance] ShowSpinner];
    }
    
    
    void _ISN_HidePreloader() {
        [[ISN_NativeUtility sharedInstance] HideSpinner];
    }
    
    
    void _ISN_GetLocale() {
        [[ISN_NativeUtility sharedInstance] GetLocale];
    }
    
    void _ISN_SetLogState(bool state) {
        [[ISN_NativeUtility sharedInstance] ISN_SetLogState:(state)];
    }
    
    void _ISN_NativeLog(char* message) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: [ISN_DataConvertor charToNSString:message]];
    }
    
    void _ISN_RequestGuidedAccessSession(bool enable) {
        UIAccessibilityRequestGuidedAccessSession(enable, ^(BOOL didSucceed) {
            if(didSucceed) {
                UnitySendMessage("IOSNativeUtility", "OnGuidedAccessSessionRequestResult", "true");
            } else {
                UnitySendMessage("IOSNativeUtility", "OnGuidedAccessSessionRequestResult", "false");
            }
        });
    }
    
    bool _ISN_IsGuidedAccessEnabled() {
        
        return UIAccessibilityIsGuidedAccessEnabled;
    }
    
    
    
    
    
    
    
    bool _ISN_isAppStoreReceiptSandbox() {
#if TARGET_IPHONE_SIMULATOR
        return NO;
#else
        NSURL *appStoreReceiptURL = NSBundle.mainBundle.appStoreReceiptURL;
        NSString *appStoreReceiptLastComponent = appStoreReceiptURL.lastPathComponent;
        
        BOOL isSandboxReceipt = [appStoreReceiptLastComponent isEqualToString:@"sandboxReceipt"];
        return isSandboxReceipt;
#endif
    }
    
    bool _ISN_hasEmbeddedMobileProvision() {
        
        if ([[NSBundle mainBundle] pathForResource:@"embedded" ofType:@"mobileprovision"]) {
            return true;
        } else {
            return false;
        }
    }
    
    
    bool _ISN_isRunningInAppStoreEnvironment() {
#if TARGET_IPHONE_SIMULATOR
        return NO;
#else
        if (_ISN_isAppStoreReceiptSandbox() || _ISN_hasEmbeddedMobileProvision()) {
            return NO;
        }
        return YES;
#endif
    }
    
    
    BOOL _ISN_isRunningInTestFlightEnvironment() {
#if TARGET_IPHONE_SIMULATOR
        return NO;
#else
        if (_ISN_isAppStoreReceiptSandbox() && !_ISN_hasEmbeddedMobileProvision()) {
            return YES;
        }
        return NO;
#endif
    }
    
    
    bool _ISN_IsRunningTestFlightBeta() {
        if (_ISN_isRunningInTestFlightEnvironment()) {
            // TestFlight
            return true;
        } else {
            // App Store (and Apple reviewers too)
            return false;
        }
    }
    
    
    //------
    // Calendar
    //------
    
    void _ISN_PickDate(int startYear) {
        [[ISN_NativeUtility sharedInstance] pickDate:startYear];
    }
    
    
    // Helper method to create C string copy
    char* ISN_MakeStringCopy (const char* string)
    {
        if (string == NULL)
            return NULL;
        
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }
    
    
    
    const char* _ISN_BuildInfo() {
        NSMutableString * data = [[NSMutableString alloc] init];
        
        NSDictionary *infoDict = [[NSBundle mainBundle] infoDictionary];
        NSString *appVersion = [infoDict objectForKey:@"CFBundleShortVersionString"]; // example: 1.0.0
        NSString *buildNumber = [infoDict objectForKey:@"CFBundleVersion"]; // example: 42
        
        
        
        [data appendString:appVersion];
        [data appendString:@"|"];
        
        
        [data appendString: buildNumber];
        
        
        return ISN_MakeStringCopy([ISN_DataConvertor NSStringToChar:data]);
    }
    
    
    const char* _ISN_TimeZoneInfo() {
        
        NSMutableString * data = [[NSMutableString alloc] init];
        
        
        [data appendString:[[NSTimeZone localTimeZone] name]];
        [data appendString:@"|"];
        
        
        [data appendString: [NSString stringWithFormat:@"%ld",(long)[[NSTimeZone localTimeZone] secondsFromGMT]]];
        
        
        return ISN_MakeStringCopy([ISN_DataConvertor NSStringToChar:data]);
        
    }
    
    
    const char* _ISN_RetriveDeviceData() {
        
        
        
        
        NSMutableString * data = [[NSMutableString alloc] init];
        
        if([[UIDevice currentDevice] name] != nil) {
            [data appendString:[[UIDevice currentDevice] name]];
            
        } else {
            [data appendString:@""];
        }
        [data appendString:@"|"];
        
        
        if([[UIDevice currentDevice] systemName] != nil) {
            [data appendString:[[UIDevice currentDevice] systemName]];
            
        } else {
            [data appendString:@""];
        }
        [data appendString:@"|"];
        
        
        if([[UIDevice currentDevice] model] != nil) {
            [data appendString:[[UIDevice currentDevice] model]];
            
        } else {
            [data appendString:@""];
        }
        [data appendString:@"|"];
        
        
        
        if([[UIDevice currentDevice] localizedModel] != nil) {
            [data appendString:[[UIDevice currentDevice] localizedModel]];
            
        } else {
            [data appendString:@""];
        }
        [data appendString:@"|"];
        
        
        
        [data appendString:[[UIDevice currentDevice] systemVersion]];
        [data appendString:@"|"];
        
        [data appendString:[NSString stringWithFormat: @"%d", [ISN_NativeUtility majorIOSVersion]]];
        [data appendString:@"|"];
        
        
        int Idiom = -1;
        switch ([[UIDevice currentDevice] userInterfaceIdiom]) {
            case UIUserInterfaceIdiomPhone:
                Idiom = 0;
                break;
            case UIUserInterfaceIdiomPad:
                Idiom = 1;
                break;
                
            default:
                Idiom = -1;
                break;
        }
        
        [data appendString:[NSString stringWithFormat: @"%d", Idiom]];
        [data appendString:@"|"];
        
        
        NSUUID *vendorIdentifier = [[UIDevice currentDevice] identifierForVendor];
        uuid_t uuid;
        [vendorIdentifier getUUIDBytes:uuid];
        
        NSData *vendorData = [NSData dataWithBytes:uuid length:16];
        NSString *encodedString = [vendorData base64Encoding];
        [data appendString:encodedString];
        [data appendString:@"|"];
        
        
        
        NSString * language = [[NSLocale preferredLanguages] objectAtIndex:0];
        [data appendString:language];
        
        
#if UNITY_VERSION < 500
        [data autorelease];
#endif
        
        return ISN_MakeStringCopy([ISN_DataConvertor NSStringToChar:data]);
    }

    
    //--------------------------------------
    //  IOS Native iCloud Section
    //--------------------------------------
    

    
    void _ISN_SetString(char* key, char* val) {
        NSString* k = [ISN_DataConvertor charToNSString:key];
        NSString* v = [ISN_DataConvertor charToNSString:val];
        
        [[CloudManager sharedInstance] setString:v key:k];
    }
    
    
    void _ISN_SetDouble(char* key, float val) {
        NSString* k = [ISN_DataConvertor charToNSString:key];
        double v = (double) val;
        
        [[CloudManager sharedInstance] setDouble:v key:k];
    }
    
    void _ISN_SetData(char* key, char* data) {
        NSString* k = [ISN_DataConvertor charToNSString:key];
        
        NSString* mDataString = [ISN_DataConvertor charToNSString:data];
        NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
        
        [[CloudManager sharedInstance] setData:mData key:k];
        
    }
    
    
    void _ISN_RequestDataForKey(char* key) {
        NSString* k = [ISN_DataConvertor charToNSString:key];
        [[CloudManager sharedInstance] requestDataForKey:k];
    }
    
    
    //--------------------------------------
    //  IOS Native Shared App API Section
    //--------------------------------------
    
    
    
    BOOL _ISN_CheckUrl(char* url) {
        NSString *urlString = [ISN_DataConvertor charToNSString:url];
        NSURL *uri = [NSURL URLWithString:urlString];
        
        return [[UIApplication sharedApplication] canOpenURL:uri];
    }
    
    void _ISN_OpenUrl(char* url) {
        NSString *uri = [ISN_DataConvertor charToNSString:url];
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:uri]];
    }
    
    
    //--------------------------------------
    //  IOS Native PopUps API Section
    //--------------------------------------
    
    void _ISN_ShowRateUsPopUp(char* title, char* message, char* b1, char* b2, char* b3) {
        
        if([ISN_NativeUtility majorIOSVersion] >= 8) {
            [ISN_NativePopUpsManager showRateUsPopUp:[ISN_DataConvertor charToNSString:title] message:[ISN_DataConvertor charToNSString:message] b1:[ISN_DataConvertor charToNSString:b1] b2:[ISN_DataConvertor charToNSString:b2] b3:[ISN_DataConvertor charToNSString:b3]];
        } else {
            
#if !TARGET_OS_TV
            
            [ISN_NativePopUpsManager showRateUsPopUp_old:[ISN_DataConvertor charToNSString:title] message:[ISN_DataConvertor charToNSString:message] b1:[ISN_DataConvertor charToNSString:b1] b2:[ISN_DataConvertor charToNSString:b2] b3:[ISN_DataConvertor charToNSString:b3]];
            
#endif
        }
    }
    
    
    
    void _ISN_ShowDialog(char* title, char* message, char* yes, char* no) {
        if([ISN_NativeUtility majorIOSVersion] >= 8) {
            [ISN_NativePopUpsManager showDialog:[ISN_DataConvertor charToNSString:title] message:[ISN_DataConvertor charToNSString:message] yesTitle:[ISN_DataConvertor charToNSString:yes] noTitle:[ISN_DataConvertor charToNSString:no]];
        } else {
            
#if !TARGET_OS_TV
            
            [ISN_NativePopUpsManager showDialog_old:[ISN_DataConvertor charToNSString:title] message:[ISN_DataConvertor charToNSString:message] yesTitle:[ISN_DataConvertor charToNSString:yes] noTitle:[ISN_DataConvertor charToNSString:no]];
            
#endif
        }
        
    }
    
    void _ISN_ShowMessage(char* title, char* message, char* ok) {
        if([ISN_NativeUtility majorIOSVersion] >= 8) {
            [ISN_NativePopUpsManager showMessage:[ISN_DataConvertor charToNSString:title] message:[ISN_DataConvertor charToNSString:message] okTitle:[ISN_DataConvertor charToNSString:ok]];
        } else {
            
#if !TARGET_OS_TV
            
            [ISN_NativePopUpsManager showMessage_old:[ISN_DataConvertor charToNSString:title] message:[ISN_DataConvertor charToNSString:message] okTitle:[ISN_DataConvertor charToNSString:ok]];
#endif
        }
    }
    
    
    
    void _ISN_DismissCurrentAlert() {
        if([ISN_NativeUtility majorIOSVersion] >= 8) {
            [ISN_NativePopUpsManager dismissCurrentAlert];
        } else {
#if !TARGET_OS_TV
            [ISN_NativePopUpsManager dismissCurrentAlert_old];
#endif
        }
        
    }

    
    //--------------------------------------
    //  IOS Native Notifications API Section
    //--------------------------------------
    
    
    
    void _ISN_CancelNotifications() {
        [[IOSNativeNotificationCenter sharedInstance] cancelNotifications];
    }
    
    
    void _ISN_CancelNotificationById(char* nId) {
        NSString* alarmID = [ISN_DataConvertor charToNSString:nId];
        [[IOSNativeNotificationCenter sharedInstance] cleanUpLocalNotificationWithAlarmID:alarmID];
    }
    
    void  _ISN_RequestNotificationPermissions ()  {
        [[IOSNativeNotificationCenter sharedInstance] RegisterForNotifications];
    }
    
    
    void  _ISN_ScheduleNotification (int time, char* message, bool* sound, char* nId, int badges, char* data, char* soundName)  {
        NSString* alarmID = [ISN_DataConvertor charToNSString:nId];
        NSString* soundNameString = [ISN_DataConvertor charToNSString:soundName];
        [[IOSNativeNotificationCenter sharedInstance] scheduleNotification:time message:[ISN_DataConvertor charToNSString:message] sound:sound alarmID:alarmID badges:badges notificationData :[ISN_DataConvertor charToNSString:data] notificationSoundName:soundNameString];
    }
    
    
    
    void _ISN_ApplicationIconBadgeNumber (int badges) {
        [[IOSNativeNotificationCenter sharedInstance] applicationIconBadgeNumber:badges];
    }
    
    
    int  _ISN_CurrentNotificationSettings () {
#if !TARGET_OS_TV
        UIUserNotificationSettings* NotificationSettings = [[UIApplication sharedApplication] currentUserNotificationSettings];
        
        
        return NotificationSettings.types;
#else
        return 0;
#endif
    }
    
    
    //--------------------------------------
    //  Cache Section
    //--------------------------------------
    
    void _ISN_CacheSave(char* c_key, char* c_data) {
        
        NSString *KEY  = [ISN_DataConvertor charToNSString:c_key];
        NSString *DATA = [ISN_DataConvertor charToNSString:c_data];
        
        NSString *file = [[NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject] stringByAppendingPathComponent:KEY];
        [DATA writeToFile:file atomically:YES encoding:NSUTF8StringEncoding error:nil];
    }
    
    char* _ISN_CacheGet(char* c_key) {
        NSString *KEY  = [ISN_DataConvertor charToNSString:c_key];
        NSString *file = [[NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject] stringByAppendingPathComponent:KEY];
        
        NSString *Data = [NSString stringWithContentsOfFile:file encoding:NSUTF8StringEncoding error:nil];
        
        if(Data == nil) {
            Data = @"";
        }
        
        
        const char* string = [ISN_DataConvertor NSStringToChar:Data];
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }
    
    
    void _ISN_CacheRemove(char* c_key) {
        
        
        NSString *KEY  = [ISN_DataConvertor charToNSString:c_key];
        NSString *file = [[NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject] stringByAppendingPathComponent:KEY];
        
        
        NSFileManager *fm = [NSFileManager defaultManager];
        [fm removeItemAtPath:file error:nil];
        
    }
    
}








