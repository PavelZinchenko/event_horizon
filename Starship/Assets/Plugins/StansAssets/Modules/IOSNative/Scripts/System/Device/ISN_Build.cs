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
using System;


#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class ISN_Build  {


	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern string _ISN_BuildInfo();

	#endif

	private string 	_Version = "1.0";
	private int  	_Number = 1;



	private static ISN_Build _Current = null;


	//--------------------------------------
	// Initialization
	//--------------------------------------


	public ISN_Build() {

	}

	public ISN_Build(string data) {
		string[] dataArray 		= data.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		_Version = dataArray[0];

		string temp_number = dataArray[1].Trim();
		if(String.IsNullOrEmpty(temp_number))
			_Number = 1;
		else			
			_Number = System.Convert.ToInt32(temp_number);
	}


	//--------------------------------------
	// Get / Set 
	//--------------------------------------


	public string Version {
		get {
			return _Version;
		}
	}


	public int Number {
		get {
			return _Number;
		}
	}


	//--------------------------------------
	// Static 
	//--------------------------------------


	public static ISN_Build Current {
		get {
			if(_Current == null) {
				#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
				_Current =  new ISN_Build(_ISN_BuildInfo());

				#else
				_Current =  new ISN_Build();
				#endif
			} 

			return _Current;
		}
	}



}
