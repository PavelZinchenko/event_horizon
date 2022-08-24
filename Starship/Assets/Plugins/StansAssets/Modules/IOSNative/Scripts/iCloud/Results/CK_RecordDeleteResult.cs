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

public class CK_RecordDeleteResult : SA.Common.Models.Result {


	private CK_RecordID _RecordID;
	private CK_Database _Database;


	public CK_RecordDeleteResult(int recordId):base() {
		_RecordID = CK_RecordID.GetRecordIdByInternalId(recordId);
	}


	public CK_RecordDeleteResult(string errorData):base(new SA.Common.Models.Error(errorData)) {

	}

	public void SetDatabase(CK_Database database) {
		_Database = database;
	}



	public CK_Database Database {
		get {
			return _Database;
		}
	}

	public CK_RecordID RecordID {
		get {
			return _RecordID;
		}
	}
}
