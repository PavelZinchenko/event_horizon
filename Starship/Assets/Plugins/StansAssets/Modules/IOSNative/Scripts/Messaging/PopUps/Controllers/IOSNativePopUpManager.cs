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
using System.Collections;
#if (UNITY_IPHONE || UNITY_TVOS && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSNativePopUpManager {

	//--------------------------------------
	//  NATIVE FUNCTIONS
	//--------------------------------------
	
	#if (UNITY_IPHONE || UNITY_TVOS && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_ShowRateUsPopUp(string title, string message, string rate, string remind, string declined);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_ShowDialog(string title, string message, string yes, string no);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_ShowMessage(string title, string message, string ok);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_DismissCurrentAlert();
	#endif


	public static void dismissCurrentAlert() {
		#if (UNITY_IPHONE || UNITY_TVOS && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_DismissCurrentAlert();
		#endif
		
		
	}
	
	
	public static void showRateUsPopUp(string title, string message, string rate, string remind, string declined) {
		#if (UNITY_IPHONE || UNITY_TVOS && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ShowRateUsPopUp(title, message, rate, remind, declined);
		#endif
	}
	
	
	public static void showDialog(string title, string message) {
		showDialog(title, message, "Yes", "No");
	}
	
	public static void showDialog(string title, string message, string yes, string no) {
		#if (UNITY_IPHONE || UNITY_TVOS && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ShowDialog(title, message, yes, no);
		#endif
	}
	
	
	public static void showMessage(string title, string message) {
		showMessage(title, message, "Ok");
	}
	
	public static void showMessage(string title, string message, string ok) {
		#if ((UNITY_IPHONE || UNITY_TVOS) && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ShowMessage(title, message, ok);
		#endif
	}
}

