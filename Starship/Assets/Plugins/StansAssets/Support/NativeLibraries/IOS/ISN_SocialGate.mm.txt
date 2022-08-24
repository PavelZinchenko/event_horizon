#if !TARGET_OS_TV

////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#import <Foundation/Foundation.h>
#import <Accounts/Accounts.h>
#import <Social/Social.h>
#import <MessageUI/MessageUI.h>

#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

#import "ISN_NativeCore.h"


@interface ISN_SocialGate : NSObject<MFMailComposeViewControllerDelegate, MFMessageComposeViewControllerDelegate>


@property (nonatomic, strong) UIDocumentInteractionController * documentInteractionController;

+ (id) sharedInstance;


-(void) SendTextMessage :(NSString*)body url: (NSArray*) recipients;

- (void) twitterPost:(NSString*)status url: (NSString*) url media: (NSString*) media;
- (void) fbPost:(NSString*)status  url: (NSString*) url media: (NSString*) media;



- (void) mediaShare:(NSString*)text media: (NSString*) media;
- (void) sendEmail:(NSString*)subject body: (NSString*) body recipients: (NSString*) recipients media: (NSString*) media;

- (void)whatsappShareText:(NSString *)msg;
- (void)whatsappShareImage:(NSString *)media;


@end




@implementation ISN_SocialGate

static ISN_SocialGate * cg_sharedInstance;


+ (id)sharedInstance {
    
    if (cg_sharedInstance == nil)  {
        cg_sharedInstance = [[self alloc] init];
    }
    
    return cg_sharedInstance;
}



-(void) SendTextMessage:(NSString *)body url:(NSArray *)recipients {
    
    MFMessageComposeViewController *controller = [[MFMessageComposeViewController alloc] init];
    
    if([MFMessageComposeViewController canSendText]) {
        controller.body = body;
        controller.recipients = recipients;
        controller.messageComposeDelegate = self;
        
        
        UIViewController *vc =  UnityGetGLViewController();
        [vc presentViewController:controller animated:YES completion:nil];
    } else {
        
        //not supported by device error code
        UnitySendMessage("IOSSocialManager", "OnTextMessageComposeResult", "3");
    }
    
}

- (void)messageComposeViewController:(MFMessageComposeViewController *)controller didFinishWithResult:(MessageComposeResult)result {
    [controller dismissViewControllerAnimated:YES completion:nil];
    
    UnitySendMessage("IOSSocialManager", "OnTextMessageComposeResult", [ISN_DataConvertor NSIntToChar:result]);
}



#define MMM_WHATSAPP_URL @"whatsapp://"
#define MMM_WHATSAPP_IMAGEFILENAME @"wa.wai"
#define MMM_WHATSAPP_IMAGEUTI @"net.whatsapp.image"

-(BOOL)whatsappInstalled{
    return [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:MMM_WHATSAPP_URL]];
}


-(void)whatsappShareText:(NSString *)msg {
    
    
    
    NSString * urlWhats = [NSString stringWithFormat:@"whatsapp://send?text=%@",msg];
    NSURL * whatsappURL = [NSURL URLWithString:[urlWhats stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding]];
    if ([[UIApplication sharedApplication] canOpenURL: whatsappURL]) {
        [[UIApplication sharedApplication] openURL: whatsappURL];
    } else {
        //Probably report the errror
    }
    
}

-(void)whatsappShareImage:(NSString *)media {
    
    NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
    UIImage *image = [[UIImage alloc] initWithData:imageData];
    
    NSString *filepath=[NSTemporaryDirectory() stringByAppendingPathComponent:MMM_WHATSAPP_IMAGEFILENAME];
    NSURL *fileURL = [NSURL fileURLWithPath:filepath];
    
    // save image to path..
    if([UIImagePNGRepresentation(image) writeToFile:filepath atomically:YES]){
        
        // setup a document interaction controller with our file ..
        UIDocumentInteractionController *dic = [self setupControllerWithURL:fileURL  usingDelegate:nil];
        self.documentInteractionController=dic;
        dic.UTI = MMM_WHATSAPP_IMAGEUTI;
        dic.name = MMM_WHATSAPP_IMAGEFILENAME;
        
        dic.annotation=@{@"message":@"Test Text",@"text":@"Test Text"};
        
        
        UIViewController *vc =  UnityGetGLViewController();
        
        [dic presentOpenInMenuFromRect:vc.view.bounds inView:vc.view animated:YES];
        
        // exit; we're not calling activityDidFinish here, but later in documentInteractionControllerDidDismissOpenInMenu.
        return;
    }
    
}

- (UIDocumentInteractionController *) setupControllerWithURL: (NSURL*) fileURL
                                               usingDelegate: (id <UIDocumentInteractionControllerDelegate>) interactionDelegate {
    
    UIDocumentInteractionController *interactionController =
    [UIDocumentInteractionController interactionControllerWithURL: fileURL];
    interactionController.delegate = interactionDelegate;
    
    return interactionController;
}


-(void) mediaShare:(NSString *)text  media:(NSString *)media {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: mediaShare"];
    UIActivityViewController *controller;
    
    
    if(media.length != 0) {
        NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
        UIImage *image = [[UIImage alloc] initWithData:imageData];
        
        //[UIPopoverPresentationController alloc] ini
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: image added"];
        if(text.length != 0) {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: text added"];
            controller = [[UIActivityViewController alloc] initWithActivityItems:@[text, image] applicationActivities:nil];
        } else {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: no text"];
            controller = [[UIActivityViewController alloc] initWithActivityItems:@[image] applicationActivities:nil];
        }
        
    } else {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: no media"];
        controller = [[UIActivityViewController alloc] initWithActivityItems:@[text] applicationActivities:nil];
    }
    
    
    
    
    UIViewController *vc =  UnityGetGLViewController();
    
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: iOS8 detected"];
        UIPopoverPresentationController *presentationController = [controller popoverPresentationController];
        presentationController.sourceView = vc.view;
    }
    
    [vc presentViewController:controller animated:YES completion:nil];
    
}

-(void) twitterPost:(NSString *)status url:(NSString *)url media:(NSString *)media {
    
    
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: twitterPost"];
    
    [SLComposeServiceViewController attemptRotationToDeviceOrientation];
    __weak SLComposeViewController *tweetSheet = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeTwitter];
    
    if(tweetSheet == NULL) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: SLServiceTypeTwitter not avaliable "];
        UnitySendMessage("IOSSocialManager", "OnTwitterPostFailed", [ISN_DataConvertor NSStringToChar:@""]);
        return;
    }
    
    
    if(status.length > 0) {
        [tweetSheet setInitialText:status];
    }
    
    if(media.length > 0) {
        NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
        UIImage *image = [[UIImage alloc] initWithData:imageData];
        [tweetSheet addImage:image];
    }
    
    if(url.length > 0) {
        NSURL *urlObject = [NSURL URLWithString:url];
        [tweetSheet addURL:urlObject];
    }
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:tweetSheet animated:YES completion:nil];
    
    tweetSheet.completionHandler = ^(SLComposeViewControllerResult result) {
        NSArray *vComp;
        switch(result) {
                //  This means the user cancelled without sending the Tweet
            case SLComposeViewControllerResultCancelled:
                vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
                if ([[vComp objectAtIndex:0] intValue] < 7) {
                    [tweetSheet dismissViewControllerAnimated:YES completion:nil];
                }
                
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Tweet message was cancelled"];
                UnitySendMessage("IOSSocialManager", "OnTwitterPostFailed", [ISN_DataConvertor NSStringToChar:@""]);
                break;
                //  This means the user hit 'Send'
            case SLComposeViewControllerResultDone:
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Done pressed successfully"];
                
                vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
                if ([[vComp objectAtIndex:0] intValue] < 7) {
                    [tweetSheet dismissViewControllerAnimated:YES completion:nil];
                }
                
                UnitySendMessage("IOSSocialManager", "OnTwitterPostSuccess", [ISN_DataConvertor NSStringToChar:@""]);
                break;
        }
    };
}

- (void)twitterPostGif:(NSString *)status url:(NSString*)gifPath {
    
    NSURL *url = [NSURL URLWithString:@"https://api.twitter.com/1.1/statuses/update_with_media.json"];
    NSMutableDictionary *paramater = [[NSMutableDictionary alloc] init];
    
    //set the parameter here. to see others acceptable parameters find it at twitter API here : http://bit.ly/Occe6R
    [paramater setObject:status forKey:@"status"];
    
    ACAccountStore *accountStore = [[ACAccountStore alloc] init];
    ACAccountType *accountType = [accountStore accountTypeWithAccountTypeIdentifier:ACAccountTypeIdentifierTwitter];
    
    [accountStore requestAccessToAccountsWithType:accountType options:nil completion:^(BOOL granted, NSError *error) {
        if (granted == YES) {
            
            NSArray *accountsArray = [accountStore accountsWithAccountType:accountType];
            
            if ([accountsArray count] > 0) {
                ACAccount *twitterAccount = [accountsArray lastObject];
                
                SLRequest *postRequest = [SLRequest requestForServiceType:SLServiceTypeTwitter requestMethod:SLRequestMethodPOST URL:url parameters:paramater];
                
                NSData *imageData = [[NSData alloc] initWithContentsOfFile:gifPath]; // GIF89a file
                
                
                [postRequest addMultipartData:imageData withName:@"media[]" type:@"image/gif" filename:@"animated.gif"];
                
                [postRequest setAccount:twitterAccount]; // or  postRequest.account = twitterAccount;
                
                [postRequest performRequestWithHandler:^(NSData *responseData, NSHTTPURLResponse *urlResponse, NSError *error) {
                    NSString *output = [NSString stringWithFormat:@"HTTP response status: %li", (long)[urlResponse statusCode]];
                    NSLog(@"output = %@",output);
                    dispatch_async(dispatch_get_main_queue(), ^{
                        
                    });
                    
                    if([urlResponse statusCode] == 200) {
                        UnitySendMessage("IOSSocialManager", "OnTwitterPostSuccess", [ISN_DataConvertor NSStringToChar:@""]);
                        
                    } else {
                        UnitySendMessage("IOSSocialManager", "OnTwitterPostFailed", [ISN_DataConvertor NSStringToChar:@""]);
                    }
                    
                }];
            }
            
        }
    }];
}




- (void) fbPost:(NSString *)status url:(NSString *)url media:(NSString *)media {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: fbPostWithMedia"];
    
    [SLComposeServiceViewController attemptRotationToDeviceOrientation];
    __weak SLComposeViewController *fbSheet = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeFacebook];
    if(fbSheet == NULL) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: SLServiceTypeFacebook not avaliable "];
        UnitySendMessage("IOSSocialManager", "OnFacebookPostFailed", [ISN_DataConvertor NSStringToChar:@""]);
        return;
    }
    
    
    if(status.length > 0) {
        [fbSheet setInitialText:status];
    }
    
    if(media.length > 0) {
        NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
        UIImage *image = [[UIImage alloc] initWithData:imageData];
        [fbSheet addImage:image];
    }
    
    if(url.length > 0) {
        NSURL *urlObject = [NSURL URLWithString:url];
        [fbSheet addURL:urlObject];
    }
    
    
    UIViewController *vc =  UnityGetGLViewController();
    
    [vc presentViewController:fbSheet animated:YES completion:nil];
    
    fbSheet.completionHandler = ^(SLComposeViewControllerResult result) {
        NSArray *vComp;
        switch(result) {
                
            case SLComposeViewControllerResultCancelled:
                
                vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
                if ([[vComp objectAtIndex:0] intValue] < 7) {
                    [fbSheet dismissViewControllerAnimated:YES completion:nil];
                }
                
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Tweet message was cancelled"];
                UnitySendMessage("IOSSocialManager", "OnFacebookPostFailed", [ISN_DataConvertor NSStringToChar:@""]);
                break;
                //  This means the user hit 'Send'
            case SLComposeViewControllerResultDone:
                
                vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
                if ([[vComp objectAtIndex:0] intValue] < 7) {
                    [fbSheet dismissViewControllerAnimated:YES completion:nil];
                }
                
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Done pressed successfully"];
                UnitySendMessage("IOSSocialManager", "OnFacebookPostSuccess", [ISN_DataConvertor NSStringToChar:@""]);
                break;
        }
        
    };
    
}



- (void) sendEmail:(NSString *)subject body:(NSString *)body recipients: (NSString*) recipients media:(NSString *)media {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: sendEmail"];
    
    
    //Create a string with HTML formatting for the email body
    NSMutableString *emailBody = [[NSMutableString alloc] initWithString:@"<html><body>"] ;
#if UNITY_VERSION < 500
    [emailBody retain];
#endif
    
    
    //Add some text to it however you want
    [emailBody appendString:@"<p>"];
    [emailBody appendString:body];
    [emailBody appendString:@"</p>"];
    
    
    /*
     if(media.length > 0) {
     // NSLog(@"media: %@",media);
     
     
     
     [emailBody appendString:[NSString stringWithFormat:@"<p><b><img src='data:image/png;base64,%@'></b></p>",media]];
     }
     */
    
    
    //close the HTML formatting
    [emailBody appendString:@"</body></html>"];
    // NSLog(@"emailBody: %@",emailBody);
    
    
    
    //Create the mail composer window
    MFMailComposeViewController *emailDialog = [[MFMailComposeViewController alloc] init];
    
    if(emailDialog == nil) {
        UnitySendMessage("IOSSocialManager", "OnMailFailed", [ISN_DataConvertor NSStringToChar:@""]);
        return;
    }
    
    emailDialog.mailComposeDelegate = self;
    [emailDialog setSubject:subject];
    [emailDialog setMessageBody:emailBody isHTML:YES];
    
    if(media.length > 0) {
        NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
        [emailDialog addAttachmentData:imageData mimeType:@"image/png" fileName:@"Attachment"];
    }
    
    
    NSArray *emails = [recipients componentsSeparatedByString:@","];
    
    [emailDialog setToRecipients:emails];
    
    
    UIViewController *vc =  UnityGetGLViewController();
    
    [vc presentViewController:emailDialog animated:YES completion:nil];
#if UNITY_VERSION < 500
    [emailDialog release];
    [emailBody release];
#endif
    
    
}


#pragma private

- (NSString*) photoFilePath {
    return [NSString stringWithFormat:@"%@/%@",[NSHomeDirectory() stringByAppendingPathComponent:@"Documents"],@"tempinstgramphoto.igo"];
}


- (void) mailComposeController:(MFMailComposeViewController *)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError *)error {
    switch (result)
    {
        case MFMailComposeResultCancelled:
            UnitySendMessage("IOSSocialManager", "OnMailFailed", [ISN_DataConvertor NSStringToChar:@""]);
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Mail cancelled"];
            break;
        case MFMailComposeResultSaved:
            UnitySendMessage("IOSSocialManager", "OnMailFailed", [ISN_DataConvertor NSStringToChar:@""]);
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Mail saved"];
            break;
        case MFMailComposeResultSent:
            UnitySendMessage("IOSSocialManager", "OnMailSuccess", [ISN_DataConvertor NSStringToChar:@""]);
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Mail sent"];
            break;
        case MFMailComposeResultFailed:
            UnitySendMessage("IOSSocialManager", "OnMailFailed", [ISN_DataConvertor NSStringToChar:@""]);
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: Mail sent failure: %@", [error localizedDescription]];
            break;
        default:
            UnitySendMessage("IOSSocialManager", "OnMailFailed", [ISN_DataConvertor NSStringToChar:@""]);
            break;
    }
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:NULL];
}
@end


@interface IOSInstaPlugin : NSObject<UIDocumentInteractionControllerDelegate>

+ (id) sharedInstance;

- (void) share:(NSString*)status media: (NSString*) media;


@end


@interface MGInstagram : NSObject <UIDocumentInteractionControllerDelegate>

extern NSString* const kInstagramAppURLString;
extern NSString* const kInstagramOnlyPhotoFileName;

//DEFAULT file name is kInstagramDefualtPhotoFileName
//DEFAULT file name is restricted to only the instagram app
//Make sure your photoFileName has a valid photo extension.
+ (void) setPhotoFileName:(NSString*)fileName;
+ (NSString*) photoFileName;

//checks to see if user has instagram installed on device
+ (BOOL) isAppInstalled;

//checks to see if image is large enough to be posted by instagram
//returns NO if image dimensions are under 612x612
//
//Technically the instagram allows for photos to be published under the size of 612x612
//BUT if you want nice quality pictures, I recommend checking the image size.
+ (BOOL) isImageCorrectSize:(UIImage*)image;

//post image to instagram by passing in the target image and
//the view in which the user will be presented with the instagram model
+ (void) postImage:(UIImage*)image inView:(UIView*)view;
//Same as above method but with the option for a photo caption
+ (void) postImage:(UIImage*)image withCaption:(NSString*)caption inView:(UIView*)view;
+ (void) postImage:(UIImage*)image withCaption:(NSString*)caption inView:(UIView*)view delegate:(id<UIDocumentInteractionControllerDelegate>)delegate;

@end


@interface IOSTwitterPlugin : NSObject

+ (id) sharedInstance;

- (void) initTwitterPlugin;
- (void) authificateUser;
- (void) loadUserData;
- (void) post:(NSString*)status;
- (void) postWithMedia:(NSString*)status media: (NSString*) media;


@end




@implementation IOSInstaPlugin

static IOSInstaPlugin *_sharedInstance;


+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}
-(void) share:(NSString *)status media:(NSString *)media {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Insta share"];
    
    NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
    UIImage *image = [[UIImage alloc] initWithData:imageData];
    
    
    
    if ([[[UIDevice currentDevice] systemVersion] floatValue] < 5.0) {
        float i = [[[UIDevice currentDevice] systemVersion] floatValue];
        NSString *str = [NSString stringWithFormat:@"We're sorry, but Instagram is not supported with your iOS %.1f version.", i];
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Message" message:str delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alert show];
        
        UnitySendMessage("IOSInstagramManager", "OnInstaPostFailed", [ISN_DataConvertor NSStringToChar:@"3"]);
        UnitySendMessage("IOSSocialManager", "OnInstaPostFailed", [ISN_DataConvertor NSStringToChar:@"3"]);
        
        
        
        
        
    } else {
        
        
        
        if ([MGInstagram isAppInstalled]) {
            UIViewController *vc =  UnityGetGLViewController();
            [MGInstagram postImage:image withCaption:status inView:vc.view delegate:self];
        } else {
            UnitySendMessage("IOSInstagramManager", "OnInstaPostFailed", [ISN_DataConvertor NSStringToChar:@"1"]);
            UnitySendMessage("IOSSocialManager", "OnInstaPostFailed", [ISN_DataConvertor NSStringToChar:@"1"]);
            
        }
        
    }
    
}


- (void)documentInteractionControllerDidDismissOpenInMenu:(UIDocumentInteractionController *)controller {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"documentInteractionControllerDidDismissOpenInMenu"];
    UnitySendMessage("IOSInstagramManager", "OnInstaPostFailed", [ISN_DataConvertor NSStringToChar:@"2"]);
    UnitySendMessage("IOSSocialManager", "OnInstaPostFailed", [ISN_DataConvertor NSStringToChar:@"2"]);
    
    
}


- (void) documentInteractionController: (UIDocumentInteractionController *) controller willBeginSendingToApplication: (NSString *) application {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"willBeginSendingToApplication"];
    UnitySendMessage("IOSInstagramManager", "OnInstaPostSuccess", [ISN_DataConvertor NSStringToChar:@""]);
    UnitySendMessage("IOSSocialManager", "OnInstaPostSuccess", [ISN_DataConvertor NSStringToChar:@""]);

     controller.delegate = nil;
}



@end



@interface MGInstagram () {
    UIDocumentInteractionController *documentInteractionController;
}

@property (nonatomic) NSString *photoFileName;

@end

@implementation MGInstagram

NSString* const kInstagramAppURLString = @"instagram://app";
NSString* const kInstagramOnlyPhotoFileName = @"tempinstgramphoto.igo";

+ (instancetype) sharedInstance
{
    static MGInstagram* sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[MGInstagram alloc] init];
    });
    return sharedInstance;
}

- (id) init {
    if (self = [super init]) {
        self.photoFileName = kInstagramOnlyPhotoFileName;
    }
    return self;
}

+ (void) setPhotoFileName:(NSString*)fileName {
    [MGInstagram sharedInstance].photoFileName = fileName;
}
+ (NSString*) photoFileName {
    return [MGInstagram sharedInstance].photoFileName;
}

+ (BOOL) isAppInstalled {
    NSURL *appURL = [NSURL URLWithString:kInstagramAppURLString];
    return [[UIApplication sharedApplication] canOpenURL:appURL];
}

//Technically the instagram allows for photos to be published under the size of 612x612
//BUT if you want nice quality pictures, I recommend checking the image size.
+ (BOOL) isImageCorrectSize:(UIImage*)image {
    CGImageRef cgImage = [image CGImage];
    return (CGImageGetWidth(cgImage) >= 612 && CGImageGetHeight(cgImage) >= 612);
}

- (NSString*) photoFilePath {
    return [NSString stringWithFormat:@"%@/%@",[NSHomeDirectory() stringByAppendingPathComponent:@"Documents"],self.photoFileName];
}

+ (void) postImage:(UIImage*)image inView:(UIView*)view {
    [self postImage:image withCaption:nil inView:view];
}
+ (void) postImage:(UIImage*)image withCaption:(NSString*)caption inView:(UIView*)view {
    [self postImage:image withCaption:caption inView:view delegate:nil];
}

+ (void) postImage:(UIImage*)image withCaption:(NSString*)caption inView:(UIView*)view delegate:(id<UIDocumentInteractionControllerDelegate>)delegate {
    [[MGInstagram sharedInstance] postImage:image withCaption:caption inView:view delegate:delegate];
}

- (void) postImage:(UIImage*)image withCaption:(NSString*)caption inView:(UIView*)view delegate:(id<UIDocumentInteractionControllerDelegate>)delegate
{
    if (!image)
        [NSException raise:NSInternalInconsistencyException format:@"Image cannot be nil!"];
    
    [UIImageJPEGRepresentation(image, 1.0) writeToFile:[self photoFilePath] atomically:YES];
    
    
    
    NSURL *fileURL = [NSURL fileURLWithPath:[self photoFilePath]];
    documentInteractionController = [UIDocumentInteractionController interactionControllerWithURL:fileURL];
#if UNITY_VERSION < 500
    [documentInteractionController retain];
#endif
    
    
    
    documentInteractionController.UTI = @"com.instagram.exclusivegram";
    documentInteractionController.delegate = delegate;
    if (caption)
        documentInteractionController.annotation = [NSDictionary dictionaryWithObject:caption forKey:@"InstagramCaption"];
    
    [documentInteractionController presentOpenInMenuFromRect:CGRectZero inView:view animated:YES];
    
}


@end


@implementation IOSTwitterPlugin


static IOSTwitterPlugin * itp_sharedInstance;


+ (id)sharedInstance {
    
    if (itp_sharedInstance == nil)  {
        itp_sharedInstance = [[self alloc] init];
    }
    
    return itp_sharedInstance;
}


- (void) initTwitterPlugin {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: Twitter init"];
    
    NSString * status = @"0";
    
    if([self IsTwitterAvaliable]) {
        if([self IsTwitterAuthed]) {
            status = @"1";
        }
    }
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: Status init %@", status];
    UnitySendMessage("IOSTwitterManager", "OnInited", [ISN_DataConvertor NSStringToChar:status]);
    
}

-(void) authificateUser {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP:  authificateUser"];
    ACAccountStore *account = [[ACAccountStore alloc] init];
    ACAccountType *twitterAccountType = [account accountTypeWithAccountTypeIdentifier:ACAccountTypeIdentifierTwitter];
    
    [account requestAccessToAccountsWithType:twitterAccountType options:NULL completion:^(BOOL granted, NSError *error)  {
        if (granted) {
            NSArray *twitterAccounts = [account accountsWithAccountType:twitterAccountType];
            if ([twitterAccounts count] > 0) {
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP:  OnAuthSuccess"];
                UnitySendMessage("IOSTwitterManager", "OnAuthSuccess", [ISN_DataConvertor NSStringToChar:@""]);
                
            } else {
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: OnAuthFailed no aacounts"];
                UnitySendMessage("IOSTwitterManager", "OnAuthFailed", [ISN_DataConvertor NSStringToChar:@"0"]);
            }
            
        } else {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: OnAuthFailed no accses"];
            UnitySendMessage("IOSTwitterManager", "OnAuthFailed", [ISN_DataConvertor NSStringToChar:@"1"]);
        }
    }];
    
    
}


-(void) loadUserData {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: loadUserData"];
    ACAccountStore *account = [[ACAccountStore alloc] init];
    ACAccountType *twitterAccountType = [account accountTypeWithAccountTypeIdentifier:ACAccountTypeIdentifierTwitter];
    
    [account requestAccessToAccountsWithType:twitterAccountType options:NULL completion:^(BOOL granted, NSError *error)  {
        if (granted) {
            NSArray *twitterAccounts = [account accountsWithAccountType:twitterAccountType];
            if ([twitterAccounts count] > 0) {
                ACAccount *twitterAccount = [twitterAccounts objectAtIndex:0];
                
                // Creating a request to get the info about a user on Twitter
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: Using twitter acc with name: %@", twitterAccount.username];
                
                SLRequest *twitterInfoRequest = [SLRequest requestForServiceType:SLServiceTypeTwitter requestMethod:SLRequestMethodGET URL:[NSURL URLWithString:@"https://api.twitter.com/1.1/users/show.json"] parameters:[NSDictionary dictionaryWithObject:twitterAccount.username forKey:@"screen_name"]];
                [twitterInfoRequest setAccount:twitterAccount];
                
                
                
                
                // Making the request
                [twitterInfoRequest performRequestWithHandler:^(NSData *responseData, NSHTTPURLResponse *urlResponse, NSError *error) {
                    dispatch_async(dispatch_get_main_queue(), ^{
                        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: twitterInfoRequest finished"];
                        
                        
                        // Check if we reached the reate limit
                        if ([urlResponse statusCode] == 429) {
                            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: Rate limit reached"];
                            UnitySendMessage("IOSTwitterManager", "OnUserDataLoadFailed", [ISN_DataConvertor NSStringToChar:@""]);
                            return;
                        }
                        
                        // Check if there was an error
                        if (error) {
                            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: Error: %@", error.localizedDescription];
                            UnitySendMessage("IOSTwitterManager", "OnUserDataLoadFailed", [ISN_DataConvertor NSStringToChar:@""]);
                            return;
                        }
                        
                        // Check if there is some response data
                        if (responseData) {
                            NSString *resp =  [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
                            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: Request Succsesful: %@", resp];
                            
                            UnitySendMessage("IOSTwitterManager", "OnUserDataLoaded", [ISN_DataConvertor NSStringToChar:resp]);
                            
                            
                        } else {
                            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: No respoce data founded"];
                            
                            UnitySendMessage("IOSTwitterManager", "OnUserDataLoadFailed", [ISN_DataConvertor NSStringToChar:@""]);
                        }
                    });
                }];
                
                
            } else {
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: OnUserDataLoadFailed no accounts founded"];
                UnitySendMessage("IOSTwitterManager", "OnUserDataLoadFailed", [ISN_DataConvertor NSStringToChar:@""]);
            }
            
        } else {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"MSP: OnUserDataLoadFailed no access"];
            UnitySendMessage("IOSTwitterManager", "OnUserDataLoadFailed", [ISN_DataConvertor NSStringToChar:@""]);
        }
    }];
    
}


-(void) postWithMedia:(NSString *)status media:(NSString *)media {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"postWithMedia"];
    
    NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
    UIImage *image = [[UIImage alloc] initWithData:imageData];
    
    __weak SLComposeViewController *tweetSheet = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeTwitter];
    [tweetSheet setInitialText:status];
    [tweetSheet addImage:image];
    
    UIViewController *vc =  UnityGetGLViewController();
    
    [vc presentViewController:tweetSheet animated:YES completion:nil];
    
    tweetSheet.completionHandler = ^(SLComposeViewControllerResult result) {
        switch(result) {
                //  This means the user cancelled without sending the Tweet
            case SLComposeViewControllerResultCancelled:
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Tweet message was cancelled"];
                UnitySendMessage("IOSTwitterManager", "OnPostFailed", [ISN_DataConvertor NSStringToChar:@""]);
                [tweetSheet dismissViewControllerAnimated:YES completion:nil];
                break;
                //  This means the user hit 'Send'
            case SLComposeViewControllerResultDone:
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Done pressed successfully"];
                UnitySendMessage("IOSTwitterManager", "OnPostSuccess", [ISN_DataConvertor NSStringToChar:@""]);
                [tweetSheet dismissViewControllerAnimated:YES completion:nil];
                break;
        }
    };
    
}

- (void) post:(NSString *)status {
    
    __weak SLComposeViewController *tweetSheet = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeTwitter];
    [tweetSheet setInitialText:status];
    
    
    UIViewController *vc =  UnityGetGLViewController();
    
    [vc presentViewController:tweetSheet animated:YES completion:nil];
    
    tweetSheet.completionHandler = ^(SLComposeViewControllerResult result) {
        switch(result) {
                //  This means the user cancelled without sending the Tweet
            case SLComposeViewControllerResultCancelled:
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Tweet message was cancelled"];
                
                UnitySendMessage("IOSTwitterManager", "OnPostFailed", [ISN_DataConvertor NSStringToChar:@""]);
                [tweetSheet dismissViewControllerAnimated:YES completion:nil];
                break;
                //  This means the user hit 'Send'
            case SLComposeViewControllerResultDone:
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Done pressed successfully"];
                UnitySendMessage("IOSTwitterManager", "OnPostSuccess", [ISN_DataConvertor NSStringToChar:@""]);
                [tweetSheet dismissViewControllerAnimated:YES completion:nil];
                break;
        }
    };
}


-(BOOL) IsTwitterAvaliable {
    return [SLComposeViewController isAvailableForServiceType:SLServiceTypeTwitter];
}

-(BOOL) IsTwitterAuthed {
    ACAccountStore *account = [[ACAccountStore alloc] init];
    ACAccountType *twitterAccountType = [account accountTypeWithAccountTypeIdentifier:ACAccountTypeIdentifierTwitter];
    
    NSArray *twitterAccounts = [account accountsWithAccountType:twitterAccountType];
    
    if(twitterAccounts.count > 0) {
        return  true;
    } else {
        return  false;
    }
}





@end


extern "C" {
    
    
    //--------------------------------------
    //  IOS Plugin Section
    //--------------------------------------
    
    
    void _ISN_TwPost(char* text, char* url, char* encodedMedia) {
        [[ISN_SocialGate sharedInstance] twitterPost:[ISN_DataConvertor charToNSString:text] url:[ISN_DataConvertor charToNSString:url] media:[ISN_DataConvertor charToNSString:encodedMedia]];
    }
    
    
    void _ISN_TwPostGIF(char* text, char* url) {
        [[ISN_SocialGate sharedInstance] twitterPostGif:[ISN_DataConvertor charToNSString:text] url:[ISN_DataConvertor charToNSString:url]];
    }
    
    
    void _ISN_FbPost(char* text, char* url, char* encodedMedia) {
        [[ISN_SocialGate sharedInstance] fbPost:[ISN_DataConvertor charToNSString:text] url:[ISN_DataConvertor charToNSString:url] media:[ISN_DataConvertor charToNSString:encodedMedia]];
    }
    
    
    
    void _ISN_MediaShare(char* text, char* encodedMedia) {
        NSString *status = [ISN_DataConvertor charToNSString:text];
        NSString *media = [ISN_DataConvertor charToNSString:encodedMedia];
        
        [[ISN_SocialGate sharedInstance] mediaShare:status media:media];
        
    }
    
    
    void _ISN_SendMail(char* subject, char* body,  char* recipients, char* encodedMedia) {
        NSString *mailSubject       = [ISN_DataConvertor charToNSString:subject];
        NSString *mailBody          = [ISN_DataConvertor charToNSString:body];
        NSString *mailRecipients    = [ISN_DataConvertor charToNSString:recipients];
        NSString *media             = [ISN_DataConvertor charToNSString:encodedMedia];
        
        
        [[ISN_SocialGate sharedInstance] sendEmail:mailSubject body:mailBody recipients:mailRecipients media:media];
    }
    
    void _ISN_WP_ShareText(char* text) {
        NSString *msg = [ISN_DataConvertor charToNSString:text];
        [[ISN_SocialGate sharedInstance] whatsappShareText:msg];
        
    }
    
    
    void _ISN_WP_ShareMedia(char* encodedMedia) {
        NSString *media = [ISN_DataConvertor charToNSString:encodedMedia];
        [[ISN_SocialGate sharedInstance] whatsappShareImage:media];
    }
    
    void _ISN_SendTextMessage(char* body, char* recipients) {
        
        NSString *msgBody          = [ISN_DataConvertor charToNSString:body];
        NSArray* msgRecipients = [ISN_DataConvertor charToNSArray:recipients];
        
        [[ISN_SocialGate sharedInstance] SendTextMessage:msgBody url:msgRecipients];
        
    }
    
    
    //--------------------------------------
    //  Mobile Social Plugin Section
    //--------------------------------------
    
    void _MSP_TwPost(char* text, char* url, char* encodedMedia) {
        _ISN_TwPost(text, url, encodedMedia);
    }
    
    
    void _MSP_FbPost(char* text, char* url, char* encodedMedia) {
        _ISN_FbPost(text, url, encodedMedia);
    }
    
    
    
    void _MSP_MediaShare(char* text, char* encodedMedia) {
        _ISN_MediaShare(text, encodedMedia);
    }
    
    
    void _MSP_SendMail(char* subject, char* body,  char* recipients, char* encodedMedia) {
        _ISN_SendMail(subject, body,  recipients, encodedMedia);
    }
    
    //--------------------------------------
    //  Mobile Social Plugin Instagram
    //--------------------------------------
    
    void _ISN_InstaShare(char* encodedMedia, char* text) {
        
        NSString *status = [ISN_DataConvertor charToNSString:text];
        NSString *media = [ISN_DataConvertor charToNSString:encodedMedia];
        
        [[IOSInstaPlugin sharedInstance] share:status media:media];
        
    }
    
    void _MSP_InstaShare(char* encodedMedia, char* text) {
        _ISN_InstaShare(encodedMedia, text);
    }
    
    
    
    //--------------------------------------
    //  Mobile Social Plugin Twitter
    //--------------------------------------
    
    
    void _twitterInit ()  {
        [[IOSTwitterPlugin sharedInstance] initTwitterPlugin];
    }
    
    
    void _twitterLoadUserData() {
        [[IOSTwitterPlugin sharedInstance] loadUserData];
    }
    
    void _twitterAuthificateUser() {
        [[IOSTwitterPlugin sharedInstance] authificateUser];
    }
    
    
    void _twitterPost(char* text) {
        NSString *status = [ISN_DataConvertor charToNSString:text];
        [[IOSTwitterPlugin sharedInstance] post:status];
    }
    
    void _twitterPostWithMedia(char* text, char* encodedMedia) {
        
        NSString *status = [ISN_DataConvertor charToNSString:text];
        NSString *media = [ISN_DataConvertor charToNSString:encodedMedia];
        
        [[IOSTwitterPlugin sharedInstance] postWithMedia:status media:media];
    }
    
    
    
}


#endif

