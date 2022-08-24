////////////////////////////////////////////////////////////////////////////////
//
// @module Mobile Native Popups
// @author Osipov Stanislav (Stan's Assets)
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#import <Foundation/Foundation.h>
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_EOF = @"endofline";
NSString * const ARRAY_SPLITTER = @"%%%";

static NSString* templateReviewURLIOS7  = @"itms-apps://itunes.apple.com/app/idAPP_ID";


@interface MNP_PopUpsManager : NSObject

@property (strong)  UIActivityIndicatorView *spinner;

+ (MNP_PopUpsManager *) sharedInstance;
+ (void) dismissCurrentAlert;
+ (void) showMessage: (NSString *) title message: (NSString*) msg Actions:(NSString*) actions;

-(void) ShowSpinner;
-(void) HideSpinner;



@end


@implementation MNP_PopUpsManager

static MNP_PopUpsManager *_sharedInstance;
static UIAlertController* _currentAlert =  nil;

+ (id)sharedInstance {
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}



+ (void) dismissCurrentAlert {
    if(_currentAlert != nil) {
        [_currentAlert dismissViewControllerAnimated:true completion:^{
            UnitySendMessage("IOSPopUp", "onPopUpCallBack", [MNP_PopUpsManager NSStringToChar:@"0"]);
            UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack", [MNP_PopUpsManager NSStringToChar:@"0"]);
        }];
        
        _currentAlert = nil;
    }
}


+(void) showMessage: (NSString *) title message: (NSString*) msg Actions:(NSString*) actions {
    NSString *positiveAction, *negativeAction, *neutralAction;
    
    NSString *original = actions;
    NSArray *tokens = [original componentsSeparatedByString:UNITY_SPLITTER];
    unsigned long tokensCount = tokens.count;
    
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:title  message:msg  preferredStyle:UIAlertControllerStyleAlert];
    
    if (tokensCount == 1) {
        positiveAction = tokens[0];
        
        UIAlertAction* defaultAction = [UIAlertAction actionWithTitle:positiveAction style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
            
            UnitySendMessage("IOSPopUp", "onPopUpCallBack", [MNP_PopUpsManager NSStringToChar:positiveAction]);
            _currentAlert = nil;
            
        }];
        
        [alert addAction:defaultAction];
        
    } else if (tokensCount == 2) {
        positiveAction = tokens[0];
        UIAlertAction* yesAction = [UIAlertAction actionWithTitle:positiveAction style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
            
            UnitySendMessage("IOSPopUp", "onPopUpCallBack", [MNP_PopUpsManager NSStringToChar:positiveAction]);
            _currentAlert = nil;
            
        }];
        
        negativeAction = tokens[1];
        UIAlertAction* noAction = [UIAlertAction actionWithTitle:negativeAction style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
            
            UnitySendMessage("IOSPopUp", "onPopUpCallBack", [MNP_PopUpsManager NSStringToChar:negativeAction]);
            _currentAlert = nil;
            
        }];
        
        [alert addAction:yesAction];
        [alert addAction:noAction];
        
    } else if (tokensCount == 3) {
        positiveAction = tokens[0];
        UIAlertAction* laterAction = [UIAlertAction actionWithTitle:positiveAction style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
            
            UnitySendMessage("IOSPopUp", "onPopUpCallBack", [MNP_PopUpsManager NSStringToChar:positiveAction]);
            _currentAlert = nil;
            
        }];
        
        negativeAction = tokens[1];
        UIAlertAction* declineAction = [UIAlertAction actionWithTitle:negativeAction style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
            
            UnitySendMessage("IOSPopUp", "onPopUpCallBack", [MNP_PopUpsManager NSStringToChar:negativeAction]);
            _currentAlert = nil;
            
        }];
        
        neutralAction = tokens[2];
        UIAlertAction* rateAction = [UIAlertAction actionWithTitle:neutralAction style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
            
            UnitySendMessage("IOSPopUp", "onPopUpCallBack", [MNP_PopUpsManager NSStringToChar:neutralAction]);
            _currentAlert = nil;
            
        }];
        
        [alert addAction:laterAction];
        [alert addAction:declineAction];
        [alert addAction:rateAction];
        
    }
    
    _currentAlert = alert;
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:alert animated:YES completion:nil];
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
        [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.width, vc.view.frame.size.height)];
    } else {
        
        if([[UIDevice currentDevice] orientation] == UIDeviceOrientationPortrait || [[UIDevice currentDevice] orientation] == UIDeviceOrientationPortraitUpsideDown) {
            [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.width, vc.view.frame.size.height)];
            
        } else {
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
    
#endif
    
}



- (void) HideSpinner {
    
    [[UIApplication sharedApplication] endIgnoringInteractionEvents];
    
    if([self spinner] != nil) {
        [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.5f];
        [UIView animateWithDuration:0.8 animations:^{
            [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.0f];
            
        } completion:^(BOOL finished) {
            [[self spinner] removeFromSuperview];
            [self setSpinner:nil];
        }];
    }
    
}



+ (const char *) NSStringToChar:(NSString *)value {
    return [value UTF8String];
}

+(NSString *) charToNSString:(char *)value {
    if (value != NULL) {
        return [NSString stringWithUTF8String: value];
    } else {
        return [NSString stringWithUTF8String: ""];
    }
}


@end






extern "C" {


    void _MNP_ShowMessage(char* title, char* message, char* actions) {
        [MNP_PopUpsManager showMessage:[MNP_PopUpsManager charToNSString:title] message:[MNP_PopUpsManager charToNSString:message] Actions:[MNP_PopUpsManager charToNSString:actions]];
    }
    
    void _MNP_DismissCurrentAlert() {
       [MNP_PopUpsManager dismissCurrentAlert];
    }
    
    void _MNP_RedirectToAppStoreRatingPage(char* appId) {
        
        NSString *app =  [MNP_PopUpsManager charToNSString:appId];
        NSString * reviewURL = [templateReviewURLIOS7 stringByReplacingOccurrencesOfString:@"APP_ID" withString:[NSString stringWithFormat:@"%@", app]];
        
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:reviewURL]];
  
    }
    
    
    void _MNP_ShowPreloader() {
        [[MNP_PopUpsManager sharedInstance] ShowSpinner];
    }
    
    
    void _MNP_HidePreloader() {
        [[MNP_PopUpsManager sharedInstance] HideSpinner];
    }
    
    
    
}
