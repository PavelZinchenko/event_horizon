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

public class GK_LocalPlayerScoreUpdateListener  {

	private int _RequestId;
	private bool _IsInternal;
	private string _leaderboardId;


	private string _ErrorData = null;


	private List<GK_Score> Scores =  new List<GK_Score>();


	//--------------------------------------
	// Initialization
	//--------------------------------------

	public GK_LocalPlayerScoreUpdateListener (int requestId, string leaderboardId, bool isInternal) {
		_RequestId = requestId;
		_leaderboardId = leaderboardId;
		_IsInternal =  isInternal;
	}


	//--------------------------------------
	// Public Methods
	//--------------------------------------
	
	public void ReportScoreUpdate(GK_Score score) {
		Scores.Add(score);
		DispatchUpdate();
	}

	public void ReportScoreUpdateFail(string errorData) {
		ISN_Logger.Log("ReportScoreUpdateFail");
		_ErrorData = errorData;
		Scores.Add(null);

		DispatchUpdate();
	}


	//--------------------------------------
	// Get / Set
	//--------------------------------------
	
	public int RequestId {
		get {
			return _RequestId;
		}
	}


	//--------------------------------------
	// Private Methods
	//--------------------------------------
	
	private void DispatchUpdate() {
		if(Scores.Count == 6) {

			GK_Leaderboard board = GameCenterManager.GetLeaderboard(_leaderboardId);
			GK_LeaderboardResult result;


			if(_ErrorData != null) {
				result =  new GK_LeaderboardResult(board, new SA.Common.Models.Error(_ErrorData) );
			} else {
				board.UpdateCurrentPlayerScore(Scores);
				result =  new GK_LeaderboardResult(board);
			}

			GameCenterManager.DispatchLeaderboardUpdateEvent(result, _IsInternal);

		}
	}
}
