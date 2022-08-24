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

public class GK_RTM_MatchStartedResult : SA.Common.Models.Result {

	private GK_RTM_Match _Match = null;
	
	public GK_RTM_MatchStartedResult(GK_RTM_Match match):base() {
		_Match = match;
	}
	
	public GK_RTM_MatchStartedResult(string errorData):base(new SA.Common.Models.Error(errorData)) {

	}
	
	
	public GK_RTM_Match Match {
		get {
			return _Match;
		}
	}
}
