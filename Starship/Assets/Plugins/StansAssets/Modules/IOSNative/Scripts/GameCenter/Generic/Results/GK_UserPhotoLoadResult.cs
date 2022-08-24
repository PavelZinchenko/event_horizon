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

public class GK_UserPhotoLoadResult : SA.Common.Models.Result {


	private Texture2D _Photo = null;
	private GK_PhotoSize _Size;


	public GK_UserPhotoLoadResult(GK_PhotoSize size, Texture2D photo):base() {
		_Size = size;
		_Photo = photo;
	}
	
	
	public GK_UserPhotoLoadResult(GK_PhotoSize size, SA.Common.Models.Error error):base(error) {
		_Size = size;
	}


	public Texture2D Photo {
		get {
			return _Photo;
		}
	}

	public GK_PhotoSize Size {
		get {
			return _Size;
		}
	}
}
