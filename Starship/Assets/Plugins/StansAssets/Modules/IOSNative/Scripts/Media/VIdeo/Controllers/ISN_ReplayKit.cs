//#define REPLAY_KIT
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
#if (UNITY_IPHONE && !UNITY_EDITOR && REPLAY_KIT) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class ISN_ReplayKit : SA.Common.Pattern.Singleton<ISN_ReplayKit> {

	#if (UNITY_IPHONE && !UNITY_EDITOR && REPLAY_KIT) || SA_DEBUG_MODE
	
	[DllImport ("__Internal")]
	private static extern void _ISN_StartRecording(bool microphoneEnabled);
	
	
	[DllImport ("__Internal")]
	private static extern void _ISN_StopRecording();

	[DllImport ("__Internal")]
	private static extern void _ISN_DiscardRecording ();


	[DllImport ("__Internal")]
	private static extern void _ISN_ShowVideoShareDialog(int ipadViewMode);

	[DllImport ("__Internal")]
	private static extern bool ISN_IsReplayKitAvaliable();


	[DllImport ("__Internal")]
	private static extern bool ISN_IsReplayKitRecording();

	
	[DllImport ("__Internal")]
	private static extern bool ISN_IsReplayKitMicEnabled();


	

	
	#endif


	public static event Action<SA.Common.Models.Result> ActionRecordStarted =  delegate {};
	public static event Action<SA.Common.Models.Result> ActionRecordStoped =  delegate {};


	public static event Action<ReplayKitVideoShareResult> ActionShareDialogFinished =  delegate {};

	public static event Action<SA.Common.Models.Error> ActionRecordInterrupted =  delegate {};
	public static event Action<bool> ActionRecorderDidChangeAvailability =  delegate {};
	


	public static event Action ActionRecordDiscard =  delegate {};


	private bool _IsRecodingAvailableToShare = false;

	//--------------------------------------
	// Public Methods
	//--------------------------------------
	void Awake(){
		DontDestroyOnLoad (gameObject);
	}

	public void StartRecording(bool microphoneEnabled = true) {
		_IsRecodingAvailableToShare = false;
		#if (UNITY_IPHONE && !UNITY_EDITOR && REPLAY_KIT) || SA_DEBUG_MODE
		_ISN_StartRecording(microphoneEnabled);
		#endif
	}
	
	public void StopRecording() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && REPLAY_KIT) || SA_DEBUG_MODE
		_ISN_StopRecording();
		#endif
	}

	public void DiscardRecording() {
		_IsRecodingAvailableToShare = false;
		#if (UNITY_IPHONE && !UNITY_EDITOR && REPLAY_KIT) || SA_DEBUG_MODE
		_ISN_DiscardRecording();
		#endif
	}



	public void ShowVideoShareDialog() {
		_IsRecodingAvailableToShare = false;
		#if (UNITY_IPHONE && !UNITY_EDITOR && REPLAY_KIT) || SA_DEBUG_MODE
		_ISN_ShowVideoShareDialog(IOSNativeSettings.Instance.RPK_iPadViewType);
		#endif

	}


	//--------------------------------------
	// Get / Set
	//--------------------------------------

	public bool IsRecording {
		get {
			#if (UNITY_IPHONE && !UNITY_EDITOR && REPLAY_KIT) || SA_DEBUG_MODE
			return ISN_IsReplayKitRecording();
			#else
			return false;
			#endif
		}
	}

	public bool IsRecodingAvailableToShare {
		get {
			return _IsRecodingAvailableToShare;
		}
	}

	public bool IsAvailable {
		get {
			#if (UNITY_IPHONE && !UNITY_EDITOR && REPLAY_KIT) || SA_DEBUG_MODE
			return ISN_IsReplayKitAvaliable();
			#else
			return false;
			#endif


		}
	}

	public bool IsMicEnabled {
		get {
			#if (UNITY_IPHONE && !UNITY_EDITOR && REPLAY_KIT) || SA_DEBUG_MODE
			return ISN_IsReplayKitMicEnabled();
			#else
			return false;
			#endif
			
			
		}
	}
	

	//--------------------------------------
	// Objective-C Delegates
	//--------------------------------------

	private void OnRecorStartSuccess(string data) {

		SA.Common.Models.Result result =  new SA.Common.Models.Result();
		ActionRecordStarted(result);
	}

	private void OnRecorStartFailed(string errorData) {
		SA.Common.Models.Result result =  new SA.Common.Models.Result(new SA.Common.Models.Error(errorData));
		ActionRecordStarted(result);
	}


	private void OnRecorStopFailed(string errorData) {
		SA.Common.Models.Result result =  new SA.Common.Models.Result(new SA.Common.Models.Error(errorData));
		ActionRecordStoped(result);
	}

	private void OnRecorStopSuccess() {
		_IsRecodingAvailableToShare = true;
		SA.Common.Models.Result result =  new SA.Common.Models.Result();
		ActionRecordStoped(result);
	}


	private void OnRecordInterrupted(string errorData) {
		_IsRecodingAvailableToShare = false;
		SA.Common.Models.Error e =  new SA.Common.Models.Error(errorData);
		ActionRecordInterrupted(e);
	}

	private void OnRecorderDidChangeAvailability(string data) {
		ActionRecorderDidChangeAvailability(IsAvailable);
	}


	private void OnSaveResult(string sourcesData) {
		string[] sources = SA.Common.Data.Converter.ParseArray(sourcesData);

		ReplayKitVideoShareResult result = new ReplayKitVideoShareResult(sources);
		ActionShareDialogFinished(result);
	}

	public void OnRecordDiscard(string data) {
		_IsRecodingAvailableToShare = false;
		ActionRecordDiscard();
	}



}
