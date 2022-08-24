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

public class GK_SaveDataLoaded : SA.Common.Models.Result {

	private GK_SavedGame _SavedGame = null;

	public GK_SaveDataLoaded(GK_SavedGame save):base() {
		_SavedGame = save;
	}

	public GK_SaveDataLoaded(SA.Common.Models.Error error):base(error) {
	
	}


	public GK_SavedGame SavedGame {
		get {
			return _SavedGame;
		}
	}
}
