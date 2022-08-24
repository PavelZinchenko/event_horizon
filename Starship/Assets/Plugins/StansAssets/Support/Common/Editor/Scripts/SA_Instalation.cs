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

namespace SA.Common.Editor {

	public class Instalation : MonoBehaviour {
		
		private const string GA_FIREBASE_IDS_FILE_START = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<resources>\n  <string name=\"google_app_id\">";
		private const string GA_FIREBASE_IDS_FILE_END = "</string>\n</resources>\n";		
		
		
		public static void IOS_UpdatePlugin() {
			IOS_InstallPlugin(false);
		}
		
		public static void IOS_InstallPlugin(bool IsFirstInstall = true) {
			
			IOS_CleanUp();			
			
			//IOS Native
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_Camera.mm.txt", 				SA.Common.Config.IOS_DESTANATION_PATH + "ISN_Camera.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_GameCenter.mm.txt", 			SA.Common.Config.IOS_DESTANATION_PATH + "ISN_GameCenter.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_iAd.mm.txt", 					SA.Common.Config.IOS_DESTANATION_PATH + "ISN_iAd.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_InApp.mm.txt", 					SA.Common.Config.IOS_DESTANATION_PATH + "ISN_InApp.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_Media.mm.txt", 					SA.Common.Config.IOS_DESTANATION_PATH + "ISN_Media.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_ReplayKit.mm.txt", 				SA.Common.Config.IOS_DESTANATION_PATH + "ISN_ReplayKit.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_GestureRecognizer.mm.txt", 		SA.Common.Config.IOS_DESTANATION_PATH + "ISN_GestureRecognizer.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_CloudKit.mm.txt", 				SA.Common.Config.IOS_DESTANATION_PATH + "ISN_CloudKit.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_NSData+Base64.h.txt", 			SA.Common.Config.IOS_DESTANATION_PATH + "ISN_NSData+Base64.h");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_NSData+Base64.m.txt", 			SA.Common.Config.IOS_DESTANATION_PATH + "ISN_NSData+Base64.m");			


			
			IOS_Install_SocialPart();
			InstallGMAPart();		
			Instal_MNP ();
		}
		
		public static void InstallGMAPart() {
			//GMA
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "GMA_SA_Lib_Proxy.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "GMA_SA_Lib_Proxy.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "GMA_SA_Lib.h.txt", 			SA.Common.Config.IOS_DESTANATION_PATH + "GMA_SA_Lib.h");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "GMA_SA_Lib.m.txt", 			SA.Common.Config.IOS_DESTANATION_PATH + "GMA_SA_Lib.m");
			
		}
		
		
		public static void IOS_Install_SocialPart() {
			//IOS Native +  MSP
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_SocialGate.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_SocialGate.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_NativeCore.h.txt", 		SA.Common.Config.IOS_DESTANATION_PATH + "ISN_NativeCore.h");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_NativeCore.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_NativeCore.mm");
		}


		public static void Instal_MNP() {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "MNP_UIController.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "MNP_UIController.mm");
		}
		
		
		
		public static void Remove_FB_SDK_WithDialog() {
			bool result = EditorUtility.DisplayDialog(
				"Removing Facebook SDK",
				"Are you sure you want to remove Facebook OAuth API?",
				"Remove",
				"Cansel");
			
			if(result) {
				Remove_FB_SDK();
			}
		}
		public static void Remove_FB_SDK() {
			
			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.ANDROID_DESTANATION_PATH + "facebook");
			SA.Common.Util.Files.DeleteFolder("Plugins/facebook", false);
			SA.Common.Util.Files.DeleteFolder("Facebook", false);
            SA.Common.Util.Files.DeleteFolder("Plugins/Facebook", false);
            SA.Common.Util.Files.DeleteFolder("FacebookSDK", false);
            SA.Common.Util.Files.DeleteFolder("Plugins/FacebookSDK", false);

            //MSP
            SA.Common.Util.Files.DeleteFile("Extensions/MobileSocialPlugin/Example/Scripts/MSPFacebookUseExample.cs", false);
			SA.Common.Util.Files.DeleteFile("Extensions/MobileSocialPlugin/Example/Scripts/MSP_FacebookAnalyticsExample.cs", false);
			SA.Common.Util.Files.DeleteFile("Extensions/MobileSocialPlugin/Example/Scripts/MSP_FacebookAndroidTurnBasedAndGiftsExample.cs", false);
			
			//FB v7
			SA.Common.Util.Files.DeleteFolder("Examples", false);
			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.IOS_DESTANATION_PATH + "Facebook", false);
			
			
			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/bolts-android-1.2.0.jar");
			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/facebook-android-sdk-4.7.0.jar");
			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/facebook-android-wrapper-release.jar");
			
			AssetDatabase.Refresh();
		}
		
		
		private static string AN_SoomlaGrowContent = SA.Common.Config.MODULS_PATH + "AndroidNative/Other/Soomla/AN_SoomlaGrow.cs";
		public static void DisableSoomlaFB() {
			ChnageDefineState(AN_SoomlaGrowContent, "FACEBOOK_ENABLED", false);
		}
		
		
		
		
		
		private static void ChnageDefineState(string file, string tag, bool IsEnabled) {
			
			if(!SA.Common.Util.Files.IsFileExists(file)) {
				Debug.Log("ChnageDefineState for tag: " + tag + " File not found at path: " + file);
				return;
			}
			
			string content = SA.Common.Util.Files.Read(file);
			
			int endlineIndex;
			endlineIndex = content.IndexOf(System.Environment.NewLine);
			if(endlineIndex == -1) {
				endlineIndex = content.IndexOf("\n");
			}
			
			string TagLine = content.Substring(0, endlineIndex);
			
			if(IsEnabled) {
				content 	= content.Replace(TagLine, "#define " + tag);
			} else {
				content 	= content.Replace(TagLine, "//#define " + tag);
			}
			
			SA.Common.Util.Files.Write(file, content);
			
		}
		
		
		public static void IOS_CleanUp() {
			
			
			//Old APi
			RemoveIOSFile("AppEventListener");
			RemoveIOSFile("CloudManager");
			RemoveIOSFile("CustomBannerView");
			RemoveIOSFile("GameCenterManager");
			RemoveIOSFile("GCHelper");
			RemoveIOSFile("iAdBannerController");
			RemoveIOSFile("iAdBannerObject");
			RemoveIOSFile("InAppPurchaseManager");
			RemoveIOSFile("IOSGameCenterManager");
			RemoveIOSFile("IOSNativeNotificationCenter");
			RemoveIOSFile("IOSNativePopUpsManager");
			RemoveIOSFile("IOSNativeUtility");
			RemoveIOSFile("ISN_NSData+Base64");
			RemoveIOSFile("ISN_Reachability");
			RemoveIOSFile("ISNCamera");
			RemoveIOSFile("ISNDataConvertor");
			RemoveIOSFile("ISNSharedApplication");
			RemoveIOSFile("ISNVideo");
			RemoveIOSFile("PopUPDelegate");
			RemoveIOSFile("RatePopUPDelegate");
			RemoveIOSFile("SKProduct+LocalizedPrice");
			RemoveIOSFile("SocialGate");
			RemoveIOSFile("StoreProductView");
			RemoveIOSFile("TransactionServer");
			
			RemoveIOSFile("OneSignalUnityRuntime");
			RemoveIOSFile("OneSignal");
			RemoveIOSFile("libOneSignal");
			RemoveIOSFile("ISN_Security");
			RemoveIOSFile("ISN_NativeUtility");
			RemoveIOSFile("ISN_NativePopUpsManager");
			RemoveIOSFile("ISN_Media");
			RemoveIOSFile("ISN_GameCenterTBM");
			RemoveIOSFile("ISN_GameCenterRTM");
			RemoveIOSFile("ISN_GameCenterManager");
			RemoveIOSFile("ISN_GameCenterListner");
			RemoveIOSFile("IOSNativeNotificationCenter");
			
			
			
			//New API
			RemoveIOSFile("ISN_Camera");
			RemoveIOSFile("ISN_GameCenter");
			RemoveIOSFile("ISN_InApp");
			RemoveIOSFile("ISN_iAd");
			RemoveIOSFile("ISN_NativeCore");
			RemoveIOSFile("ISN_SocialGate");
			RemoveIOSFile("ISN_ReplayKit");
			RemoveIOSFile("ISN_CloudKit");
			RemoveIOSFile("ISN_Soomla");
			RemoveIOSFile("ISN_GestureRecognizer");
			
			
			
			//Google Ad old v1
			RemoveIOSFile("GADAdMobExtras");
			RemoveIOSFile("GADAdNetworkExtras");
			RemoveIOSFile("GADAdSize");
			RemoveIOSFile("GADBannerViewDelegate");
			RemoveIOSFile("GADInAppPurchase");
			RemoveIOSFile("GADInAppPurchaseDelegate");
			RemoveIOSFile("GADInterstitialDelegate");
			RemoveIOSFile("GADModules");
			RemoveIOSFile("GADRequest");
			RemoveIOSFile("GADRequestError");
			RemoveIOSFile("libGoogleAdMobAds");
			
			//Google Ad old v2
			RemoveIOSFile("GoogleMobileAdBanner");
			RemoveIOSFile("GoogleMobileAdController");
			
			
			//Google Ad new
			RemoveIOSFile("GMA_SA_Lib");
			
			
			//MSP old
			RemoveIOSFile("IOSInstaPlugin");
			RemoveIOSFile("IOSTwitterPlugin");
			RemoveIOSFile("MGInstagram");
			

			//MNP
			RemoveIOSFile("MNP_UIController");
			
			
			
		}
		
		
		public static void RemoveIOSFile(string filename) {
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.IOS_DESTANATION_PATH + filename + ".h");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.IOS_DESTANATION_PATH + filename + ".m");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.IOS_DESTANATION_PATH + filename + ".mm");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.IOS_DESTANATION_PATH + filename + ".a");
		}
		
		
		public static void Android_UpdatePlugin() {
			Android_InstallPlugin(false);
		}
		
		public static void EnableFirebaseAnalytics() {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "firebase/firebase-analytics.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/firebase-analytics.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "firebase/firebase-analytics-impl.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/firebase-analytics-impl.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "firebase/firebase-common.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/firebase-common.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "firebase/firebase-iid.txt", 					SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/firebase-iid.aar");
		}

		public static void DisableFirebaseAnalytics() {
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/firebase-analytics.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/firebase-analytics-impl.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/firebase-common.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/firebase-iid.aar");
		}

		public static void EnableFirebaseDynamicLinks() {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "firebase/firebase-dynamic-links.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/firebase-dynamic-links.aar");
		}

		public static void DisableFirebaseDynamicLinks() {
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/firebase-dynamic-links.aar");
		}

		public static void EnableGoogleFit() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "fitness/an_fitness.jar.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_fitness.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "fitness/play-services-fitness.jar.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-fitness.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "fitness/an_fitness.txt", 					SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_fitness.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "fitness/play-services-fitness.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-fitness.aar");
			#endif
		}

		public static void DisableGoogleFit() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_fitness.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-fitness.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_fitness.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-fitness.aar");
			#endif
		}
		
		public static void EnableGooglePlayAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/an_googleplay.jar.txt", 					SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_googleplay.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-base.jar.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-base.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-basement.jar.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-basement.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-ads.jar.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-ads-lite.jar.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-games.jar.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-games.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-iid.jar.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-iid.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-gcm.jar.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-gcm.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-plus.jar.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-plus.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-appinvite.jar.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-analytics.jar.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-analytics-impl.jar.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics-impl.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-auth.jar.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-auth-base.jar.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-drive.jar.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-drive.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-tasks.jar.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-tasks.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/an_googleplay.txt", 					SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_googleplay.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-base.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-base.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-basement.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-basement.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-ads.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-ads-lite.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-games.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-games.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-iid.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-iid.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-gcm.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-gcm.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-plus.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-plus.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-appinvite.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-analytics.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-analytics-impl.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics-impl.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-auth.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-auth-base.txt", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-drive.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-drive.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-tasks.txt", 				SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-tasks.aar");
			#endif
		}
		
		public static void DisableGooglePlayAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_googleplay.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-base.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-basement.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-games.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-iid.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-gcm.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-plus.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics-impl.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-drive.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-tasks.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_googleplay.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-base.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-basement.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-games.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-iid.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-gcm.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-plus.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics-impl.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-drive.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-tasks.aar");
			#endif
		}

		public static void EnableDriveAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-drive.jar.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-drive.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-drive.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-drive.aar");
			#endif
		}

		public static void DisableDriveAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-drive.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-drive.aar");
			#endif
		}

		public static void EnableOAuthAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-auth.jar.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-auth-base.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.aar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-auth.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-auth-base.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.aar");
			#endif
		}

		public static void DisableOAuthAPI(){
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.aar");
			#endif
		}

		public static void EnableAnalyticsAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-analytics.jar.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-analytics-impl.jar.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics-impl.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-analytics.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-analytics-impl.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics-impl.aar");
			#endif
		}

		public static void DisableAnalyticsAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics-impl.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-analytics-impl.aar");
			#endif
		}

		public static void EnableAppInvitesAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-appinvite.jar.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-appinvite.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.aar");
			#endif
		}

		public static void DisableAppInvitesAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.aar");
			#endif
		}

		public static void EnableGooglePlusAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-plus.jar.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-plus.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-plus.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-plus.aar");
			#endif
		}

		public static void DisableGooglePlusAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-plus.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-plus.aar");
			#endif
		}

		public static void EnablePushNotificationsAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-iid.jar.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-iid.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-gcm.jar.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-gcm.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-iid.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-iid.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-gcm.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-gcm.aar");
			#endif
		}

		public static void DisablePushNotificationsAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-iid.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-gcm.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-iid.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-gcm.aar");
			#endif
		}

		public static void EnableGoogleAdMobAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-ads.jar.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-ads-lite.jar.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-safetynet.jar.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-safetynet.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-ads.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-ads-lite.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-safetynet.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-safetynet.aar");
			#endif
		}

		public static void DisableGoogleAdMobAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-safetynet.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-safetynet.aar");
			#endif
		}

		public static void EnableGooglePlayServicesAPI () {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-games.jar.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-games.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/play-services-games.txt", 		SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-games.aar");
			#endif
		}

		public static void DisableGooglePlayServicesAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-games.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/play-services-games.aar");
			#endif
		}
		
		public static void EnableAndroidCampainAPI() {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/sa_analytics.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/sa_analytics.jar");
		}
		
		
		public static void DisableAndroidCampainAPI() {
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/sa_analytics.jar");
		}


		public static void GAEnableFirebaseAPI(string appId) {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/sa_firebase.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/sa_firebase.jar");

			string file = SA.Common.Config.ANDROID_DESTANATION_PATH + "GA_Firebase/res/values/ids.xml";
			if (!SA.Common.Util.Files.IsFileExists (file)) {
				SA.Common.Util.Files.CreateFile (file);
			}
			SA.Common.Util.Files.Write (file, GA_FIREBASE_IDS_FILE_START + appId + GA_FIREBASE_IDS_FILE_END);

			file = "GA_Firebase/project.properties";
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/" + file, 	SA.Common.Config.ANDROID_DESTANATION_PATH + file);

			file = "GA_Firebase/AndroidManifest";
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/" + file + ".txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + file + ".xml");

#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/android-support-v4.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/android-support-v4.jar");
#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/support-v4-24.1.1.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/support-v4-24.1.1.aar");
#endif

			EnableGooglePlayAPI ();
			EnableFirebaseAnalytics ();
		}


		public static void GADisableFirebaseAPI() {
			SA.Common.Util.Files.DeleteFile (SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/sa_firebase.jar");
			SA.Common.Util.Files.DeleteFolder (SA.Common.Config.ANDROID_DESTANATION_PATH + "GA_Firebase");

#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/android-support-v4.jar");
#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/support-v4-24.1.1.aar");
#endif

			DisableGooglePlayAPI ();
			DisableFirebaseAnalytics ();
		}

		
		public static void EnableAppLicensingAPI() {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "app_licensing/an_licensing_library.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_licensing_library.jar");
		}
		
		
		public static void DisableAppLicensingAPI() {
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_licensing_library.jar");
		}
		
		
		public static void EnableSoomlaAPI() {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/an_sa_soomla.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_sa_soomla.jar");
		}
		
		
		public static void DisableSoomlaAPI() {
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_sa_soomla.jar");
		}
		
		
		
		public static void EnableBillingAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "billing/an_billing.jar.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_billing.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "billing/an_billing.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_billing.aar");
			#endif
		}
		
		public static void DisableBillingAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_billing.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_billing.aar");
			#endif
		}
		
		
		
		
		public static void EnableSocialAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "social/an_social.jar.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_social.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "social/twitter4j-core-4.0.4.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/twitter4j-core-4.0.4.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "social/an_social.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_social.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "social/twitter4j-core-4.0.4.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/twitter4j-core-4.0.4.jar");
			#endif
		}
		
		public static void DisableSocialAPI() {
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_social.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/twitter4j-core-4.0.4.jar");
			#else
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/an_social.aar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/twitter4j-core-4.0.4.jar");
			#endif
		}
		
		
		
		
		
		
		public static void EnableCameraAPI() {
			//Unity 5 upgdare:
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/image-chooser-library-1.6.0.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/image-chooser-library-1.6.0.jar");
		}
		
		public static void DisableCameraAPI() {
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/image-chooser-library-1.6.0.jar");
		}
		
		
		
		
		
		public static void Android_InstallPlugin(bool IsFirstInstall = true) {
			
			
			//Unity 5 upgdare:
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/httpclient-4.3.1.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/signpost-commonshttp4-1.2.1.2.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/signpost-core-1.2.1.2.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/libGoogleAnalyticsServices.jar");

			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/android-support-v4.jar");

			//Remove previous Image Chooser Library version
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/image-chooser-library-1.3.0.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/image-chooser-library-1.6.0.jar");

			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/twitter4j-core-3.0.5.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/google-play-services.jar");
			
			
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "social/an_social.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "social/twitter4j-core-3.0.5.jar");
			
			
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/an_googleplay.jar");
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "google_play/google-play-services.jar");
			
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.ANDROID_SOURCE_PATH + "billing/an_billing.jar");
			
			#if UNITY_4_6 || UNITY_4_7
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/android-support-v4.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/android-support-v4.jar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "androidnative.jar.txt", 	        	SA.Common.Config.ANDROID_DESTANATION_PATH + "androidnative.jar");
			#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "libs/support-v4-24.1.1.txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + "libs/support-v4-24.1.1.aar");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "androidnative.txt", 	        	SA.Common.Config.ANDROID_DESTANATION_PATH + "androidnative.aar");
			#endif

			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "sa_analytics.txt", 	        	SA.Common.Config.ANDROID_DESTANATION_PATH + "sa_analytics.jar");

	#if UNITY_4_6 || UNITY_4_7
	        SA.Common.Util.Files.CopyFile (SA.Common.Config.ANDROID_SOURCE_PATH + "mobile-native-popups.jar.txt",             SA.Common.Config.ANDROID_DESTANATION_PATH + "mobile-native-popups.jar");
	#else
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + "mobile-native-popups.txt", SA.Common.Config.ANDROID_DESTANATION_PATH + "mobile-native-popups.aar");
	#endif
	        SA.Common.Util.Files.DeleteFile (SA.Common.Config.ANDROID_SOURCE_PATH + "mobilenativepopups.txt");
			SA.Common.Util.Files.DeleteFile (SA.Common.Config.ANDROID_DESTANATION_PATH + "mobilenativepopups.jar");
			
			SA.Common.Util.Files.CopyFolder(SA.Common.Config.ANDROID_SOURCE_PATH + "facebook", 			SA.Common.Config.ANDROID_DESTANATION_PATH + "facebook");
			
	#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
			
	#else
			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.ANDROID_SOURCE_PATH + "facebook");
	#endif
			
			if(IsFirstInstall) {
				EnableBillingAPI();
				EnableGooglePlayAPI();
				EnableSocialAPI();
				EnableCameraAPI();
				EnableAppLicensingAPI();
			}
			
			
			
			
			string file;
			file = "AN_Res/res/values/analytics.xml";
			if(!SA.Common.Util.Files.IsFileExists(SA.Common.Config.ANDROID_DESTANATION_PATH + file)) {
				SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + file, 	SA.Common.Config.ANDROID_DESTANATION_PATH + file);
			}
			
			
			file = "AN_Res/res/values/ids.xml";
			if(!SA.Common.Util.Files.IsFileExists(SA.Common.Config.ANDROID_DESTANATION_PATH + file)) {
				SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + file, 	SA.Common.Config.ANDROID_DESTANATION_PATH + file);
			}
			
			file = "AN_Res/res/xml/file_paths.xml";
			if(!SA.Common.Util.Files.IsFileExists(SA.Common.Config.ANDROID_DESTANATION_PATH + file)) {
				SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + file, 	SA.Common.Config.ANDROID_DESTANATION_PATH + file);
			}
			
			
			file = "AN_Res/res/values/version.xml";
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + file, 	SA.Common.Config.ANDROID_DESTANATION_PATH + file);
			
			file = "AN_Res/project.properties";
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + file, 	SA.Common.Config.ANDROID_DESTANATION_PATH + file);
			
			file = "AN_Res/AndroidManifest";
			SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + file + ".txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + file + ".xml");
			
			//First install dependense		
			
			file = "AndroidManifest";
			if(!SA.Common.Util.Files.IsFileExists(SA.Common.Config.ANDROID_DESTANATION_PATH + file + ".xml")) {
				SA.Common.Util.Files.CopyFile(SA.Common.Config.ANDROID_SOURCE_PATH + file + ".txt", 	SA.Common.Config.ANDROID_DESTANATION_PATH + file + ".xml");
			} 
			
			AssetDatabase.Refresh();
			
		}
		
		
		
		public static bool IsFacebookInstalled {
			get {
				return SA.Common.Util.Files.IsFileExists("Facebook/Scripts/FB.cs")
                    || SA.Common.Util.Files.IsFileExists("Plugins/Facebook/Scripts/FB.cs")
                    || SA.Common.Util.Files.IsFileExists("FacebookSDK/SDK/Scripts/FB.cs")
					|| SA.Common.Util.Files.IsFileExists("FacebookSDK/Plugins/Facebook.Unity.dll")
					|| SA.Common.Util.Files.IsFileExists("Plugins/FacebookSDK/Plugins/Facebook.Unity.dll")
					|| SA.Common.Util.Files.IsFileExists("FacebookSDK/SDK/Resources/FacebookSettings.asset");
			}
		}
		
		
	}

}
#endif
