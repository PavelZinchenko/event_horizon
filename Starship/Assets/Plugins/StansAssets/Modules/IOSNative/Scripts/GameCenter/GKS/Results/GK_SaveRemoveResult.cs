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

public class GK_SaveRemoveResult : SA.Common.Models.Result {

	private string _SaveName = string.Empty;



	public GK_SaveRemoveResult(string name):base() {
		_SaveName = name;
	}
	
	public GK_SaveRemoveResult(string name, string errorData):base(new SA.Common.Models.Error(errorData)) {
		_SaveName = name;
	}




	public string SaveName {
		get {
			return _SaveName;
		}
	}
}
