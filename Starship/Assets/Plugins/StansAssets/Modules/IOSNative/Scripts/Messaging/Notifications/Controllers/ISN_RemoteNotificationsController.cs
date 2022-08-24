//#define PUSH_ENABLED

using System;
using UnityEngine;
using System.Collections;


#if (UNITY_IOS && PUSH_ENABLED && !UNITY_EDITOR  && UNITY_5) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif



public class ISN_RemoteNotificationsController :  SA.Common.Pattern.Singleton<ISN_RemoteNotificationsController> {

	private static Action<ISN_RemoteNotificationsRegistrationResult> _RegistrationCallback = null;
	private ISN_RemoteNotification _LaunchNotification = null;


	#if (UNITY_IOS && PUSH_ENABLED && !UNITY_EDITOR  && UNITY_5) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_RegisterForRemoteNotifications();

	#endif


	public static event Action<ISN_RemoteNotification> OnRemoteNotificationReceived = delegate {};



	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	void Awake() {
		DontDestroyOnLoad(gameObject);

		#if (UNITY_IOS && PUSH_ENABLED && !UNITY_EDITOR && UNITY_5) || SA_DEBUG_MODE

		if (UnityEngine.iOS.NotificationServices.remoteNotificationCount > 0) {
			string alertBody = UnityEngine.iOS.NotificationServices.remoteNotifications [0].alertBody;
			ISN_RemoteNotification n = new ISN_RemoteNotification (alertBody);
			_LaunchNotification = n;

			UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
		}

		#endif
	}



	//--------------------------------------
	//  Public Methods
	//--------------------------------------


	public void RegisterForRemoteNotifications(Action<ISN_RemoteNotificationsRegistrationResult> callback = null) {
		_RegistrationCallback = callback;

		#if (UNITY_IOS && PUSH_ENABLED && !UNITY_EDITOR  && UNITY_5) || SA_DEBUG_MODE
		_ISN_RegisterForRemoteNotifications();
		#endif

	}



	//--------------------------------------
	// Get / Set
	//--------------------------------------

	public ISN_RemoteNotification LaunchNotification {
		get {
			return _LaunchNotification;
		}
	}

	//--------------------------------------
	// Hadnlers
	//--------------------------------------


	private void DidFailToRegisterForRemoteNotifications(string errorData) {

		var error = new SA.Common.Models.Error (errorData);
		var result = new ISN_RemoteNotificationsRegistrationResult(error);


		if(_RegistrationCallback != null) {
			_RegistrationCallback (result);
		}
	}

	private void DidRegisterForRemoteNotifications(string data) {

		string[] DataArray 	= data.Split(SA.Common.Data.Converter.DATA_SPLITTER);
		string deviceId 	= DataArray[0];
		string base64String = DataArray[1];

		ISN_DeviceToken token = new ISN_DeviceToken (base64String, deviceId);
		var result = new ISN_RemoteNotificationsRegistrationResult(token);

		if(_RegistrationCallback != null) {
			_RegistrationCallback (result);
		}

	}

	private void DidReceiveRemoteNotification(string notificationBody) {
		ISN_RemoteNotification notif = new ISN_RemoteNotification (notificationBody);

		OnRemoteNotificationReceived (notif);
	}
}
