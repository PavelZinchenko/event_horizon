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

public class IOSImagePickResult : SA.Common.Models.Result {


	private Texture2D _image = null;

	public IOSImagePickResult(string ImageData):base() {
		if(ImageData.Length == 0) {
			_Error = new SA.Common.Models.Error (0, "No Image Data");
			return;
		}

		
		byte[] decodedFromBase64 = System.Convert.FromBase64String(ImageData);
		_image = new Texture2D(1, 1);
	//	_image = new Texture2D(1, 1, TextureFormat.DXT5, false);
		_image.LoadImage(decodedFromBase64);
		_image.hideFlags = HideFlags.DontSave;

		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			ISN_Logger.Log("IOSImagePickResult: w" + _image.width + " h: " + _image.height);
	}
	

	public Texture2D Image {
		get {
			return _image;
		}
	}
}
