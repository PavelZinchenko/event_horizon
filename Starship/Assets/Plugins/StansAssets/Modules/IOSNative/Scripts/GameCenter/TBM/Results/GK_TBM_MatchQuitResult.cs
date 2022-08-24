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

public class GK_TBM_MatchQuitResult : SA.Common.Models.Result {

	private string _MatchId = null;
	
	
	
	public GK_TBM_MatchQuitResult(string matchId):base() {
		_MatchId = matchId;
	}
	
	public GK_TBM_MatchQuitResult():base(new SA.Common.Models.Error()) {
	
	}
	
	public string MatchId {
		get {
			return _MatchId;
		}
	}
}
