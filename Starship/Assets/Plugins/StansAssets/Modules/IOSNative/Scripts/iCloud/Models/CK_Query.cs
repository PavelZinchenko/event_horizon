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

public class CK_Query  {

	private string _Predicate;
	private string _RecordType;

	public CK_Query(string predicate, string recordType) {
		_Predicate = predicate;
		_RecordType = recordType;
	}

	public string Predicate {
		get {
			return _Predicate;
		}
	}

	public string RecordType {
		get {
			return _RecordType;
		}
	}
}

