//
//  ISN_ReplayKit.m
//  Unity-iPhone
//
//  Created by lacost on 9/18/15.
//
//

#import <Foundation/Foundation.h>
#import <ReplayKit/ReplayKit.h>

#import "ISN_NativeCore.h"


@interface ISN_ReplayKit : NSObject< RPPreviewViewControllerDelegate, RPScreenRecorderDelegate>

@property (nonatomic, strong) RPPreviewViewController* rpk_previewViewController;

+ (id) sharedInstance;

- (void) startRecordingWithMicrophoneEnabled:(BOOL)microphoneEnabled;
- (void) stopRecording;
- (void) discardRecording;
- (void) showVideoShareDialog:(int)ipadViewMode;

@end

@implementation ISN_ReplayKit

static ISN_ReplayKit * rpk_sharedInstance;


+ (id)sharedInstance {
    
    if (rpk_sharedInstance == nil)  {
        rpk_sharedInstance = [[self alloc] init];
    }
    return rpk_sharedInstance;
}

-(id) init {
    if(self = [super init]){
        [RPScreenRecorder sharedRecorder].delegate = self;
    }
    return self;
}



- (void) startRecordingWithMicrophoneEnabled:(BOOL)microphoneEnabled {
    
    if([ISN_NativeUtility majorIOSVersion] < 9) {
        UnitySendMessage("ISN_ReplayKit", "OnRecorStartFailed", [ISN_DataConvertor serializeErrorWithData:@"Not supported by this IOS verison" code:0]);
        return;
    }
    
    if(microphoneEnabled) {
        [[RPScreenRecorder sharedRecorder] startRecordingWithMicrophoneEnabled:microphoneEnabled handler:^(NSError * _Nullable error) {
            if(error ==  nil) {
                NSLog(@"ISN_ReplayKit Start rectording");
                UnitySendMessage("ISN_ReplayKit", "OnRecorStartSuccess", "");
            } else {
                NSLog(@"ISN_ReplayKit Start record Error: %@", error.description);
                UnitySendMessage("ISN_ReplayKit", "OnRecorStartFailed", [ISN_DataConvertor serializeError:error]);
            }
        }];

    } else {
        [[RPScreenRecorder sharedRecorder] startRecordingWithHandler:^(NSError * _Nullable error) {
            if(error ==  nil) {
                NSLog(@"ISN_ReplayKit Start rectording");
                UnitySendMessage("ISN_ReplayKit", "OnRecorStartSuccess", "");
            } else {
                NSLog(@"ISN_ReplayKit Start record Error: %@", error.description);
                UnitySendMessage("ISN_ReplayKit", "OnRecorStartFailed", [ISN_DataConvertor serializeError:error]);
            }
            
        }];

    }
    
    
    return;
    
  }

//static RPPreviewViewController* saved_previewViewController;

-(void) showVideoShareDialog:(int)ipadViewMode {
    if([self rpk_previewViewController] ==  NULL) {
        NSLog(@"ISN_ReplayKit Replay Preview controller is null");
        return;
    }
    
    
    UIViewController *vc =  UnityGetGLViewController();
    
    if([ISN_NativeUtility IsIPad]) {
        if(ipadViewMode == 0) {
            [self rpk_previewViewController].modalPresentationStyle = UIModalPresentationCurrentContext;
        }
        [self rpk_previewViewController].popoverPresentationController.sourceView = vc.view;
    }
    
    [vc presentViewController:[self rpk_previewViewController] animated:YES completion:nil];
    
}


- (void) discardRecording {
    [[RPScreenRecorder sharedRecorder] discardRecordingWithHandler:^{
        
         [self setRpk_previewViewController:NULL];
          UnitySendMessage("ISN_ReplayKit", "OnRecordDiscard", "");
    }];
}

- (void) stopRecording {
    [[RPScreenRecorder sharedRecorder] stopRecordingWithHandler:^(RPPreviewViewController * _Nullable previewViewController, NSError * _Nullable error) {
        NSLog(@"Stop rectording");
        
        if(error == nil) {
            [self setRpk_previewViewController:previewViewController];
            previewViewController.previewControllerDelegate = self;
            
            
            UnitySendMessage("ISN_ReplayKit", "OnRecorStopSuccess", "");
            
        } else {
            NSLog(@"ISN_ReplayKit Stop record Error: %@", error.description);
            UnitySendMessage("ISN_ReplayKit", "OnRecorStopFailed", [ISN_DataConvertor serializeError:error]);
        }
        
    }];
}



#pragma RPPreviewViewController delegate block


- (void)previewControllerDidFinish:(RPPreviewViewController *)previewController {
    NSLog(@"ISN_ReplayKit previewControllerDidFinish");
    
    // [previewController dismissModalViewControllerAnimated:true];
    
}


- (void)previewController:(RPPreviewViewController *)previewController didFinishWithActivityTypes:(NSSet <NSString *> *)activityTypes {
    
    
    [previewController dismissViewControllerAnimated:YES completion:nil];
    NSMutableArray *array = [NSMutableArray arrayWithArray:[activityTypes allObjects]];
    [self setRpk_previewViewController:NULL];
    
    
    NSLog(@"ISN_ReplayKit didFinishWithActivityTypes %@", activityTypes);
    UnitySendMessage("ISN_ReplayKit", "OnSaveResult", [ISN_DataConvertor NSStringsArrayToChar:array]);
    
}

#pragma RPScreenRecorderDelegate delegate block


- (void)screenRecorder:(RPScreenRecorder *)screenRecorder didStopRecordingWithError:(NSError *)error previewViewController:(nullable RPPreviewViewController *)previewViewController {
    NSLog(@"ISN_ReplayKit didStopRecordingWithError  delegate block");
    UnitySendMessage("ISN_ReplayKit", "OnRecordInterrupted", [ISN_DataConvertor serializeError:error]);
}


- (void)screenRecorderDidChangeAvailability:(RPScreenRecorder *)screenRecorder {
    NSLog(@"ISN_ReplayKit screenRecorderDidChangeAvailability  delegate block");
    UnitySendMessage("ISN_ReplayKit", "OnRecorderDidChangeAvailability", "");
}

@end

extern "C" {
    
    void _ISN_StartRecording (bool microphoneEnabled)  {
        [[ISN_ReplayKit sharedInstance] startRecordingWithMicrophoneEnabled:microphoneEnabled];
    }
    
    void _ISN_StopRecording () {
        [[ISN_ReplayKit sharedInstance] stopRecording];
    }
    
    void _ISN_ShowVideoShareDialog (int ipadViewMode) {
        [[ISN_ReplayKit sharedInstance] showVideoShareDialog:ipadViewMode];
    }
    
    void _ISN_DiscardRecording () {
        [[ISN_ReplayKit sharedInstance] discardRecording];
    }
    
    BOOL ISN_IsReplayKitAvaliable() {
        return   [[RPScreenRecorder sharedRecorder] isAvailable];
    }
    
    BOOL ISN_IsReplayKitRecording() {
        return   [[RPScreenRecorder sharedRecorder] isRecording];
    }
    
    BOOL ISN_IsReplayKitMicEnabled() {
#if !TARGET_OS_TV
        return   [[RPScreenRecorder sharedRecorder] isMicrophoneEnabled];
#else
        return false;
#endif
    }
    
   
    
    
   
}
