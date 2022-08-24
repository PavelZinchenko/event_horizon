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


public class GK_TBM_MatchDataUpdateResult : SA.Common.Models.Result {

	private GK_TBM_Match _Match = null;



	public GK_TBM_MatchDataUpdateResult(GK_TBM_Match updatedMatch):base() {
		_Match = updatedMatch;
	}

	public GK_TBM_MatchDataUpdateResult(string errorData):base(new SA.Common.Models.Error(errorData)) {
		
	}

	public GK_TBM_MatchDataUpdateResult(SA.Common.Models.Error error):base(error) {

	}

	public GK_TBM_Match Match {
		get {
			return _Match;
		}
	}
}
