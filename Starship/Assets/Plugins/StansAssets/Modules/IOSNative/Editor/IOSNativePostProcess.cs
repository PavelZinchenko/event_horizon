#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;

public class IOSNativePostProcess  {

	#if UNITY_IPHONE
	[PostProcessBuild(50)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {


		if(IOSNativeSettings.Instance.EnableForceTouchAPI && IOSNativeSettings.Instance.ForceTouchMenu.Count > 0) {

			SA.IOSDeploy.Variable UIApplicationShortcutItems =  new SA.IOSDeploy.Variable();
			UIApplicationShortcutItems.Name = "UIApplicationShortcutItems";
			UIApplicationShortcutItems.Type = SA.IOSDeploy.PlistValueTypes.Array;

			foreach(var item in IOSNativeSettings.Instance.ForceTouchMenu) {
				var ShortcutItem  = new SA.IOSDeploy.Variable();
				ShortcutItem.Type = SA.IOSDeploy.PlistValueTypes.Dictionary;
				UIApplicationShortcutItems.AddChild (ShortcutItem);


				var ShortcutItemTitle =   new SA.IOSDeploy.Variable();
				ShortcutItemTitle.Name = "UIApplicationShortcutItemTitle";
				ShortcutItemTitle.StringValue = item.Title;
				ShortcutItem.AddChild (ShortcutItemTitle);

				var ShortcutItemSubtitle =   new SA.IOSDeploy.Variable();
				ShortcutItemSubtitle.Name = "UIApplicationShortcutItemSubtitle";
				ShortcutItemSubtitle.StringValue = item.Subtitle;
				ShortcutItem.AddChild (ShortcutItemSubtitle);


				var ShortcutItemType =   new SA.IOSDeploy.Variable();
				ShortcutItemType.Name = "UIApplicationShortcutItemType";
				ShortcutItemType.StringValue = item.Action;
				ShortcutItem.AddChild (ShortcutItemType);

			}


			SA.IOSDeploy.ISD_Settings.Instance.AddVariable(UIApplicationShortcutItems);


		}


		if(IOSNativeSettings.Instance.EnablePermissionAPI) {
			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.Photos);
			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.Contacts);
			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.EventKit);
		}


		if(IOSNativeSettings.Instance.EnablePushNotificationsAPI) {
			SA.IOSDeploy.Variable UIBackgroundModes =  new SA.IOSDeploy.Variable();
			UIBackgroundModes.Name = "UIBackgroundModes";
			UIBackgroundModes.Type = SA.IOSDeploy.PlistValueTypes.Array;

			SA.IOSDeploy.Variable remoteNotification =  new SA.IOSDeploy.Variable();
			remoteNotification.Name = "remote-notification";
			remoteNotification.StringValue = "remote-notification";
			remoteNotification.Type = SA.IOSDeploy.PlistValueTypes.String;

			UIBackgroundModes.AddChild (remoteNotification);
			SA.IOSDeploy.ISD_Settings.Instance.AddVariable(UIBackgroundModes);

		}




		if(IOSNativeSettings.Instance.EnableInAppsAPI) {
			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.StoreKit);
		}

		if(IOSNativeSettings.Instance.EnableGameCenterAPI) {

			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.GameKit);

			SA.IOSDeploy.Variable UIRequiredDeviceCapabilities =  new SA.IOSDeploy.Variable();
			UIRequiredDeviceCapabilities.Name = "UIRequiredDeviceCapabilities";
			UIRequiredDeviceCapabilities.Type = SA.IOSDeploy.PlistValueTypes.Array;

			SA.IOSDeploy.Variable gamekit =  new SA.IOSDeploy.Variable();
			gamekit.StringValue = "gamekit";
			gamekit.Type = SA.IOSDeploy.PlistValueTypes.String;
			UIRequiredDeviceCapabilities.AddChild(gamekit);


			SA.IOSDeploy.Variable armv7 =  new SA.IOSDeploy.Variable();
			armv7.StringValue = "armv7";
			armv7.Type = SA.IOSDeploy.PlistValueTypes.String;
			UIRequiredDeviceCapabilities.AddChild(armv7);


			SA.IOSDeploy.ISD_Settings.Instance.AddVariable(UIRequiredDeviceCapabilities);

		}

		if(IOSNativeSettings.Instance.UrlTypes.Count > 0) {
			SA.IOSDeploy.Variable CFBundleURLTypes =  new SA.IOSDeploy.Variable();
			CFBundleURLTypes.Name = "CFBundleURLTypes";
			CFBundleURLTypes.Type = SA.IOSDeploy.PlistValueTypes.Array;



			foreach(SA.IOSNative.Models.UrlType url in IOSNativeSettings.Instance.UrlTypes) {
				SA.IOSDeploy.Variable URLTypeHolder =  new SA.IOSDeploy.Variable();
				URLTypeHolder.Type = SA.IOSDeploy.PlistValueTypes.Dictionary;

				CFBundleURLTypes.AddChild (URLTypeHolder);


				SA.IOSDeploy.Variable CFBundleURLName =  new SA.IOSDeploy.Variable();
				CFBundleURLName.Type = SA.IOSDeploy.PlistValueTypes.String;
				CFBundleURLName.Name = "CFBundleURLName";
				CFBundleURLName.StringValue = url.Identifier;
				URLTypeHolder.AddChild (CFBundleURLName);


				SA.IOSDeploy.Variable CFBundleURLSchemes =  new SA.IOSDeploy.Variable();
				CFBundleURLSchemes.Type = SA.IOSDeploy.PlistValueTypes.Array;
				CFBundleURLSchemes.Name = "CFBundleURLSchemes";
				URLTypeHolder.AddChild (CFBundleURLSchemes);

				foreach(string scheme in url.Schemes) {
					SA.IOSDeploy.Variable Scheme =  new SA.IOSDeploy.Variable();
					Scheme.Type = SA.IOSDeploy.PlistValueTypes.String;
					Scheme.StringValue = scheme;

					CFBundleURLSchemes.AddChild (Scheme);
				}
			}

			foreach(SA.IOSDeploy.Variable v in  SA.IOSDeploy.ISD_Settings.Instance.PlistVariables) {
				if(v.Name.Equals(CFBundleURLTypes.Name)) {
					SA.IOSDeploy.ISD_Settings.Instance.PlistVariables.Remove (v);
					break;
				}
			}
			SA.IOSDeploy.ISD_Settings.Instance.PlistVariables.Add (CFBundleURLTypes);
		}




		if(IOSNativeSettings.Instance.EnableSocialSharingAPI) {

			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.Accounts);
			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.Social);
			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.MessageUI);
			


			string QueriesSchemesName = "LSApplicationQueriesSchemes";
			SA.IOSDeploy.Variable LSApplicationQueriesSchemes = SA.IOSDeploy.ISD_Settings.Instance.GetVariableByName (QueriesSchemesName);
			if(LSApplicationQueriesSchemes == null) {
				LSApplicationQueriesSchemes = new SA.IOSDeploy.Variable();
				LSApplicationQueriesSchemes.Name = QueriesSchemesName;
				LSApplicationQueriesSchemes.Type = SA.IOSDeploy.PlistValueTypes.Array;
			}	

			SA.IOSDeploy.Variable instagram =  new SA.IOSDeploy.Variable();
			instagram.StringValue = "instagram";
			instagram.Type = SA.IOSDeploy.PlistValueTypes.String;
			LSApplicationQueriesSchemes.AddChild(instagram);

			SA.IOSDeploy.Variable whatsapp =  new SA.IOSDeploy.Variable();
			whatsapp.StringValue = "whatsapp";
			whatsapp.Type = SA.IOSDeploy.PlistValueTypes.String;
			LSApplicationQueriesSchemes.AddChild(whatsapp);


			SA.IOSDeploy.ISD_Settings.Instance.AddVariable (LSApplicationQueriesSchemes);

		}
			

		if(IOSNativeSettings.Instance.ApplicationQueriesSchemes.Count > 0) {
			string QueriesSchemesName = "LSApplicationQueriesSchemes";
			SA.IOSDeploy.Variable LSApplicationQueriesSchemes = SA.IOSDeploy.ISD_Settings.Instance.GetVariableByName (QueriesSchemesName);
			if(LSApplicationQueriesSchemes == null) {
				LSApplicationQueriesSchemes = new SA.IOSDeploy.Variable();
				LSApplicationQueriesSchemes.Name = QueriesSchemesName;
				LSApplicationQueriesSchemes.Type = SA.IOSDeploy.PlistValueTypes.Array;
			}	


			foreach(var scheme in IOSNativeSettings.Instance.ApplicationQueriesSchemes) {
				SA.IOSDeploy.Variable schemeName =  new SA.IOSDeploy.Variable();
				schemeName.StringValue = scheme.Identifier;
				schemeName.Type = SA.IOSDeploy.PlistValueTypes.String;
				LSApplicationQueriesSchemes.AddChild(schemeName);
			}

			SA.IOSDeploy.ISD_Settings.Instance.AddVariable(LSApplicationQueriesSchemes);
		}




		if(IOSNativeSettings.Instance.EnableMediaPlayerAPI) {
			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.MediaPlayer);
				

			var NSAppleMusicUsageDescription =  new SA.IOSDeploy.Variable();
			NSAppleMusicUsageDescription.Name = "NSAppleMusicUsageDescription";
			NSAppleMusicUsageDescription.StringValue = IOSNativeSettings.Instance.AppleMusicUsageDescription;
			NSAppleMusicUsageDescription.Type = SA.IOSDeploy.PlistValueTypes.String;


			SA.IOSDeploy.ISD_Settings.Instance.AddVariable(NSAppleMusicUsageDescription);

		}
	

		if(IOSNativeSettings.Instance.EnableCameraAPI) {

			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.MobileCoreServices);


			var NSCameraUsageDescription =  new SA.IOSDeploy.Variable();
			NSCameraUsageDescription.Name = "NSCameraUsageDescription";
			NSCameraUsageDescription.StringValue = IOSNativeSettings.Instance.CameraUsageDescription;
			NSCameraUsageDescription.Type = SA.IOSDeploy.PlistValueTypes.String;


			SA.IOSDeploy.ISD_Settings.Instance.AddVariable(NSCameraUsageDescription);



			var NSPhotoLibraryUsageDescription =  new SA.IOSDeploy.Variable();
			NSPhotoLibraryUsageDescription.Name = "NSPhotoLibraryUsageDescription";
			NSPhotoLibraryUsageDescription.StringValue = IOSNativeSettings.Instance.PhotoLibraryUsageDescription;
			NSPhotoLibraryUsageDescription.Type = SA.IOSDeploy.PlistValueTypes.String;


			SA.IOSDeploy.ISD_Settings.Instance.AddVariable(NSPhotoLibraryUsageDescription);


			var NSPhotoLibraryAddUsageDescription =  new SA.IOSDeploy.Variable();
			NSPhotoLibraryAddUsageDescription.Name = "NSPhotoLibraryAddUsageDescription";
			NSPhotoLibraryAddUsageDescription.StringValue = IOSNativeSettings.Instance.PhotoLibraryUsageDescription;
			NSPhotoLibraryAddUsageDescription.Type = SA.IOSDeploy.PlistValueTypes.String;


			SA.IOSDeploy.ISD_Settings.Instance.AddVariable(NSPhotoLibraryAddUsageDescription);

		}

		if(IOSNativeSettings.Instance.EnableReplayKit) {
			var ReplayKit = SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.ReplayKit);
			ReplayKit.IsOptional = true;
		}


		if(IOSNativeSettings.Instance.EnableCloudKit) {

			var CloudKit = SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.CloudKit);
			CloudKit.IsOptional = true;

	
			string inheritedflag = "$(inherited)";
			SA.IOSDeploy.ISD_Settings.Instance.AddLinkerFlag (inheritedflag);

		}

		if(IOSNativeSettings.Instance.EnablePickerAPI) {
			var AssetsLibrary = SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.AssetsLibrary);
			AssetsLibrary.IsOptional = true;
		}


		if(IOSNativeSettings.Instance.EnableContactsAPI) {

			var Contacts = SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.Contacts);
			Contacts.IsOptional = true;

			var ContactsUI = SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.ContactsUI);
			ContactsUI.IsOptional = true;


			var NSContactsUsageDescription =  new SA.IOSDeploy.Variable();
			NSContactsUsageDescription.Name = "NSContactsUsageDescription";
			NSContactsUsageDescription.StringValue = IOSNativeSettings.Instance.ContactsUsageDescription;
			NSContactsUsageDescription.Type = SA.IOSDeploy.PlistValueTypes.String;


			SA.IOSDeploy.ISD_Settings.Instance.AddVariable(NSContactsUsageDescription);

		}

		if(IOSNativeSettings.Instance.EnableSoomla) {

			SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.AdSupport);
			SA.IOSDeploy.ISD_Settings.Instance.AddLibrary (SA.IOSDeploy.iOSLibrary.libsqlite3);

			#if UNITY_5
				string soomlaLinkerFlag = "-force_load Libraries/Plugins/iOS/libSoomlaGrowLite.a";
			#else
				string soomlaLinkerFlag = "-force_load Libraries/libSoomlaGrowLite.a";
			#endif

			SA.IOSDeploy.ISD_Settings.Instance.AddLinkerFlag (soomlaLinkerFlag);
		}

		if(IOSNativeSettings.Instance.EnableUserNotificationsAPI) {
			var AssetsLibrary = SA.IOSDeploy.ISD_Settings.Instance.AddFramework (SA.IOSDeploy.iOSFramework.UserNotifications);
			AssetsLibrary.IsOptional = false;
		}

		Debug.Log("ISN Postprocess Done");

	
	}
	#endif
}
#endif