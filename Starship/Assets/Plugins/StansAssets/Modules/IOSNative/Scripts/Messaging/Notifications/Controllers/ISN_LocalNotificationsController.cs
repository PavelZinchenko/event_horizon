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


#if (UNITY_IOS && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif



public class ISN_LocalNotificationsController : SA.Common.Pattern.Singleton<ISN_LocalNotificationsController> {

	private const string PP_KEY = "IOSNotificationControllerKey";
	private const string PP_ID_KEY = "IOSNotificationControllerrKey_ID";



	private ISN_LocalNotification _LaunchNotification = null;
	

	//Actions
	public static event Action<SA.Common.Models.Result> 	OnNotificationScheduleResult 		= delegate {};
	public static event Action<ISN_LocalNotification>  		OnLocalNotificationReceived 		= delegate {};

	

	#if (UNITY_IOS && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_ScheduleNotification (int time, string message, bool sound, string nId, int badges, string data, string soundName);

	[DllImport ("__Internal")]
	private static extern void _ISN_CancelNotifications();


	[DllImport ("__Internal")]
	private static extern void _ISN_RequestNotificationPermissions();

	[DllImport ("__Internal")]
	private static extern void _ISN_CancelNotificationById(string nId);

	[DllImport ("__Internal")]
	private static extern  void _ISN_ApplicationIconBadgeNumber (int badges);

	[DllImport ("__Internal")]
	private static extern int _ISN_CurrentNotificationSettings();

	#endif

	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	

	void Awake() {

		DontDestroyOnLoad(gameObject);

		#if UNITY_IOS && UNITY_5

			if( UnityEngine.iOS.NotificationServices.localNotificationCount > 0) {
				UnityEngine.iOS.LocalNotification n = UnityEngine.iOS.NotificationServices.localNotifications[0];
				
				ISN_LocalNotification notif = new ISN_LocalNotification(DateTime.Now, n.alertBody, true);
				
				int id = 0;
				if(n.userInfo.Contains("AlarmKey")) {
					id = System.Convert.ToInt32(n.userInfo["AlarmKey"]);
				}
				
				if(n.userInfo.Contains("data")) {
					notif.SetData(System.Convert.ToString(n.userInfo["data"]));
				}
				notif.SetId(id);
				_LaunchNotification = notif;
			}
		#endif
	}






	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------


	public void RequestNotificationPermissions() {
			if(ISN_Device.CurrentDevice.MajorSystemVersion < 8) {
				return;
			}


			#if (UNITY_IOS && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_RequestNotificationPermissions ();
			#endif

	}
	

	public void ShowGmaeKitNotification (string title, string message) {
		GameCenterManager.ShowGmaeKitNotification(title, message);
	}


	public void CancelAllLocalNotifications () {
			SaveNotifications(new List<ISN_LocalNotification>());

			#if (UNITY_IOS && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_CancelNotifications();
		#endif
	}



	public void CancelLocalNotification (ISN_LocalNotification notification) {
		CancelLocalNotificationById(notification.Id);
	}


	public void CancelLocalNotificationById (int notificationId) {

		#if (UNITY_IOS && !UNITY_EDITOR) || SA_DEBUG_MODE

			List<ISN_LocalNotification> scheduled = LoadPendingNotifications();
			List<ISN_LocalNotification> newList =  new List<ISN_LocalNotification>();
			
			foreach(ISN_LocalNotification n in scheduled) {
				if(n.Id != notificationId) {
					newList.Add(n);
				}
			}

			SaveNotifications(newList);
				


		
			_ISN_CancelNotificationById(notificationId.ToString());
		#endif
	}


	public void ScheduleNotification (ISN_LocalNotification notification) {

		#if (UNITY_IOS && !UNITY_EDITOR) || SA_DEBUG_MODE

			int time =  System.Convert.ToInt32((notification.Date -DateTime.Now).TotalSeconds); 

			List<ISN_LocalNotification> scheduled = LoadPendingNotifications();
			scheduled.Add(notification);
			SaveNotifications(scheduled);



			_ISN_ScheduleNotification (time, notification.Message, notification.UseSound, notification.Id.ToString(), notification.Badges, notification.Data, notification.SoundName);
		#endif
	}


	public  List<ISN_LocalNotification> LoadPendingNotifications(bool includeAll = false) {
		#if UNITY_IOS

		string data = string.Empty;
		if(PlayerPrefs.HasKey(PP_KEY)) {
			data = PlayerPrefs.GetString(PP_KEY);
		}

		List<ISN_LocalNotification>  tpls = new List<ISN_LocalNotification>();
		
		if(data != string.Empty) {
			string[] notifications = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);

			foreach(string n in notifications) {
				
				String templateData = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(n) );
				
				try {
					ISN_LocalNotification notification = new ISN_LocalNotification(templateData);
					
					if(!notification.IsFired || includeAll) {
						tpls.Add(notification);
					}
				} catch(Exception e) {
					ISN_Logger.Log("IOS Native. IOSNotificationController loading notification data failed: " + e.Message);
				}
				
			}
		}
		return tpls;
		#else
		return null;
		#endif

	}


	public void ApplicationIconBadgeNumber (int badges) {
		#if (UNITY_IOS && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ApplicationIconBadgeNumber (badges);
		#endif

	}

	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------


	public static int AllowedNotificationsType {
		get {
			#if (UNITY_IOS && !UNITY_EDITOR) || SA_DEBUG_MODE
			return _ISN_CurrentNotificationSettings ();
			#else
			return 0;
			#endif
		}
	}

	public ISN_LocalNotification LaunchNotification {
		get {
			return _LaunchNotification;
		}
	}
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------


	private void OnNotificationScheduleResultAction (string array) {

		string[] data;
		data = array.Split("|" [0]);


		SA.Common.Models.Result result = null;

		if(data[0].Equals("0")) {
			result =  new SA.Common.Models.Result(new SA.Common.Models.Error());
		} else {
			result =  new SA.Common.Models.Result();
		}
			

		OnNotificationScheduleResult(result);

	
	}
		

	private void OnLocalNotificationReceived_Event(string array) {
		string[] data;
		data = array.Split("|" [0]);

		string msg = data[0];
		int Id = System.Convert.ToInt32(data[1]);
		string notifDta = data[2];
		int badges = System.Convert.ToInt32(data[3]);

		ISN_LocalNotification n =  new ISN_LocalNotification(DateTime.Now, msg);
		n.SetData(notifDta);
		n.SetBadgesNumber(badges);
		n.SetId(Id);

		OnLocalNotificationReceived(n);
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------


	private void SaveNotifications(List<ISN_LocalNotification> notifications) {
		
		if(notifications.Count == 0) {
			PlayerPrefs.DeleteKey(PP_KEY);
			return;
		}
		
		string srialzedNotifications = "";
		int len = notifications.Count;
		for(int i = 0; i < len; i++) {
			if(i != 0) {
				srialzedNotifications += SA.Common.Data.Converter.DATA_SPLITTER;
			}
			
			srialzedNotifications += notifications[i].SerializedString;
		}
		
		PlayerPrefs.SetString(PP_KEY, srialzedNotifications);
	}
}
