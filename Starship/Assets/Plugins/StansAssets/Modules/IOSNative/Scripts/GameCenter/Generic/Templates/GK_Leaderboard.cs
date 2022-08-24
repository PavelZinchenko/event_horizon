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


[System.Serializable]
public class GK_Leaderboard  {

	//Editor Use Only
	public bool IsOpen = true;

	private bool _CurrentPlayerScoreLoaded = false;

	public GK_ScoreCollection SocsialCollection =  new GK_ScoreCollection();
	public GK_ScoreCollection GlobalCollection =  new GK_ScoreCollection();

	private List<GK_Score> CurrentPlayerScore =  new List<GK_Score>();
	private Dictionary<int, GK_LocalPlayerScoreUpdateListener> ScoreUpdateListners =  new Dictionary<int, GK_LocalPlayerScoreUpdateListener>();

	[SerializeField]
	private GK_LeaderBoardInfo _info;

	//--------------------------------------
	// initialization
	//--------------------------------------


	public GK_Leaderboard(string leaderboardId) {
		_info =   new GK_LeaderBoardInfo();
		_info.Identifier = leaderboardId;
	}

	public void Refresh() {
		SocsialCollection =  new GK_ScoreCollection();
		GlobalCollection =  new GK_ScoreCollection();
		
		CurrentPlayerScore =  new List<GK_Score>();
		ScoreUpdateListners =  new Dictionary<int, GK_LocalPlayerScoreUpdateListener>();
	}


	//--------------------------------------
	// Public Methods
	//--------------------------------------

	

	public GK_Score GetCurrentPlayerScore(GK_TimeSpan timeSpan, GK_CollectionType collection) {

		foreach(GK_Score score in CurrentPlayerScore) {
			if(score.TimeSpan == timeSpan && score.Collection == collection) {
				return score;
			}
		}

		return null;
	}


	public GK_Score GetScoreByPlayerId(string playerId, GK_TimeSpan timeSpan, GK_CollectionType collection) {

		if(playerId.Equals(GameCenterManager.Player.Id)) {
			return GetCurrentPlayerScore(timeSpan, collection);
		}

		List<GK_Score> scores = GetScoresList(timeSpan, collection);
		foreach(GK_Score s in scores) {
			if(s.PlayerId.Equals(playerId)) {
				return s;
			}
		}
		
		return null;
	}



	public List<GK_Score> GetScoresList(GK_TimeSpan timeSpan, GK_CollectionType collection) {
		GK_ScoreCollection col = GlobalCollection;
		
		switch(collection) {
		case GK_CollectionType.GLOBAL:
			col = GlobalCollection;
			break;
		case GK_CollectionType.FRIENDS:
			col = SocsialCollection;
			break;
		}
		
		
		Dictionary<int, GK_Score> scoreDict = col.AllTimeScores;
		
		switch(timeSpan) {
		case GK_TimeSpan.ALL_TIME:
			scoreDict = col.AllTimeScores;
			break;
		case GK_TimeSpan.TODAY:
			scoreDict = col.TodayScores;
			break;
		case GK_TimeSpan.WEEK:
			scoreDict = col.WeekScores;
			break;
		}
		
		List<GK_Score> scores = new List<GK_Score>();
		scores.AddRange(scoreDict.Values);

		return scores;
	}
	
	

	public GK_Score GetScore(int rank, GK_TimeSpan timeSpan, GK_CollectionType collection) {

		GK_ScoreCollection col = GlobalCollection;
		
		switch(collection) {
		case GK_CollectionType.GLOBAL:
			col = GlobalCollection;
			break;
		case GK_CollectionType.FRIENDS:
			col = SocsialCollection;
			break;
		}

		Dictionary<int, GK_Score> scoreDict = col.AllTimeScores;
		
		switch(timeSpan) {
		case GK_TimeSpan.ALL_TIME:
			scoreDict = col.AllTimeScores;
			break;
		case GK_TimeSpan.TODAY:
			scoreDict = col.TodayScores;
			break;
		case GK_TimeSpan.WEEK:
			scoreDict = col.WeekScores;
			break;
		}



		if(scoreDict.ContainsKey(rank)) {
			return scoreDict[rank];
		} else {
			return null;
		}

	}


	//--------------------------------------
	// Get / Set
	//--------------------------------------

	public GK_LeaderBoardInfo Info {
		get {
			return _info;
		}
	}


	public string Id {
		get {
			return _info.Identifier;
		}
	}

	public bool CurrentPlayerScoreLoaded {
		get {
			return _CurrentPlayerScoreLoaded;
		}
	}

	//--------------------------------------
	// Internal Use
	//--------------------------------------


	public void CreateScoreListener(int requestId, bool isInternal) {
		GK_LocalPlayerScoreUpdateListener listener = new GK_LocalPlayerScoreUpdateListener(requestId, Id, isInternal);
		ScoreUpdateListners.Add(listener.RequestId, listener);
	}

	public void ReportLocalPlayerScoreUpdate (GK_Score score, int requestId) {
		GK_LocalPlayerScoreUpdateListener listener = ScoreUpdateListners[requestId];
		listener.ReportScoreUpdate(score);
	}

	public void UpdateCurrentPlayerScore(List<GK_Score> newScores) {
		CurrentPlayerScore.Clear();
		foreach(GK_Score s in newScores) {
			CurrentPlayerScore.Add(s);
		}
		_CurrentPlayerScoreLoaded = true;
	}

	public void UpdateCurrentPlayerScore(GK_Score score) {
		GK_Score currentScore = GetCurrentPlayerScore(score.TimeSpan, score.Collection);
		if (currentScore != null) {
			CurrentPlayerScore.Remove(currentScore);
		}
		CurrentPlayerScore.Add(score);
		_CurrentPlayerScoreLoaded = true;
	}

	public void ReportLocalPlayerScoreUpdateFail(string errorData, int requestId) {
		GK_LocalPlayerScoreUpdateListener listener = ScoreUpdateListners[requestId];
		listener.ReportScoreUpdateFail(errorData);
	}

	public void UpdateScore(GK_Score s) {
		
		GK_ScoreCollection col = GlobalCollection;
		
		switch(s.Collection) {
		case GK_CollectionType.GLOBAL:
			col = GlobalCollection;
			break;
		case GK_CollectionType.FRIENDS:
			col = SocsialCollection;
			break;
		}
		
		
		
		
		Dictionary<int, GK_Score> scoreDict = col.AllTimeScores;
		
		switch(s.TimeSpan) {
		case GK_TimeSpan.ALL_TIME:
			scoreDict = col.AllTimeScores;
			break;
		case GK_TimeSpan.TODAY:
			scoreDict = col.TodayScores;
			break;
		case GK_TimeSpan.WEEK:
			scoreDict = col.WeekScores;
			break;
		}
		
		
		if(scoreDict.ContainsKey(s.Rank)) {
			scoreDict[s.Rank] = s;
		} else {
			scoreDict.Add(s.Rank, s);
		}
	}

	

	
	[System.Obsolete("id is depreciated, plase use Id instead")]
	public string id {
		get {
			return _info.Identifier;
		}
	}


	public void UpdateMaxRange(int MR) {
		_info.MaxRange = MR;
	}

}

