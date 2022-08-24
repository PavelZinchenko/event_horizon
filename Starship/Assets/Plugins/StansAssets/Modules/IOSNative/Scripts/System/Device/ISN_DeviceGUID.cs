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

public class ISN_DeviceGUID  {

	private byte[] _Bytes = null;
	private string _Base64 = null;

	public ISN_DeviceGUID(string data) {
		_Base64 = data;
		_Bytes = System.Convert.FromBase64String(data);
	}


	public string Base64String {
		get {
			return _Base64;
		}

	}

	public byte[] Bytes {
		get {
			return _Bytes;
		}
	}
}
