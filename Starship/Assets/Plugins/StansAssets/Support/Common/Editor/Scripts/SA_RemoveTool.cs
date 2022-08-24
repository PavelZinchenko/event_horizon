////////////////////////////////////////////////////////////////////////////////
//  
// @module Stan's Assets Commons Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;


namespace SA.Common.Editor {

	public class RemoveTool  {
		
		
		public static void RemoveOneSignal() {
			RemoveNativeFileIOS("libOneSignal");
			RemoveNativeFileIOS("OneSignal");
			RemoveNativeFileIOS("OneSignalUnityRuntime");
			SA.Common.Util.Files.DeleteFolder("StansAssetsCommon/OneSignal");
		}
		
		
		
		public static void RemovePlugins() {
			
			int option = EditorUtility.DisplayDialogComplex(
				"Remove Stan's Assets Plugins",
				"Following plugins wiil be removed:\n" + VersionsManager.InstalledPluginsList,
				"Remove",
				"Cancel",
				"Documentation");
			
			
			switch(option) {
			case 0:
				ProcessRemove();
				break;
				
			case 2:
				string url = "https://goo.gl/CCBFIZ";
				Application.OpenURL(url);
				break;
			}
			
		}
		
		
		
		private static void ProcessRemove() {

			//Old plugins location, just in case
			SA.Common.Util.Files.DeleteFolder ("Extensions/AllDocumentation");
			SA.Common.Util.Files.DeleteFolder ("Extensions/FlashLikeEvents");
			SA.Common.Util.Files.DeleteFolder ("Extensions/AndroidManifestManager");
			SA.Common.Util.Files.DeleteFolder ("Extensions/GooglePlayCommon");
			SA.Common.Util.Files.DeleteFolder ("Extensions/StansAssetsCommon");
			SA.Common.Util.Files.DeleteFolder ("Extensions/StansAssetsPreviewUI");
			SA.Common.Util.Files.DeleteFolder ("Extensions/IOSDeploy");
			SA.Common.Util.Files.DeleteFolder ("Extensions/AndroidNative");
			SA.Common.Util.Files.DeleteFolder ("Extensions/MobileSocialPlugin");
			SA.Common.Util.Files.DeleteFolder ("Extensions/GoogleMobileAd");
			SA.Common.Util.Files.DeleteFolder("Extensions/IOSNative");
			SA.Common.Util.Files.DeleteFolder("Extensions/UltimateMobile");
			SA.Common.Util.Files.DeleteFolder("Extensions/WP8Native");
			SA.Common.Util.Files.DeleteFolder("WebPlayerTemplates");
			SA.Common.Util.Files.DeleteFolder("Extensions/GoogleAnalytics");
			SA.Common.Util.Files.DeleteFolder("Extensions/MobileNativePopUps");




			RemoveWP8Part();
			RemoveIOSPart();
			RemoveAndroidPart();


			SA.Common.Util.Files.DeleteFolder (SA.Common.Config.MODULS_PATH);
			SA.Common.Util.Files.DeleteFolder (SA.Common.Config.BUNDLES_PATH);

	
			SA.Common.Util.Files.DeleteFolder (SA.Common.Config.COMMON_LIB_PATH);
			SA.Common.Util.Files.DeleteFolder (SA.Common.Config.VERSION_INFO_PATH);
			SA.Common.Util.Files.DeleteFolder (SA.Common.Config.NATIVE_LIBRARIES_PATH);
			SA.Common.Util.Files.DeleteFolder (SA.Common.Config.EDITOR_TESTING_LIB_PATH);
		



			AssetDatabase.Refresh();
			EditorUtility.DisplayDialog("Plugins Removed Successfully", "Unity Editor Restart Required.", "Okay");
		}
		
		
		
		
		
		private static void RemoveAndroidPart() {
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "androidnative.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "mobilenativepopups.jar");

			SA.Common.Util.Files.DeleteFolder (SA.Common.Config.ANDROID_DESTANATION_PATH + "libs");
		}
		
		
		private static void RemoveWP8Part() {
			SA.Common.Util.Files.DeleteFile ("Plugins/WP8/GoogleAds.dll");
			SA.Common.Util.Files.DeleteFile ("Plugins/WP8/GoogleAds.xml");
			SA.Common.Util.Files.DeleteFile ("Plugins/WP8/MockIAPLib.dll");
			SA.Common.Util.Files.DeleteFile ("Plugins/WP8/WP8Native.dll");
			SA.Common.Util.Files.DeleteFile ("Plugins/WP8/WP8PopUps.dll");
			SA.Common.Util.Files.DeleteFile ("Plugins/WP8/GoogleAdsWP8.dll");
			SA.Common.Util.Files.DeleteFile ("Plugins/GoogleAdsWP8.dll");
			SA.Common.Util.Files.DeleteFile ("Plugins/Metro/WP8Native.dll");
			SA.Common.Util.Files.DeleteFile ("Plugins/Metro/WP8PopUps.dll");
		}
		
		
		private static void RemoveIOSPart() {
			//TODO просмотреть не забыли ли чего лучге смотреть в УМ
			
			//ISN
			RemoveNativeFileIOS("AppEventListener");
			RemoveNativeFileIOS("CloudManager");
			RemoveNativeFileIOS("CustomBannerView");
			RemoveNativeFileIOS("iAdBannerController");
			RemoveNativeFileIOS("iAdBannerObject");
			RemoveNativeFileIOS("InAppPurchaseManager");
			RemoveNativeFileIOS("IOSNativeNotificationCenter");
			RemoveNativeFileIOS("ISN_GameCenterListner");
			RemoveNativeFileIOS("ISN_GameCenterManager");
			RemoveNativeFileIOS("ISN_GameCenter");
			RemoveNativeFileIOS("ISN_Media");
			RemoveNativeFileIOS("ISN_iAd");
			RemoveNativeFileIOS("ISN_InApp");
			RemoveNativeFileIOS("ISN_NativePopUpsManager");
			RemoveNativeFileIOS("ISN_NativeUtility");
			RemoveNativeFileIOS("ISN_NSData+Base64");
			RemoveNativeFileIOS("ISN_Reachability");
			RemoveNativeFileIOS("ISN_Security");
			RemoveNativeFileIOS("ISN_Camera");
			RemoveNativeFileIOS("ISN_ReplayKit");
			RemoveNativeFileIOS("ISN_SocialGate");
			RemoveNativeFileIOS("ISN_NativeCore");
			RemoveNativeFileIOS("ISNDataConvertor");
			RemoveNativeFileIOS("ISNSharedApplication");
			RemoveNativeFileIOS("ISNVideo");
			RemoveNativeFileIOS("SKProduct+LocalizedPrice");
			RemoveNativeFileIOS("SocialGate");
			RemoveNativeFileIOS("StoreProductView");
			RemoveNativeFileIOS("TransactionServer");
			
			
			//UM
			RemoveNativeFileIOS("UM_IOS_INSTALATION_MARK");
			
			//GMA
			RemoveNativeFileIOS("GoogleMobileAdBanner");
			RemoveNativeFileIOS("GoogleMobileAdController");
			
			//MPS
			RemoveNativeFileIOS("IOSInstaPlugin");
			RemoveNativeFileIOS("IOSTwitterPlugin");
			RemoveNativeFileIOS("MGInstagram");
			
			
			RemoveOneSignal();
		}
		
		
		
		
		
		
		private static void RemoveNativeFileIOS(string filename) {
			string filePath = SA.Common.Config.IOS_DESTANATION_PATH  + filename;
			
			SA.Common.Util.Files.DeleteFile (filePath + ".h");
			SA.Common.Util.Files.DeleteFile (filePath + ".m");
			SA.Common.Util.Files.DeleteFile (filePath + ".mm");
			SA.Common.Util.Files.DeleteFile (filePath + ".a");
			SA.Common.Util.Files.DeleteFile (filePath + ".txt");
			
		}
		
	}
}
#endif
