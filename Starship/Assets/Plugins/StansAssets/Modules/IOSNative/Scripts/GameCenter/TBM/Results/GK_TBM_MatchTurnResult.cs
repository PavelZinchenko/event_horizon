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

public class GK_TBM_MatchTurnResult : SA.Common.Models.Result {
	
	private GK_TBM_Match _Match = null;

	public GK_TBM_MatchTurnResult(GK_TBM_Match match):base() {
		_Match = match;
	}
	
	public GK_TBM_MatchTurnResult(string errorData):base(new SA.Common.Models.Error(errorData)) {
	}
	
	
	public GK_TBM_Match Match {
		get {
			return _Match;
		}
	}
}