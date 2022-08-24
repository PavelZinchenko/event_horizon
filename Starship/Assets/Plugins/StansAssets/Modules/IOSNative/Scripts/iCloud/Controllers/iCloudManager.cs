//#define SA_DEBUG_MODE
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
#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class iCloudManager : SA.Common.Pattern.Singleton<iCloudManager> {
	

	//Actions
	public static event Action<SA.Common.Models.Result> OnCloudInitAction = delegate {};
	public static event Action<iCloudData> OnCloudDataReceivedAction = delegate {};
	public static event Action<List<iCloudData>> OnStoreDidChangeExternally = delegate {};


    private Dictionary<string, List<Action<iCloudData>>> s_requestDataCallbacks = new Dictionary<string, List<Action<iCloudData>>>();




	#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
    private static extern void _ISN_SetString(string key, string val);

	[DllImport ("__Internal")]
    private static extern void _ISN_SetDouble(string key, float val);
	
	[DllImport ("__Internal")]
    private static extern void _ISN_SetData(string key, string val);

	[DllImport ("__Internal")]
    private static extern void _ISN_RequestDataForKey(string key);
	#endif
	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

    [System.Obsolete("use SetString instead")]
    public void setString(string key, string val) {
        SetString(key, val);
    }


	public void SetString(string key, string val) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR) || SA_DEBUG_MODE
        _ISN_SetString(key, val);
		#endif
	}


    [System.Obsolete("use SetFloat instead")]
    public void setFloat(string key, float val) {
        SetFloat(key, val);
    }

    public void SetFloat(string key, float val) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR) || SA_DEBUG_MODE
        _ISN_SetDouble(key, val);
		#endif
	}


    [System.Obsolete("use SetData instead")]
    public void setData(string key, byte[] val) {
        SetData(key, val);
    }


    public void SetData(string key, byte[] val) {

        //JsonUtility.to


		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR) || SA_DEBUG_MODE
			string bytesString = System.Convert.ToBase64String (val);
            _ISN_SetData(key, bytesString);
		#endif
	}


    public void SetObject(string key, object val) {
#if UNITY_2017
		string serializedValue = JsonUtility.ToJson(val);
        SetString(key, serializedValue);
#endif
    }


    public void SetInt(string key, int val) {
        string serializedValue = System.Convert.ToString(val);
        SetString(key, serializedValue);
    }

    public void SetLong(string key, long val) {
        string serializedValue = System.Convert.ToString(val);
        SetString(key, serializedValue);
    }

    public void SetUlong(string key, ulong val) {
        string serializedValue = System.Convert.ToString(val);
        SetString(key, serializedValue);
    }

    public void SetArray(string key, List<object> val) {
        string serializedValue = SA.Common.Data.Json.Serialize(val);
        SetString(key, serializedValue);
    }

    public void SetDictionary(string key, Dictionary<object, object> val) {
        string serializedValue = SA.Common.Data.Json.Serialize(val);
        SetString(key, serializedValue);
    }


    [System.Obsolete("use RequestDataForKey instead")]
    public void requestDataForKey(string key) {
        RequestDataForKey(key);
    }


    public void RequestDataForKey(string key) {
        RequestDataForKey(key, null);
    }

    public void RequestDataForKey(string key, Action<iCloudData> callback) {


        if(callback != null) {
            if (s_requestDataCallbacks.ContainsKey(key)) {
                s_requestDataCallbacks[key].Add(callback);
                return;
            } else {
                s_requestDataCallbacks.Add(key, new List<Action<iCloudData>>() { callback });
            }
        }
       

#if ((UNITY_IPHONE || UNITY_TVOS) && !UNITY_EDITOR) || SA_DEBUG_MODE
        _ISN_RequestDataForKey(key);
#endif
	}


	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnCloudInit() {
		SA.Common.Models.Result res =  new SA.Common.Models.Result();
		OnCloudInitAction(res);
	}

	private void OnCloudInitFail() {
		SA.Common.Models.Result res =  new SA.Common.Models.Result(new SA.Common.Models.Error());
		OnCloudInitAction(res);
	}

	private void OnCloudDataChanged(string data) {

		List<iCloudData> changedData =  new List<iCloudData>();

		string[] DataArray = data.Split(SA.Common.Data.Converter.DATA_SPLITTER); 

		for(int i = 0; i < DataArray.Length; i += 2 ) {
			if(DataArray[i] == SA.Common.Data.Converter.DATA_EOF) {
				break;
			}

			iCloudData pair =  new iCloudData(DataArray[i], DataArray[i + 1]);
			changedData.Add(pair);
		}

		OnStoreDidChangeExternally(changedData);

	}


	private void OnCloudData(string array) {
		string[] data;
		data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER); 

		iCloudData package = new iCloudData (data[0], data [1]);



        if(s_requestDataCallbacks.ContainsKey(package.Key)) {
            List<Action<iCloudData>> registredCallbacks = s_requestDataCallbacks[package.Key];
            s_requestDataCallbacks.Remove(package.Key);

            foreach (var cb in registredCallbacks) {
                cb.Invoke(package);
            }
        }

       

		OnCloudDataReceivedAction(package);
	}

	private void OnCloudDataEmpty(string array) {
		string[] data;
		data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER); 

		iCloudData package = new iCloudData (data[0], "null");


		OnCloudDataReceivedAction(package);
	}



	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
