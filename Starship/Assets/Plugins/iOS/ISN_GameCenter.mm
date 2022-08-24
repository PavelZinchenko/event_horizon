////////////////////////////////////////////////////////////////////////////////
//
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets)
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif



#import "ISN_NativeCore.h"


NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_SPLITTER2 = @"|%|";
NSString * const UNITY_EOF = @"endofline";

#if !TARGET_OS_TV

@interface ISN_SaveGame : NSObject


@property (nonatomic, strong) NSMutableDictionary* LoadedGameSaves;

- (void) saveGameData:(NSData *)data withName:(NSString *)name;
- (void) fetchSavedGames;
- (void) deleteSavedGames:(NSString *)name;
- (void) resolveConflictingSavedGames:(NSArray<NSString *> *)conflictingSavedGames  withData:(NSData *)data;
- (void) loadSaveData:(NSString*)name;
@end

@implementation ISN_SaveGame

static ISN_SaveGame *sg_sharedHelper = nil;
+ (ISN_SaveGame *) sharedInstance {
    if (!sg_sharedHelper) {
        sg_sharedHelper = [[ISN_SaveGame alloc] init];
        
    }
    return sg_sharedHelper;
}

- (id)init {
    self = [super init];
    if (self) {
        [self setLoadedGameSaves:[[NSMutableDictionary alloc] init]];
    }
    
    return self;
}

-(void) saveGameData:(NSData *)data withName:(NSString *)name {
    [[GKLocalPlayer localPlayer] saveGameData:data withName:name completionHandler:^(GKSavedGame * _Nullable savedGame, NSError * _Nullable error) {
        if(error == NULL) {
            UnitySendMessage("ISN_GameSaves", "OnSaveSuccess",  [ISN_DataConvertor NSStringToChar:[self SerlializeGameSave:savedGame]]);
        } else {
            UnitySendMessage("ISN_GameSaves", "OnSaveFailed",  [ISN_DataConvertor serializeError:error] );
        }
    }];
}


-(void) fetchSavedGames {
    [[GKLocalPlayer localPlayer] fetchSavedGamesWithCompletionHandler:^(NSArray<GKSavedGame *> * _Nullable savedGames, NSError * _Nullable error) {
        if(error == NULL) {
            NSMutableString *savesData = [[NSMutableString alloc] init];
            for (GKSavedGame *save in savedGames) {
                NSString *serializedData = [self SerlializeGameSave:save];
                [savesData appendString:serializedData];
                [savesData appendString:UNITY_SPLITTER2];
            }
            
            [savesData appendString:UNITY_EOF];
            
            UnitySendMessage("ISN_GameSaves", "OnFetchSuccess",  [ISN_DataConvertor NSStringToChar:savesData] );
            
        } else {
            UnitySendMessage("ISN_GameSaves", "OnFetchFailed",  [ISN_DataConvertor serializeError:error] );
        }
    }];
}

-(void) deleteSavedGames:(NSString *)name {
    [[GKLocalPlayer localPlayer] deleteSavedGamesWithName:name completionHandler:^(NSError * _Nullable error) {
        if(error == NULL) {
            UnitySendMessage("ISN_GameSaves", "OnDeleteSuccess",  [ISN_DataConvertor NSStringToChar:name]);
        } else {
            
            NSMutableString * data = [[NSMutableString alloc] init];
            [data appendString:name];
            [data appendString:UNITY_SPLITTER2];
            [data appendString:[ISN_DataConvertor serializeErrorToNSString:error]];
            
            UnitySendMessage("ISN_GameSaves", "OnDeleteFailed",  [ISN_DataConvertor NSStringToChar:data]);
        }
        
    }];
}

-(void) resolveConflictingSavedGames:(NSArray<NSString *> *)conflictingSavedGames withData:(NSData *)data {
    NSMutableArray <GKSavedGame *> * conflicts = [[NSMutableArray alloc] init];
    for (NSString *saveKey in conflictingSavedGames) {
        GKSavedGame *save = [[self LoadedGameSaves] objectForKey:saveKey];
        if(save != nil) {
            [conflicts addObject:save];
        }
    }
    
    [[GKLocalPlayer localPlayer] resolveConflictingSavedGames:conflicts withData:data completionHandler:^(NSArray<GKSavedGame *> * _Nullable savedGames, NSError * _Nullable error) {
        if(error == NULL) {
            NSMutableString *resolvingData = [[NSMutableString alloc] init];
            for (GKSavedGame *data in savedGames) {
                NSString *serializedData = [self SerlializeGameSave:data];
                [resolvingData appendString:serializedData];
                [resolvingData appendString:UNITY_SPLITTER2];
            }
            
            [resolvingData appendString:UNITY_EOF];
            
            UnitySendMessage("ISN_GameSaves", "OnResolveSuccess",  [ISN_DataConvertor NSStringToChar:resolvingData] );
        } else {
            UnitySendMessage("ISN_GameSaves", "OnResolveFailed",  [ISN_DataConvertor serializeError:error] );
        }
    }];
}



-(void) loadSaveData:(NSString *)name {
    GKSavedGame *save = [[self LoadedGameSaves] objectForKey:name];
    if(save != NULL) {
        [save loadDataWithCompletionHandler:^(NSData * _Nullable data, NSError * _Nullable error) {
            if(error == NULL) {
                
                NSMutableString * saveData = [[NSMutableString alloc] init];
                NSString *SaveKey = [self CacheGameSave:save];
                
                
                [saveData appendString:SaveKey];
                [saveData appendString:UNITY_SPLITTER2];
                [saveData appendString: [data base64Encoding]];
                
                UnitySendMessage("ISN_GameSaves", "OnSaveDataLoaded",  [ISN_DataConvertor NSStringToChar:saveData] );
                
            } else {
                
                NSMutableString * data = [[NSMutableString alloc] init];
                [data appendString:name];
                [data appendString:UNITY_SPLITTER2];
                [data appendString:[ISN_DataConvertor serializeErrorToNSString:error]];
                
                UnitySendMessage("ISN_GameSaves", "OnSaveDataLoadFailed",  [ISN_DataConvertor NSStringToChar:data]);
                
                
            }
        }];
    } else {
        
        NSMutableString * data = [[NSMutableString alloc] init];
        [data appendString:name];
        [data appendString:UNITY_SPLITTER2];
        [data appendString:[ISN_DataConvertor serializeErrorWithDataToNSString:@"save not found" code:999]];
        
        UnitySendMessage("ISN_GameSaves", "OnSaveDataLoadFailed", [ISN_DataConvertor NSStringToChar:data]);
    }
    
}


- (NSString*) SerlializeGameSave:(GKSavedGame *)save {
    NSMutableString * serializedData = [[NSMutableString alloc] init];
    
    NSString* key = [self CacheGameSave:save];
    
    [serializedData appendString:key];
    [serializedData appendString:UNITY_SPLITTER];
    [serializedData appendString:[save name]];
    [serializedData appendString:UNITY_SPLITTER];
    [serializedData appendString:[save deviceName]];
    [serializedData appendString:UNITY_SPLITTER];
    
    
    
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    
    [dateFormatter setDateFormat: @"yyyy-MM-dd HH:mm:ss"];
    NSString *dateString = [dateFormatter stringFromDate:[save modificationDate]];
    
    [serializedData appendString:dateString];
    
    return serializedData;
}

- (NSString*) CacheGameSave:(GKSavedGame *)save {
    
    NSMutableString * key = [[NSMutableString alloc] init];
    [key appendString:[save name]];
    [key appendString:[save deviceName]];
    
    NSString *dateString = [NSDateFormatter localizedStringFromDate:[save modificationDate] dateStyle:NSDateFormatterShortStyle  timeStyle:NSDateFormatterFullStyle];
    [key appendString:dateString];
    
    [[self LoadedGameSaves] setObject:save forKey:key];
    
    return  key;
}


@end

#endif


@interface ISN_GameCenterListner : NSObject <GKLocalPlayerListener> {
    BOOL isInited;
    
}

+ (id) sharedInstance;
-(void) subscribe;

@end




@protocol GameCenterManagerDelegate <NSObject>
@optional
- (void) processGameCenterAuth: (NSError*) error;
- (void) scoreReported: (NSError*) error;
- (void) reloadScoresComplete: (GKLeaderboard*) leaderboard error: (NSError*) error;
- (void) achievementSubmitted: (GKAchievement*) ach error:(NSError*) error;
- (void) achievementResetResult: (NSError*) error;
- (void) mappedPlayerIDToPlayer: (GKPlayer*) player error: (NSError*) error;
@end

#if !TARGET_OS_TV

@interface ISN_GameCenterManager : NSObject <GKGameCenterControllerDelegate, GKAchievementViewControllerDelegate> {
    NSMutableDictionary* earnedAchievementCache;
    
#else
    @interface ISN_GameCenterManager : NSObject<GKGameCenterControllerDelegate>  {
        NSMutableDictionary* earnedAchievementCache;
#endif
        
        
        
#if UNITY_VERSION < 500
        id <GameCenterManagerDelegate, NSObject> delegate;
#else
        id <GameCenterManagerDelegate, NSObject> __unsafe_unretained delegate;
#endif
    }
    
    //This property must be attomic to ensure that the cache is always in a viable state...
    @property (retain) NSMutableDictionary* earnedAchievementCache;
    @property (nonatomic, assign)  id <GameCenterManagerDelegate> delegate;
    @property (nonatomic, strong) UIViewController* GLViewController;
    
    
    + (ISN_GameCenterManager *) sharedInstance;
    
    - (void) getSignature;
    
    - (void) reportScore: (long long) score forCategory: (NSString*)category scoreContext: (long long) context;
    
    - (void) authenticateLocalPlayer;
    - (void) showLeaderboard: (NSString*)leaderboardId scope: (int) scope;
    - (void) loadLeaderboardInfo:(NSString *) identifier requestId:(int) requestId;
    - (void) retriveScores:(NSString*)category scope: (int) scope collection: (int) collection from: (int) from to: (int) to;
    
    
    
    - (void) savePlayerInfo:(GKPlayer*) player;
    - (void) loadPlayerInfoForPlayerWithId:(NSString *)playerId;
    - (void) loadImageForPlayerWithPlayerId:(NSString *)playerId size:(GKPhotoSize) size;
    -(GKPlayer*) getPlayerWithId:(NSString*) playerId;
    
    - (void) inviteFirends:(int) requestId emails:(NSArray*) emails players: (NSArray* ) players;
    
    
    - (NSString*) saveInvite:(GKInvite*) invite;
    - (NSString*) serialiseInvite:(GKInvite*) invite;
    - (GKInvite*) getInviteWithId:(NSString*)inviteId;
    
    
    - (void) sendLeaderboardChallenge:(NSString*) leaderboardId message:(NSString*) message playerIds: (NSArray*) playerIds;
    - (void) sendLeaderboardChallengeWithFriendsPicker:(NSString *)leaderboardId message:(NSString *)message;
    
    - (void) sendAchievementChallenge:(NSString*) achievementId  message:(NSString*) message playerIds: (NSArray*) playerIds;
    - (void) sendAchievementChallengeWithFriendsPicker:(NSString *)achievementId message:(NSString *)message;
    
    
    - (void) showAchievements;
    - (void) resetAchievements;
    - (void) submitAchievement: (double) percentComplete identifier: (NSString*) identifier notifyComplete: (BOOL) notifyComplete;
    
    - (void) retrieveFriends;
    - (void) loadLeaderboardSetInfo;
    - (void) loadLeaderboardsForSet:(NSString *)uid;
    
    - (void) showNotificationBanner: (NSString*) title message: (NSString*) message ;
    
    - (BOOL) isGameCenterAvailable;
    @end
    
    
    
    
    
    
    
    
    
    @interface ISN_GameCenterRTM : NSObject <GKMatchmakerViewControllerDelegate, GKMatchDelegate>
    
    
    @property (nonatomic, retain) UIViewController *vc;
    @property (nonatomic, strong) GKMatch* currentMatch;
    
    
    + (ISN_GameCenterRTM *)sharedInstance;
    
    - (void) initNotificationHandler;
    
    -(void) findMatch:(int)minPlayers maxPlayers:(int) maxPlayers inviteMessage:(NSString*) inviteMessage invitationsList:(NSArray*) invitationsList;
    -(void) findMatchWithNativeUI:(int)minPlayers maxPlayers:(int) maxPlayers inviteMessage:(NSString*) inviteMessage invitationsList:(NSArray*) invitationsList;
    -(void) startMatchWithInviteID:(NSString*) inviteId useNativeUI:(BOOL) useNativeUI;
    -(void) cancelPendingInviteToPlayerWithId:(NSString*) playerId;
    
    -(void) cancelMatchSeartch;
    -(void) finishMatchmaking;
    
    -(void) queryActivity;
    -(void) queryPlayerGroupActivity:(int) group;
    
    -(void) startBrowsingForNearbyPlayers;
    -(void) stopBrowsingForNearbyPlayers;
    
    -(void) rematch;
    -(void) disconnect;
    
    -(void) sendData:(NSData *)data toPlayersWithIds:(NSArray *)toPlayersWithIds withDataMode:(GKMatchSendDataMode)withDataMode;
    -(void) sendDataToAll:(NSData *)data withDataMode:(GKMatchSendDataMode)withDataMode;
    
    @end
    
    
    
    
    
    @interface ISN_GameCenterTBM : NSObject<GKTurnBasedMatchmakerViewControllerDelegate, GKTurnBasedEventListener>
    
    @property (nonatomic, retain) UIViewController *vc;
    
    + (id) sharedInstance;
    
    
    -(void) loadMatches;
    -(void) loadMatch:(NSString*) matchId;
    
    -(void) findMatch:(int)minPlayers maxPlayers:(int) maxPlayers inviteMessage:(NSString*) inviteMessage invitationsList:(NSArray*) invitationsList;
    -(void) findMatchWithNativeUI:(int)minPlayers maxPlayers:(int) maxPlayers inviteMessage:(NSString*) inviteMessage invitationsList:(NSArray*) invitationsList;
    
    -(void) saveCurrentTurn:(NSString*) matchId updatedMatchData: (NSData *) updatedMatchData;
    
    
    -(void) endTurn:(NSString*) matchId updatedMatchData: (NSData *) updatedMatchData nextPlayerId:(NSString*) nextPlayerId;
    
    
    -(void) quitInTurn: (NSString*) matchId outcome:(int) outcome  nextPlayerId:(NSString*)nextPlayerId matchData: (NSData *) matchData;
    -(void) quitOutOfTurn: (NSString*) matchId outcome:(int) outcome;
    
    -(void) updatePlayerOutcome: (NSString*) matchId Outcome:(int) Outcome playerId:(NSString*) playerId;
    -(void) endMatch:(NSString*) matchId matchData: (NSData *) matchData;
    -(void) rematch:(NSString*) matchId;
    -(void) removeMatch:(NSString*) matchId;
    
    -(void) acceptInvite:(NSString*) matchId;
    -(void) declineInvite:(NSString*) matchId;
    
    
    -(NSString*) serializeMathcData:(GKTurnBasedMatch *)match;
    -(void) updateMatchInfo:(GKTurnBasedMatch *)match;
    
    @end
    
    
    
    
    
    
    @implementation ISN_GameCenterListner
    
    
#pragma mark Initialization
    
    static ISN_GameCenterListner *gcl_sharedHelper = nil;
    + (ISN_GameCenterListner *) sharedInstance {
        if (!gcl_sharedHelper) {
            gcl_sharedHelper = [[ISN_GameCenterListner alloc] init];
            
        }
        return gcl_sharedHelper;
    }
    
    
    
    - (id)init {
        if ((self = [super init])) {
            isInited = false;
        }
        
        return self;
    }
    
    - (void) subscribe {
        
        if([ISN_NativeUtility majorIOSVersion] >= 8) {
            GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
            [localPlayer registerListener:self];
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"subscribed"];
        }
    }
    
#pragma GKInviteEventListener
    
    
    // player:didAcceptInvite: gets called when another player accepts the invite from the local player
    - (void)player:(GKPlayer *)player didAcceptInvite:(GKInvite *)invite  {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GKInviteEventListener::didAcceptInvite"];
        // UnitySendMessage("GameCenterInvitations", "OnPlayerAcceptedInvitation_TBM", [ISNDataConvertor NSStringToChar:[[ISN_GameCenterManager sharedInstance] serialiseInvite:invite]]);
    }
    
    // didRequestMatchWithRecipients: gets called when the player chooses to play with another player from Game Center and it launches the game to start matchmaking
    - (void)player:(GKPlayer *)player didRequestMatchWithRecipients:(NSArray *)recipientPlayers  {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GKInviteEventListener::didRequestMatchWithRecipients"];
        
        NSMutableArray* requestedInvitationsArray = [[NSMutableArray alloc] init];
        for(NSObject* playerInfo in recipientPlayers) {
            
            if([playerInfo respondsToSelector:@selector(playerID)] ) {
                GKPlayer *player = (GKPlayer*) playerInfo;
                [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
                [requestedInvitationsArray addObject:player.playerID];
                
            } else {
                
                NSString *PlayerId =  (NSString*) playerInfo;
                [[ISN_GameCenterManager sharedInstance] loadPlayerInfoForPlayerWithId:PlayerId];
                [requestedInvitationsArray addObject:PlayerId];
            }
            
        }
        
        UnitySendMessage("GameCenterInvitations", "OnPlayerRequestedMatchWithRecipients_TBM", [ISN_DataConvertor NSStringsArrayToChar:requestedInvitationsArray]);
        
    }
    
    
    
#pragma GKTurnBasedEventListener
    
    
    // If Game Center initiates a match the developer should create a GKTurnBasedMatch from playersToInvite and present a GKTurnbasedMatchmakerViewController.
    - (void)player:(GKPlayer *)player didRequestMatchWithOtherPlayers:(NSArray *)playersToInvite {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GKTurnBasedEventListener::didRequestMatchWithOtherPlayers"];
        
        NSMutableArray* requestedInvitationsArray = [[NSMutableArray alloc] init];
        for(GKPlayer* player in playersToInvite) {
            [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
            [requestedInvitationsArray addObject:player.playerID];
        }
        
        UnitySendMessage("GameCenterInvitations", "OnPlayerRequestedMatchWithRecipients_TBM", [ISN_DataConvertor NSStringsArrayToChar:requestedInvitationsArray]);
        
    }
    
    // called when it becomes this player's turn.  It also gets called under the following conditions:
    //      the player's turn has a timeout and it is about to expire.
    //      the player accepts an invite from another player.
    // when the game is running it will additionally recieve turn events for the following:
    //      turn was passed to another player
    //      another player saved the match data
    // Because of this the app needs to be prepared to handle this even while the player is taking a turn in an existing match.  The boolean indicates whether this event launched or brought to forground the app.
    
    
    - (void)player:(GKPlayer *)player receivedTurnEventForMatch:(GKTurnBasedMatch *)match didBecomeActive:(BOOL)didBecomeActive {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GKTurnBasedEventListener::receivedTurnEventForMatch"];
        
        [[ISN_GameCenterTBM sharedInstance] updateMatchInfo:match];
        NSString* matchData = [[ISN_GameCenterTBM sharedInstance] serializeMathcData:match];
        UnitySendMessage("GameCenter_TBM", "OnTrunReceived", [ISN_DataConvertor NSStringToChar:matchData]);
        
    }
    
    // called when the match has ended.
    - (void)player:(GKPlayer *)player matchEnded:(GKTurnBasedMatch *)match {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GKTurnBasedEventListener::matchEnded"];
        
        [[ISN_GameCenterTBM sharedInstance] updateMatchInfo:match];
        
        NSString* matchData = [[ISN_GameCenterTBM sharedInstance] serializeMathcData:match];
        UnitySendMessage("GameCenter_TBM", "OnEndMatch", [ISN_DataConvertor NSStringToChar:matchData]);
    }
    
    
    
    
    
    // this is called when a player receives an exchange request from another player.
    - (void)player:(GKPlayer *)player receivedExchangeRequest:(GKTurnBasedExchange *)exchange forMatch:(GKTurnBasedMatch *)match {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GKTurnBasedEventListener::receivedExchangeRequest"];
    }
    
    // this is called when an exchange is canceled by the sender.
    - (void)player:(GKPlayer *)player receivedExchangeCancellation:(GKTurnBasedExchange *)exchange forMatch:(GKTurnBasedMatch *)match  {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GKTurnBasedEventListener::receivedExchangeCancellation"];
    }
    
    // called when all players either respond or timeout responding to this request.  This is sent to both the turn holder and the initiator of the exchange
    - (void)player:(GKPlayer *)player receivedExchangeReplies:(NSArray *)replies forCompletedExchange:(GKTurnBasedExchange *)exchange forMatch:(GKTurnBasedMatch *)match {
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GKTurnBasedEventListener::receivedExchangeReplies"];
    }
    
    @end
    
    
    
    
    
    @implementation ISN_GameCenterManager
    
    static NSMutableDictionary *loadedPlayers;
    
    static NSArray *loadedLeaderboardsSets;
    static NSMutableDictionary* receivedInvites = nil;
    static GKGameCenterViewController* leaderbaordsView;
    
#if !TARGET_OS_TV
    static GKAchievementViewController* achievementView;
#endif
    
    
    
    
    static ISN_GameCenterManager * gc_sharedInstance;
    
    @synthesize earnedAchievementCache;
    @synthesize delegate;
    
    
#pragma init
    
    
    + (id)sharedInstance {
        if (gc_sharedInstance == nil)  {
            gc_sharedInstance = [[self alloc] init];
        }
        
        return gc_sharedInstance;
    }
    
    
    
    - (id)init {
        self = [super init];
        if (self) {
            earnedAchievementCache= NULL;
            loadedPlayers = [[NSMutableDictionary alloc] init];
            
#if UNITY_VERSION < 500
            [loadedPlayers retain];
#endif
            
            [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationDidEnterBackground:) name:UIApplicationDidEnterBackgroundNotification object:nil];
            
            
            [self setGLViewController:UnityGetGLViewController()];
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN IOSGameCenterManager initialized"];
        }
        
        return self;
    }
    
    
    
    - (void)dealloc {
        self.earnedAchievementCache= NULL;
#if UNITY_VERSION < 500
        [super dealloc];
#endif
    }
    
    
    
    -(void) sendPlayerAuthenticationFailedEvent: (NSError*) error {
        UnitySendMessage("GameCenterManager", "onAuthenticationFailed", [ISN_DataConvertor serializeError:error]);
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GK PLAYER AUTHENTICATION ERROR: %@", error.description];
    }
    
    -(void) sendPlayerAuthenticatedEvent {
        NSMutableString * data = [[NSMutableString alloc] init];
        
        GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
        
        if(localPlayer.playerID != nil) {
            [data appendString:localPlayer.playerID];
        } else {
            [data appendString:@""];
        }
        [data appendString:UNITY_SPLITTER];
        
        
        if(localPlayer.displayName != nil) {
            [data appendString:localPlayer.displayName];
        } else {
            [data appendString:@""];
        }
        [data appendString:UNITY_SPLITTER];
        
        NSString* alias = @"";
        if(localPlayer.alias != nil) {
            alias =localPlayer.alias;
        }
        [data appendString:alias];
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GK PLAYER AUTHENTICATED %@", alias];
        UnitySendMessage("GameCenterManager", "onAuthenticateLocalPlayer", [ISN_DataConvertor NSStringToChar:data]);
        
        [[ISN_GameCenterRTM sharedInstance] initNotificationHandler];
        [[ISN_GameCenterRTM sharedInstance] setVc:UnityGetGLViewController()];
        [[ISN_GameCenterTBM sharedInstance] setVc:UnityGetGLViewController()];
        
        [[ISN_GameCenterListner sharedInstance] subscribe];
        
        
        [self savePlayerInfo:localPlayer];
        
        
    }
    
    - (void) authenticateLocalPlayer {
        
        NSLog(@"authenticateLocalPlayer");
        
        GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
        
        [localPlayer setAuthenticateHandler:(^(UIViewController* viewcontroller, NSError *error) {
            
            if (!error && viewcontroller) {
                [[self GLViewController]  presentViewController:viewcontroller animated:YES completion:nil];
            } else {
                
                if(localPlayer.playerID != NULL) {
                    [self sendPlayerAuthenticatedEvent];
                } else {
                    [self sendPlayerAuthenticationFailedEvent:error];
                }
            }
        })];
        
    }
    
    
    -(void) getSignature {
        GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
        [localPlayer generateIdentityVerificationSignatureWithCompletionHandler:^(NSURL *publicKeyUrl, NSData *signature, NSData *salt, uint64_t timestamp, NSError *error) {
            
            if(error != nil) {
                [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"error: %@", error.description];
                
                NSMutableString * ErrorData = [[NSMutableString alloc] init];
                [ErrorData appendFormat:@"%li", (long)error.code];
                [ErrorData appendString:@"| "];
                [ErrorData appendString:error.description];
                
                
                
                
                NSString *ErrorDataString = [ErrorData copy];
#if UNITY_VERSION < 500
                [ErrorDataString autorelease];
#endif
                UnitySendMessage("GameCenterManager", "VerificationSignatureRetrieveFailed", [ISN_DataConvertor NSStringToChar:ErrorDataString]);
                return;
            }
            
            NSMutableString *sig = [[NSMutableString alloc] init];
            const char *db = (const char *) [signature bytes];
            for (int i = 0; i < [signature length]; i++) {
                if(i != 0) {
                    [sig appendFormat:@","];
                }
                
                [sig appendFormat:@"%i", (unsigned char)db[i]];
            }
            
            
            NSMutableString *slt = [[NSMutableString alloc] init];
            const char *db2 = (const char *) [salt bytes];
            for (int i = 0; i < [salt length]; i++) {
                if(i != 0) {
                    [slt appendFormat:@","];
                }
                
                [slt appendFormat:@"%i", (unsigned char)db2[i]];
            }
            
            
            
            
            
            NSString *path = [[NSString alloc] initWithString:[publicKeyUrl absoluteString]];
            
            
            
            
            
            NSMutableString * array = [[NSMutableString alloc] init];
            [array appendString:path];
            [array appendString:@"| "];
            [array appendString:sig];
            [array appendString:@"| "];
            [array appendString:slt];
            [array appendString:@"| "];
            [array appendFormat:@"%llu", timestamp];
            
            
            NSString *str = [array copy];
#if UNITY_VERSION < 500
            [str autorelease];
#endif
            UnitySendMessage("GameCenterManager", "VerificationSignatureRetrieved", [ISN_DataConvertor NSStringToChar:str]);
            
            
            
        }];
    }
    
    
    
    
    
    //--------------------------------------
    // Leaderbaords
    //--------------------------------------
    
    - (void) showLeaderboardsPopUp {
        
        UIViewController *vc =  UnityGetGLViewController();
        if([vc presentedViewController] != nil) {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Show Leaderboards Denied"];
            return;
        }
        
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Show Leaderboards"];
        
        
        leaderbaordsView = [[GKGameCenterViewController alloc] init];
        leaderbaordsView.gameCenterDelegate = self;
        
#if !TARGET_OS_TV
        leaderbaordsView.viewState = GKGameCenterViewControllerStateLeaderboards;
#endif
        
        CGSize screenSize = [[UIScreen mainScreen] bounds].size;
        
        [vc presentViewController: leaderbaordsView animated: YES completion:nil];
        leaderbaordsView.view.transform = CGAffineTransformMakeRotation(0.0f);
        [leaderbaordsView.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
        leaderbaordsView.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
        
        
        
        
    }
    
    - (void) showLeaderboard:(NSString *)leaderboardId scope:(int)scope {
        
        
        UIViewController *vc =  UnityGetGLViewController();
        if([vc presentedViewController] != nil) {
            [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Show Leaderboards Denied"];
            return;
        }
        
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Show Leaderboard: %@", leaderboardId];
        
        leaderbaordsView = [[GKGameCenterViewController alloc] init];
        leaderbaordsView.gameCenterDelegate = self;
        
        
#if !TARGET_OS_TV
        
        
        leaderbaordsView.leaderboardIdentifier = leaderboardId;
        
        switch (scope) {
            case 2:
                leaderbaordsView.leaderboardTimeScope = GKLeaderboardTimeScopeAllTime;
                break;
            case 1:
                leaderbaordsView.leaderboardTimeScope = GKLeaderboardTimeScopeWeek;
                break;
            case 0:
                leaderbaordsView.leaderboardTimeScope = GKLeaderboardTimeScopeToday;
                break;
                
            default:
                leaderbaordsView.leaderboardTimeScope = GKLeaderboardTimeScopeAllTime;
                break;
        }
        
        leaderbaordsView.viewState = GKGameCenterViewControllerStateLeaderboards;
#endif
        
        
        CGSize screenSize = [[UIScreen mainScreen] bounds].size;
        
        [vc presentViewController: leaderbaordsView animated: YES completion:nil];
        leaderbaordsView.view.transform = CGAffineTransformMakeRotation(0.0f);
        [leaderbaordsView.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
        leaderbaordsView.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
    }
    
    - (void) reportScore:(long long)score forCategory:(NSString *)category scoreContext:(long long)context {
        
        
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN reportScore: %lld", score];
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN category %@", category];
        [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN scoreContext %lld", context];
        
        
#if !TARGET_OS_TV
        
        
        GKScore *scoreReporter = [[GKScore alloc] initWithCategory:category] ;
#else
        GKScore *scoreReporter = [[GKScore alloc] initWithLeaderboardIdentifier:category] ;
#endif
        
        
#if UNITY_VERSION < 500
        [scoreReporter autorelease];
#endif
        scoreReporter.value = score;
        scoreReporter.context = context;
        
#if !TARGET_OS_TV
        [scoreReporter reportScoreWithCompletionHandler: ^(NSError *error) {
#else
            NSMutableArray *scoresToReport = [[NSMutableArray alloc] init];
            [scoresToReport addObject:scoreReporter];
            [GKScore reportScores:scoresToReport withCompletionHandler:^(NSError * _Nullable error) {
#endif
                
                NSMutableString * data = [[NSMutableString alloc] init];
                [data appendString:category];
                [data appendString:@"|%|"];
                [data appendString:[NSString stringWithFormat: @"%lld", score]];
                [data appendString:@"|%|"];
                [data appendString:[NSString stringWithFormat: @"%lld", context]];
                
                
                
                if (error != nil) {
                    [data appendString:@"|%|"];
                    [data appendString:[ISN_DataConvertor serializeErrorToNSString:error]];
                    NSString *str = [data copy];
                    
#if UNITY_VERSION < 500
                    [str autorelease];
#endif
                    
                    
                    
                    UnitySendMessage("GameCenterManager", "onScoreSubmittedFailed", [ISN_DataConvertor NSStringToChar:str]);
                    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Error while submitting score: %@", error.description];
                } else {
                    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN new high score sumbitted success: %lld", score];
                    [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN GK_Score context: %lld", context];
                    NSString *str = [data copy];
                    
#if UNITY_VERSION < 500
                    [str autorelease];
#endif
                    UnitySendMessage("GameCenterManager", "onScoreSubmittedEvent", [ISN_DataConvertor NSStringToChar:str]);
                    
                    
                }
                
            }];
            
        }
         
         -(void) loadLeaderboardInfo:(NSString *) identifier requestId:(int)requestId {
             
             [self retrieveScoreForLocalPlayerWithCategory:identifier scope:0 collection:0 requestId: requestId];
             [self retrieveScoreForLocalPlayerWithCategory:identifier scope:0 collection:1 requestId: requestId];
             
             [self retrieveScoreForLocalPlayerWithCategory:identifier scope:1 collection:0 requestId: requestId];
             [self retrieveScoreForLocalPlayerWithCategory:identifier scope:1 collection:1 requestId: requestId];
             
             [self retrieveScoreForLocalPlayerWithCategory:identifier scope:2 collection:0 requestId: requestId];
             [self retrieveScoreForLocalPlayerWithCategory:identifier scope:2 collection:1 requestId: requestId];
             
         }
         
         -(void) retrieveScoreForLocalPlayerWithCategory:(NSString *)category scope:(int)scope collection:(int)collection requestId:(int)requestId {
             
             GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
#if UNITY_VERSION < 500
             [leaderboardRequest autorelease];
#endif
             leaderboardRequest.identifier = category;
             leaderboardRequest.timeScope = [self IntToTimeScope:scope];
             leaderboardRequest.playerScope = [self IntToPlayerScope:collection];
             
             
             if (leaderboardRequest != nil) {
                 
                 NSMutableString * data = [[NSMutableString alloc] init];
                 [data appendString:category];
                 [data appendString:@"|%|"];
                 [data appendString:[NSString stringWithFormat:@"%d", scope]];
                 [data appendString:@"|%|"];
                 [data appendString:[NSString stringWithFormat:@"%d", collection]];
                 [data appendString:@"|%|"];
                 [data appendString:[NSString stringWithFormat:@"%d", requestId]];
                 [data appendString:@"|%|"];
                 
                 
                 [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
                     if (error != nil) {
                         [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Error scores loading %@", error.description];
                         
                         [data appendString:[ISN_DataConvertor serializeErrorToNSString:error]];
                         
                         UnitySendMessage("GameCenterManager", "OnLoaderBoardInfoRetrivedFail", [ISN_DataConvertor NSStringToChar:data]);
                         
                     }  else {
                         
                         
                         
                         
                         [data appendString:[NSString stringWithFormat:@"%lld", leaderboardRequest.localPlayerScore.value]];
                         [data appendString:@"|%|"];
                         [data appendString:[NSString stringWithFormat:@"%ld", (long)leaderboardRequest.localPlayerScore.rank]];
                         [data appendString:@"|%|"];
                         [data appendString:[NSString stringWithFormat:@"%lld", (long long)leaderboardRequest.localPlayerScore.context]];
                         [data appendString:@"|%|"];
                         [data appendString:[NSString stringWithFormat:@"%lu", (unsigned long)leaderboardRequest.maxRange]];
                         [data appendString:@"|%|"];
                         if(leaderboardRequest.title != nil) {
                             [data appendString:leaderboardRequest.title];
                         } else {
                             [data appendString:@""];
                         }
                         
                         [data appendString:@"|%|"];
                         if(leaderboardRequest.description != nil) {
                             [data appendString:leaderboardRequest.description];
                         } else {
                             [data appendString:@""];
                         }
                         
                         NSString *str = [data copy];
#if UNITY_VERSION < 500
                         [str autorelease];
#endif
                         
                         UnitySendMessage("GameCenterManager", "OnLoaderBoardInfoRetrived", [ISN_DataConvertor NSStringToChar:str]);
                         
                         
                         
                         [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Retrieved localScore:%lld",leaderboardRequest.localPlayerScore.value];
                     }
                 }];
             }
         }
         
         
         
         -(void) retriveScores:(NSString *)category scope:(int)scope collection: (int) collection from:(int)from to:(int)to {
             GKLeaderboard *board = [[GKLeaderboard alloc] init];
             
             if(board != nil) {
                 
                 
                 board.range = NSMakeRange(from, to);
                 board.identifier = category;
                 board.timeScope = [self IntToTimeScope:scope];
                 board.playerScope = [self IntToPlayerScope:collection];
                 
                 NSMutableString * data = [[NSMutableString alloc] init];
                 [data appendString:category];
                 [data appendString:@"|%|"];
                 [data appendString:[NSString stringWithFormat:@"%d", scope]];
                 [data appendString:@"|%|"];
                 [data appendString:[NSString stringWithFormat:@"%d", collection]];
                 [data appendString:@"|%|"];
                 
                 
                 [board loadScoresWithCompletionHandler: ^(NSArray *scores, NSError *error) {
                     
                     
                     if (error != nil) {
                         // handle the error.
                         [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Error retrieving score: %@", error.description];
                         
                         [data appendString:[ISN_DataConvertor serializeErrorToNSString:error]];
                         UnitySendMessage("GameCenterManager", "OnLeaderboardScoreListLoadFailed", [ISN_DataConvertor NSStringToChar:data]);
                         return;
                     }
                     
                     if(scores == nil) {
                         [data appendString:[ISN_DataConvertor serializeErrorWithDataToNSString:@"No Score avaliable" code:999]];
                         UnitySendMessage("GameCenterManager", "OnLeaderboardScoreListLoadFailed", [ISN_DataConvertor NSStringToChar:data]);
                         return;
                     }
                     
                     if(scores.count == 0) {
                         [data appendString:[ISN_DataConvertor serializeErrorWithDataToNSString:@"No Score avaliable" code:999]];
                         UnitySendMessage("GameCenterManager", "OnLeaderboardScoreListLoadFailed", [ISN_DataConvertor NSStringToChar:data]);
                         return;
                         
                     }
                     
                     
                     BOOL first = YES;
                     
                     NSEnumerator *e = [scores objectEnumerator];
                     id object;
                     while (object = [e nextObject]) {
                         GKScore* s =((GKScore*) object);
                         
                         
                         if(!first) {
                             [data appendString:@"|%|"];
                         }
                         
#if !TARGET_OS_TV
                         [data appendString:s.playerID];
#else
                         if(s.player != NULL) {
                             [data appendString:s.player.playerID];
                         } else {
                             [data appendString:@""];
                         }
                         
#endif
                         [data appendString:@"|%|"];
                         
                         [data appendString:[NSString stringWithFormat:@"%lld", s.value]];
                         [data appendString:@"|%|"];
                         
                         [data appendString:[NSString stringWithFormat:@"%ld", (long)s.rank]];
                         [data appendString:@"|%|"];
                         
                         [data appendString:[NSString stringWithFormat:@"%lld", (long long)s.context]];
                         [self savePlayerInfo:s.player];
                         
                         first = NO;
                     }
                     
                     UnitySendMessage("GameCenterManager", "OnLeaderboardScoreListLoaded", [ISN_DataConvertor NSStringToChar:data]);
                     
                     
                 }];
             }
             
#if UNITY_VERSION < 500
             [board release];
#endif
             
         }
         
         -(GKLeaderboardTimeScope) IntToTimeScope:(int) value {
             switch (value) {
                 case 2:
                     return GKLeaderboardTimeScopeAllTime;
                 case 1:
                     return GKLeaderboardTimeScopeWeek;
                     break;
                 case 0:
                     return GKLeaderboardTimeScopeToday;
                     
                 default:
                     return GKLeaderboardTimeScopeAllTime;
             }
         }
         
         -(GKLeaderboardPlayerScope) IntToPlayerScope:(int) value {
             switch (value) {
                 case 1:
                     return GKLeaderboardPlayerScopeGlobal;
                 case 0:
                     return GKLeaderboardPlayerScopeFriendsOnly;
                     break;
                     
                 default:
                     return GKLeaderboardPlayerScopeFriendsOnly;
             }
         }
         
         
         
         
         - (void) sendLeaderboardChallengeWithFriendsPicker:(NSString *)leaderboardId message:(NSString *)message {
             
#if !TARGET_OS_TV
             
             GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
#if UNITY_VERSION < 500
             [leaderboardRequest autorelease];
#endif
             
             
             leaderboardRequest.category = leaderboardId;
             
             leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
             
             
             
             if (leaderboardRequest != nil) {
                 
                 [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
                     if (error != nil) {
                         [               [ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Error challenge scores loading %@", error.description];
                     }  else {
                         
                         UIViewController *composeVC = [leaderboardRequest.localPlayerScore challengeComposeControllerWithPlayers:nil message:message completionHandler:^(UIViewController *composeController, BOOL didIssueChallenge, NSArray *sentPlayerIDs){
                             
                             
                             [composeController dismissViewControllerAnimated:YES completion:nil];
                             
                             [composeController.view.superview removeFromSuperview];
                             
                         }];
                         
                         
                         UIViewController *vc =  UnityGetGLViewController();
                         
                         [vc presentViewController: composeVC animated: YES completion:nil];
                         
                         
                         
                         
                     }
                 }];
             }
#endif
             
         }
         
         
         -(void) sendLeaderboardChallenge:(NSString *)leaderboardId message:(NSString *)message playerIds:(NSArray *)playerIds {
             
#if !TARGET_OS_TV
             
             GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
#if UNITY_VERSION < 500
             [leaderboardRequest autorelease];
#endif
             leaderboardRequest.category = leaderboardId;
             
             leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
             
             
             if (leaderboardRequest != nil) {
                 
                 [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
                     if (error != nil) {
                         [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Error challenge scores loading"];
                     }  else {
                         
                         [leaderboardRequest.localPlayerScore issueChallengeToPlayers:playerIds message:message];
                         
                     }
                 }];
             }
             
#endif
             
         }
         
         
         
         -(void) loadLeaderboardSetInfo {
             [GKLeaderboardSet loadLeaderboardSetsWithCompletionHandler:^(NSArray *leaderboardSets, NSError *error) {
                 
                 if(error != NULL) {
                     [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Error loadLeaderboardSetInfo loading %@", error.description];
                     
                     UnitySendMessage("GameCenterManager", "ISN_OnLBSetsLoadFailed", "");
                 } else {
                     NSMutableString * data = [[NSMutableString alloc] init];
                     BOOL first = YES;
                     for (GKLeaderboardSet *lb in leaderboardSets) {
                         if(!first) {
                             [data appendString:@"|"];
                         }
                         first = NO;
                         [data appendString:lb.title];
                         [data appendString:@"|"];
                         [data appendString:lb.identifier];
                         [data appendString:@"|"];
                         
                         if(lb.groupIdentifier != nil) {
                             [data appendString:lb.groupIdentifier];
                         } else {
                             [data appendString:@""];
                         }
                         
                     }
                     
                     
                     loadedLeaderboardsSets = leaderboardSets;
                     
                     
                     
                     NSString *str = [data copy];
#if UNITY_VERSION < 500
                     [str autorelease];
#endif
                     UnitySendMessage("GameCenterManager", "ISN_OnLBSetsLoaded", [ISN_DataConvertor NSStringToChar:str]);
                 }
                 
             }];
         }
         
         
         -(void) loadLeaderboardsForSet:(NSString *)uid {
             for (GKLeaderboardSet *lb in loadedLeaderboardsSets) {
                 if([lb.identifier isEqualToString:uid] ) {
                     
                     [lb loadLeaderboardsWithCompletionHandler:^(NSArray *leaderboards, NSError *error) {
                         if(error != NULL) {
                             [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Error loadLeaderboardsWithCompletionHandler loading %@", error.description];
                             
                             UnitySendMessage("GameCenterManager", "ISN_OnLBSetsBoardsLoadFailed", [ISN_DataConvertor NSStringToChar:lb.identifier]);
                         } else {
                             
                             
                             NSMutableString * data = [[NSMutableString alloc] init];
                             [data appendString:lb.identifier];
                             
                             BOOL first = YES;
                             for (GKLeaderboard *l in leaderboards) {
                                 if(!first) {
                                     [data appendString:@"|"];
                                 }
                                 first = NO;
                                 [data appendString:l.title];
                                 [data appendString:@"|"];
                                 [data appendString:l.description];
                                 [data appendString:@"|"];
                                 [data appendString:l.identifier];
                                 
                                 
                             }
                             
                             NSString *str = [data copy];
#if UNITY_VERSION < 500
                             [str autorelease];
#endif
                             
                             UnitySendMessage("GameCenterManager", "ISN_OnLBSetsBoardsLoaded", [ISN_DataConvertor NSStringToChar:str]);
                             
                         }
                         
                     }];
                     
                     return;
                 }
             }
             
             
         }
         
         
         
#pragma achivments
         
         
         -(void) resetAchievements {
             self.earnedAchievementCache= NULL;
             [GKAchievement resetAchievementsWithCompletionHandler: ^(NSError *error)  {
                 
                 if(error != nil) {
                     [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN resetAchievements failed: %@", error.description];
                     
                     UnitySendMessage("GameCenterManager", "onAchievementsResetFailed", [ISN_DataConvertor serializeError:error]);
                 } else {
                     [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN resetAchievements complete"];
                     
                     UnitySendMessage("GameCenterManager", "onAchievementsReset", "");
                     
                 }
                 
             }];
         }
         
         
         
         -(void) submitAchievement:(double)percentComplete identifier:(NSString *)identifier  notifyComplete: (BOOL) notifyComplete {
             
             
             //GameCenter check for duplicate achievements when the achievement is submitted, but if you only want to report
             // new achievements to the user, then you need to check if it's been earned
             // before you submit.  Otherwise you'll end up with a race condition between loadAchievementsWithCompletionHandler
             // and reportAchievementWithCompletionHandler.  To avoid this, we fetch the current achievement list once,
             // then cache it and keep it updated with any new achievements.
             [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN submitAchievement %@", identifier];
             
             if(self.earnedAchievementCache == NULL) {
                 [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN loading Achievements cache...."];
                 
                 [GKAchievement loadAchievementsWithCompletionHandler: ^(NSArray *scores, NSError *error) {
                     if(error == NULL)  {
                         NSMutableDictionary* tempCache= [NSMutableDictionary dictionaryWithCapacity: [scores count]];
                         for (GKAchievement* score in tempCache) {
                             [tempCache setObject: score forKey: score.identifier];
                         }
                         
                         
                         
                         
                         self.earnedAchievementCache= tempCache;
                         [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Achievements cache loaded, resubmitting achievement"];
                         
                         [self submitAchievement:percentComplete identifier:identifier notifyComplete:notifyComplete];
                         
                     }
                     else{
                         [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Achievements cache load error: %@", error.description];
                     }
                     
                 }];
             } else {
                 //Search the list for the ID we're using...
                 GKAchievement* achievement= [self.earnedAchievementCache objectForKey: identifier];
                 if(achievement != NULL) {
                     if((achievement.percentComplete >= 100.0) || (achievement.percentComplete >= percentComplete)) {
                         //Achievement has already been earned so we're done.
                         achievement= NULL;
                     }
                     
                     achievement.percentComplete= percentComplete;
                 } else {
                     
                     achievement= [[GKAchievement alloc] initWithIdentifier: identifier];
                     achievement.showsCompletionBanner = notifyComplete;
                     
                     if(percentComplete > 100.0) {
                         percentComplete = 100.0;
                     }
                     
                     achievement.percentComplete= percentComplete;
                     
                     //Add achievement to achievement cache...
                     [self.earnedAchievementCache setObject: achievement forKey: achievement.identifier];
                 }
                 
                 if(achievement!= NULL) {
                     [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Submit Achievement"];
                     //Submit the Achievement...
                     

#if !TARGET_OS_TV
                    [achievement reportAchievementWithCompletionHandler: ^(NSError *error) {
#else
                    
                        
                    NSMutableArray *achievmentsList =  [[NSMutableArray alloc] init];
                    [achievmentsList addObject:achievement];
                        
                    [GKAchievement reportAchievements:achievmentsList withCompletionHandler:^(NSError * _Nullable error) {
#endif
                             if(error == NULL) {
                                 NSMutableString * data = [[NSMutableString alloc] init];
                                 [data appendString:achievement.identifier];
                                 [data appendString:UNITY_SPLITTER];
                                 [data appendString:[NSString stringWithFormat:@"%f", achievement.percentComplete]];
                                 
                                 
                                 NSString *str = [data copy];
                                 
                                 [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Submitted"];
                                 [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN identifier: %@", achievement.identifier];
                                 [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN percentComplete: %f", achievement.percentComplete];
                                 
                                 UnitySendMessage("GameCenterManager", "onAchievementProgressChanged", [ISN_DataConvertor NSStringToChar:str]);
                             } else {
                                 [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Submit failed with error %@", error.description];
                             }
                         }];
                     }
                      }
                      
                      
                      }
                      
                      
                      
                      
                      
                      - (void) showAchievements {
                          
                          UIViewController *vc =  UnityGetGLViewController();
                          if([vc presentedViewController] != nil) {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Show Achievements Denied"];
                              return;
                          }
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Show Achievements"];
                          
                          
#if !TARGET_OS_TV
                          
                          
                          achievementView = [[GKAchievementViewController alloc] init];
                          achievementView.viewState = GKGameCenterViewControllerStateAchievements;
                          
                          achievementView.achievementDelegate = self;
                          
                          CGSize screenSize = [[UIScreen mainScreen] bounds].size;
                          [vc presentViewController: achievementView animated: YES completion:nil];
                          
                          achievementView.view.transform = CGAffineTransformMakeRotation(0.0f);
                          [achievementView.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
                          achievementView.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
                          
#else
                          
                          leaderbaordsView  = [[GKGameCenterViewController alloc] init];
                          leaderbaordsView.gameCenterDelegate = self;
                          [vc presentViewController: leaderbaordsView animated: YES completion:nil];
#endif
                          
                      }
                      
                      
                      - (void) loadAchievements {
                          [GKAchievement loadAchievementsWithCompletionHandler:^(NSArray *achievements, NSError *error) {
                              if (error == nil) {
                                  [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN loadAchievementsWithCompletionHandler"];
                                  [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN count %lu", (unsigned long)achievements.count];
                                  
                                  NSMutableString * data = [[NSMutableString alloc] init];
                                  BOOL first = YES;
                                  for (GKAchievement* achievement in achievements) {
                                      
                                      
                                      if(!first) {
                                          [data appendString:UNITY_SPLITTER];
                                      }
                                      
                                      
                                      [data appendString:achievement.identifier];
                                      [data appendString:UNITY_SPLITTER];
                                      
                                      [data appendString:achievement.description];
                                      [data appendString:UNITY_SPLITTER];
                                      
                                      [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN achievement.percentComplete:  %f", achievement.percentComplete];
                                      
                                      [data appendString:[NSString stringWithFormat:@"%f", achievement.percentComplete]];
                                      
                                      first = NO;
                                      
                                  }
                                  
                                  NSString *str = [data copy];
                                  
#if UNITY_VERSION < 500
                                  [str autorelease];
#endif
                                  
                                  UnitySendMessage("GameCenterManager", "onAchievementsLoaded", [ISN_DataConvertor NSStringToChar:str]);
                              } else {
                                  [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN loadAchievements failed:  %@", error.description];
                                  UnitySendMessage("GameCenterManager", "onAchievementsLoadedFailed",  [ISN_DataConvertor serializeError:error]);
                              }
                          }];
                      }
                      
                      -(void) sendAchievementChallengeWithFriendsPicker:(NSString *)achievementId message:(NSString *)message {
                          
#if !TARGET_OS_TV
                          
                          GKAchievement *achievement = [[GKAchievement alloc] initWithIdentifier: achievementId];
                          
#if UNITY_VERSION < 500
                          [achievement autorelease];
#endif
                          
                          UIViewController *composeVC = [achievement challengeComposeControllerWithPlayers:nil message:message completionHandler:^(UIViewController *composeController, BOOL didIssueChallenge, NSArray *sentPlayerIDs) {
                              [composeController dismissViewControllerAnimated:YES completion:nil];
                              [composeController.view.superview removeFromSuperview];
                          }];
                          
                          
                          UIViewController *vc =  UnityGetGLViewController();
                          [vc presentViewController: composeVC animated: YES completion:nil];
#endif
                      }
                      
                      
                      -(void) sendAchievementChallenge:(NSString *)achievementId message:(NSString *)message playerIds:(NSArray *)playerIds {
#if !TARGET_OS_TV
                          GKAchievement *achievement = [[GKAchievement alloc] initWithIdentifier: achievementId];
#if UNITY_VERSION < 500
                          [achievement autorelease];
#endif
                          
                          [achievement issueChallengeToPlayers:playerIds message:message];
                          
#endif
                      }
                      
                      
#pragma invites
                      
                      -(NSString* ) serialiseInvite:(GKInvite*) invite {
                          NSString* inviteId = [self saveInvite:invite];
                          
                          [self savePlayerInfo:invite.sender];
                          
                          NSMutableString * data = [[NSMutableString alloc] init];
                          
                          [data appendString:inviteId];
                          [data appendString: UNITY_SPLITTER];
                          
                          [data appendString:invite.sender.playerID];
                          [data appendString: UNITY_SPLITTER];
                          
                          [data appendString: [NSString stringWithFormat:@"%lu", (unsigned long)invite.playerGroup]];
                          [data appendString: UNITY_SPLITTER];
                          
                          [data appendString: [NSString stringWithFormat:@"%d", invite.playerAttributes]];
                          
                          return data;
                          
                      }
                      
                      - (NSString*) saveInvite:(GKInvite*) invite {
                          if(receivedInvites == nil) {
                              receivedInvites = [[NSMutableDictionary alloc] init];
                          }
                          
                          NSString* inviteId =  [NSString stringWithFormat:@"%lu", receivedInvites.count + 1];
                          [receivedInvites setObject:invite forKey:inviteId];
                          
                          return inviteId;
                      }
                      
                      - (GKInvite*) getInviteWithId:(NSString*)inviteId {
                          GKInvite* invite = [receivedInvites objectForKey:inviteId];
                          return  invite;
                      }
                      
                      
#pragma friends
                      
                      -(void) inviteFirends:(int)requestId emails:(NSArray *)emails players:(NSArray *)players {
                          
#if !TARGET_OS_TV
                          
                          GKFriendRequestComposeViewController *friendRequest = [[GKFriendRequestComposeViewController alloc] init];
                          //friendRequest.composeViewDelegate = self;
                          
                          if(emails.count > 0) {
                              [friendRequest addRecipientsWithEmailAddresses:emails];
                          }
                          
                          if(players.count > 0) {
                              [friendRequest addRecipientPlayers:players];
                          }
                          
                          [[self GLViewController]  presentViewController:friendRequest animated:YES completion:nil];
#endif
                          
                      }
                      
                      
                      //Not implemented
                      
#if !TARGET_OS_TV
                      - (void)friendRequestComposeViewControllerDidFinish:(GKFriendRequestComposeViewController *)viewController {
                          NSLog(@"friendRequestComposeViewControllerDidFinish");
                      }
#endif
                      
                      
                      -(GKPlayer*) getPlayerWithId:(NSString*) playerId {
                          return [loadedPlayers objectForKey:playerId];
                      }
                      
                      
                      -(void) savePlayerInfo:(GKPlayer *)player {
                          
                          if(player == nil) {
                              return;
                          }
                          
                          if(player.playerID == nil) {
                              return;
                          }
                          
                          if([loadedPlayers objectForKey:player.playerID] != nil) {
                              return;
                          }
                          
                          
                          [loadedPlayers setObject:player forKey:player.playerID];
                          
                          NSMutableString * data = [[NSMutableString alloc] init];
                          [data appendString:player.playerID];
                          [data appendString:UNITY_SPLITTER];
                          
                          
                          if(player.alias != nil) {
                              [data appendString:player.alias];
                          } else {
                              [data appendString:@""];
                          }
                          
                          [data appendString:UNITY_SPLITTER];
                          
                          if(player.displayName != nil) {
                              [data appendString:player.displayName];
                          } else {
                              [data appendString:@""];
                          }
                          
                          UnitySendMessage("GameCenterManager", "OnUserInfoLoadedEvent", [ISN_DataConvertor NSStringToChar:data]);
                          
                      }
                      
                      -(void) loadPlayerInfoForPlayerWithId:(NSString *)playerId {
                          if([loadedPlayers objectForKey:playerId] == nil) {
                              return;
                          }
                          
                          NSArray *playerIdArray = [NSArray arrayWithObject:playerId];
                          [GKPlayer loadPlayersForIdentifiers:playerIdArray withCompletionHandler:^(NSArray *players, NSError *error) {
                              
                              if (error != nil) {
                                  if(error.description != NULL) {
                                      [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Player failed to load: %@", error.description];
                                  } else {
                                      [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN Player failed to load, no erro describtion provided"];
                                  }
                                  
                                  UnitySendMessage("GameCenterManager", "OnUserInfoLoadFailedEvent", [ISN_DataConvertor NSStringToChar:playerId]);
                              } else {
                                  GKPlayer *player = [players objectAtIndex:0];
                                  [self savePlayerInfo:player];
                              }
                              
                          }];
                          
                      }
                      
                      
                      -(void) loadImageForPlayerWithPlayerId:(NSString *)playerId size:(GKPhotoSize)size {
                          
                          GKPlayer* player = [loadedPlayers objectForKey:playerId];
                          
                          if(player == nil) {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN player with id %@ not found in the loadedPlayers array", playerId];
                              return;
                          }
                          
                          [player loadPhotoForSize:size withCompletionHandler:^(UIImage *photo, NSError *error) {
                              
                              
                              if(error != nil) {
                                  
                                  NSString* errorData = [ISN_DataConvertor serializeErrorToNSString:error];
                                  
                                  NSMutableString * data = [[NSMutableString alloc] init];
                                  [data appendString:player.playerID];
                                  [data appendString:UNITY_SPLITTER2];
                                  [data appendString: [NSString stringWithFormat:@"%ld", (long)size]];
                                  [data appendString:UNITY_SPLITTER2];
                                  [data appendString:errorData];
                                  
                                  UnitySendMessage("GameCenterManager", "OnUserPhotoLoadFailedEvent", [ISN_DataConvertor NSStringToChar:data]);
                                  
                                  return;
                              }
                              
                              NSString *encodedImage = @"";
                              
                              
                              if (photo == nil) {
                                  [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN No photo for user with ID: %@", playerId];
                              } else {
                                  NSData *imageData = UIImagePNGRepresentation(photo);
                                  encodedImage = [imageData base64Encoding];
                              }
                              
                              
                              NSMutableString * data = [[NSMutableString alloc] init];
                              [data appendString:player.playerID];
                              [data appendString:UNITY_SPLITTER];
                              [data appendString: [NSString stringWithFormat:@"%ld", (long)size]];
                              [data appendString:UNITY_SPLITTER];
                              [data appendString:encodedImage];
                              
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN User Photo Loaded for player with ID:  %@", playerId];
                              
                              UnitySendMessage("GameCenterManager", "OnUserPhotoLoadedEvent", [ISN_DataConvertor NSStringToChar:data]);
                              
                          }];
                          
                      }
                      
                      
                      
                      -(void) retrieveFriends {
                          GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
                          if (localPlayer.authenticated) {
                              
#if !TARGET_OS_TV
                              
                              if([ISN_NativeUtility majorIOSVersion] >= 8) {
                                  [self IOS8LoadFriends];
                              } else  {
                                  [self OldIOSLoadFreinds];
                              }
                              
#else
                              [self IOS8LoadFriends];
#endif
                              
                          } else {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN User friends cannot be loaded before player auth, sending fail event"];
                              UnitySendMessage("GameCenterManager", "OnFriendListLoadFailEvent", [ISN_DataConvertor serializeErrorWithData:@"User has to be authenticated to perform friends load operation" code:0]);
                          }
                          
                      }
                      
                      -(void) OldIOSLoadFreinds {
                          
#if !TARGET_OS_TV
                          
                          GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
                          [localPlayer loadFriendsWithCompletionHandler:^(NSArray *friends, NSError *error) {
                              
                              if(error != NULL) {
                                  UnitySendMessage("GameCenterManager", "OnFriendListLoadFailEvent", [ISN_DataConvertor serializeError:error]);
                                  return;
                              }
                              
                              
                              if (friends != nil && friends.count > 0) {
                                  
                                  BOOL first = YES;
                                  NSMutableString * data = [[NSMutableString alloc] init];
                                  for (NSString *friendId in friends) {
                                      
                                      [self loadPlayerInfoForPlayerWithId:friendId];
                                      
                                      if(!first) {
                                          [data appendString:UNITY_SPLITTER];
                                      }
                                      first = NO;
                                      
                                      [data appendString:friendId];
                                      
                                  }
                                  
                                  NSString *str = [data copy];
                                  
#if UNITY_VERSION < 500
                                  [str autorelease];
#endif
                                  
                                  
                                  UnitySendMessage("GameCenterManager", "OnFriendListLoadedEvent", [ISN_DataConvertor NSStringToChar:str]);
                              } else {
                                  UnitySendMessage("GameCenterManager", "OnFriendListLoadedEvent", [ISN_DataConvertor NSStringToChar:@""]);
                              }
                              
                              
                          }];
                          
#endif
                      }
                      
                      -(void) IOS8LoadFriends {
                          
                          GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
                          
                          [localPlayer loadFriendPlayersWithCompletionHandler:^(NSArray *friendPlayers, NSError *error) {
                              if(error != NULL) {
                                  UnitySendMessage("GameCenterManager", "OnFriendListLoadFailEvent", [ISN_DataConvertor serializeError:error]);
                                  return;
                              }
                              
                              if (friendPlayers != nil && friendPlayers.count > 0) {
                                  
                                  BOOL first = YES;
                                  NSMutableString * data = [[NSMutableString alloc] init];
                                  for (GKPlayer *userFriend in friendPlayers) {
                                      [self savePlayerInfo:userFriend];
                                      
                                      if(!first) {
                                          [data appendString:UNITY_SPLITTER];
                                      }
                                      first = NO;
                                      
                                      [data appendString:userFriend.playerID];
                                      
                                  }
                                  
                                  NSString *str = [data copy];
                                  
#if UNITY_VERSION < 500
                                  [str autorelease];
#endif
                                  
                                  
                                  UnitySendMessage("GameCenterManager", "OnFriendListLoadedEvent", [ISN_DataConvertor NSStringToChar:str]);
                              } else {
                                  UnitySendMessage("GameCenterManager", "OnFriendListLoadedEvent", [ISN_DataConvertor NSStringToChar:@""]);
                              }
                              
                          }];
                          
                      }
                      
                      
#pragma private methods
                      
                      - (void)gameCenterViewControllerDidFinish:(GKGameCenterViewController *)vc {
                          [self dismissGameCenterView:leaderbaordsView];
                          leaderbaordsView =  nil;
                      }
                      
#if !TARGET_OS_TV
                      
                      - (void)achievementViewControllerDidFinish:(GKAchievementViewController *)vc; {
                          [self dismissGameCenterView:achievementView];
                          achievementView = nil;
                      }
                      
#endif
                      
                      
                      -(void) dismissGameCenterView: (GKGameCenterViewController *)vc {
                          
                          if(vc == nil) {
                              return;
                          }
                          
                          if (![vc isBeingPresented] && ![vc isBeingDismissed]) {
                              [vc dismissViewControllerAnimated:YES completion:nil];
                              // [vc.view.superview removeFromSuperview];
                          }
                          
#if UNITY_VERSION < 500
                          [vc release];
#endif
                          
                          vc = nil;
                          
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"GameCenter View Dismissed"];
                          UnitySendMessage("GameCenterManager", "OnGameCenterViewDismissedEvent", [ISN_DataConvertor NSStringToChar:@""]);
                      }
                      
                      - (void)applicationDidEnterBackground:(UIApplication*)application {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"applicationDidEnterBackground"];
                          
                          [self dismissGameCenterView:leaderbaordsView];
                          leaderbaordsView = nil;
                          
#if !TARGET_OS_TV
                          
                          [self dismissGameCenterView:achievementView];
                          achievementView = nil;
                          
#endif
                      }
                      
                      -(void) showNotificationBanner:(NSString *)title message:(NSString *)message {
                          [GKNotificationBanner showBannerWithTitle:title message:message completionHandler:^{}];
                      }
                      
                      
                      - (BOOL)isGameCenterAvailable {
                          BOOL localPlayerClassAvailable = (NSClassFromString(@"GKLocalPlayer")) != nil;
                          NSString *reqSysVer = @"4.1";
                          NSString *currSysVer = [[UIDevice currentDevice] systemVersion];
                          BOOL osVersionSupported = ([currSysVer compare:reqSysVer options:NSNumericSearch] != NSOrderedAscending);
                          return (localPlayerClassAvailable && osVersionSupported);
                      }
                      
                      @end
                      
                      
                      
                      
                      @implementation ISN_GameCenterRTM
                      
                      
                      static int rtm_playerGroup = 0;
                      static int rtm_playerAttributes = 0;
                      static BOOL isInited = FALSE;
                      
                      static unsigned long CurrentMatchHash = 0;
                      
                      
                      @synthesize vc;
                      
#pragma mark Initialization
                      
                      static ISN_GameCenterRTM * rtm_sharedHelper = nil;
                      + (ISN_GameCenterRTM *) sharedInstance {
                          if (!rtm_sharedHelper) {
                              rtm_sharedHelper = [[ISN_GameCenterRTM alloc] init];
                              
                          }
                          return rtm_sharedHelper;
                      }
                      
                      
#pragma mark public
                      
                      -(void) findMatch:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage invitationsList:(NSArray *)invitationsList {
                          [self prepareforMacthRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList useNativeUI:false];
                      }
                      
                      - (void)findMatchWithNativeUI:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage invitationsList:(NSArray *)invitationsList{
                          [self prepareforMacthRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList useNativeUI:true];
                      }
                      
                      -(void) prepareforMacthRequest:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage invitationsList:(NSArray *)invitationsList useNativeUI:(BOOL) useNativeUI {
                          
                          if(invitationsList.count > 0) {
                              [GKPlayer loadPlayersForIdentifiers:invitationsList withCompletionHandler:^(NSArray *players, NSError *error) {
                                  if(error == nil) {
                                      [self startFindMatchRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage recipients:players useNativeUI:useNativeUI];
                                  } else {
                                      UnitySendMessage("GameCenter_RTM", "OnMatchStartFailed", [ISN_DataConvertor serializeError:error]);
                                  }
                              }];
                          } else {
                              [self startFindMatchRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage recipients:invitationsList useNativeUI:useNativeUI];
                          }
                      }
                      
                      -(void) startFindMatchRequest:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage recipients:(NSArray *)recipients useNativeUI:(BOOL) useNativeUI  {
                          
                          GKMatchRequest *request = [[GKMatchRequest alloc] init];
                          request.minPlayers = minPlayers;
                          request.maxPlayers = maxPlayers;
                          
                          if(rtm_playerGroup != 0) {
                              request.playerGroup = rtm_playerGroup;
                          }
                          
                          if(rtm_playerAttributes != 0) {
                              request.playerAttributes = rtm_playerAttributes;
                          }
                          
                          
                          if(![inviteMessage isEqualToString:@""]) {
                              request.inviteMessage = inviteMessage;
                          }
                          
                          if(recipients.count > 0) {
                              request.recipients = recipients;
                              request.recipientResponseHandler = ^( GKPlayer *player, GKInviteeResponse response) {
                                  
                                  [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
                                  
                                  NSMutableString * data = [[NSMutableString alloc] init];
                                  
                                  [data appendString:player.playerID];
                                  [data appendString: UNITY_SPLITTER];
                                  [data appendString: [NSString stringWithFormat:@"%ld", (long)response]];
                                  
                                  NSString *str = [data copy];
#if UNITY_VERSION < 500
                                  [str autorelease];
#endif
                                  [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN RTM OnInviteeResponse"];
                                  UnitySendMessage("GameCenterInvitations", "OnInviteeResponse", [ISN_DataConvertor NSStringToChar:str]);
                              };
                              
                          }
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"startFindMatchRequest useNativeUI %hhd", useNativeUI];
                          
                          if(useNativeUI) {
                              
                              GKMatchmakerViewController *mmvc = [[GKMatchmakerViewController alloc] initWithMatchRequest:request];
                              mmvc.matchmakerDelegate = self;
                              
                              [self.vc presentViewController:mmvc animated:YES completion:nil];
                              
                              
                          } else  {
                              [[GKMatchmaker sharedMatchmaker] findMatchForRequest:request withCompletionHandler:^(GKMatch *match, NSError *error) {
                                  
                                  [self processMatchStartResult:match error:error];
                                  
                              }];
                          }
                      }
                      
                      - (void) cancelMatchSeartch {
                          [[GKMatchmaker sharedMatchmaker] cancel];
                      }
                      
                      -(void) initNotificationHandler {
                          
#if !TARGET_OS_TV
                          if(isInited) { return; }  isInited = TRUE;
                          
                          [GKMatchmaker sharedMatchmaker].inviteHandler = ^(GKInvite *acceptedInvite, NSArray *playersToInvite) {
                              
                              if (acceptedInvite) {
                                  
                                  UnitySendMessage("GameCenterInvitations", "OnPlayerAcceptedInvitation_RTM", [ISN_DataConvertor NSStringToChar:[[ISN_GameCenterManager sharedInstance] serialiseInvite:acceptedInvite]]);
                                  
                              } else {
                                  if(playersToInvite) {
                                      
                                      NSMutableArray* requestedInvitationsArray = [[NSMutableArray alloc] init];
                                      
                                      
                                      for(NSObject* playerInfo in playersToInvite) {
                                          
                                          if([playerInfo respondsToSelector:@selector(playerID)] ) {
                                              GKPlayer *player = (GKPlayer*) playerInfo;
                                              [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
                                              [requestedInvitationsArray addObject:player.playerID];
                                              
                                          } else {
                                              
                                              NSString *PlayerId =  (NSString*) playerInfo;
                                              [[ISN_GameCenterManager sharedInstance] loadPlayerInfoForPlayerWithId:PlayerId];
                                              [requestedInvitationsArray addObject:PlayerId];
                                          }
                                          
                                      }
                                      
                                      
                                      UnitySendMessage("GameCenterInvitations", "OnPlayerRequestedMatchWithRecipients_RTM", [ISN_DataConvertor NSStringsArrayToChar:requestedInvitationsArray]);
                                  }
                              }
                              
                          };
#endif
                      }
                      
                      -(void) startMatchWithInviteID:(NSString*) inviteId useNativeUI:(BOOL) useNativeUI {
                          
                          GKInvite* invite = [[ISN_GameCenterManager sharedInstance] getInviteWithId:inviteId];
                          if(invite == nil) {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN startMatchWithInviteID, invite with id %@ not found", inviteId];
                              return;
                          }
                          
                          if(useNativeUI) {
                              GKMatchmakerViewController *mmvc = [[GKMatchmakerViewController alloc] initWithInvite:invite];
                              mmvc.matchmakerDelegate = self;
                              
                              [self.vc presentViewController:mmvc animated:YES completion:nil];
                          } else {
                              [[GKMatchmaker sharedMatchmaker] matchForInvite:invite completionHandler:^(GKMatch *match, NSError *error) {
                                  [self processMatchStartResult:match error:error];
                              }];
                          }
                      }
                      
                      
                      
                      
                      -(void) cancelPendingInviteToPlayerWithId:(NSString*) playerId {
                          
                          GKPlayer* player = [[ISN_GameCenterManager sharedInstance] getPlayerWithId:playerId];
                          if(player == nil) {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN cancelPendingInviteToPlayerWithId, player with id %@ not found", playerId];
                          }
                          
                          [[GKMatchmaker sharedMatchmaker] cancelPendingInviteToPlayer:player];
                      }
                      
                      -(void) finishMatchmaking {
                          if([self currentMatch] == nil) {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN finishMatchmaking, currentMatch is mill"];
                              return;
                          }
                          
                          [[GKMatchmaker sharedMatchmaker] finishMatchmakingForMatch:[self currentMatch]];
                      }
                      
                      -(void) queryActivity {
                          [[GKMatchmaker sharedMatchmaker] queryActivityWithCompletionHandler:^(NSInteger activity, NSError *error) {
                              [self sendQueryActivityResult:activity error:error];
                          }];
                      }
                      
                      -(void) queryPlayerGroupActivity:(int) group {
                          [[GKMatchmaker sharedMatchmaker] queryPlayerGroupActivity:group withCompletionHandler:^(NSInteger activity, NSError *error) {
                              [self sendQueryActivityResult:activity error:error];
                          }];
                      }
                      
                      
                      -(void) startBrowsingForNearbyPlayers {
                          [[GKMatchmaker sharedMatchmaker] startBrowsingForNearbyPlayersWithHandler:^(GKPlayer *player, BOOL reachable) {
                              
                              
                              [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
                              
                              NSMutableString * data = [[NSMutableString alloc] init];
                              [data appendString:player.playerID];
                              [data appendString: UNITY_SPLITTER];
                              
                              if(reachable) {
                                  [data appendString: @"True"];
                              } else {
                                   [data appendString: @"False"];
                              }
                              
                              
                              
                              UnitySendMessage("GameCenter_RTM", "OnNearbyPlayerInfoReceived", [ISN_DataConvertor NSStringToChar:data]);
                              
                          }];
                      }
                      
                      -(void) stopBrowsingForNearbyPlayers {
                          [[GKMatchmaker sharedMatchmaker] stopBrowsingForNearbyPlayers];
                      }
                      
                      
                      -(void) disconnect {
                          if([self currentMatch] != nil) {
                              [[self currentMatch] disconnect];
                          }
                      }
                      
                      -(void) rematch {
                          if([self currentMatch] != nil) {
                              [[self currentMatch] rematchWithCompletionHandler:^(GKMatch *match, NSError *error) {
                                  [self processMatchStartResult:match error:error];
                              }];
                          }
                      }
                      
                      -(void)sendDataToAll:(NSData *)data withDataMode:(GKMatchSendDataMode)withDataMode {
                          
                          if([self currentMatch] == NULL) {
                              return;
                          }
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"sendDataToAll"];
                          
                          NSError *error;
                          BOOL IsdataWasSent =  [[self currentMatch] sendDataToAllPlayers:data withDataMode:withDataMode error:&error];
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"IsdataWasSent: %hhd", IsdataWasSent];
                          if (error != nil) {
                              [self HandleDataSendError:error];
                          }
                      }
                      
                      
                      - (void) sendData:(NSData *)data toPlayersWithIds:(NSArray *)toPlayersWithIds withDataMode:(GKMatchSendDataMode)withDataMode {
                          
#if !TARGET_OS_TV
                          
                          if([self currentMatch] == NULL) {
                              return;
                          }
                          
                          /*
                           
                           NSMutableArray* players = [[NSMutableArray alloc] init];
                           for (NSString* playerId : toPlayersWithIds) {
                           GKPlayer* player = [[ISN_GameCenterManager sharedInstance] getPlayerWithId:playerId];
                           [players addObject:player];
                           
                           NSLog(@"player added: %@", player.playerID);
                           }
                           */
                          
                          
                          NSError *error = nil;
                          BOOL IsdataWasSent =   [[self currentMatch] sendData:data toPlayers:toPlayersWithIds withDataMode:withDataMode error:&error];
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"IsdataWasSent: %hhd", IsdataWasSent];
                          
                          if (error != nil) {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"error.code: %d", error.code];
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"error.description: %@", error.description];
                              
                              [self HandleDataSendError:error];
                          }
                          
#endif
                          
                      }
                      
                      
                      -(void) HandleDataSendError: (NSError *) error {
                          // UnitySendMessage("GameCenter_RTM", "OnSendDataError", [ISNDataConvertor serializeError:error]);
                      }
                      
                      
                      
#pragma private
                      
                      -(void) sendQueryActivityResult:(NSInteger) activity error:(NSError *)error {
                          
                          if(error != nil) {
                              UnitySendMessage("GameCenter_RTM", "OnQueryActivityFailed", [ISN_DataConvertor serializeError:error]);
                          } else {
                              NSString *activityResult = [NSString stringWithFormat:@"%ld", (long)activity];
                              UnitySendMessage("GameCenter_RTM", "OnQueryActivity", [ISN_DataConvertor NSStringToChar:activityResult]);
                          }
                      }
                      
                      -(void) processMatchStartResult:(GKMatch *)match error:(NSError *) error {
                          if(error != nil) {
                              UnitySendMessage("GameCenter_RTM", "OnMatchStartFailed", [ISN_DataConvertor serializeError:error]);
                              return;
                          }
                          
                          NSString* matchData = [self serializeMathcData:match];
                          UnitySendMessage("GameCenter_RTM", "OnMatchStarted", [ISN_DataConvertor NSStringToChar:matchData]);
                      }
                      
                      
                      
                      
                      
                      -(void) updateCurrentMatch:(GKMatch *)match  {
                          [self setCurrentMatch:match];
                          [self currentMatch].delegate = self;
                          CurrentMatchHash = [[self currentMatch] hash];
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: RTM updateCurrentMatch %lu", CurrentMatchHash];
                          
                      }
                      
                      -(NSString*) serializeMathcData:(GKMatch *)match  {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"serialize match data"];
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"match.players.count %d", match.players.count];
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"match.expectedPlayerCount %d", match.expectedPlayerCount];
                          
                          [self updateCurrentMatch:match];
                          
                          NSMutableString * data = [[NSMutableString alloc] init];
                          
                          
                          [data appendString: [NSString stringWithFormat:@"%lu", (unsigned long)match.expectedPlayerCount]];
                          [data appendString: UNITY_SPLITTER2];
                          
                          
                          NSMutableArray* playersIds = [[NSMutableArray alloc] init];
                          
                          
                          for(GKPlayer * player in match.players) {
                              [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
                              [playersIds addObject:player.playerID];
                          }
                          
                          [data appendString:[ISN_DataConvertor serializeNSStringsArray:playersIds]];
                          
                          return data;
                      }
                      
                      
#pragma mark GKMatchDelegate
                      
                      // The match received data sent from the player.
                      - (void)match:(GKMatch *)match didReceiveData:(NSData *)data fromRemotePlayer:(GKPlayer *)player {
                          
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"RTM match didReceiveData"];
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Match hash: %lu", (unsigned long)[match hash]];
                          
                          
                          if(CurrentMatchHash != (unsigned long) [match hash]) {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Ignoring the event from old match"];
                              return;
                          }
                          
                          
                          
                          NSMutableString * str = [[NSMutableString alloc] init];
                          
                          [str appendString:player.playerID];
                          [str appendString: UNITY_SPLITTER];
                          [str appendString: [data base64Encoding]];
                          
                          
                          NSString *info = [str copy] ;
                          
#if UNITY_VERSION < 500
                          [info autorelease];
#endif
                          
                          UnitySendMessage("GameCenter_RTM", "OnMatchDataReceived", [ISN_DataConvertor NSStringToChar:info]);
                          
                      }
                      
                      
                      // The player state changed (eg. connected or disconnected)
                      - (void)match:(GKMatch *)match player:(GKPlayer *)player didChangeConnectionState:(GKPlayerConnectionState)state  {
                          
                          if(CurrentMatchHash != (unsigned long) [match hash]) {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Ignoring the event from old match"];
                          }
                          
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"RTM player didChangeConnectionState"];
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Match hash: %lu", (unsigned long)[match hash]];
                          
                          NSMutableString * str = [[NSMutableString alloc] init];
                          
                          
                          
                          [str appendString:player.playerID];
                          [str appendString: UNITY_SPLITTER];
                          [str appendString: [NSString stringWithFormat:@"%ld", (long)state]];
                          
                          
                          NSString *info = [str copy] ;
                          
#if UNITY_VERSION < 500
                          [info autorelease];
#endif
                          
                          NSString* matchData = [self serializeMathcData:match];
                          UnitySendMessage("GameCenter_RTM", "OnMatchInfoUpdated", [ISN_DataConvertor NSStringToChar:matchData]);
                          
                          UnitySendMessage("GameCenter_RTM", "OnMatchPlayerStateChanged", [ISN_DataConvertor NSStringToChar:info]);
                      }
                      
                      
                      // The match was unable to be established with any players due to an error.
                      - (void)match:(GKMatch *)match didFailWithError:(NSError *)error {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"RTM match didFailWithError"];
                          UnitySendMessage("GameCenter_RTM", "OnMatchFailed", [ISN_DataConvertor serializeError:error]);
                      }
                      
                      
                      
                      // This method is called when the match is interrupted; if it returns YES, a new invite will be sent to attempt reconnection. This is supported only for 1v1 games
                      - (BOOL)match:(GKMatch *)match shouldReinviteDisconnectedPlayer:(GKPlayer *)player {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"RTM match shouldReinviteDisconnectedPlayer"];
                          [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
                          UnitySendMessage("GameCenter_RTM", "OnDiconnectedPlayerReinvited", [ISN_DataConvertor NSStringToChar:player.playerID]);
                          return YES;
                      }
                      
                      
                      
#pragma mark GKMatchmakerViewControllerDelegate
                      
                      
                      // The user has cancelled matchmaking
                      - (void)matchmakerViewControllerWasCancelled:(GKMatchmakerViewController *)viewController {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: RTM WasCancelled"];
                          
                          [self.vc dismissViewControllerAnimated:YES completion:nil];
                          UnitySendMessage("GameCenter_RTM", "OnMatchStartFailed", [ISN_DataConvertor serializeErrorWithData:@"User Cancelled" code:0]);
                      }
                      
                      // Matchmaking has failed with an error
                      - (void)matchmakerViewController:(GKMatchmakerViewController *)viewController didFailWithError:(NSError *)error  {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: RTM didFailWithError"];
                          
                          [self.vc dismissViewControllerAnimated:YES completion:nil];
                          UnitySendMessage("GameCenter_RTM", "OnMatchStartFailed", [ISN_DataConvertor serializeError:error]);
                      }
                      
                      - (void)matchmakerViewController:(GKMatchmakerViewController *)viewController didFindMatch:(GKMatch *)match  {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: RTM didFindMatch"];
                          [self.vc dismissViewControllerAnimated:YES completion:nil];
                          
                          NSString* matchData = [self serializeMathcData:match];
                          UnitySendMessage("GameCenter_RTM", "OnMatchStarted", [ISN_DataConvertor NSStringToChar:matchData]);
                          
                      }
                      
                      
                      
                      
                      +(GKMatchSendDataMode) getDataModeByIntValue:(int) value {
                          switch (value) {
                              case 0:
                                  return GKMatchSendDataReliable;
                                  break;
                                  
                              default:
                                  return GKMatchSendDataUnreliable;
                                  break;
                          }
                      }
                      
                      @end
                      
                      
                      
                      @implementation ISN_GameCenterTBM
                      
                      
                      static ISN_GameCenterTBM * _tbm_sharedInstance;
                      static NSMutableArray *matches;
                      
                      static int tbm_playerGroup = 0;
                      static int tbm_playerAttributes = 0;
                      
                      @synthesize vc;
                      
                      + (id)sharedInstance {
                          
                          if (_tbm_sharedInstance == nil)  {
                              matches  = [[NSMutableArray alloc] init];
                              _tbm_sharedInstance = [[self alloc] init];
                              
                          }
                          
                          return _tbm_sharedInstance;
                      }
                      
                      
                      
                      -(void) findMatch:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage invitationsList:(NSArray *)invitationsList {
                          [self prepareforMacthRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList useNativeUI:false];
                      }
                      
                      - (void)findMatchWithNativeUI:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage invitationsList:(NSArray *)invitationsList{
                          [self prepareforMacthRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList useNativeUI:true];
                      }
                      
                      -(void) prepareforMacthRequest:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage invitationsList:(NSArray *)invitationsList useNativeUI:(BOOL) useNativeUI {
                          
                          if(invitationsList.count > 0) {
                              [GKPlayer loadPlayersForIdentifiers:invitationsList withCompletionHandler:^(NSArray *players, NSError *error) {
                                  if(error == nil) {
                                      [self startFindMatchRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage recipients:players useNativeUI:useNativeUI];
                                  } else {
                                      UnitySendMessage("GameCenter_TBM", "OnMatchFoundResultFailed", [ISN_DataConvertor serializeError:error]);
                                  }
                              }];
                          } else {
                              [self startFindMatchRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage recipients:invitationsList useNativeUI:useNativeUI];
                          }
                      }
                      
                      -(void) startFindMatchRequest:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage recipients:(NSArray *)recipients useNativeUI:(BOOL) useNativeUI  {
                          
                          GKMatchRequest *request = [[GKMatchRequest alloc] init];
                          request.minPlayers = minPlayers;
                          request.maxPlayers = maxPlayers;
                          
                          if(tbm_playerGroup != 0) {
                              request.playerGroup = tbm_playerGroup;
                          }
                          
                          if(tbm_playerAttributes != 0) {
                              request.playerAttributes = tbm_playerAttributes;
                          }
                          
                          if(![inviteMessage isEqualToString:@""]) {
                              request.inviteMessage = inviteMessage;
                          }
                          
                          if(recipients.count > 0) {
                              request.recipients = recipients;
                              request.recipientResponseHandler = ^( GKPlayer *player, GKInviteeResponse response) {
                                  
                                  NSMutableString * data = [[NSMutableString alloc] init];
                                  
                                  [data appendString:player.playerID];
                                  [data appendString: UNITY_SPLITTER];
                                  [data appendString: [NSString stringWithFormat:@"%ld", (long)response]];
                                  
                                  [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN RTM OnInviteeResponse"];
                                  UnitySendMessage("GameCenterInvitations", "OnInviteeResponse", [ISN_DataConvertor NSStringToChar:data]);
                                  
                              };
                              
                          }
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN startFindMatchRequest useNativeUI %hhd", useNativeUI];
                          
                          if(useNativeUI) {
                              
                              GKTurnBasedMatchmakerViewController *mmvc = [[GKTurnBasedMatchmakerViewController alloc] initWithMatchRequest:request];
                              mmvc.turnBasedMatchmakerDelegate = self;
                              [self.vc presentViewController:mmvc animated:YES completion:nil];
                              
                          } else  {
                              [GKTurnBasedMatch findMatchForRequest: request withCompletionHandler:^(GKTurnBasedMatch *match, NSError *error) {
                                  if (error == NULL) {
                                      NSString* matchData = [self serializeMathcData:match];
                                      [self updateMatchInfo:match];
                                      UnitySendMessage("GameCenter_TBM", "OnMatchFoundResult", [ISN_DataConvertor NSStringToChar:matchData]);
                                  } else {
                                      UnitySendMessage("GameCenter_TBM", "OnMatchFoundResultFailed", [ISN_DataConvertor serializeError:error]);
                                  }
                              }];
                          }
                          
                      }
                      
                      -(void) startMatchWithInviteID:(NSString*) inviteId useNativeUI:(BOOL) useNativeUI {
                          
                          GKInvite* invite = [[ISN_GameCenterManager sharedInstance] getInviteWithId:inviteId];
                          if(invite == nil) {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN startMatchWithInviteID, invite with id %@ not found", inviteId];
                              return;
                          }
                          
                      }
                      
                      
                      
                      -(void) loadMatches {
                          [GKTurnBasedMatch loadMatchesWithCompletionHandler:^(NSArray *matches, NSError *error) {
                              if(error == nil) {
                                  if (matches) {
                                      [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Loaded matches count %lu", (unsigned long)matches.count];
                                      
                                      NSMutableString * data = [[NSMutableString alloc] init];
                                      
                                      
                                      for(GKTurnBasedMatch * m in matches){
                                          [data appendString: [self serializeMathcData:m]];
                                          [data appendString: UNITY_SPLITTER2];
                                          [self updateMatchInfo:m];
                                      }
                                      
                                      [data appendString: UNITY_EOF];
                                      
                                      
                                      UnitySendMessage("GameCenter_TBM", "OnLoadMatchesResult", [ISN_DataConvertor NSStringToChar:data]);
                                  } else {
                                      UnitySendMessage("GameCenter_TBM", "OnLoadMatchesResult", [ISN_DataConvertor NSStringToChar:@""]);
                                  }
                              } else {
                                  
                                  
                                  UnitySendMessage("GameCenter_TBM", "OnLoadMatchesResultFailed", [ISN_DataConvertor serializeError:error]);
                              }
                              
                          }];
                      }
                      
                      
                      -(void) loadMatch:(NSString *)matchId {
                          [GKTurnBasedMatch loadMatchWithID:matchId withCompletionHandler:^(GKTurnBasedMatch *match, NSError *error) {
                              if(error == nil) {
                                  
                                  [match loadMatchDataWithCompletionHandler:^(NSData *matchData, NSError *error) {
                                      if(error == nil) {
                                          [self updateMatchInfo:match];
                                          NSString* matchData = [self serializeMathcData:match];
                                          UnitySendMessage("GameCenter_TBM", "OnLoadMatchResult", [ISN_DataConvertor NSStringToChar:matchData]);
                                      } else {
                                          UnitySendMessage("GameCenter_TBM", "OnLoadMatchResultFailed",  [ISN_DataConvertor serializeError:error]);
                                      }
                                  }];
                                  
                                  
                                  
                                  
                              } else {
                                  UnitySendMessage("GameCenter_TBM", "OnLoadMatchResultFailed",  [ISN_DataConvertor serializeError:error]);
                              }
                          }];
                      }
                      
                      
                      -(void) saveCurrentTurn:(NSString *)matchId updatedMatchData:(NSData *)updatedMatchData {
#if !TARGET_OS_TV
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN saveCurrentTurn"];
                          GKTurnBasedMatch* match = [self getMatchWithId:matchId];
                          if(match != NULL) {
                              [match saveCurrentTurnWithMatchData:updatedMatchData completionHandler:^(NSError *error) {
                                  if(error != nil) {
                                      UnitySendMessage("GameCenter_TBM", "OnUpdateMatchResultFailed", [ISN_DataConvertor serializeError:error]);
                                  } else {
                                      NSString *encodedData = [updatedMatchData base64Encoding];
                                      
                                      NSMutableString * data = [[NSMutableString alloc] init];
                                      
                                      
                                      [data appendString: matchId];
                                      [data appendString: UNITY_SPLITTER];
                                      [data appendString: encodedData];
                                      
                                      NSString *str = [data copy];
                                      
#if UNITY_VERSION < 500
                                      [str autorelease];
#endif
                                      
                                      
                                      
                                      UnitySendMessage("GameCenter_TBM", "OnUpdateMatchResult", [ISN_DataConvertor NSStringToChar:str]);
                                  }
                              }];
                          } else {
                              UnitySendMessage("GameCenter_TBM", "OnUpdateMatchResultFailed", [ISN_DataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
                          }
                          
#endif
                      }
                      
                      -(void) quitInTurn:(NSString *)matchId outcome:(int)outcome nextPlayerId:(NSString*)nextPlayerId matchData:(NSData *)matchData {
#if !TARGET_OS_TV
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN quitInTurn"];
                          GKTurnBasedMatch* match = [self getMatchWithId:matchId];
                          if(match != NULL) {
                              
                              if(matchData.length == 0) {
                                  matchData = [match matchData];
                              }
                              
                              
                              if(matchData == nil) {
                                  matchData = [[NSData alloc] init];
                              }
                              
                              GKTurnBasedMatchOutcome participantOutcome = [ISN_GameCenterTBM getOutcomeByIntValue:outcome];
                              GKTurnBasedParticipant *nextParticipantObject = [self getMathcParticipantById:match playerId:nextPlayerId];
                              
                              [match participantQuitInTurnWithOutcome:participantOutcome nextParticipant:nextParticipantObject matchData:matchData completionHandler:^(NSError *error) {
                                  if(error == nil) {
                                      UnitySendMessage("GameCenter_TBM", "OnMatchQuitResult", [ISN_DataConvertor NSStringToChar:matchId]);
                                  } else {
                                      UnitySendMessage("GameCenter_TBM", "OnMatchQuitResultFailed", [ISN_DataConvertor serializeError:error]);
                                  }
                              }];
                              
                          } else {
                              UnitySendMessage("GameCenter_TBM", "OnMatchQuitResultFailed", [ISN_DataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
                          }
                          
#endif
                      }
                      
                      - (void) quitOutOfTurn:(NSString *)matchId outcome:(int)outcome {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN quitOutOfTurn"];
                          GKTurnBasedMatch* match = [self getMatchWithId:matchId];
                          if(match != NULL) {
                              
                              GKTurnBasedMatchOutcome participantOutcome = [ISN_GameCenterTBM getOutcomeByIntValue:outcome];
                              [match participantQuitOutOfTurnWithOutcome:participantOutcome withCompletionHandler:^(NSError *error) {
                                  if(error == nil) {
                                      UnitySendMessage("GameCenter_TBM", "OnMatchQuitResult", [ISN_DataConvertor NSStringToChar:matchId]);
                                  } else {
                                      UnitySendMessage("GameCenter_TBM", "OnMatchQuitResultFailed", [ISN_DataConvertor serializeError:error]);
                                  }
                              }];
                              
                          } else {
                              UnitySendMessage("GameCenter_TBM", "OnMatchQuitResultFailed", [ISN_DataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
                          }
                      }
                      
                      
                      -(void) updatePlayerOutcome:(NSString *)matchId Outcome:(int)Outcome playerId:(NSString*)playerId {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN updatePlayerOutcome"];
                          GKTurnBasedMatch* match = [self getMatchWithId:matchId];
                          GKTurnBasedMatchOutcome participantOutcome = [ISN_GameCenterTBM getOutcomeByIntValue:Outcome];
                          if(match != NULL) {
                              
                              GKTurnBasedParticipant *participant =  [self getMathcParticipantById:match playerId:playerId];
                              if(participant != nil) {
                                  [participant setMatchOutcome:participantOutcome];
                              } else {
                                  [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN::updatePlayerOutcome participant not found"];
                              }
                          } else {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN::updatePlayerOutcome match not found"];
                          }
                      }
                      
                      
                      -(void) endTurn:(NSString *)matchId updatedMatchData:(NSData *)updatedMatchData nextPlayerId:(NSString*)nextPlayerId {
#if !TARGET_OS_TV
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN endTurn with next player"];
                          GKTurnBasedMatch* match = [self getMatchWithId:matchId];
                          if(match != NULL) {
                              GKTurnBasedParticipant *nextParticipant = [self getMathcParticipantById:match playerId:nextPlayerId];
                              
                              [match endTurnWithNextParticipant:nextParticipant matchData:updatedMatchData completionHandler:^(NSError *error) {
                                  [self handleCompleteTurnResult:error requestedMatch:match];
                              }];
                              
                          } else {
                              UnitySendMessage("GameCenter_TBM", "OnEndTurnResultFailed", [ISN_DataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
                          }
#endif
                      }
                      
                      
                      -(void) handleCompleteTurnResult: (NSError* )error requestedMatch: (GKTurnBasedMatch*)requestedMatch {
                          if(error != nil) {
                              UnitySendMessage("GameCenter_TBM", "OnEndTurnResultFailed", [ISN_DataConvertor serializeError: error]);
                              return;
                          }
                          
                          [GKTurnBasedMatch loadMatchWithID:requestedMatch.matchID withCompletionHandler:^(GKTurnBasedMatch *match, NSError *error) {
                              if(error != nil) {
                                  UnitySendMessage("GameCenter_TBM", "OnEndTurnResultFailed", [ISN_DataConvertor serializeError: error]);
                              } else {
                                  NSString* matchData = [self serializeMathcData:match];
                                  UnitySendMessage("GameCenter_TBM", "OnEndTurnResult", [ISN_DataConvertor NSStringToChar:matchData]);
                              }
                          }];
                      }
                      
                      
                      -(void) endMatch:(NSString *)matchId matchData:(NSData *)matchData {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN endMatch"];
                          GKTurnBasedMatch* match = [self getMatchWithId:matchId];
                          if(match != NULL) {
                              [match endMatchInTurnWithMatchData:matchData completionHandler:^(NSError *error) {
                                  if(error == nil) {
                                      NSString* matchData = [self serializeMathcData:match];
                                      UnitySendMessage("GameCenter_TBM", "OnEndMatch", [ISN_DataConvertor NSStringToChar:matchData]);
                                  } else {
                                      UnitySendMessage("GameCenter_TBM", "OnEndMatchResult", [ISN_DataConvertor serializeError: error]);
                                  }
                              }];
                          } else {
                              UnitySendMessage("GameCenter_TBM", "OnEndMatchResult", [ISN_DataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
                          }
                      }
                      
                      
                      -(void) rematch:(NSString *)matchId {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN rematch"];
                          GKTurnBasedMatch* match = [self getMatchWithId:matchId];
                          if(match != NULL) {
                              [match rematchWithCompletionHandler:^(GKTurnBasedMatch *match, NSError *error) {
                                  if (error == NULL) {
                                      [self updateMatchInfo:match];
                                      NSString* matchData = [self serializeMathcData:match];
                                      UnitySendMessage("GameCenter_TBM", "OnRematchResult", [ISN_DataConvertor NSStringToChar:matchData]);
                                  } else {
                                      UnitySendMessage("GameCenter_TBM", "OnRematchFailed", [ISN_DataConvertor serializeError:error]);
                                  }
                              }];
                          } else {
                              UnitySendMessage("GameCenter_TBM", "OnRematchFailed", [ISN_DataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
                          }
                          
                      }
                      
                      -(void) removeMatch:(NSString *)matchId {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN removeMatch"];
                          GKTurnBasedMatch* match = [self getMatchWithId:matchId];
                          if(match != NULL) {
                              [match removeWithCompletionHandler:^(NSError *error) {
                                  if(error == nil) {
                                      UnitySendMessage("GameCenter_TBM", "OnMatchRemoved", [ISN_DataConvertor NSStringToChar:matchId]);
                                  } else {
                                      UnitySendMessage("GameCenter_TBM", "OnMatchRemoveFailed", [ISN_DataConvertor serializeError:error]);
                                  }
                              }];
                          } else {
                              UnitySendMessage("GameCenter_TBM", "OnMatchRemoveFailed", [ISN_DataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
                          }
                      }
                      
                      -(void) acceptInvite:(NSString *)matchId {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN acceptInvite"];
                          GKTurnBasedMatch* InvitedMatch = [self getMatchWithId:matchId];
                          if(InvitedMatch != NULL) {
                              [InvitedMatch acceptInviteWithCompletionHandler:^(GKTurnBasedMatch * _Nullable match, NSError * _Nullable error) {
                                  if(error == nil) {
                                      [self updateMatchInfo:match];
                                      NSString* matchData = [self serializeMathcData:match];
                                      UnitySendMessage("GameCenter_TBM", "OnMatchInvitationAccepted", [ISN_DataConvertor NSStringToChar:matchData]);
                                  } else {
                                      UnitySendMessage("GameCenter_TBM", "OnMatchInvitationAcceptedFailed",  [ISN_DataConvertor serializeError:error]);
                                  }
                              }];
                          } else {
                              
                              UnitySendMessage("GameCenter_TBM", "OnMatchInvitationAcceptedFailed", [ISN_DataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
                          }
                      }
                      
                      -(void) declineInvite:(NSString *)matchId {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN acceptInvite"];
                          GKTurnBasedMatch* match = [self getMatchWithId:matchId];
                          
                          if(match == NULL) {
                              UnitySendMessage("GameCenter_TBM", "OnMatchInvitationDeclineFailed", [ISN_DataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
                              return;
                          }
                          
                          [match declineInviteWithCompletionHandler:^(NSError * _Nullable error) {
                              if(error == nil) {
                                  UnitySendMessage("GameCenter_TBM", "OnMatchInvitationDeclined", [ISN_DataConvertor NSStringToChar:matchId]);
                              } else {
                                  UnitySendMessage("GameCenter_TBM", "OnMatchInvitationDeclineFailed", [ISN_DataConvertor serializeError:error]);
                              }
                          }];
                      }
                      
                      
                      
#pragma private
                      
                      
                      
                      -(GKTurnBasedParticipant *) getMathcParticipantById: (GKTurnBasedMatch*) match playerId: (NSString*)playerId {
                          
#if !TARGET_OS_TV
                          
                          NSArray *participants = match.participants;
                          for(GKTurnBasedParticipant * p in participants){
                              if(p.player == nil) {
                                  if(playerId.length == 0) {
                                      return  p;
                                  }
                              } else {
                                  if([p.playerID isEqualToString:playerId]) {
                                      return p;
                                  }
                              }
                          }
#endif
                          
                          return  nil;
                      }
                      
                      
                      
                      
                      
                      -(void) updateMatchInfo:(GKTurnBasedMatch *)match {
                          
                          BOOL macthFound = FALSE;
                          int replaceIndex = 0;
                          int currentIndex = 0;
                          for(GKTurnBasedMatch * m in matches){
                              
                              
                              if([m.matchID isEqualToString:match.matchID]) {
                                  macthFound = TRUE;
                                  replaceIndex = currentIndex;
                                  break;
                              }
                              
                              currentIndex++;
                          }
                          
                          if(macthFound) {
                              [matches replaceObjectAtIndex:replaceIndex withObject:match];
                          } else {
                              [matches addObject:match];
                          }
                          
                          //TODO: send match state update event
                      }
                      
                      -(GKTurnBasedMatch *) getMatchWithId:(NSString*) matchId {
                          for(GKTurnBasedMatch * m in matches){
                              if([m.matchID isEqualToString:matchId]) {
                                  return m;
                              }
                          }
                          
                          return NULL;
                      }
                      
#pragma Serizlization
                      
                      
                      
                      
                      -(NSString*) serializeMathcData:(GKTurnBasedMatch *)match  {
                          
                          
                          NSMutableString * data = [[NSMutableString alloc] init];
                          
                          
                          [data appendString: match.matchID];
                          [data appendString: UNITY_SPLITTER];
                          
                          [data appendString: [NSString stringWithFormat:@"%ld", (long)match.status]];
                          [data appendString: UNITY_SPLITTER];
                          
                          
                          if(match.message != nil) {
                              [data appendString: match.message];
                          } else {
                              [data appendString:@""];
                          }
                          
                          [data appendString: UNITY_SPLITTER];
                          
                          
                          NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
#if UNITY_VERSION < 500
                          [dateFormatter autorelease];
#endif
                          
                          [dateFormatter setDateFormat: @"yyyy-MM-dd HH:mm:ss"];
                          NSString *creationDateString = [dateFormatter stringFromDate:match.creationDate];
                          if(creationDateString ==  NULL) {
                              creationDateString = [dateFormatter stringFromDate:[NSDate date]];
                          }
                          
                          [data appendString: creationDateString];
                          [data appendString: UNITY_SPLITTER];
                          
                          if(match.matchData != nil) {
                              [data appendString: [match.matchData base64Encoding]];
                          } else {
                              [data appendString: @""];
                          }
                          
                          [data appendString: UNITY_SPLITTER];
                          
                          if(match.currentParticipant.player != nil) {
                              [data appendString:match.currentParticipant.player.playerID];
                          } else {
                              [data appendString: @""];
                          }
                          
                          [data appendString: UNITY_SPLITTER];
                          
                          
                          
                          [data appendString:[self serializeParticipantsInfo:match.participants]];
                          
                          NSString *str = [data copy];
#if UNITY_VERSION < 500
                          [str autorelease];
#endif
                          
                          return str;
                          
                      }
                      
                      -(NSString*) serializeParticipantsInfo :(NSArray*)participants {
                          NSMutableString * data = [[NSMutableString alloc] init];
                          
                          for(GKTurnBasedParticipant * p in participants){
                              [data appendString:[self serializeParticipantInfo:p]];
                              [data appendString: UNITY_SPLITTER];
                          }
                          
                          [data appendString: UNITY_EOF];
                          
                          
                          NSString *str = [data copy];
#if UNITY_VERSION < 500
                          [str autorelease];
#endif
                          
                          return str;
                      }
                      
                      -(NSString*) serializeParticipantInfo :(GKTurnBasedParticipant *) participant {
                          NSMutableString * data = [[NSMutableString alloc] init];
                          
                          
                          if(participant.player != nil) {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Parsing participant ith id: %@",participant.player.playerID];
                              
                              [[ISN_GameCenterManager sharedInstance] savePlayerInfo:participant.player];
                              [data appendString: participant.player.playerID];
                          } else {
                              [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"Parsing participant ith id none "];
                              
                              [data appendString:@""];
                          }
                          [data appendString: UNITY_SPLITTER];
                          
                          
                          [data appendString: [NSString stringWithFormat:@"%ld", (long)participant.status]];
                          [data appendString: UNITY_SPLITTER];
                          [data appendString: [NSString stringWithFormat:@"%ld", (long)participant.matchOutcome]];
                          [data appendString: UNITY_SPLITTER];
                          
                          NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
#if UNITY_VERSION < 500
                          [dateFormatter autorelease];
#endif
                          
                          
                          
                          [dateFormatter setDateFormat: @"yyyy-MM-dd HH:mm:ss"];
                          
                          if(participant.timeoutDate != nil) {
                              NSString *timeoutDateString = [dateFormatter stringFromDate:participant.timeoutDate];
                              [data appendString: timeoutDateString];
                          } else {
                              [data appendString: @"1970-01-01 00:00:00"];
                          }
                          [data appendString: UNITY_SPLITTER];
                          
                          
                          
                          if(participant.lastTurnDate != nil) {
                              NSString *lastTurnDateString = [dateFormatter stringFromDate:participant.lastTurnDate];
                              [data appendString: lastTurnDateString];
                          } else {
                              [data appendString: @"1970-01-01 00:00:00"];
                          }
                          
                          
                          NSString *str = [data copy];
#if UNITY_VERSION < 500
                          [str autorelease];
#endif
                          
                          return str;
                      }
                      
#pragma TMB Delegate methods
                      
                      -(void) turnBasedMatchmakerViewControllerWasCancelled:(GKTurnBasedMatchmakerViewController *)viewController {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: turnBasedMatchmakerViewControllerWasCancelled"];
                          
                          [self.vc dismissViewControllerAnimated:YES completion:nil];
                          UnitySendMessage("GameCenter_TBM", "OnMatchFoundResultFailed", [ISN_DataConvertor serializeErrorWithData:@"User Cancelled" code:0]);
                      }
                      
                      -(void) turnBasedMatchmakerViewController:(GKTurnBasedMatchmakerViewController *)viewController didFailWithError:(NSError *)error {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: turnBasedMatchmakerViewController"];
                          
                          [self.vc dismissViewControllerAnimated:YES completion:nil];
                          UnitySendMessage("GameCenter_TBM", "OnMatchFoundResultFailed", [ISN_DataConvertor serializeError:error]);
                      }
                      
                      -(void) turnBasedMatchmakerViewController:(GKTurnBasedMatchmakerViewController *)viewController playerQuitForMatch:(GKTurnBasedMatch *)match {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: turnBasedMatchmakerViewController"];
                          
                          [self.vc dismissViewControllerAnimated:YES completion:nil];
                          
                          NSString* matchData = [self serializeMathcData:match];
                          UnitySendMessage("GameCenter_TBM", "OnPlayerQuitForMatch", [ISN_DataConvertor NSStringToChar:matchData]);
                          
                      }
                      
                      -(void) turnBasedMatchmakerViewController:(GKTurnBasedMatchmakerViewController *)viewController didFindMatch:(GKTurnBasedMatch *)match {
                          [[ISN_NativeUtility sharedInstance] ISN_NativeLog: @"ISN: turnBasedMatchmakerViewController"];
                          [self.vc dismissViewControllerAnimated:YES completion:nil];
                          
                          NSString* matchData = [self serializeMathcData:match];
                          UnitySendMessage("GameCenter_TBM", "OnMatchFoundResult", [ISN_DataConvertor NSStringToChar:matchData]);
                      }
                      
                      
                      +(GKTurnBasedMatchOutcome) getOutcomeByIntValue:(int) value {
                          switch (value) {
                              case 0:
                                  return GKTurnBasedMatchOutcomeNone;
                              case 1:
                                  return GKTurnBasedMatchOutcomeQuit;
                              case 2:
                                  return GKTurnBasedMatchOutcomeWon;
                              case 3:
                                  return GKTurnBasedMatchOutcomeLost;
                              case 4:
                                  return GKTurnBasedMatchOutcomeTied;
                              case 5:
                                  return GKTurnBasedMatchOutcomeTimeExpired;
                              case 6:
                                  return GKTurnBasedMatchOutcomeFirst;
                              case 7:
                                  return GKTurnBasedMatchOutcomeSecond;
                              case 8:
                                  return GKTurnBasedMatchOutcomeThird;
                              case 9:
                                  return GKTurnBasedMatchOutcomeFourth;
                                  
                              default:
                                  return GKTurnBasedMatchOutcomeNone;
                                  break;
                          }
                      }
                      
                      @end
                      
                      
                      
                      
                      extern "C" {
                          
                          void _initGameCenter ()  {
                              NSLog(@"_initGameCenter");
                              [[ISN_GameCenterManager sharedInstance] authenticateLocalPlayer];
                          }
                          
                          BOOL _ISN_GK_IsUnderage() {
                              return [[GKLocalPlayer localPlayer] isUnderage];
                          }
                          
                          
                          BOOL _ISN_GK_IsAuthenticated() {
                              return  [[GKLocalPlayer localPlayer] isAuthenticated];
                          }
                          
                          
                          void _showLeaderboard(char* leaderboardId, int scope) {
                              [[ISN_GameCenterManager sharedInstance] showLeaderboard:[ISN_DataConvertor charToNSString:leaderboardId] scope:scope];
                          }
                          
                          void _showLeaderboards() {
                              [[ISN_GameCenterManager sharedInstance] showLeaderboardsPopUp];
                          }
                          
                          void _ISN_loadLeaderboardInfo (char* leaderboardId, int requestId) {
                              [[ISN_GameCenterManager sharedInstance] loadLeaderboardInfo:[ISN_DataConvertor charToNSString:leaderboardId] requestId:requestId];
                          }
                          
                          void _ISN_LoadAchievements() {
                              [[ISN_GameCenterManager sharedInstance] loadAchievements];
                          }
                          
                          void _ISN_loadLeaderboardScore(char* leaderboardId, int scope, int collection, int from, int to) {
                              
                              [[ISN_GameCenterManager sharedInstance] retriveScores:[ISN_DataConvertor charToNSString:leaderboardId] scope:scope  collection: collection from:from to:to];
                          }
                          
                          void _showAchievements() {
                              //[GCManager authenticateLocalPlayer];
                              [[ISN_GameCenterManager sharedInstance] showAchievements];
                          }
                          
                          void _ISN_issueLeaderboardChallenge(char* leaderboardId, char* message, char* playerIds) {
                              
                              
                              NSString* str = [ISN_DataConvertor charToNSString:playerIds];
                              NSArray *ids = [str componentsSeparatedByString:@","];
                              
                              [[ISN_GameCenterManager sharedInstance] sendLeaderboardChallenge:[ISN_DataConvertor charToNSString:leaderboardId] message:[ISN_DataConvertor charToNSString:message] playerIds:ids];
                              
                          }
                          
                          void _ISN_issueLeaderboardChallengeWithFriendsPicker(char* leaderboardId, char* message) {
                              
                              NSString* lid = [ISN_DataConvertor charToNSString:leaderboardId];
                              NSString* mes = [ISN_DataConvertor charToNSString:message];
                              
                              [[ISN_GameCenterManager sharedInstance] sendLeaderboardChallengeWithFriendsPicker:lid message:mes];
                              
                          }
                          
                          
                          void _ISN_issueAchievementChallenge(char* achievementId, char* message, char* playerIds) {
                              
                              NSString* str = [ISN_DataConvertor charToNSString:playerIds];
                              NSArray *ids = [str componentsSeparatedByString:@","];
                              
                              [[ISN_GameCenterManager sharedInstance] sendAchievementChallenge:[ISN_DataConvertor charToNSString:achievementId] message:[ISN_DataConvertor charToNSString:message] playerIds:ids];
                          }
                          
                          void _ISN_issueAchievementChallengeWithFriendsPicker(char* leaderboardId, char* message, char* playerIds) {
                              
                              NSString* lid = [ISN_DataConvertor charToNSString:leaderboardId];
                              NSString* mes = [ISN_DataConvertor charToNSString:message];
                              
                              [[ISN_GameCenterManager sharedInstance] sendAchievementChallengeWithFriendsPicker:lid message:mes];
                          }
                          
                          
                          
                          
                          void _reportScore(char* score, char* leaderboardId, char* context) {
                              
                              NSString* lid = [ISN_DataConvertor charToNSString:leaderboardId];
                              NSString* scoreValue = [ISN_DataConvertor charToNSString:score];
                              NSString* scoreContext = [ISN_DataConvertor charToNSString:context];
                              
                              [[ISN_GameCenterManager sharedInstance] reportScore:[scoreValue longLongValue] forCategory:lid scoreContext:[scoreContext longLongValue]];
                          }
                          
                          void _submitAchievement(float percents, char* achievementID, BOOL notifyComplete) {
                              double v = (double) percents;
                              [[ISN_GameCenterManager sharedInstance] submitAchievement:v identifier:[ISN_DataConvertor charToNSString:achievementID] notifyComplete:notifyComplete];
                          }
                          
                          void _resetAchievements() {
                              [[ISN_GameCenterManager sharedInstance] resetAchievements];
                          }
                          
                          void _ISN_loadGKPlayerData(char* playerId) {
                              [[ISN_GameCenterManager sharedInstance] loadPlayerInfoForPlayerWithId:[ISN_DataConvertor charToNSString:playerId]];
                          }
                          
                          void _ISN_loadGKPlayerPhoto(char* playerId, int size) {
                              NSString* mPlayerId = [ISN_DataConvertor charToNSString:playerId];
                              
                              if(size == 0) {
                                  [[ISN_GameCenterManager sharedInstance] loadImageForPlayerWithPlayerId:mPlayerId size:GKPhotoSizeSmall];
                              } else {
                                  [[ISN_GameCenterManager sharedInstance] loadImageForPlayerWithPlayerId:mPlayerId size:GKPhotoSizeNormal];
                              }
                          }
                          
                          void _ISN_RetrieveFriends() {
                              [[ISN_GameCenterManager sharedInstance] retrieveFriends];
                          }
                          
                          
                          void _ISN_getSignature() {
                              [[ISN_GameCenterManager sharedInstance] getSignature];
                          }
                          
                          void _ISN_loadLeaderboardSetInfo() {
                              [[ISN_GameCenterManager sharedInstance] loadLeaderboardSetInfo];
                          }
                          
                          void _ISN_loadLeaderboardsForSet(char* setId) {
                              NSString* lid = [ISN_DataConvertor charToNSString:setId];
                              [[ISN_GameCenterManager sharedInstance] loadLeaderboardsForSet:lid];
                          }
                          
                          void _ISN_ShowNotificationBanner (char* title, char* message)  {
                              [[ISN_GameCenterManager sharedInstance] showNotificationBanner:[ISN_DataConvertor charToNSString:title] message:[ISN_DataConvertor charToNSString:message]];
                          }
                          
                          
                          //--------------------------------------
                          //  RTM
                          //--------------------------------------
                          
                          
                          void _ISN_RTM_FindMatch(int minPlayers, int maxPlayers, char* msg, char* invitations)  {
                              
                              NSString* inviteMessage = [ISN_DataConvertor charToNSString:msg];
                              NSArray* invitationsList = [ISN_DataConvertor charToNSArray:invitations];
                              
                              [[ISN_GameCenterRTM sharedInstance] findMatch:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList];
                          }
                          
                          void _ISN_RTM_FindMatchWithNativeUI(int minPlayers, int maxPlayers, char* msg, char* invitations)  {
                              
                              NSString* inviteMessage = [ISN_DataConvertor charToNSString:msg];
                              NSArray* invitationsList = [ISN_DataConvertor charToNSArray:invitations];
                              
                              [[ISN_GameCenterRTM sharedInstance] findMatchWithNativeUI:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList];
                          }
                          
                          
                          void _ISN_RTM_SetPlayerGroup(int group)  {
                              rtm_playerGroup = group;
                          }
                          
                          void _ISN_RTM_SetPlayerAttributes(int attributes)  {
                              rtm_playerAttributes = attributes;
                          }
                          
                          void ISN_RTM_StartMatchWithInviteID(char* inviteId, BOOL useNativeUI) {
                              NSString* m_inviteId = [ISN_DataConvertor charToNSString:inviteId];
                              
                              [[ISN_GameCenterRTM sharedInstance] startMatchWithInviteID:m_inviteId useNativeUI:useNativeUI];
                          }
                          
                          
                          void ISN_RTM_CancelPendingInviteToPlayerWithId(char* playerId)  {
                              NSString* m_playerId = [ISN_DataConvertor charToNSString:playerId];
                              
                              [[ISN_GameCenterRTM sharedInstance] cancelPendingInviteToPlayerWithId:m_playerId];
                          }
                          
                          void ISN_RTM_CancelMatchSeartch() {
                              [[ISN_GameCenterRTM sharedInstance] cancelMatchSeartch];
                          }
                          
                          void ISN_RTM_FinishMatchmaking () {
                              [[ISN_GameCenterRTM sharedInstance] finishMatchmaking];
                          }
                          
                          void ISN_RTM_QueryActivity () {
                              [[ISN_GameCenterRTM sharedInstance] queryActivity];
                          }
                          
                          
                          void ISN_RTM_QueryPlayerGroupActivity(int group)  {
                              [[ISN_GameCenterRTM sharedInstance] queryPlayerGroupActivity:group];
                          }
                          
                          void ISN_RTM_StartBrowsingForNearbyPlayers() {
                              [[ISN_GameCenterRTM sharedInstance] startBrowsingForNearbyPlayers];
                          }
                          
                          
                          void ISN_RTM_StopBrowsingForNearbyPlayers () {
                              [[ISN_GameCenterRTM sharedInstance] stopBrowsingForNearbyPlayers];
                          }
                          
                          void ISN_RTM_Rematch() {
                              [[ISN_GameCenterRTM sharedInstance] rematch];
                          }
                          void ISN_RTM_Disconnect() {
                              [[ISN_GameCenterRTM sharedInstance] disconnect];
                          }
                          
                          void ISN_RTM_SendData(char* data, char* playersIds, int dataMode) {
                              
                              NSString* mDataString = [ISN_DataConvertor charToNSString:data];
                              NSData * s_data = [[NSData alloc] initWithBase64Encoding:mDataString];
                              
                              NSArray* s_playerIds =  [ISN_DataConvertor charToNSArray:playersIds];
                              
                              
                              GKMatchSendDataMode mode = [ISN_GameCenterRTM getDataModeByIntValue:dataMode];
                              [[ISN_GameCenterRTM sharedInstance] sendData:s_data toPlayersWithIds:s_playerIds withDataMode:mode];
                          }
                          
                          
                          void ISN_RTM_SendDataToAll(char* data, int dataMode) {
                              NSString* mDataString = [ISN_DataConvertor charToNSString:data];
                              NSData * s_data = [[NSData alloc] initWithBase64Encoding:mDataString];
                              
                              GKMatchSendDataMode mode = [ISN_GameCenterRTM getDataModeByIntValue:dataMode];
                              
                              [[ISN_GameCenterRTM sharedInstance] sendDataToAll:s_data withDataMode:mode];
                          }
                          
                          
                          
                          //--------------------------------------
                          //  TBM
                          //--------------------------------------
                          
                          
                          
                          void _ISN_TBM_LoadMatchesInfo()  {
                              [[ISN_GameCenterTBM sharedInstance] loadMatches];
                          }
                          
                          
                          void _ISN_TBM_LoadMatch(char* mId)  {
                              NSString* matchId = [ISN_DataConvertor charToNSString:mId];
                              [[ISN_GameCenterTBM sharedInstance] loadMatch:matchId];
                          }
                          
                          
                          void _ISN_TBM_FindMatch(int minPlayers, int maxPlayers, char* msg, char* invitations)  {
                              NSString* inviteMessage = [ISN_DataConvertor charToNSString:msg];
                              NSArray* invitationsList = [ISN_DataConvertor charToNSArray:invitations];
                              
                              [[ISN_GameCenterTBM sharedInstance] findMatch:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList];
                          }
                          
                          void _ISN_TBM_FindMatchWithNativeUI(int minPlayers, int maxPlayers, char* msg, char* invitations)  {
                              NSString* inviteMessage = [ISN_DataConvertor charToNSString:msg];
                              NSArray* invitationsList = [ISN_DataConvertor charToNSArray:invitations];
                              
                              [[ISN_GameCenterTBM sharedInstance] findMatchWithNativeUI:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList];
                          }
                          
                          void _ISN_TBM_SetPlayerGroup(int group)  {
                              tbm_playerGroup = group;
                          }
                          
                          void _ISN_TBM_SetPlayerAttributes(int attributes)  {
                              tbm_playerAttributes = attributes;
                          }
                          
                          void _ISN_TBM_SaveCurrentTurn(char* matchId, char* data)  {
                              NSString* mId = [ISN_DataConvertor charToNSString:matchId];
                              
                              NSString* mDataString = [ISN_DataConvertor charToNSString:data];
                              NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
                              
                              [[ISN_GameCenterTBM sharedInstance] saveCurrentTurn:mId updatedMatchData:mData];
                          }
                          
                          
                          void _ISN_TBM_EndTurn(char* matchId, char* data, char* nextPlayerId)  {
                              NSString* mId = [ISN_DataConvertor charToNSString:matchId];
                              
                              NSString* mDataString = [ISN_DataConvertor charToNSString:data];
                              NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
                              NSString* mNextPlayerId = [ISN_DataConvertor charToNSString:nextPlayerId];
                              
                              [[ISN_GameCenterTBM sharedInstance] endTurn:mId updatedMatchData:mData nextPlayerId:mNextPlayerId];
                              
                          }
                          
                          void _ISN_TBM_QuitInTurn(char* matchId, int outcome, char* nextPlayerId, char* data) {
                              NSString* mId = [ISN_DataConvertor charToNSString:matchId];
                              
                              NSString* mDataString = [ISN_DataConvertor charToNSString:data];
                              NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
                              NSString* mNextPlayerId = [ISN_DataConvertor charToNSString:nextPlayerId];
                              
                              
                              
                              
                              [[ISN_GameCenterTBM sharedInstance] quitInTurn:mId outcome:outcome nextPlayerId:mNextPlayerId matchData:mData];
                          }
                          
                          void _ISN_TBM_QuitOutOfTurn(char* matchId, int outcome) {
                              NSString* mId = [ISN_DataConvertor charToNSString:matchId];
                              [[ISN_GameCenterTBM sharedInstance] quitOutOfTurn:mId outcome:outcome];
                          }
                          
                          void _ISN_TBM_UpdateParticipantOutcome(char* matchId, int outcome, char* playerId) {
                              NSString* mId = [ISN_DataConvertor charToNSString:matchId];
                              NSString* mPlayerId = [ISN_DataConvertor charToNSString:playerId];
                              
                              [[ISN_GameCenterTBM sharedInstance] updatePlayerOutcome:mId Outcome:outcome playerId:mPlayerId];
                          }
                          
                          
                          void _ISN_TBM_EndMatch(char* matchId, char* data)  {
                              NSString* mId = [ISN_DataConvertor charToNSString:matchId];
                              
                              NSString* mDataString = [ISN_DataConvertor charToNSString:data];
                              NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
                              
                              [[ISN_GameCenterTBM sharedInstance] endMatch:mId matchData:mData];
                          }
                          
                          void _ISN_TBM_Rematch(char* matchId)  {
                              NSString* mId = [ISN_DataConvertor charToNSString:matchId];
                              
                              [[ISN_GameCenterTBM sharedInstance] rematch:mId];
                          }
                          
                          void _ISN_TBM_RemoveMatch(char* matchId)  {
                              NSString* mId = [ISN_DataConvertor charToNSString:matchId];
                              [[ISN_GameCenterTBM sharedInstance] removeMatch:mId];
                          }
                          
                          void _ISN_TBM_DeclineInvite(char* matchId)  {
                              NSString* mId = [ISN_DataConvertor charToNSString:matchId];
                              [[ISN_GameCenterTBM sharedInstance] declineInvite:mId];
                          }
                          
                          void _ISN_TBM_AcceptInvite(char* matchId)  {
                              NSString* mId = [ISN_DataConvertor charToNSString:matchId];
                              [[ISN_GameCenterTBM sharedInstance] acceptInvite:mId];
                          }
                          
                          
                          //--------------------------------------
                          //  Game Saves
                          //--------------------------------------
                          
#if !TARGET_OS_TV
                          void _ISN_SaveGame(char* data, char* name) {
                              
                              NSString* mName = [ISN_DataConvertor charToNSString:name];
                              
                              
                              NSString* mDataString = [ISN_DataConvertor charToNSString:data];
                              NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
                              
                              [[ISN_SaveGame sharedInstance] saveGameData:mData withName:mName];
                              
                          }
                          
                          
                          void _ISN_FetchSavedGames() {
                              [[ISN_SaveGame sharedInstance] fetchSavedGames];
                          }
                          
                          void _ISN_DeleteSavedGame(char* name) {
                              NSString* mName = [ISN_DataConvertor charToNSString:name];
                              
                              [[ISN_SaveGame sharedInstance] deleteSavedGames:mName];
                          }
                          
                          void _ISN_ResolveConflictingSavedGames(char* saves, char* data) {
                              NSArray* savesList = [ISN_DataConvertor charToNSArray:saves];
                              
                              NSString* mDataString = [ISN_DataConvertor charToNSString:data];
                              NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
                              
                              [[ISN_SaveGame sharedInstance] resolveConflictingSavedGames:savesList withData:mData];
                          }
                          
                          void _ISN_LoadSaveData(char* name) {
                              NSString* mName = [ISN_DataConvertor charToNSString:name];
                              
                              [[ISN_SaveGame sharedInstance] loadSaveData:mName];
                          }
                          
#endif
                          
                          
                          //--------------------------------------
                          //  Friends Invite
                          //--------------------------------------
                          
                          void  _ISN_GK_SendFriendRequest(int requestId, char* emails, char* players) {
                              NSArray* emailsList = [ISN_DataConvertor charToNSArray:emails];
                              NSArray* playersIdsList = [ISN_DataConvertor charToNSArray:players];
                              
                              NSMutableArray *PlayersArray = [[NSMutableArray alloc] init];
                              
                              for(NSString* playerId in playersIdsList) {
                                  
                                  GKPlayer* player = [[ISN_GameCenterManager sharedInstance] getPlayerWithId:playerId];
                                  if(player != nil) {
                                      [PlayersArray addObject:player];
                                  }
                              }
                              [[ISN_GameCenterManager sharedInstance] inviteFirends:requestId emails:emailsList players:PlayersArray];
                              
                          }
                          
                          
                          
                      }
