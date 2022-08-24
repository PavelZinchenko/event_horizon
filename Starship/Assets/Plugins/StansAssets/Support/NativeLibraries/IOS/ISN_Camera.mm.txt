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
#import <MobileCoreServices/MobileCoreServices.h>
#import "ISN_NativeCore.h"
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif


@interface NonRotatingUIImagePickerController : UIImagePickerController

@end

@implementation NonRotatingUIImagePickerController


- (BOOL)shouldAutorotate {
    return NO;
}

- (NSUInteger)supportedInterfaceOrientations{
    
    if([[UIDevice currentDevice] orientation] == UIDeviceOrientationPortrait || [[UIDevice currentDevice] orientation] == UIDeviceOrientationPortraitUpsideDown) {
        return [[UIDevice currentDevice] orientation];
    }

    return UIInterfaceOrientationMaskLandscape;
}

@end

@interface UIImage (fixOrientation)

- (UIImage *)fixOrientation;

@end

@implementation UIImage (fixOrientation)

- (UIImage *)fixOrientation {
    
    // No-op if the orientation is already correct
    if (self.imageOrientation == UIImageOrientationUp) return self;
    
    // We need to calculate the proper transformation to make the image upright.
    // We do it in 2 steps: Rotate if Left/Right/Down, and then flip if Mirrored.
    CGAffineTransform transform = CGAffineTransformIdentity;
    
    switch (self.imageOrientation) {
        case UIImageOrientationDown:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.width, self.size.height);
            transform = CGAffineTransformRotate(transform, M_PI);
            break;
            
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.width, 0);
            transform = CGAffineTransformRotate(transform, M_PI_2);
            break;
            
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, 0, self.size.height);
            transform = CGAffineTransformRotate(transform, -M_PI_2);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationUpMirrored:
            break;
    }
    
    switch (self.imageOrientation) {
        case UIImageOrientationUpMirrored:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.width, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
            
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.height, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationDown:
        case UIImageOrientationLeft:
        case UIImageOrientationRight:
            break;
    }
    
    // Now we draw the underlying CGImage into a new context, applying the transform
    // calculated above.
    CGContextRef ctx = CGBitmapContextCreate(NULL, self.size.width, self.size.height,
                                             CGImageGetBitsPerComponent(self.CGImage), 0,
                                             CGImageGetColorSpace(self.CGImage),
                                             CGImageGetBitmapInfo(self.CGImage));
    CGContextConcatCTM(ctx, transform);
    switch (self.imageOrientation) {
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            // Grr...
            CGContextDrawImage(ctx, CGRectMake(0,0,self.size.height,self.size.width), self.CGImage);
            break;
            
        default:
            CGContextDrawImage(ctx, CGRectMake(0,0,self.size.width,self.size.height), self.CGImage);
            break;
    }
    
    // And now we just create a new UIImage from the drawing context
    CGImageRef cgimg = CGBitmapContextCreateImage(ctx);
    UIImage *img = [UIImage imageWithCGImage:cgimg];
    CGContextRelease(ctx);
    CGImageRelease(cgimg);
    return img;
}

@end


@interface ISN_Camera : NSObject<UIImagePickerControllerDelegate, UINavigationControllerDelegate>

@property float ImageCompressionRate;
@property int MaxImageSize;
@property int encodingType;

+ (id)   sharedInstance;
- (void) saveToCameraRoll:(NSString*)media;

-(void) PickImage:(int) source;
- (NSString*) EncodeImage:(UIImage *)image;

@end


@implementation ISN_Camera

static ISN_Camera * cam_sharedInstance;
static UIImagePickerController *_imagePicker = NULL;


+ (id)sharedInstance {
    
    if (cam_sharedInstance == nil)  {
        cam_sharedInstance = [[self alloc] init];
    }
    
    return cam_sharedInstance;
}

- (void) saveToCameraRoll:(NSString *)media {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"saveToCameraRoll"];
    NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
    UIImage *image = [[UIImage alloc] initWithData:imageData];

#if UNITY_VERSION < 500
    [imageData release];
#endif
    
    UIImageWriteToSavedPhotosAlbum(image,
                                   self, // send the message to 'self' when calling the callback
                                   @selector(thisImage:hasBeenSavedInPhotoAlbumWithError:usingContextInfo:), // the selector to tell the method to call on completion
                                   NULL); // you generally won't need a contextInfo here
    

    
    
}

- (void)thisImage:(UIImage *)image hasBeenSavedInPhotoAlbumWithError:(NSError *)error usingContextInfo:(void*)ctxInfo {
   
#if UNITY_VERSION < 500
    [image release];
    image=  nil;
#endif
    
    if (error) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"image not saved: %@", error.description];
        UnitySendMessage("IOSCamera", "OnImageSaveFailed", [ISN_DataConvertor NSStringToChar:@""]);
        
    } else {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"image saved"];
        UnitySendMessage("IOSCamera", "OnImageSaveSuccess", [ISN_DataConvertor NSStringToChar:@""]);
    }
    
    
}


-(void) PickImage:(int)source {
    
    UIImagePickerControllerSourceType SourceType;
    
    switch (source) {
        case 0:
            SourceType = UIImagePickerControllerSourceTypePhotoLibrary;
            break;
            
        case 1:
            SourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
            break;
            
        case 2:
            SourceType =  UIImagePickerControllerSourceTypeCamera;
            break;
            
        default:
            break;
    }
    
    if(SourceType == UIImagePickerControllerSourceTypeCamera) {
        [self GetImageFromCamera];
    } else {
        [self GetImage:SourceType];
    }
    
    
    
}


-(void) StartCameraImagePic {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"StartCameraImagePic"];
    [self GetImage:UIImagePickerControllerSourceTypeCamera];
}

-(void) GetImageFromCamera {
    BOOL cameraAvailableFlag = [UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera];
    if (cameraAvailableFlag) {
        [self performSelector:@selector(StartCameraImagePic) withObject:nil afterDelay:0.9];
    }
}


-(void) GetVideoPathFromAlbum {
    [self GetVideo];
}





- (void)GetVideo {
    UIViewController *vc =  UnityGetGLViewController();
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    picker.delegate = self;
    picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    picker.mediaTypes = [[NSArray alloc] initWithObjects:(NSString *)kUTTypeMovie,      nil];
    [vc presentViewController:picker animated:YES completion:nil];
#if UNITY_VERSION < 500
    [picker release];
#endif
   
}




-(void) GetImage: (UIImagePickerControllerSourceType )source {
    UIViewController *vc =  UnityGetGLViewController();
    
    if(_imagePicker == NULL) {
        _imagePicker = [[NonRotatingUIImagePickerController alloc] init];
        _imagePicker.delegate = self;

    }
    //NonRotatingUIImagePickerController
    
    _imagePicker.sourceType = source;
    
    
    [vc presentViewController:_imagePicker animated:YES completion:nil];
 
    
}


-(void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    
    
    // added video support
    NSString *mediaType = [info objectForKey: UIImagePickerControllerMediaType]; // get media type
    // if mediatype is video
    if (CFStringCompare ((__bridge CFStringRef) mediaType, kUTTypeMovie, 0) == kCFCompareEqualTo) {
        NSURL *videoUrl=(NSURL*)[info objectForKey:UIImagePickerControllerMediaURL];
        NSString *moviePath = [videoUrl path];
        UnitySendMessage("IOSCamera", "OnVideoPickedEvent", [ISN_DataConvertor NSStringToChar:moviePath]);
    } else{
        // it must be an image
        UIImage *photo = [info objectForKey:UIImagePickerControllerOriginalImage];
        NSString *encodedImage = @"";
        if (photo == nil) {
             [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"no photo"];
        } else {
            encodedImage = [self EncodeImage:photo];
        }
        
        UnitySendMessage("IOSCamera", "OnImagePickedEvent", [ISN_DataConvertor NSStringToChar:encodedImage]);
        
    }
    
}

- (NSString*) EncodeImage:(UIImage *)image {
    image = [image fixOrientation];
    
    
    if(image.size.width > [self MaxImageSize] || image.size.height > [self MaxImageSize] ) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"resizing image"];
        CGSize s = CGSizeMake([self MaxImageSize], [self MaxImageSize]);
        
        if(image.size.width > image.size.height) {
            CGFloat new_height = [self MaxImageSize] / (image.size.width / image.size.height);
            s.height = new_height;
            
        } else {
            CGFloat new_width = [self MaxImageSize] / (image.size.height / image.size.width);
            s.width = new_width;
            
        }
        
        image =   [ISN_Camera imageWithImage:image scaledToSize:s];
        
    }
    
    NSData *imageData = nil;
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ImageCompressionRate: %f", [self ImageCompressionRate]];
    
    if([self encodingType] == 0) {
        imageData = UIImagePNGRepresentation(image);
    } else {
        imageData = UIImageJPEGRepresentation(image, [self ImageCompressionRate]);
    }
    
    return  [imageData base64Encoding];
}



+ (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize {
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 1.0);
    [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newImage;
}

-(void) imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    //[vc dismissModalViewControllerAnimated:YES];
    
    UnitySendMessage("IOSCamera", "OnImagePickedEvent", [ISN_DataConvertor NSStringToChar:@""]);
}

extern "C" {
    
    
    //--------------------------------------
    //  IOS Native Plugin Section
    //--------------------------------------
    
    
    void _ISN_SaveToCameraRoll(char* encodedMedia) {
        NSString *media = [ISN_DataConvertor charToNSString:encodedMedia];
        [[ISN_Camera sharedInstance] saveToCameraRoll:media];
    }
    
    
    void _ISN_GetVideoPathFromAlbum() {
        [[ISN_Camera sharedInstance] GetVideoPathFromAlbum];
    }
    
    
    void _ISN_PickImage(int source) {
        [[ISN_Camera sharedInstance] PickImage:source];
    }

    
    void _ISN_InitCameraAPI(float compressionRate, int maxSize, int encodingType) {
        [[ISN_Camera sharedInstance] setImageCompressionRate:compressionRate];
        [[ISN_Camera sharedInstance] setMaxImageSize:maxSize];
        [[ISN_Camera sharedInstance] setEncodingType:encodingType];
    }
    
}


@end

#endif
