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

public class GK_UserInfoLoadResult : SA.Common.Models.Result {

	private string _playerId;
	private GK_Player _tpl = null;
	
	

	public GK_UserInfoLoadResult(GK_Player tpl):base() {
		_tpl = tpl;
	}
	
	public GK_UserInfoLoadResult(string id):base(new SA.Common.Models.Error(0, "unknown erro")) {
		_playerId = id;
	}


	
	public string playerId {
		get {
			return _playerId;
		}
	}	
	
	public GK_Player playerTemplate {
		get {
			return _tpl;
		}
	}
}
