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
#import <MediaPlayer/MediaPlayer.h>
#import "ISN_NativeCore.h"


NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_EOF = @"endofline";


@interface ISN_Media : NSObject<MPMediaPickerControllerDelegate> {
    
    MPMusicPlayerController     *musicPlayer;
    
}


@property (nonatomic, retain)   UIViewController *vc;

+ (ISN_Media *)sharedInstance;

- (void) initialize;
- (void) setShuffleMode: (MPMusicShuffleMode) mode;
- (void) setRepeatMode: (MPMusicRepeatMode) mode;


- (void) play;
- (void) pause;
- (void) skipToNextItem;
- (void) skipToBeginning;
- (void) skipToPreviousItem;



- (void) showMediaPicker;
- (void) setCollection: (NSArray*) itemsIds;
- (void) addItemWithProductID:(NSString *)productID;



@end







@interface ISNVideo : NSObject

@property (strong, nonatomic) MPMoviePlayerViewController *streamPlayer;

+ (id) sharedInstance;

- (void) streamVideo:(NSString*)url;
- (void) openYouTubeVideo:(NSString*)url;

@end









static ISN_Media * media_sharedInstance;


#pragma init

@implementation ISN_Media




+ (id)sharedInstance {
    if (media_sharedInstance == nil)  {
        media_sharedInstance = [[self alloc] init];
    }
    
    return media_sharedInstance;
}

- (void) initialize {
    
    
    musicPlayer = [MPMusicPlayerController applicationMusicPlayer];
   
    [musicPlayer beginGeneratingPlaybackNotifications];
  //  musicPlayer.currentPlaybackTime
    
    NSNotificationCenter *notificationCenter = [NSNotificationCenter defaultCenter];
    
    [notificationCenter addObserver: self
                           selector: @selector (handle_NowPlayingItemChanged:)
                               name: MPMusicPlayerControllerNowPlayingItemDidChangeNotification
                             object: musicPlayer];
    
    [notificationCenter addObserver: self
                           selector: @selector (handle_PlaybackStateChanged:)
                               name: MPMusicPlayerControllerPlaybackStateDidChangeNotification
                             object: musicPlayer];
    
    [self setVc:UnityGetGLViewController()];
    [self showMediaPicker];
}


-(void) setShuffleMode:(MPMusicShuffleMode)mode {
    [musicPlayer setShuffleMode:mode];
}


-(void) setRepeatMode:(MPMusicRepeatMode)mode {
    [musicPlayer setRepeatMode:mode];
}


-(void) play {
    MPMusicPlaybackState playbackState = [musicPlayer playbackState];
    if (playbackState == MPMusicPlaybackStateStopped || playbackState == MPMusicPlaybackStatePaused) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN_Media: play"];
        [musicPlayer play];
    }
}

-(void) pause {
    MPMusicPlaybackState playbackState = [musicPlayer playbackState];
    if ( !(playbackState == MPMusicPlaybackStateStopped || playbackState == MPMusicPlaybackStatePaused)) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN_Media: pause"];
        [musicPlayer pause];
    }
}

-(void) skipToNextItem {
    [musicPlayer skipToNextItem];
}

-(void) skipToBeginning {
    [musicPlayer skipToBeginning];
}

-(void) skipToPreviousItem {
    [musicPlayer skipToPreviousItem];
}




-(void) showMediaPicker {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"showMediaPicker"];
    
    MPMediaPickerController *picker = [[MPMediaPickerController alloc] initWithMediaTypes: MPMediaTypeAnyAudio];
    
    picker.delegate                     = self;
    picker.allowsPickingMultipleItems   = YES;
 //   picker.prompt                     = NSLocalizedString (@"AddSongsPrompt", @"Prompt to user to choose some songs to play");
    
   
    [[self vc] presentViewController: picker animated: YES completion:nil];
  
#if UNITY_VERSION < 500
   [picker release];
#endif
    
    
}

-(void) setCollection:(NSArray *)itemsIds {
    
    NSMutableArray *items = [[NSMutableArray alloc] init];
    
    for(NSString* itemId in itemsIds) {
        MPMediaPropertyPredicate *predicate = [MPMediaPropertyPredicate predicateWithValue:itemId forProperty:MPMediaItemPropertyPersistentID];
        
        MPMediaQuery *mySongQuery = [[MPMediaQuery alloc] init];
        [mySongQuery addFilterPredicate:predicate];
        
        if(mySongQuery.items != nil) {
            if(mySongQuery.items.count > 0) {
                [items addObject:mySongQuery.items.firstObject];
            }
        }
        
        
        MPMediaItemCollection *collection = [[MPMediaItemCollection alloc] initWithItems:items];
        [musicPlayer setQueueWithItemCollection: collection];
        
        NSString* serializedItems = [self serializeMediaItemsInfo:collection.items];
        UnitySendMessage("ISN_MediaController", "OnQueueUpdate", [ISN_DataConvertor NSStringToChar:serializedItems]);

    }
}

-(void) addItemWithProductID:(NSString *)productID  {
    
    [[MPMediaLibrary defaultMediaLibrary] addItemWithProductID:productID completionHandler:^(NSArray<__kindof MPMediaEntity *> * _Nonnull entities, NSError * _Nullable error) {
        
         NSLog(@"added id%@ entities: %@ and error is %@", productID, entities, error);
        
        if(error == nil) {
            NSArray *tracksToPlay = [NSArray arrayWithObject:productID];
            [musicPlayer setQueueWithStoreIDs:tracksToPlay];
            
            NSString* serializedItems = [self serializeMediaItemsInfo:tracksToPlay];
            UnitySendMessage("ISN_MediaController", "OnQueueUpdate", [ISN_DataConvertor NSStringToChar:serializedItems]);
        } else {
             UnitySendMessage("ISN_MediaController", "OnQueueUpdateFailed", [ISN_DataConvertor serializeError:error]);
        }
        
    }];
}



- (void) updatePlayerQueueWithMediaCollection: (MPMediaItemCollection *) mediaItemCollection {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN_Media: updatePlayerQueueWithMediaCollection"];
    // Configure the music player, but only if the user chose at least one song to play
    if (mediaItemCollection) {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN_Media: setQueueWithItemCollection"];
 
        NSString* serializedItems = [self serializeMediaItemsInfo:mediaItemCollection.items];
        
        [musicPlayer setQueueWithItemCollection: mediaItemCollection];
        UnitySendMessage("ISN_MediaController", "OnMediaPickerResult", [ISN_DataConvertor NSStringToChar:serializedItems]);
    } else {
        UnitySendMessage("ISN_MediaController", "OnMediaPickerFailed", [ISN_DataConvertor serializeErrorWithData:@"No Items Picked" code:1]);
    }
}




#pragma private methods


-(NSString*) serializeMediaItemsInfo :(NSArray*)items {
    NSMutableString * data = [[NSMutableString alloc] init];
    
    
    
    for(MPMediaEntity * item in items){
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"id: %@", [item valueForProperty: MPMediaItemPropertyPersistentID]];
        [data appendString:[self serializeMPMediaItemInfo:item]];
        [data appendString: UNITY_SPLITTER];
    }
    
    [data appendString: UNITY_EOF];
    
    return data;
}

-(NSString*) serializeMPMediaItemInfo :(MPMediaEntity *) mediaItem {
    NSMutableString * data = [[NSMutableString alloc] init];
   

    
    [data appendString: [NSString stringWithFormat:@"%@", [mediaItem valueForProperty: MPMediaItemPropertyPersistentID]]];
    [data appendString: UNITY_SPLITTER];
    
    if([mediaItem valueForProperty: MPMediaItemPropertyTitle] != nil) {
        [data appendString: [mediaItem valueForProperty: MPMediaItemPropertyTitle]];
    }
    [data appendString: UNITY_SPLITTER];
    
    
    if([mediaItem valueForProperty: MPMediaItemPropertyArtist] != nil) {
        [data appendString: [mediaItem valueForProperty: MPMediaItemPropertyArtist]];
    }
    [data appendString: UNITY_SPLITTER];
    
    
    if([mediaItem valueForProperty: MPMediaItemPropertyAlbumTitle] != nil) {
        [data appendString: [mediaItem valueForProperty: MPMediaItemPropertyAlbumTitle]];
    }
    [data appendString: UNITY_SPLITTER];
    
    
    if([mediaItem valueForProperty: MPMediaItemPropertyAlbumArtist] != nil) {
        [data appendString: [mediaItem valueForProperty: MPMediaItemPropertyAlbumArtist]];
    }
    [data appendString: UNITY_SPLITTER];
    
    
    if([mediaItem valueForProperty: MPMediaItemPropertyGenre] != nil) {
        [data appendString: [mediaItem valueForProperty: MPMediaItemPropertyGenre]];
    }
    [data appendString: UNITY_SPLITTER];
    
    
    if([mediaItem valueForProperty: MPMediaItemPropertyPlaybackDuration] != nil) {
        id value =  [mediaItem valueForProperty:MPMediaItemPropertyPlaybackDuration];
        
        NSString* interval = [NSString stringWithFormat:@"%f", [value doubleValue]];
        [data appendString: interval];
    }
    [data appendString: UNITY_SPLITTER];

    
    if([mediaItem valueForProperty: MPMediaItemPropertyComposer] != nil) {
        [data appendString: [mediaItem valueForProperty: MPMediaItemPropertyComposer]];
    }
    
    
    
    return data;
}




+(MPMusicPlaybackState) intToMPMusicPlaybackState:(int)val {
    switch (val) {
        case 0:
            return MPMusicPlaybackStateStopped;
            break;
        case 1:
            return  MPMusicPlaybackStatePlaying;
            break;
        case 2:
            return MPMusicPlaybackStatePaused;
            break;
        case 3:
            return MPMusicPlaybackStateInterrupted;
            break;
        case 4:
            return MPMusicPlaybackStateSeekingForward;
            break;
        case 5:
            return MPMusicPlaybackStateSeekingBackward;
            break;
            
    }
    
    return MPMusicPlaybackStateStopped;
}

+(MPMusicRepeatMode) intToMPMusicRepeatMode:(int)val {
    switch (val) {
        case 0:
            return MPMusicRepeatModeDefault;
            break;
        case 1:
            return  MPMusicRepeatModeNone;
            break;
        case 2:
            return MPMusicRepeatModeOne;
            break;
        case 3:
            return MPMusicRepeatModeAll;
            break;
    }
    
    return MPMusicRepeatModeDefault;
}

+(MPMusicShuffleMode) intToMPMusicShuffleMode:(int)val {
    switch (val) {
        case 0:
            return MPMusicShuffleModeDefault;
            break;
        case 1:
            return  MPMusicShuffleModeOff;
            break;
        case 2:
            return MPMusicShuffleModeSongs;
            break;
        case 3:
            return MPMusicShuffleModeAlbums;
            break;
    }
    
    return MPMusicShuffleModeDefault;
}



#pragma MPMediaPickerControllerDelegate


- (void)mediaPicker:(MPMediaPickerController *)mediaPicker didPickMediaItems:(MPMediaItemCollection *)mediaItemCollection {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN_Media: mediaPicker"];
    
    [[self vc] dismissViewControllerAnimated:YES completion:nil];
    [self updatePlayerQueueWithMediaCollection:mediaItemCollection];
}

- (void)mediaPickerDidCancel:(MPMediaPickerController *)mediaPicker {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN_Media: mediaPickerDidCancel"];
    
    [[self vc] dismissViewControllerAnimated:YES completion:nil];
    
    UnitySendMessage("ISN_MediaController", "OnMediaPickerFailed", [ISN_DataConvertor serializeErrorWithData:@"User Cancelled" code:0]);
}


#pragma mark Music notification handlers


- (void) handle_NowPlayingItemChanged: (id) notification {
    
    NSString* serializedItem = [self serializeMPMediaItemInfo:[musicPlayer nowPlayingItem]];
    UnitySendMessage("ISN_MediaController", "OnNowPlayingItemchanged", [ISN_DataConvertor NSStringToChar:serializedItem]);
    
 
    /*
    
    // Assume that there is no artwork for the media item.
    UIImage *artworkImage = nil;
    
    // Get the artwork from the current media item, if it has artwork.
    MPMediaItemArtwork *artwork = [currentItem valueForProperty: MPMediaItemPropertyArtwork];
    
    // Obtain a UIImage object from the MPMediaItemArtwork object
    if (artwork) {
        artworkImage = [artwork imageWithSize: CGSizeMake (30, 30)];
    }
     */

    
}

- (void) handle_PlaybackStateChanged: (id) notification {
    UnitySendMessage("ISN_MediaController", "OnPlaybackStateChanged", [ISN_DataConvertor NSStringToChar:[NSString stringWithFormat:@"%ld", (long)[musicPlayer playbackState]]]);
}

@end






@implementation ISNVideo

static ISNVideo * video_sharedInstance;

+ (id)sharedInstance {
    
    if (video_sharedInstance == nil)  {
        video_sharedInstance = [[self alloc] init];
    }
    
    return video_sharedInstance;
}





-(void) streamVideo:(NSString *)url {
    
    UIViewController *vc =  UnityGetGLViewController();
    
    
    NSURL *streamURL = [NSURL URLWithString:url];
    
    _streamPlayer = [[MPMoviePlayerViewController alloc] initWithContentURL:streamURL];
    
    [vc presentMoviePlayerViewControllerAnimated:self.streamPlayer];
    
    [self.streamPlayer.moviePlayer play];
    
}



-(void) openYouTubeVideo:(NSString *)url {
    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"openYouTubeVideo"];
    
    NSMutableString *str = [[NSMutableString alloc] init];
    [str appendString:@"http://www.youtube.com/v/"];
    [str appendString:url];
    
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:str]];
    
}


@end




extern "C" {
    
    void _ISN_InitMediaController() {
        [[ISN_Media sharedInstance] initialize];
    }
    
    void _ISN_SetRepeatMode(int mode) {
        [[ISN_Media sharedInstance] setRepeatMode:[ISN_Media intToMPMusicRepeatMode:mode]];
    }
    
    void _ISN_SetShuffleMode(int mode) {
        [[ISN_Media sharedInstance] setShuffleMode:[ISN_Media intToMPMusicShuffleMode:mode ]];
    }
    
    void _ISN_Play() {
        [[ISN_Media sharedInstance] play];
    }
    
    void _ISN_Pause() {
        [[ISN_Media sharedInstance] pause];
    }
    
    void _ISN_SkipToNextItem() {
        [[ISN_Media sharedInstance] skipToNextItem];
    }
    
    void _ISN_SkipToBeginning() {
        [[ISN_Media sharedInstance] skipToBeginning];
    }
    
    void _ISN_SkipToPreviousItem() {
        [[ISN_Media sharedInstance] skipToPreviousItem];
    }
    
    
    void _ISN_ShowMediaPicker() {
        [[ISN_Media sharedInstance] showMediaPicker];
    }
    
    void _ISN_SetCollection(char* itemsIds) {
        NSArray* ids = [ISN_DataConvertor charToNSArray:itemsIds];
        [[ISN_Media sharedInstance] setCollection:ids];
    }
    

    void ISN_MP_AddItemWithProductID(char* productID ) {
        NSString *pid = [ISN_DataConvertor charToNSString:productID];
        [[ISN_Media sharedInstance] addItemWithProductID:pid];
    }
    
    
    
    void _ISN_StreamVideo(char* videoUrl) {
        NSString *url = [ISN_DataConvertor charToNSString:videoUrl];
        [[ISNVideo sharedInstance] streamVideo:url];
    }
    
    void _ISN_OpenYouTubeVideo(char* videoUrl) {
        NSString *url = [ISN_DataConvertor charToNSString:videoUrl];
        [[ISNVideo sharedInstance] openYouTubeVideo:url];
    }
    
  
    
    ;

    
}


#endif



