////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////



using System;
using System.Collections.Generic;
using UnityEngine;

public class GK_Player {

	private string _PlayerId;
	private string _DisplayName;
	private string _Alias;


	private Texture2D _SmallPhoto = null;
	private Texture2D _BigPhoto = null;


	public event Action<GK_UserPhotoLoadResult> OnPlayerPhotoLoaded =  delegate {};


	private static Dictionary<string, Texture2D> LocalPhotosCache =  new Dictionary<string, Texture2D>();


	//--------------------------------------
	// Initialization
	//--------------------------------------

	public GK_Player (string pId, string pName, string pAlias) {
		_PlayerId = pId;
		_DisplayName = pName;
		_Alias = pAlias;


		_SmallPhoto = GetLocalCachedPhotoByKey(SmallPhotoCacheKey);
		_BigPhoto 	= GetLocalCachedPhotoByKey(BigPhotoCacheKey);

		if(IOSNativeSettings.Instance.AutoLoadUsersBigImages) {
			LoadPhoto(GK_PhotoSize.GKPhotoSizeNormal);
		}

		if(IOSNativeSettings.Instance.AutoLoadUsersSmallImages) {
			LoadPhoto(GK_PhotoSize.GKPhotoSizeSmall);
		}

	}


	//--------------------------------------
	// Public Methods
	//--------------------------------------

	public void LoadPhoto(GK_PhotoSize size) {
		if(size == GK_PhotoSize.GKPhotoSizeSmall) {
			if(_SmallPhoto != null) {
				GK_UserPhotoLoadResult result = new GK_UserPhotoLoadResult(size, _SmallPhoto);
				OnPlayerPhotoLoaded(result);
				return;
			}
		} else {
			if(_BigPhoto != null) {
				GK_UserPhotoLoadResult result = new GK_UserPhotoLoadResult(size, _BigPhoto);
				OnPlayerPhotoLoaded(result);
				return;
			}
		}
		GameCenterManager.LoadGKPlayerPhoto(Id, size);
	}


	//--------------------------------------
	// Do not use this methods, plugin internal use only
	//--------------------------------------


	public void SetPhotoData(GK_PhotoSize size, string base64String) {

		if(base64String.Length == 0) {
			return;
		}

		byte[] decodedFromBase64 = System.Convert.FromBase64String(base64String);

		Texture2D loadedPhoto = new Texture2D(1, 1);
		loadedPhoto.LoadImage(decodedFromBase64);

		if(size == GK_PhotoSize.GKPhotoSizeSmall) {
			_SmallPhoto = loadedPhoto;
			UpdatePhotosCache(SmallPhotoCacheKey, _SmallPhoto);
		} else {
			_BigPhoto = loadedPhoto;
			UpdatePhotosCache(BigPhotoCacheKey, _BigPhoto);
		}

		GK_UserPhotoLoadResult result = new GK_UserPhotoLoadResult(size, loadedPhoto);
		OnPlayerPhotoLoaded(result);
	}

	public void SetPhotoLoadFailedEventData(GK_PhotoSize size, string errorData) {
		GK_UserPhotoLoadResult result = new GK_UserPhotoLoadResult(size, new SA.Common.Models.Error(errorData));
		OnPlayerPhotoLoaded(result);
	}


	//--------------------------------------
	// Get / Set
	//--------------------------------------

	public string Id {
		get {
			return _PlayerId;
		}
	}

	public string Alias {
		get {
			return _Alias;
		}
	}


	public string DisplayName {
		get {
			return _DisplayName;
		}
	}




	public Texture2D SmallPhoto {
		get {
			return _SmallPhoto;
		}
	}

	public Texture2D BigPhoto {
		get {
			return _BigPhoto;
		}
	}


	private string SmallPhotoCacheKey {
		get {
			return Id + GK_PhotoSize.GKPhotoSizeSmall.ToString();
		}
	}


	//--------------------------------------
	// Private Section
	//--------------------------------------

	private string BigPhotoCacheKey {
		get {
			return Id + GK_PhotoSize.GKPhotoSizeNormal.ToString();
		}
	}


	public static void UpdatePhotosCache(string key, Texture2D photo) {
		if(LocalPhotosCache.ContainsKey(key)) {
			LocalPhotosCache[key] = photo;
		} else {
			LocalPhotosCache.Add(key, photo);
		}
	}

	public static Texture2D GetLocalCachedPhotoByKey(string key) {
		if(LocalPhotosCache.ContainsKey(key)) {
			return LocalPhotosCache[key];
		} else {
			return null;
		}
	}
}


