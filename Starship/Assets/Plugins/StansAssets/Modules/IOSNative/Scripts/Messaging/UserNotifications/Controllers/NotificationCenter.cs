//#define USER_NOTIFICATIONS_API

////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IPHONE && !UNITY_EDITOR && USER_NOTIFICATIONS_API
using System.Runtime.InteropServices;
#endif

namespace SA.IOSNative.UserNotifications {
	
	public static class NotificationCenter  {


		/// <summary>
		/// Schedules a local notification for delivery.
		/// <param name="request"> The notification request to schedule.This parameter must not be null. </param> 
		/// <param name="callback"> The callback to fired with the results. </param> 
		/// </summary>
		/// 

		private static Dictionary<string, Action<SA.Common.Models.Result>> OnCallbackDictionary;
		private static Action<List<NotificationRequest>> OnPendingNotificationsCallback;
		private static Action<SA.Common.Models.Result> RequestPermissionsCallback;


        public static event Action<NotificationRequest> OnWillPresentNotification = delegate { };


		static NotificationCenter() {
			NativeReceiver.Instance.Init ();

			OnCallbackDictionary = new Dictionary<string, Action<SA.Common.Models.Result>>();
		}

		#if UNITY_IPHONE && !UNITY_EDITOR && USER_NOTIFICATIONS_API
		[DllImport ("__Internal")]
		private static extern void _ISN_RequestUserNotificationPermissions();

		[DllImport ("__Internal")]
		private static extern void _ISN_ScheduleUserNotification(string jsonString);

		[DllImport ("__Internal")]
		private static extern void _ISN_CancelUserNotifications();

		[DllImport ("__Internal")]
		private static extern void _ISN_CancelUserNotificationById(string nId);

		[DllImport ("__Internal")]
		private static extern void _ISN_GetPendingNotifications();

		#endif

		public static NotificationRequest LastNotificationRequest;

		public static void RequestPermissions(Action<SA.Common.Models.Result> callback) {
			RequestPermissionsCallback = callback;
			#if UNITY_IPHONE && !UNITY_EDITOR && USER_NOTIFICATIONS_API
			_ISN_RequestUserNotificationPermissions();
			#endif
		}

		public static void AddNotificationRequest(NotificationRequest request, Action<SA.Common.Models.Result> callback) {
			
			string notificationID = request.Id;

			NotificationContent content = request.Content;

			OnCallbackDictionary [notificationID] = callback; // storing callback in dictionary for future call

			string jsonString = "{" + string.Format ("\"id\" : \"{0}\", \"content\" : {1}, \"trigger\" : {2}", notificationID, request.Content.ToString(), request.Trigger.ToString()) + "}";

			ScheduleUserNotification (jsonString);
		}

		private static void ScheduleUserNotification(string notificationJSONData) {
			#if UNITY_IPHONE && !UNITY_EDITOR && USER_NOTIFICATIONS_API
			_ISN_ScheduleUserNotification (notificationJSONData);
			#endif
		}

		public static void CancelAllNotifications () {
			#if UNITY_IPHONE && !UNITY_EDITOR && USER_NOTIFICATIONS_API
			_ISN_CancelUserNotifications();
			#endif
		}

		public static void CancelUserNotificationById(string nId) {
			#if UNITY_IPHONE && !UNITY_EDITOR && USER_NOTIFICATIONS_API
			_ISN_CancelUserNotificationById(nId);
			#endif
		}

		public static void GetPendingNotificationRequests(Action<List<NotificationRequest>> callback) {
			OnPendingNotificationsCallback = callback;
			#if UNITY_IPHONE && !UNITY_EDITOR && USER_NOTIFICATIONS_API
			_ISN_GetPendingNotifications ();
			#endif
		}


		public static NotificationRequest LaunchNotification {
			get {
                return SA.IOSNative.Core.AppController.LaunchNotification;
			}
		}


		internal static void RequestPermissionsResponse(string dataString) {// TODO: Reaction on Request Reponse
			SA.Common.Models.Result result;
			if (dataString.Equals ("success")) {
				result = new SA.Common.Models.Result ();
			} else {				
				result = new SA.Common.Models.Result (new SA.Common.Models.Error ());
					
			}
			RequestPermissionsCallback (result);
		}

		internal static void AddNotificationRequestResponse(string dataString) {
			
			string[] DataArray = dataString.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);
			string idKey = DataArray [0];
			string statusString = DataArray [1];

			SA.Common.Models.Result result;
			if (statusString.Equals ("success")) {
				result = new SA.Common.Models.Result ();
			} else {
				result = new SA.Common.Models.Result (new SA.Common.Models.Error(statusString));
			}
			Action<SA.Common.Models.Result> callback = OnCallbackDictionary [idKey];

			if (callback != null) {
				callback (result);
			}
		}

		internal static void WillPresentNotification(string data) {
			NotificationRequest request = new NotificationRequest(data);
            OnWillPresentNotification(request);
		}

		internal static void PendingNotificationsRequestResponse(string data) {
			if (data.Length > 0) {
				string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);

				List<NotificationRequest> requests = new List<NotificationRequest>();

				for (int i = 0; i < DataArray.Length; i++) {
					if (DataArray[i] == SA.Common.Data.Converter.DATA_EOF) {
						break;
					}

					NotificationRequest request = new NotificationRequest (data);

					requests.Add (request);
				}

				OnPendingNotificationsCallback (requests);
			}

		}

		internal static void SetLastNotifification(string data) {
			NotificationRequest request = new NotificationRequest(data);
			LastNotificationRequest = request;
		}

	}
}
