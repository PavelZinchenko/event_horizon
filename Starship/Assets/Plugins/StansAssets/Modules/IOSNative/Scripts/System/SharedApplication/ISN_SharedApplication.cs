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
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif



namespace SA.IOSNative.System {

	public class SharedApplication  {


		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		
		[DllImport ("__Internal")]
		private static extern bool _ISN_CheckUrl(string url);

		[DllImport ("__Internal")]
		private static extern void _ISN_OpenUrl(string url);

		#endif



		//--------------------------------------
		// Public Methods
		//--------------------------------------


		public static bool CheckUrl(string url) {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			return _ISN_CheckUrl(url);
			#else
			return false;
			#endif
		}


		public static void OpenUrl(string url) {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_OpenUrl(url);
			#endif
		}
			
	}
}
