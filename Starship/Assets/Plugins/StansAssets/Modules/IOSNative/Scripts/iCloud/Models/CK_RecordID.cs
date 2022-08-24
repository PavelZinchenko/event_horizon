////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections.Generic;

public class CK_RecordID  {


	private int _internalId;
	private string _Name;
	private static Dictionary<int, CK_RecordID> _Ids =  new Dictionary<int, CK_RecordID>();


	/// <summary>
	/// Initializes and returns a new record ID with the specified name in the default zone.
	/// 
	/// Use this method when you are creating or searching for records in the default zone.
	/// </summary>
	/// <param name="recordName">The name to use to identify the record. The string must contain only ASCII characters and must not exceed 255 characters. If you specify nil or an empty string for this parameter, this method throws an exception.</param>
	public CK_RecordID(string recordName) {
		_internalId = SA.Common.Util.IdFactory.NextId;
		_Name = recordName;

		ISN_CloudKit.CreateRecordId_Object(_internalId, _Name);
		_Ids.Add(_internalId, this);

	}
		




	public string Name {
		get {
			return _Name;
		}
	}



	//--------------------------------------
	// Internal Use Only
	//--------------------------------------

	public int Internal_Id {
		get {
			return _internalId;
		}
	}

	public static CK_RecordID  GetRecordIdByInternalId(int id) {
		return _Ids[id];
	}


}

