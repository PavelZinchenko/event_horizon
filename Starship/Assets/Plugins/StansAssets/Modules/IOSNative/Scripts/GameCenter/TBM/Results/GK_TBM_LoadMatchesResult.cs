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


public class GK_TBM_LoadMatchesResult : SA.Common.Models.Result {

	public Dictionary<string, GK_TBM_Match> LoadedMatches = new Dictionary<string, GK_TBM_Match>();

	public GK_TBM_LoadMatchesResult(bool IsResultSucceeded):base() {

	}

	public GK_TBM_LoadMatchesResult(string errorData):base(new SA.Common.Models.Error(errorData)) {
		
	}
}
