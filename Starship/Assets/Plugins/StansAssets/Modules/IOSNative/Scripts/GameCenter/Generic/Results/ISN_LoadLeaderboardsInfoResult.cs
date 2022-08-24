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

public class ISN_LoadSetLeaderboardsInfoResult : SA.Common.Models.Result {

	public GK_LeaderboardSet _LeaderBoardsSet;

	public ISN_LoadSetLeaderboardsInfoResult(GK_LeaderboardSet lbset):base() {
		_LeaderBoardsSet = lbset;
	}

	public ISN_LoadSetLeaderboardsInfoResult(GK_LeaderboardSet lbset, SA.Common.Models.Error error):base(error) {
		_LeaderBoardsSet = lbset;
	}

	public GK_LeaderboardSet LeaderBoardsSet {
		get {
			return _LeaderBoardsSet;
		}
	}
}
