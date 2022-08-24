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


NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_EOF = @"endofline";



@interface ISN_GestureRecognizer : NSObject

+ (ISN_GestureRecognizer *)sharedInstance;

-(void) Start;

@end



static ISN_GestureRecognizer * gesture_sharedInstance;



@implementation ISN_GestureRecognizer


+ (id)sharedInstance {
    if (gesture_sharedInstance == nil)  {
        gesture_sharedInstance = [[self alloc] init];
    }
    
    return gesture_sharedInstance;
}


- (void)Start {
    
    UISwipeGestureRecognizer * swipe;
    UIViewController *vc =  UnityGetGLViewController();
    
    swipe = [[UISwipeGestureRecognizer alloc]initWithTarget:self action:@selector(SwipeAction:)];
    swipe.direction=UISwipeGestureRecognizerDirectionRight;
    [vc.view addGestureRecognizer:swipe];
    
    swipe = [[UISwipeGestureRecognizer alloc]initWithTarget:self action:@selector(SwipeAction:)];
    swipe.direction=UISwipeGestureRecognizerDirectionLeft;
    [vc.view addGestureRecognizer:swipe];
    
    swipe = [[UISwipeGestureRecognizer alloc]initWithTarget:self action:@selector(SwipeAction:)];
    swipe.direction=UISwipeGestureRecognizerDirectionUp;
    [vc.view addGestureRecognizer:swipe];
    
    swipe = [[UISwipeGestureRecognizer alloc]initWithTarget:self action:@selector(SwipeAction:)];
    swipe.direction=UISwipeGestureRecognizerDirectionDown;
    [vc.view addGestureRecognizer:swipe];
    
}

-(void)SwipeAction:(UISwipeGestureRecognizer*)gestureRecognizer {
    //Do what you want here
    
    if(gestureRecognizer.direction == UISwipeGestureRecognizerDirectionUp) {
        UnitySendMessage("ISN_GestureRecognizer", "OnSwipeAction", "0");
    }
    
    if(gestureRecognizer.direction == UISwipeGestureRecognizerDirectionDown) {
        UnitySendMessage("ISN_GestureRecognizer", "OnSwipeAction", "1");
    }
    
    if(gestureRecognizer.direction == UISwipeGestureRecognizerDirectionLeft) {
        UnitySendMessage("ISN_GestureRecognizer", "OnSwipeAction", "2");
    }
    
    if(gestureRecognizer.direction == UISwipeGestureRecognizerDirectionRight) {
        UnitySendMessage("ISN_GestureRecognizer", "OnSwipeAction", "3");
    }
    
}


@end



extern "C" {
    
    
    
    void _ISN_InitTvOsGestureRecognizer() {
        [[ISN_GestureRecognizer sharedInstance] Start];
    }
    
}






