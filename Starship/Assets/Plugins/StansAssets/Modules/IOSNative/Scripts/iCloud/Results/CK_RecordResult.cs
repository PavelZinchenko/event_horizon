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

public class CK_RecordResult : SA.Common.Models.Result {


	private CK_Record _Record;
	private CK_Database _Database;

	public CK_RecordResult(int recordId):base() {
		_Record = CK_Record.GetRecordByInternalId(recordId);

	}


	public CK_RecordResult(string errorData):base(new SA.Common.Models.Error(errorData)) {

	}


	public void SetDatabase(CK_Database database) {
		_Database = database;
	}


	public CK_Record Record {
		get {
			return _Record;
		}
	}

	public CK_Database Database {
		get {
			return _Database;
		}
	}
}
