//#define VIDEO_API
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
#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSVideoManager : SA.Common.Pattern.Singleton<IOSVideoManager>  {

	#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
	
	[DllImport ("__Internal")]
	private static extern void _ISN_StreamVideo(string videoUrl);
	

	[DllImport ("__Internal")]
	private static extern void _ISN_OpenYouTubeVideo(string videoUrl);
	
	#endif



	public void PlayStreamingVideo(string videoUrl) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_StreamVideo(videoUrl);
		#endif
	}
	
	public void OpenYouTubeVideo(string videoUrl) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_OpenYouTubeVideo(videoUrl);
		#endif
	}
}
