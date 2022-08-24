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

public class CK_QueryResult : SA.Common.Models.Result {


	//private List
	private CK_Database _Database;
	private List<CK_Record> _Records =  new List<CK_Record>();


	public CK_QueryResult(List<CK_Record> records):base() {
		_Records = records;
	}


	public CK_QueryResult(string errorData):base(new SA.Common.Models.Error(errorData)) {

	}

	public void SetDatabase(CK_Database database) {
		_Database = database;
	}



	public CK_Database Database {
		get {
			return _Database;
		}
	}

	public List<CK_Record> Records {
		get {
			return _Records;
		}
	}
}
