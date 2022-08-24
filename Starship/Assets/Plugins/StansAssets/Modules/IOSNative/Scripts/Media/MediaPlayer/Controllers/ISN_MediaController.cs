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
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class ISN_MediaController : SA.Common.Pattern.Singleton<ISN_MediaController> {


	#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE

	
	[DllImport ("__Internal")]
	private static extern  void _ISN_InitMediaController();


	[DllImport ("__Internal")]
	private static extern void _ISN_SetRepeatMode(int mode);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_SetShuffleMode(int mode);


	[DllImport ("__Internal")]
	private static extern void _ISN_Play();
	
	[DllImport ("__Internal")]
	private static extern void _ISN_Pause();

	[DllImport ("__Internal")]
	private static extern void _ISN_SkipToNextItem();
	
	[DllImport ("__Internal")]
	private static extern void _ISN_SkipToBeginning();
	
	[DllImport ("__Internal")]
	private static extern void _ISN_SkipToPreviousItem();
	
	[DllImport ("__Internal")]
	private static extern void _ISN_ShowMediaPicker();

	[DllImport ("__Internal")]
	private static extern void _ISN_SetCollection(string itemsIds);
	
	[DllImport ("__Internal")]
	private static extern void ISN_MP_AddItemWithProductID(string productID);
	
	#endif


	private MP_MediaItem _NowPlayingItem = null;
	private MP_MusicPlaybackState _State = MP_MusicPlaybackState.Stopped;

	private List<MP_MediaItem> _CurrentQueue = new List<MP_MediaItem>();


	public static event Action<MP_MediaPickerResult> ActionMediaPickerResult = delegate {};
	public static event Action<MP_MediaPickerResult> ActionQueueUpdated = delegate {};
	public static event Action<MP_MediaItem> ActionNowPlayingItemChanged = delegate {};
	public static event Action<MP_MusicPlaybackState> ActionPlaybackStateChanged = delegate {};


	//--------------------------------------
	// Initialize
	//--------------------------------------
	
	void Awake() {

		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_InitMediaController();
		#endif

		DontDestroyOnLoad(gameObject);
	}


	//--------------------------------------
	// Public Methods
	//--------------------------------------

	public void SetRepeatMode(MP_MusicRepeatMode mode) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_SetRepeatMode((int) mode);

		#endif
	}

	public void SetShuffleMode(MP_MusicShuffleMode mode) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_SetShuffleMode((int) mode);
		#endif
	}

	public void Play() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_Play();
		#endif
	}

	public void Pause() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_Pause();
		#endif
	}


	public void SkipToNextItem() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_SkipToNextItem();
		#endif
	}

	public void SkipToBeginning() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_SkipToBeginning();
		#endif
	}

	public void SkipToPreviousItem() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_SkipToPreviousItem();
		#endif
	}

	public void ShowMediaPicker() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_ShowMediaPicker();
		#endif
	}



	public void SetCollection(params MP_MediaItem[] items) {
		List<string> ids =  new List<string>();

		foreach(MP_MediaItem item in items) {
			ids.Add(item.Id);
		}

		SetCollection(ids.ToArray());
	}


	public void AddItemWithProductID(string productID) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		ISN_MP_AddItemWithProductID(productID);
		#endif
	}

	public void SetCollection(params string[] itemIds) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && VIDEO_API) || SA_DEBUG_MODE
		_ISN_SetCollection(SA.Common.Data.Converter.SerializeArray(itemIds));
		#endif
	}

	//--------------------------------------
	// Get / Set
	//--------------------------------------

	public MP_MediaItem NowPlayingItem {
		get {
			return _NowPlayingItem;
		}
	}

	public List<MP_MediaItem> CurrentQueue {
		get {
			return _CurrentQueue;
		}
	}

	public MP_MusicPlaybackState State {
		get {
			return _State;
		}
	}

	//--------------------------------------
	//  Private Methods
	//--------------------------------------


	private List<MP_MediaItem> ParseMediaItemsList(string[] data, int index = 0 ) {
		List<MP_MediaItem> items =  new List<MP_MediaItem>();

		for(int i = index; i < data.Length; i += 8) {
			if(data[i] == SA.Common.Data.Converter.DATA_EOF) {
				break;
			}
			
			MP_MediaItem item = ParseMediaItemData(data, i);
			items.Add(item);
		}

		return items;
	}

	private MP_MediaItem ParseMediaItemData(string[] data, int index ) {
		return new MP_MediaItem(data[index], data[index + 1], data[index + 2], data[index + 3], data[index + 4], data[index + 5], data[index + 6], data[index + 7]);
	}



	//--------------------------------------
	//  Events
	//--------------------------------------

	private void OnQueueUpdate(string data) {
		string[] DataArray = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);
		
		_CurrentQueue =  ParseMediaItemsList(DataArray);
		MP_MediaPickerResult result =  new MP_MediaPickerResult(_CurrentQueue);

		ActionQueueUpdated(result);
	}

	private void OnQueueUpdateFailed(string errorData)  {
		MP_MediaPickerResult result =  new MP_MediaPickerResult(errorData);
		ActionQueueUpdated(result);
	}

	
	private void OnMediaPickerResult(string data) {
		string[] DataArray = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		_CurrentQueue =  ParseMediaItemsList(DataArray);

		MP_MediaPickerResult result =  new MP_MediaPickerResult(_CurrentQueue);
		ActionMediaPickerResult(result);

		ActionQueueUpdated(result);
	}

	private void OnMediaPickerFailed(string errorData) {
		MP_MediaPickerResult result =  new MP_MediaPickerResult(errorData);
		ActionMediaPickerResult(result);
	}

	private void OnNowPlayingItemchanged(string data) {
		string[] DataArray = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		_NowPlayingItem = ParseMediaItemData(DataArray, 0);
		ActionNowPlayingItemChanged(_NowPlayingItem);
	}

	private void OnPlaybackStateChanged(string state) {
		int newState = System.Convert.ToInt32(state);
		_State = (MP_MusicPlaybackState) newState;
		ActionPlaybackStateChanged(_State);
	}
}
