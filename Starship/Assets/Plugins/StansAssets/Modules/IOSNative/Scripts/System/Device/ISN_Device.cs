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


public class ISN_Device  {
	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern string _ISN_RetriveDeviceData();

	#endif





	private static ISN_Device _CurrentDevice = null;

	private string _Name = "Test Name";
	private string _SystemName = "iPhone OS";
	private string _Model = "iPhone";
	private string _LocalizedModel = "iPhone";
	

	private string _SystemVersion = "9.0.0";
	private int _MajorSystemVersion = 9;

	private string _PreferredLanguage_ISO639_1 = "en-US";

	

	private ISN_InterfaceIdiom _InterfaceIdiom = ISN_InterfaceIdiom.Phone;
	private ISN_DeviceGUID _GUID =  new ISN_DeviceGUID(string.Empty);


	//--------------------------------------
	// Initialization
	//--------------------------------------


	public ISN_Device() {
		
	}
	

	public ISN_Device(string deviceData) {

		string[] dataArray 		= deviceData.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		_Name = dataArray[0];
		_SystemName = dataArray[1];
		_Model = dataArray[2];
		_LocalizedModel = dataArray[3];

		_SystemVersion = dataArray[4];
		_MajorSystemVersion = System.Convert.ToInt32(dataArray[5]);

		_InterfaceIdiom = (ISN_InterfaceIdiom) System.Convert.ToInt32(dataArray[6]);
		_GUID = new ISN_DeviceGUID(dataArray[7]);

		_PreferredLanguage_ISO639_1 = dataArray [8];

	}


	//--------------------------------------
	// Get / Set
	//--------------------------------------

	public string Name {
		get {
			return _Name;
		}
	}

	public string SystemName {
		get {
			return _SystemName;
		}
	}

	public string Model {
		get {
			return _Model;
		}
	}

	public string LocalizedModel {
		get {
			return _LocalizedModel;
		}
	}



	public string SystemVersion {
		get {
			return _SystemVersion;
		}
	}

	public int MajorSystemVersion {
		get {
			return _MajorSystemVersion;
		}
	}


	public ISN_InterfaceIdiom InterfaceIdiom {
		get {
			return _InterfaceIdiom;
		}
	}

	public ISN_DeviceGUID GUID {
		get {
			return _GUID;
		}
	}


	public string PreferredLanguageCode {
		get {
			return PreferredLanguage_ISO639_1.Substring(0, 2);
		}
	}

	public string PreferredLanguage_ISO639_1 {
		get {
			return _PreferredLanguage_ISO639_1;
		}
	}

	public string AdvertisingIdentifier {
		get {

			#if (UNITY_5 && UNITY_IPHONE)
			return UnityEngine.iOS.Device.advertisingIdentifier;
			#else
			return "00000000-0000-0000-0000-000000000000";
			#endif

		}
	}

	public bool AdvertisingTrackingEnabled {
		get {
			#if (UNITY_5 && UNITY_IPHONE)
			return UnityEngine.iOS.Device.advertisingTrackingEnabled;
			#else
			return false;
			#endif

		}
	}




	//--------------------------------------
	// Static 
	//--------------------------------------



	public static ISN_Device CurrentDevice {
		get {
			

			if(_CurrentDevice == null) {

				#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
					_CurrentDevice =  new ISN_Device(_ISN_RetriveDeviceData());
				#else
					_CurrentDevice =  new ISN_Device();
				#endif

			}

			return _CurrentDevice;
		}
	}



}
