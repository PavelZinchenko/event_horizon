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
using System.Collections.Generic;

public class iCloudData  {


    private string m_key;
    private string m_val;

    private bool m_IsEmpty = false;

	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public iCloudData(string k, string v) {
		m_key = k;
		m_val = v;

		if(m_val.Equals("null")) {
			if(!IOSNativeSettings.Instance.DisablePluginLogs) 
				ISN_Logger.Log ("ISN iCloud Empty set");
			m_IsEmpty = true;
		}
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------


    public T GetObject<T>() {
#if UNITY_2017
		return JsonUtility.FromJson<T>(StringValue);
#else
		return default(T);
#endif
	}

	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

    [System.Obsolete("use Key instead")]
    public string key {get { return Key; } }

	public string Key {
		get {
			return m_key;
		}
	}

    public bool IsEmpty {
        get {
            return m_IsEmpty;
        }
    }



    [System.Obsolete("use StringValue instead")]
    public string stringValue { get { return StringValue; } }

	public string StringValue {
		get {
			if(m_IsEmpty) {
				return null;
			}

			return m_val;
		}
	}

    [System.Obsolete("use FloatValue instead")]
    public float floatValue { get { return FloatValue; } }

	public float FloatValue {
		get {

			if(m_IsEmpty) {
				return 0f;
			}

			return System.Convert.ToSingle (m_val);
		}
	}

    [System.Obsolete("use BytesValue instead")]
    public byte[] bytesValue { get { return BytesValue; } }

	public byte[] BytesValue {
		get {

			if(m_IsEmpty) {
				return null;
			}

			return System.Convert.FromBase64String(m_val);
		}
	}

    public List<object> ListValue {
        get {

            if (m_IsEmpty) {
                return new List<object>();
            }

            return (List<object>) SA.Common.Data.Json.Deserialize(m_val);
        }
    }

    public Dictionary<string, object> DictionaryValue {
        get {

            if (m_IsEmpty) {
                return new Dictionary<string, object>();
            }

            return (Dictionary<string, object>)SA.Common.Data.Json.Deserialize(m_val);
        }
    }
  


    public int IntValue {
        get {
            if (m_IsEmpty) {
                return 0;
            }
            return System.Convert.ToInt32(m_val);
        }
    }

    public long LongValue {
        get {
            if (m_IsEmpty) {
                return 0;
            }
            return System.Convert.ToInt64(m_val);
        }
    }

    public ulong UlongValue {
        get {
            if (m_IsEmpty) {
                return 0;
            }
            return System.Convert.ToUInt64(m_val);
        }
    }


	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
