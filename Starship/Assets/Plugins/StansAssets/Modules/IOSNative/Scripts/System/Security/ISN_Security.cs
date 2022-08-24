//#define INAPP_API_ENABLED
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
#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class ISN_Security : SA.Common.Pattern.Singleton<ISN_Security> { 

	#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _ISN_RetrieveLocalReceipt();
	

	[DllImport ("__Internal")]
	private static extern void _ISN_ReceiptRefreshRequest();

	

	#endif


	public static event Action<ISN_LocalReceiptResult> OnReceiptLoaded = delegate{};
	public static event Action<SA.Common.Models.Result> OnReceiptRefreshComplete = delegate{};


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	public void RetrieveLocalReceipt() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
		_ISN_RetrieveLocalReceipt();
		#endif
	}

	

	public void StartReceiptRefreshRequest() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
		_ISN_ReceiptRefreshRequest();
		#endif
	}


	

	private void Event_ReceiptLoaded(string data) {
		ISN_LocalReceiptResult result =  new ISN_LocalReceiptResult(data);
		OnReceiptLoaded(result);
	}

	private void Event_ReceiptRefreshRequestReceived(string data) {
	
		SA.Common.Models.Result result;
		if(data.Equals("1")) {
			result =  new SA.Common.Models.Result();
		} else {
			result =  new SA.Common.Models.Result(new SA.Common.Models.Error());
		}

		OnReceiptRefreshComplete(result);
	}



}
