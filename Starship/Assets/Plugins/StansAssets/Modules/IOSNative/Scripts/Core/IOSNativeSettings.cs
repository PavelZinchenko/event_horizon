////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.IO;
using System.Collections.Generic;
using SA.IOSNative.StoreKit;


#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class IOSNativeSettings : ScriptableObject {

    public const string VERSION_NUMBER = "9.11/" + SA.Common.Config.LIB_VERSION;


	//--------------------------------------
	// Modules State
	//--------------------------------------

	public bool EnableGameCenterAPI 		= true;
	public bool EnableInAppsAPI 			= true;
	public bool EnableCameraAPI 			= true;
	public bool EnableSocialSharingAPI 		= true;

	public bool EnablePickerAPI 		= false;
	public bool EnableMediaPlayerAPI 	= false;
	public bool EnableReplayKit 		= false;
	public bool EnableCloudKit 			= false;
	public bool EnableSoomla 			= false;
	public bool EnableGestureAPI 		= false;
	public bool EnableForceTouchAPI 	= false;

	public bool EnablePushNotificationsAPI = false;
	public bool EnableContactsAPI = false;
	public bool EnableAppEventsAPI = false;
	public bool EnableUserNotificationsAPI = false;

	public bool EnablePermissionAPI = false;



	//--------------------------------------
	// IOS Applications
	//--------------------------------------

	public string AppleId = "XXXXXXXXX";


	//--------------------------------------
	// Editor
	//--------------------------------------

	public int ToolbarIndex = 0;
	public bool ExpandMoreActionsMenu = true;
	public bool ExpandModulesSettings = true;


	//--------------------------------------
	// StoreKit Settings
	//--------------------------------------

	public bool InAppsEditorTesting = true;
	public bool CheckInternetBeforeLoadRequest = false;
	public bool PromotedPurchaseSupport = true;
	public TransactionsHandlingMode  TransactionsHandlingMode = TransactionsHandlingMode.Automatic;

	public List<string> DefaultStoreProductsView = new List<string>();
	public List<Product> InAppProducts =  new List<Product>();

	//EditorOnly
	public bool ShowStoreKitProducts = true;



	//--------------------------------------
	// Game Center Settings
	//--------------------------------------

	public List<GK_Leaderboard> Leaderboards =  new List<GK_Leaderboard>();
	public List<GK_AchievementTemplate> Achievements =  new List<GK_AchievementTemplate>();

	public bool UseGCRequestCaching = false;
	public bool UsePPForAchievements = false;
	public bool AutoLoadUsersSmallImages = true;
	public bool AutoLoadUsersBigImages = false;

	//EditorOnly
	public bool ShowLeaderboards = true;
	public bool ShowAchievementsParams = true;


	//--------------------------------------
	// Advertisement
	//--------------------------------------

	public bool AdEditorTesting = true;
	public int EditorFillRateIndex = 4;
	public int EditorFillRate = 100;



	//--------------------------------------
	// Camera  / Gallery
	//--------------------------------------


	public int  MaxImageLoadSize = 512;
	public float JPegCompressionRate = 0.8f;
	public IOSGalleryLoadImageFormat GalleryImageFormat = IOSGalleryLoadImageFormat.JPEG;


	//--------------------------------------
	// Reaply Kit
	//--------------------------------------

	public int RPK_iPadViewType = 0;


	//--------------------------------------
	// Build Settings
	//--------------------------------------

	public string CameraUsageDescription = "for making pictures";
	public string PhotoLibraryUsageDescription = "for taking pictures";
	public string AppleMusicUsageDescription = "for playing music";
	public string ContactsUsageDescription = "for contacts reading";

	//URL Schemes
	public List<SA.IOSNative.Models.UrlType> UrlTypes = new List<SA.IOSNative.Models.UrlType> ();
	public List<SA.IOSNative.Models.UrlType> ApplicationQueriesSchemes = new List<SA.IOSNative.Models.UrlType> ();

	//Force Touch
	public List<SA.IOSNative.Gestures.ForceTouchMenuItem> ForceTouchMenu = new List<SA.IOSNative.Gestures.ForceTouchMenuItem> ();


	//--------------------------------------
	// IOS Native Settings
	//--------------------------------------

	public bool DisablePluginLogs = false;



	//--------------------------------------
	// Third Party
	//--------------------------------------



	//Soomla
	public string SoomlaDownloadLink = "http://goo.gl/7LYwuj";
	public string SoomlaDocsLink = "https://goo.gl/JFkpNa";
	public string SoomlaGameKey = "" ;
	public string SoomlaEnvKey = "" ;

	//One Signal
	public bool OneSignalEnabled = false;
	public string OneSignalDocsLink = "https://goo.gl/Royty6";




	private const string ISNSettingsAssetName = "IOSNativeSettings";
	private const string ISNSettingsAssetExtension = ".asset";

	private static IOSNativeSettings instance = null;

	public static IOSNativeSettings Instance {
		get {
			if (instance == null) {
				instance = Resources.Load(ISNSettingsAssetName) as IOSNativeSettings;
				
				if (instance == null) {
					
					// If not found, autocreate the asset object.
					instance = CreateInstance<IOSNativeSettings>();
					#if UNITY_EDITOR


					SA.Common.Util.Files.CreateFolder(SA.Common.Config.SETTINGS_PATH);
					string fullPath = Path.Combine(Path.Combine("Assets", SA.Common.Config.SETTINGS_PATH),
					                               ISNSettingsAssetName + ISNSettingsAssetExtension
					                               );
					
					AssetDatabase.CreateAsset(instance, fullPath);
					#endif
				}
			}
			return instance;
		}
	}

}
