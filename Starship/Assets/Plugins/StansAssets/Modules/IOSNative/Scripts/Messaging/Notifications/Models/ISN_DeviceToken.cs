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

public class ISN_DeviceToken  {

	private string _tokenString;
	private byte[] _tokenBytes;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public ISN_DeviceToken(string base64String, string token)  {
		_tokenBytes 	=  System.Convert.FromBase64String(base64String);
		_tokenString	= token;
	}



	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public string DeviceId {
		get {
			return _tokenString;
		}
	}


	public byte[] Bytes {
		get {
			return _tokenBytes;
		}
	}

	public string TokenString {
		get {
			return _tokenString;
		}
	}

}
