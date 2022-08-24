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

public class GK_LeaderboardResult : SA.Common.Models.Result {

	private GK_Leaderboard _Leaderboard;

	
	public GK_LeaderboardResult(GK_Leaderboard leaderboard):base() {
		Setinfo(leaderboard);
	}
	
	public GK_LeaderboardResult(GK_Leaderboard leaderboard, SA.Common.Models.Error error):base(error) {
		Setinfo(leaderboard);
	}
	
	
	
	private void Setinfo(GK_Leaderboard leaderboard) {
		_Leaderboard = leaderboard;
	}
	
	
	
	public GK_Leaderboard Leaderboard {
		get {
			return _Leaderboard;
		}
	}
	

}
