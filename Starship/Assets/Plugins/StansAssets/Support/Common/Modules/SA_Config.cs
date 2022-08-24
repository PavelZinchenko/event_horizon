////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System;
using System.Reflection;
using System.Collections;


namespace SA.Common {
	
	public class Config  {

        public const string LIB_VERSION = "24";

		public const int VERSION_UNDEFINED = 0;
		public const string VERSION_UNDEFINED_STRING 	= "Undefined";
		public const string SUPPORT_EMAIL 				= "support@stansassets.com";
		public const string WEBSITE_ROOT_URL 			= "https://stansassets.com/";

        public const string BUNDLES_PATH            	 = "Plugins/StansAssets/Bundles/";
        public const string MODULS_PATH 	 			 = "Plugins/StansAssets/Modules/";
		public const string SUPPORT_MODULS_PATH 	 	= "Plugins/StansAssets/Support/";


		public const string COMMON_LIB_PATH 			= SUPPORT_MODULS_PATH 	+ "Common/";
		public const string VERSION_INFO_PATH 			= SUPPORT_MODULS_PATH 	+ "Versions/";
		public const string NATIVE_LIBRARIES_PATH 		= SUPPORT_MODULS_PATH 	+ "NativeLibraries/";
		public const string EDITOR_TESTING_LIB_PATH 	= SUPPORT_MODULS_PATH 	+ "EditorTesting/";
		public const string SETTINGS_REMOVE_PATH 		= SUPPORT_MODULS_PATH 	+ "Settings/";
		public const string SETTINGS_PATH 				= SUPPORT_MODULS_PATH 	+ "Settings/Resources/";




		public const string ANDROID_DESTANATION_PATH  = "Plugins/Android/";
		public const string ANDROID_SOURCE_PATH       = NATIVE_LIBRARIES_PATH + "Android/";


		public const string IOS_DESTANATION_PATH 	 = "Plugins/IOS/";
		public const string IOS_SOURCE_PATH       	 = NATIVE_LIBRARIES_PATH + "IOS/";







        public const string SPOTIFY_VERSION_INFO_PATH   = VERSION_INFO_PATH     + "Spotify_VersionInfo.txt";
        public const string AN_VERSION_INFO_PATH 	    = VERSION_INFO_PATH 	+ "AN_VersionInfo.txt";
		public const string UM_VERSION_INFO_PATH 	    = VERSION_INFO_PATH 	+ "UM_VersionInfo.txt";
		public const string GMA_VERSION_INFO_PATH 	    = VERSION_INFO_PATH 	+ "GMA_VersionInfo.txt";
		public const string MSP_VERSION_INFO_PATH 	    = VERSION_INFO_PATH 	+ "MSP_VersionInfo.txt";
		public const string ISN_VERSION_INFO_PATH 	    = VERSION_INFO_PATH 	+ "ISN_VersionInfo.txt";
		public const string MNP_VERSION_INFO_PATH 	    = VERSION_INFO_PATH 	+ "MNP_VersionInfo.txt";
		public const string AMN_VERSION_INFO_PATH	    = VERSION_INFO_PATH 	+ "AMN_VersionInfo.txt";



		public static string FB_SDK_VersionCode {
			get {
				string versionCode = VERSION_UNDEFINED_STRING;
				#if !(UNITY_WP8 || UNITY_WSA)
				foreach (System.Reflection.Assembly a in System.AppDomain.CurrentDomain.GetAssemblies()) {

					Type FBBuildVersionAttribute_type 	= a.GetType("Facebook.FBBuildVersionAttribute");
					Type IFacebook_type 				= a.GetType("Facebook.IFacebook");

					if(IFacebook_type != null && FBBuildVersionAttribute_type != null) {
						MethodInfo method  = FBBuildVersionAttribute_type.GetMethod("GetVersionAttributeOfType", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

						if(method != null) {
							object  MethodValue =  method.Invoke(null, new object[] { IFacebook_type } );
							PropertyInfo SdkVersionProp = FBBuildVersionAttribute_type.GetProperty("SdkVersion");
							if(MethodValue != null && SdkVersionProp != null) {
								String vc =   SdkVersionProp.GetValue(MethodValue, null)  as String;
								if(vc != null) {
									versionCode = vc;
								}
							}
						}

						break;

					}
				}

				Type FacebookSdkVersion_type 	= Type.GetType("Facebook.Unity.FacebookSdkVersion");
				if(FacebookSdkVersion_type != null) {
					System.Reflection.PropertyInfo propert  = FacebookSdkVersion_type.GetProperty("Build", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
					if(propert != null) {
						versionCode = (string) propert.GetValue(null, null);
					}
				} else {
					foreach (System.Reflection.Assembly a in System.AppDomain.CurrentDomain.GetAssemblies()) {
						Type FbType = a.GetType("Facebook.Unity.FacebookSdkVersion");
						if(FbType != null) {
							System.Reflection.PropertyInfo propert  = FbType.GetProperty("Build", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
							if(propert != null) {
								versionCode = (string) propert.GetValue(null, null);
							}
						}

					}
				}
				#endif
				return versionCode;
			}
		}

		public static int FB_SDK_MajorVersionCode {
			get {
				string versionCode = FB_SDK_VersionCode;
				int MajorVersion = VERSION_UNDEFINED;
				#if !(UNITY_WP8 || UNITY_WSA)
				if(versionCode.Equals(VERSION_UNDEFINED_STRING)) {
					return MajorVersion;
				}

				try {
					string[] SplittedVersionCode = versionCode.Split (new char[] {'.'});
					MajorVersion = System.Convert.ToInt32(SplittedVersionCode[0]);
				} catch (Exception ex) {
					Debug.LogWarning("FB_SDK_MajorVersionCode failed: " + ex.Message);
				}
				#endif
				return MajorVersion;
			}
		}


	}

}
