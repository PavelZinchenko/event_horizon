//#define USER_NOTIFICATIONS_API

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
#if UNITY_IPHONE && !UNITY_EDITOR && USER_NOTIFICATIONS_API
using System.Runtime.InteropServices;
#endif


namespace SA.IOSNative.UserNotifications {

	public class NativeReceiver : SA.Common.Pattern.Singleton<NativeReceiver>  {
		
		public void Init() {

		}

		void Awake() {
			DontDestroyOnLoad(gameObject);
		}

		void RequestPermissionsCallbackEvent(string data) {
			NotificationCenter.RequestPermissionsResponse (data);
		}

		void AddNotificationRequestEvent(string data) {
			NotificationCenter.AddNotificationRequestResponse (data);
		}

		void WillPresentNotification(string data) {
			NotificationCenter.WillPresentNotification (data);
		}

		void PendingNotificationsRequest(string data) {
			NotificationCenter.PendingNotificationsRequestResponse (data);
		}

		void LaunchNotification(string data) {
			NotificationCenter.SetLastNotifification (data);
		}

	}
}
