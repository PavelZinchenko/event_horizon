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

public class GK_FetchResult : SA.Common.Models.Result {

	private List<GK_SavedGame> _SavedGames = new List<GK_SavedGame>();



	public GK_FetchResult(List<GK_SavedGame> saves):base() {
		_SavedGames = saves;
	}

	public GK_FetchResult(string errorData):base(new SA.Common.Models.Error(errorData)) {} 


	public List<GK_SavedGame> SavedGames {
		get {
			return _SavedGames;
		}
	}
}
