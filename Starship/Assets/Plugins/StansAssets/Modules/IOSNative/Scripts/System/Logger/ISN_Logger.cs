//#define SA_DEBUG_MODE
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
#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class ISN_Logger : SA.Common.Pattern.Singleton<ISN_Logger> {

	//--------------------------------------
	//  Initialize
	//--------------------------------------

	#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_NativeLog(string msg);

	[DllImport ("__Internal")]
	private static extern void _ISN_SetLogState(bool isEnabled);
	#endif

	void Awake() {
		DontDestroyOnLoad (gameObject);

		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_SetLogState(!IOSNativeSettings.Instance.DisablePluginLogs);
		#endif
	}

	public void Create() {	}

	//--------------------------------------
	//  Public methods
	//--------------------------------------

	public static void Log(object message, LogType logType = LogType.Log) {
		Instance.Create ();

		if (message == null || IOSNativeSettings.Instance.DisablePluginLogs) {			
			return;
		}

		if (Application.isEditor) {
			ISNEditorLog (logType, message);
		} else {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR) || SA_DEBUG_MODE
			string msg = message.ToString ();
			_ISN_NativeLog(msg);
			#endif
		}
	}

	//--------------------------------------
	//  Private methods
	//--------------------------------------

	private static void ISNEditorLog(LogType logType, object message) {
		switch(logType) {
		case LogType.Error:
			Debug.LogError (message);

			break;
		case LogType.Exception:
			Debug.LogException ((Exception) message);

			break;
		case LogType.Warning:
			Debug.LogWarning (message);

			break;
		default://simple log
			Debug.Log(message);

			break;
		}
	}

	//--------------------------------------
	//  Get / Set
	//--------------------------------------
}
