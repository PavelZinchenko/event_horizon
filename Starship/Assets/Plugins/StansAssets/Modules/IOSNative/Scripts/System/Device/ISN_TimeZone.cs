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


#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class ISN_TimeZone  {


	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern string _ISN_TimeZoneInfo();

	#endif

	private long 	_SecondsFromGMT = 7200;
	private string  _Name = "Europe/Kiev";

	private static ISN_TimeZone _LocalTimeZone = null;


	//--------------------------------------
	// Initialization
	//--------------------------------------


	public ISN_TimeZone() {

	}

	public ISN_TimeZone(string data) {
		string[] dataArray 		= data.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		_Name = dataArray[0];
		_SecondsFromGMT = System.Convert.ToInt64(dataArray[1]);
	}


	//--------------------------------------
	// Get / Set 
	//--------------------------------------


	public string Name {
		get {
			return _Name;
		}
	}


	public long SecondsFromGMT {
		get {
			return _SecondsFromGMT;
		}
	}


	//--------------------------------------
	// Static 
	//--------------------------------------


	public static ISN_TimeZone LocalTimeZone {
		get {
			if(_LocalTimeZone == null) {
				#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
				_LocalTimeZone =  new ISN_TimeZone(_ISN_TimeZoneInfo());

				#else
				_LocalTimeZone =  new ISN_TimeZone();
				#endif
			} 

			return _LocalTimeZone;
		}
	}



}
