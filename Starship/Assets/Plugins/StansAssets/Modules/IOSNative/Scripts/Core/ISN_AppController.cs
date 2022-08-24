//#define APP_CONTROLLER_ENABLED
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR)
using System.Runtime.InteropServices;
#endif


namespace SA.IOSNative.Core {

	public class AppController : SA.Common.Pattern.Singleton<AppController> {

		#if (UNITY_IPHONE && !UNITY_EDITOR && APP_CONTROLLER_ENABLED) 

		[DllImport ("__Internal")]
		private static extern void _ISN_AppController_Subscribe();


		[DllImport ("__Internal")]
		private static extern string _ISN_GetLunchURLData();


		[DllImport ("__Internal")]
		private static extern string _ISN_GetLunchUniversalLink();


        [DllImport ("__Internal")]
        private static extern string _ISN_GetLunchUserNotification();
	
		#endif


		public static event Action OnApplicationDidEnterBackground	 		= delegate {};
		public static event Action OnApplicationDidBecomeActive 			= delegate {};
		public static event Action OnApplicationDidReceiveMemoryWarning 	= delegate {};
		public static event Action OnApplicationWillResignActive 			= delegate {};
		public static event Action OnApplicationWillTerminate 				= delegate {};

		public static event Action<Models.LaunchUrl> OnOpenURL 						= delegate {};
		public static event Action<Models.UniversalLink> OnContinueUserActivity 	= delegate {};

		void Awake() {
			DontDestroyOnLoad(gameObject);
			#if (UNITY_IPHONE && !UNITY_EDITOR && APP_CONTROLLER_ENABLED) 
			_ISN_AppController_Subscribe();
			#endif
		}


		//--------------------------------------
		//  Public Methods
		//--------------------------------------

		public static void Subscribe() {
			AppController.Instance.enabled = true;
		}

		//--------------------------------------
		//  Get / Set
		//--------------------------------------

		public static Models.LaunchUrl  LaunchUrl {
			get {
				#if (UNITY_IPHONE && !UNITY_EDITOR && APP_CONTROLLER_ENABLED) 
				string data = _ISN_GetLunchURLData ();
				var url =  new Models.LaunchUrl(data);
				return url;
				#else
				return new Models.LaunchUrl(string.Empty, string.Empty);
				#endif
			}
		}


		public static Models.UniversalLink  LaunchUniversalLink {
			get {
				#if (UNITY_IPHONE && !UNITY_EDITOR && APP_CONTROLLER_ENABLED) 
				string data = _ISN_GetLunchUniversalLink ();
				var url =  new Models.UniversalLink(data);
				return url;
				#else
				return new Models.UniversalLink(string.Empty);
				#endif
			}
		}

		//--------------------------------------
		//  Launch User Notification Property
		//--------------------------------------



        public static UserNotifications.NotificationRequest LaunchNotification {
            get {
                #if (UNITY_IPHONE && !UNITY_EDITOR && APP_CONTROLLER_ENABLED)
                string data = _ISN_GetLunchUserNotification();
                if(!string.IsNullOrEmpty(data)) {
                    SA.IOSNative.UserNotifications.NotificationRequest request = new SA.IOSNative.UserNotifications.NotificationRequest(data);
                    return request;
                } else {
                    return new SA.IOSNative.UserNotifications.NotificationRequest();
                }
                #else
                return new SA.IOSNative.UserNotifications.NotificationRequest();
                #endif
            }
        }



		//--------------------------------------
		//  Native Events
		//--------------------------------------

		private void openURL(string data) {
			Models.LaunchUrl url = new SA.IOSNative.Models.LaunchUrl (data);
			OnOpenURL (url);
		}


		private void continueUserActivity(string absoluteUrl) {
			Models.UniversalLink url = new SA.IOSNative.Models.UniversalLink (absoluteUrl);
			OnContinueUserActivity (url);
		}


		private void applicationDidEnterBackground() {
			OnApplicationDidEnterBackground();
		}
		
		private void applicationDidBecomeActive() {
			OnApplicationDidBecomeActive();
		}
		
		private void applicationDidReceiveMemoryWarning() {
			OnApplicationDidReceiveMemoryWarning();
		}
		
		
		private void applicationWillResignActive() {
			OnApplicationWillResignActive();
		}
		
		
		private void applicationWillTerminate() {
			OnApplicationWillTerminate();
		}


		protected override void OnApplicationQuit ()  {
			base.OnApplicationQuit ();
			AppController.OnApplicationWillTerminate ();
		}


	}

}
