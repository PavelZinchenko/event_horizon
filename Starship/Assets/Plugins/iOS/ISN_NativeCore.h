////////////////////////////////////////////////////////////////////////////////
//
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets)
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


@interface NSData (Base64)

+ (NSData *)InitFromBase64String:(NSString *)aString;
- (NSString *)AsBase64String;

@end

@interface NSDictionary (JSON)

- (NSString *)AsJSONString;

@end

@interface ISN_DataConvertor : NSObject

+ (NSString*) charToNSString: (char*)value;
+ (const char *) NSIntToChar: (NSInteger) value;
+ (const char *) NSStringToChar: (NSString *) value;
+ (NSArray*) charToNSArray: (char*)value;

+ (const char *) serializeErrorWithData:(NSString *)description code: (int) code;
+ (const char *) serializeError:(NSError *)error;

+ (NSMutableString *) serializeErrorWithDataToNSString:(NSString *)description code: (int) code;
+ (NSMutableString *) serializeErrorToNSString:(NSError *)error;


+ (const char *) NSStringsArrayToChar:(NSArray *) array;
+ (NSString *) serializeNSStringsArray:(NSArray *) array;

@end


@interface ISN_NativeUtility : NSObject

@property (strong)  UIActivityIndicatorView *spinner;

+ (id) sharedInstance;
+ (int) majorIOSVersion;
+ (BOOL) IsIPad;

- (void) redirectToRatingPage: (NSString *) appId;
- (void) setApplicationBagesNumber:(int) count;

- (void) ShowSpinner;
- (void) HideSpinner;
- (void) ISN_NativeLog: (NSString *) appId, ...;
- (void) ISN_SetLogState: (BOOL) appId;

@end

@interface CloudManager : NSObject


+ (id) sharedInstance;

- (void) initialize;
- (void) setString:(NSString*) val key:(NSString*) key;
- (void) setDouble:(double) val key:(NSString*) key;
- (void) setData:(NSData*) val key:(NSString*) key;

-(void) requestDataForKey:(NSString*) key;

@end





@interface ISN_NativePopUpsManager : NSObject
+ (ISN_NativePopUpsManager *) sharedInstance;
@end



@interface IOSNativeNotificationCenter : NSObject


+ (IOSNativeNotificationCenter *)sharedInstance;
- (void) scheduleNotification: (int) time message: (NSString*) message sound: (bool *)sound alarmID:(NSString *)alarmID badges: (int)badges notificationData: (NSString*) notificationData notificationSoundName: (NSString*) notificationSoundName;
- (void) cleanUpLocalNotificationWithAlarmID: (NSString *)alarmID;
- (void) cancelNotifications;
- (void) applicationIconBadgeNumber: (int)badges;
- (void) RegisterForNotifications;

@end


@interface UIView (ZolaZoomSnapshot)
- (UIImage *)zo_snapshot;
@end

@protocol ZOZolaZoomTransitionDelegate;

typedef NS_ENUM(NSInteger, ZOTransitionType) {
    ZOTransitionTypePresenting,
    ZOTransitionTypeDismissing
};

@interface ZOZolaZoomTransition : NSObject <UIViewControllerAnimatedTransitioning>

- (instancetype)initWithTargetView:(UIView *)targetView
                              type:(ZOTransitionType)type
                          duration:(NSTimeInterval)duration
                          delegate:(id<ZOZolaZoomTransitionDelegate>)delegate NS_DESIGNATED_INITIALIZER;


+ (instancetype)transitionFromView:(UIView *)targetView
                              type:(ZOTransitionType)type
                          duration:(NSTimeInterval)duration
                          delegate:(id<ZOZolaZoomTransitionDelegate>)delegate;


- (instancetype)init NS_UNAVAILABLE;


@property (strong, nonatomic) UIColor *fadeColor;
@property (weak, nonatomic) id<ZOZolaZoomTransitionDelegate> delegate;
@property (strong, nonatomic) UIView *targetView;
@property (assign, nonatomic) ZOTransitionType type;
@property (assign, nonatomic) NSTimeInterval duration;

@end

@protocol ZOZolaZoomTransitionDelegate <NSObject>

@required


- (CGRect)zolaZoomTransition:(ZOZolaZoomTransition *)zoomTransition
        startingFrameForView:(UIView *)targetView
              relativeToView:(UIView *)relativeView
          fromViewController:(UIViewController *)fromViewController
            toViewController:(UIViewController *)toViewController;


- (CGRect)zolaZoomTransition:(ZOZolaZoomTransition *)zoomTransition
       finishingFrameForView:(UIView *)targetView
              relativeToView:(UIView *)relativeView
          fromViewController:(UIViewController *)fromViewController
            toViewController:(UIViewController *)toViewController;

@optional

- (NSArray *)supplementaryViewsForZolaZoomTransition:(ZOZolaZoomTransition *)zoomTransition;

- (CGRect)zolaZoomTransition:(ZOZolaZoomTransition *)zoomTransition
   frameForSupplementaryView:(UIView *)supplementaryView
              relativeToView:(UIView *)relativeView;

@end

@interface MonthLayerCollectionViewCell : UICollectionViewCell
@property CATextLayer *monthTitle;
@property int startWeekDay, numberOfDays;
- (void)setStartDay:(int)startDay;
- (void)setNumberOfDays:(int)numberOfDays;
@end

#if !TARGET_OS_TV

@interface CalendarPickerController : UINavigationController
+ (instancetype)defaultPicker;
+ (instancetype)initWithStartYear:(NSInteger)calendarStartYear endYear:(NSInteger)calendarEndYear withStartDayIsSunday:(BOOL)startDayIsSunday;
@property NSBundle *bundle;
@property NSCalendar *calendar;
@property NSArray<NSString *> *monthNames, *dayNumberStrings;
@property UIToolbar* topToolbar;
@property BOOL startDayIsSunday;
@end

@interface MonthViewController : UICollectionViewController
+ (instancetype)defaultControllerWithYear:(NSInteger)year andMonth:(NSInteger)month withStartDayIsSunday:(BOOL)startDayIsSunday;
@property NSInteger year, currentMonth;
@property NSCalendar *calendar;
@property NSArray<NSString *> *monthNames, *dayNumberStrings;
@property NSMutableDictionary *cellDaysDataByIndexPath;
@property CGSize itemSize;
@property BOOL startDayIsSunday;
@property NSArray *holidays;
@end

@interface YearViewController : UICollectionViewController<UINavigationControllerDelegate, ZOZolaZoomTransitionDelegate>
+ (instancetype) initWithStartYear:(NSInteger)calendarStartYear endYear:(NSInteger)calendarEndYear withStartDayIsSunday:(BOOL)startDayIsSunday;
+ (instancetype) defaultController;

@property NSDate *currentDate;
@property NSInteger startYear, endYear, currentYear;
@property NSCalendar *calendar;
@property NSMutableArray<NSNumber *> *startWeekdays, *numberOfDays;
@property NSArray<NSString *> *monthNames, *dayNumberStrings;
@property NSMutableDictionary *cellDaysDataByIndexPath;
@property CGSize itemSize;
@property BOOL startDayIsSunday;
@property UIView *viewForZooming;
@end

@interface YearSectionHeaderReusableView : UICollectionReusableView
@property CATextLayer *yearTitle;
@property CALayer *bottomLine;
@end

@interface LCTextLayer : CATextLayer
@end

@interface DayInYearViewCell : UICollectionViewCell
@property LCTextLayer *dayNumber;
@property CALayer *topLine;
@end

@interface MonthSectionHeaderReusableView : UICollectionReusableView

@property CATextLayer *monthTitle;
@property CGFloat offset, width;

@end
#endif
