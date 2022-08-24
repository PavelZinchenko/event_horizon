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

public class CK_Record  {



	private static Dictionary<int, CK_Record> _Records =  new Dictionary<int, CK_Record>();


	private CK_RecordID _Id;
	private string _Type = string.Empty;
	private Dictionary<string, string> _Data  = new Dictionary<string, string>();



	private int _internalId;


	/// <summary>
	/// Initializes and returns a record using an id that you provide..
	/// 
	/// Use this method to initialize a new record object with the specified ID. The newly created record contains no data.
	/// </summary>
	/// <param name="id">The ID to assign to the record itself. The ID cannot currently be in use by any other record and must not be nil. </param>
	/// <param name="type">A string reflecting the type of record that you want to create. Define the record types that your app supports, and use them to distinguish between records with different types of data. </param>
	public CK_Record(CK_RecordID id, string type) {
		_Id = id;
		_Type = type;

		IndexRecord();
	}

	/// <summary>
	/// Do not use this method. It is created for plugin internal use only.
	/// </summary>
	public CK_Record(string name, string template) {


		_Id = new CK_RecordID(name);

		string[] DataArray = template.Split(SA.Common.Data.Converter.DATA_SPLITTER); 
		_Type = DataArray[0];

		for(int i = 1; i < DataArray.Length; i += 2) {
			if(DataArray[i] == SA.Common.Data.Converter.DATA_EOF) {
				break;
			}

			SetObject(DataArray[i], DataArray[i + 1]);
		}

		IndexRecord();
	}



	//--------------------------------------
	// Public Methods
	//--------------------------------------

	public void SetObject(string key, string value) {
		if(_Data.ContainsKey(key)) {
			_Data[key] = value;
		} else {
			_Data.Add(key, value);
		}
	}

	public string GetObject(string key) {
		if(_Data.ContainsKey(key)) {
			return _Data[key];
		} else {
			return string.Empty;
		}
	}


	//--------------------------------------
	// Get / Set
	//--------------------------------------


	public CK_RecordID Id {
		get {
			return _Id;
		}
	}

	public string Type {
		get {
			return _Type;
		}
	}
		
	//--------------------------------------
	// Private Methods
	//--------------------------------------

	private void IndexRecord() {
		_internalId = SA.Common.Util.IdFactory.NextId;
		_Records.Add(_internalId, this);
	}


	//--------------------------------------
	// Internal Use Only
	//--------------------------------------


	public void UpdateRecord() {


		List<string> keys =  new List<string>();
		List<string> values =  new List<string>();
		foreach(KeyValuePair<string, string> pair in _Data) {
			keys.Add(pair.Key);
			values.Add(pair.Value);
		}
		ISN_CloudKit.UpdateRecord_Object(Internal_Id, Type, SA.Common.Data.Converter.SerializeArray(keys.ToArray()), SA.Common.Data.Converter.SerializeArray(values.ToArray()), Id.Internal_Id);
	}

	public int Internal_Id {
		get {
			return _internalId;
		}
	}

	public static CK_Record GetRecordByInternalId(int id) {
		return _Records[id];
	}



}

