#define GAME_CENTER_ENABLED
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class GameCenter_TBM : SA.Common.Pattern.Singleton<GameCenter_TBM> {
	
	
	#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_LoadMatchesInfo();
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_LoadMatch(string matchId);
	
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_FindMatch(int minPlayers, int maxPlayers, string msg, string invitations);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_FindMatchWithNativeUI(int minPlayers, int maxPlayers, string msg, string invitations);
	
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_SaveCurrentTurn(string matchId, string data);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_SetPlayerGroup(int group);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_SetPlayerAttributes(int attributes);
	
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_EndTurn(string matchId, string data, string nextPlayerId);
	
	
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_QuitInTurn(string matchId, int outcome, string nextPlayerId, string data);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_QuitOutOfTurn(string matchId, int outcome);
	
	
	
	[DllImport ("__Internal")]
	public static extern void _ISN_TBM_UpdateParticipantOutcome(string matchId, int outcome, string playerId);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_EndMatch(string matchId, string data);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_Rematch(string matchId);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_RemoveMatch(string matchId);
	
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_DeclineInvite(string matchId);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TBM_AcceptInvite(string matchId);
	
	#endif
	
	public static event Action<GK_TBM_LoadMatchResult> ActionMatchInfoLoaded = delegate {};
	public static event Action<GK_TBM_LoadMatchesResult> ActionMatchesInfoLoaded = delegate {};
	
	public static event Action<GK_TBM_MatchDataUpdateResult> ActionMatchDataUpdated = delegate {};
	public static event Action<GK_TBM_MatchInitResult> ActionMatchFound = delegate {};
	public static event Action<GK_TBM_MatchQuitResult> ActionMatchQuit = delegate {};
	
	public static event Action<GK_TBM_EndTrunResult> ActionTrunEnded = delegate {};
	public static event Action<GK_TBM_MatchEndResult> ActionMacthEnded = delegate {};
	
	public static event Action<GK_TBM_RematchResult> ActionRematched = delegate {};
	public static event Action<GK_TBM_MatchRemovedResult> ActionMatchRemoved = delegate {};
	
	
	public static event Action<GK_TBM_MatchInitResult> ActionMatchInvitationAccepted = delegate {};
	public static event Action<GK_TBM_MatchRemovedResult> ActionMatchInvitationDeclined = delegate {};
	
	
	
	//from GC UI
	public static event Action<GK_TBM_Match> ActionPlayerQuitForMatch = delegate {};
	public static event Action<GK_TBM_MatchTurnResult> ActionTrunReceived = delegate {};
	
	
	private Dictionary<string, GK_TBM_Match> _Matches = new Dictionary<string, GK_TBM_Match>();
	
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	
	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------
	
	public void LoadMatchesInfo() {
		
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_LoadMatchesInfo();
		#endif
	}
	
	public void LoadMatch(string matchId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_LoadMatch(matchId);
		#endif
	}
	
	public void FindMatch(int minPlayers, int maxPlayers, string msg = "", string[] playersToInvite = null) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_FindMatch(minPlayers, maxPlayers, msg, SA.Common.Data.Converter.SerializeArray(playersToInvite));
		#endif
	}
	
	public void FindMatchWithNativeUI(int minPlayers, int maxPlayers, string msg = "", string[] playersToInvite = null) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_FindMatchWithNativeUI(minPlayers, maxPlayers, msg, SA.Common.Data.Converter.SerializeArray(playersToInvite));
		#endif
	}
	
	
	public void SetPlayerGroup(int group) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_SetPlayerGroup(group);
		#endif
	}
	
	
	public void SetPlayerAttributes(int attributes)  {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_SetPlayerAttributes(attributes);
		#endif
	}
	
	
	
	
	public void SaveCurrentTurn(string matchId, byte[] matchData) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		string bytesString = System.Convert.ToBase64String (matchData);
		_ISN_TBM_SaveCurrentTurn(matchId, bytesString);
		#endif
	}
	
	
	
	public void EndTurn(string matchId, byte[] matchData, string nextPlayerId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		string bytesString = System.Convert.ToBase64String (matchData);
		_ISN_TBM_EndTurn(matchId, bytesString, nextPlayerId);
		#endif
	}
	
	
	public void QuitInTurn(string matchId, GK_TurnBasedMatchOutcome outcome, string nextPlayerId, byte[] matchData) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		string bytesString = System.Convert.ToBase64String (matchData);
		_ISN_TBM_QuitInTurn(matchId, (int)outcome, nextPlayerId, bytesString);
		#endif
	}
	
	public void QuitOutOfTurn(string matchId, GK_TurnBasedMatchOutcome outcome) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_QuitOutOfTurn(matchId, (int)outcome);
		#endif
	}
	
	
	
	public void EndMatch(string matchId, byte[] matchData) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		string bytesString = System.Convert.ToBase64String (matchData);
		_ISN_TBM_EndMatch(matchId, bytesString);
		#endif
	}
	
	public void Rematch(string matchId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_Rematch(matchId);
		#endif
	}
	
	public void RemoveMatch(string matchId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_RemoveMatch(matchId);
		#endif
	}
	
	
	public void AcceptInvite(string matchId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_AcceptInvite(matchId);
		#endif
	}
	
	public void DeclineInvite(string matchId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_DeclineInvite(matchId);
		#endif
	}
	
	
	
	public void UpdateParticipantOutcome(string matchId, int outcome, string playerId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_TBM_UpdateParticipantOutcome(matchId, outcome, playerId);
		#endif
	}
	
	
	
	public GK_TBM_Match GetMatchById(string matchId) {
		if(_Matches.ContainsKey(matchId)) {
			return _Matches[matchId];
		} else {
			return null;
		}
	}
	
	public static void PrintMatchInfo(GK_TBM_Match match) {
		string MatchInfo = string.Empty;
		
		MatchInfo = MatchInfo + "----------------------------------------" + "\n";
		MatchInfo = MatchInfo + "Printing basic match info, for " + "\n";
		MatchInfo = MatchInfo + "Match ID: " + match.Id + "\n";
		
		MatchInfo = MatchInfo + "Status:" + match.Status + "\n";
		
		if(match.CurrentParticipant != null) {
			if(match.CurrentParticipant.Player != null) {
				MatchInfo = MatchInfo + "CurrentPlayerID: " + match.CurrentParticipant.Player.Id + "\n";
			} else {
				MatchInfo = MatchInfo + "CurrentPlayerID: ---- \n";
			}
		} else {
			MatchInfo = MatchInfo + "CurrentPlayerID: ---- \n";
		}
		
		
		MatchInfo = MatchInfo + "Data: " + match.UTF8StringData + "\n";
		MatchInfo = MatchInfo + "*******Participants*******" + "\n";
		foreach(GK_TBM_Participant p in match.Participants) {
			if(p.Player != null) {
				MatchInfo = MatchInfo + "PlayerId: " + p.Player.Id + "\n";
			} else {
				MatchInfo = MatchInfo + "PlayerId: ---  \n";
			}
			
			MatchInfo = MatchInfo + "Status: " + p.Status + "\n";
			MatchInfo = MatchInfo + "MatchOutcome: " + p.MatchOutcome + "\n";
			MatchInfo = MatchInfo + "TimeoutDate: " +  p.TimeoutDate.ToString("DD MMM YYYY HH:mm:ss")   + "\n";
			MatchInfo = MatchInfo + "LastTurnDate: " + p.LastTurnDate.ToString("DD MMM YYYY HH:mm:ss")  + "\n";
			MatchInfo = MatchInfo + "**********************" + "\n";
		}
		
		
		MatchInfo = MatchInfo + "----------------------------------------" + "\n";
		
		ISN_Logger.Log(MatchInfo);
	} 
	
	// --------------------------------------
	// Get / Set
	// --------------------------------------
	
	
	public Dictionary<string, GK_TBM_Match> Matches {
		get {
			return _Matches;
		}
	}
	
	public List<GK_TBM_Match> MatchesList {
		get {
			List<GK_TBM_Match> matchesList  = new List<GK_TBM_Match>();
			foreach(KeyValuePair<string, GK_TBM_Match> pair in _Matches) {
				matchesList.Add(pair.Value);
			}
			
			return matchesList;
		}
	}
	
	// --------------------------------------
	// Native Events
	// --------------------------------------
	
	public void OnLoadMatchesResult(string data) {
		ISN_Logger.Log("TBM::OnLoadMatchesResult: " + data);
		
		GK_TBM_LoadMatchesResult result = new GK_TBM_LoadMatchesResult(true);
		
		_Matches =  new Dictionary<string, GK_TBM_Match>();
		
		if(data.Length == 0) {
			ActionMatchesInfoLoaded(result);
			return;
		}
		
		
		
		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);
		if (DataArray.Length > 0) {
			result.LoadedMatches = new Dictionary<string, GK_TBM_Match>();
			
			for (int i = 0; i < DataArray.Length; i++) {
				if (DataArray[i] == SA.Common.Data.Converter.DATA_EOF) {
					break;
				}
				
				GK_TBM_Match match = ParceMatchInfo(DataArray[i]);
				UpdateMatchInfo(match);
				result.LoadedMatches.Add(match.Id, match);
			}
		}
		
		ActionMatchesInfoLoaded(result);
	}
	
	private void OnLoadMatchesResultFailed(string errorData) {
		GK_TBM_LoadMatchesResult result = new GK_TBM_LoadMatchesResult(errorData);
		ActionMatchesInfoLoaded(result);
	}
	
	private void OnLoadMatchResult(string data) {
		GK_TBM_Match match = ParceMatchInfo(data);
		GK_TBM_LoadMatchResult result =  new GK_TBM_LoadMatchResult(match);
		ActionMatchInfoLoaded(result);
	}
	
	private void OnLoadMatchResultFailed(string errorData) {
		GK_TBM_LoadMatchResult result =  new GK_TBM_LoadMatchResult(errorData);
		ActionMatchInfoLoaded(result);
	}
	
	
	
	private void OnUpdateMatchResult(string data) {
		string[] DataArray = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);
		
		GK_TBM_MatchDataUpdateResult result;
		string matchId = DataArray[0];
		GK_TBM_Match m = GetMatchById(matchId);
		
		if(m == null) {
			
			SA.Common.Models.Error e =  new SA.Common.Models.Error(0, "Match with id: " + matchId + " not found");
			result =  new GK_TBM_MatchDataUpdateResult(e);
		} else {
			m.SetData(DataArray[1]);
			result =  new GK_TBM_MatchDataUpdateResult(m);
		}
		
		ActionMatchDataUpdated(result);
	}
	
	private void OnUpdateMatchResultFailed(string errorData) {
		GK_TBM_MatchDataUpdateResult result = new GK_TBM_MatchDataUpdateResult(errorData);
		ActionMatchDataUpdated(result);
	}
	
	
	private void OnMatchFoundResult(string data) {
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		GK_TBM_MatchInitResult result = new GK_TBM_MatchInitResult(match);
		ActionMatchFound(result);
	}
	
	private void OnMatchFoundResultFailed(string errorData) {
		GK_TBM_MatchInitResult result = new GK_TBM_MatchInitResult(errorData);
		ActionMatchFound(result);
	}
	
	private void OnPlayerQuitForMatch(string data) {
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		ActionPlayerQuitForMatch(match);
	}
	
	
	private void OnMatchQuitResult(string matchId) {
		
		GK_TBM_MatchQuitResult result = new GK_TBM_MatchQuitResult(matchId);
		ActionMatchQuit(result);
	}
	
	private void OnMatchQuitResultFailed(string errorData) {
		GK_TBM_MatchQuitResult result = new GK_TBM_MatchQuitResult(errorData);
		ActionMatchQuit(result);
	}
	
	
	private void OnEndTurnResult(string data) {
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		
		GK_TBM_EndTrunResult result = new GK_TBM_EndTrunResult(match);
		ActionTrunEnded(result);
	}
	
	private void OnEndTurnResultFailed(string errorData) {
		GK_TBM_EndTrunResult result = new GK_TBM_EndTrunResult(errorData);
		ActionTrunEnded(result);
	}
	
	private void OnEndMatch(string data) {
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		
		GK_TBM_MatchEndResult result = new GK_TBM_MatchEndResult(match);
		ActionMacthEnded(result);
	}
	
	private void OnEndMatchResult(string errorData) {
		GK_TBM_MatchEndResult result = new GK_TBM_MatchEndResult(errorData);
		ActionMacthEnded(result);
	}
	
	
	
	private void OnRematchResult(string data) {
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		
		GK_TBM_RematchResult result = new GK_TBM_RematchResult(match);
		ActionRematched(result);
	}
	
	private void OnRematchFailed(string errorData) {
		GK_TBM_RematchResult result = new GK_TBM_RematchResult(errorData);
		ActionRematched(result);
	}
	
	
	private void OnMatchRemoved(string matchId) {
		GK_TBM_MatchRemovedResult result = new GK_TBM_MatchRemovedResult(matchId);
		if(_Matches.ContainsKey(matchId)) {
			_Matches.Remove(matchId);
		}
		
		ActionMatchRemoved(result);
	}
	
	
	private void OnMatchRemoveFailed(string errorData) {
		GK_TBM_MatchRemovedResult result = new GK_TBM_MatchRemovedResult(errorData);
		ActionMatchRemoved(result);
	}
	
	
	private void OnMatchInvitationAccepted(string data) {
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		GK_TBM_MatchInitResult result = new GK_TBM_MatchInitResult(match);
		ActionMatchInvitationAccepted(result);
	}
	
	private void OnMatchInvitationAcceptedFailed(string errorData) {
		GK_TBM_MatchInitResult result = new GK_TBM_MatchInitResult(errorData);
		ActionMatchInvitationAccepted(result);
	}
	
	
	private void OnMatchInvitationDeclined(string matchId) {
		GK_TBM_MatchRemovedResult result = new GK_TBM_MatchRemovedResult(matchId);
		if(_Matches.ContainsKey(matchId)) {
			_Matches.Remove(matchId);
		}
		
		ActionMatchInvitationDeclined(result);
	}
	
	
	private void OnMatchInvitationDeclineFailed(string errorData) {
		GK_TBM_MatchRemovedResult result = new GK_TBM_MatchRemovedResult(errorData);
		ActionMatchInvitationDeclined(result);
	}
	
	// --------------------------------------
	// GK Turn-Based Event Listener
	// --------------------------------------
	
	private void OnTrunReceived(string data) {
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		
		GK_TBM_MatchTurnResult result = new GK_TBM_MatchTurnResult(match);
		ActionTrunReceived(result);
	}
	
	// --------------------------------------
	// Private Methods
	// --------------------------------------
	
	private void UpdateMatchInfo(GK_TBM_Match match) {
		if (_Matches.ContainsKey(match.Id)) {
			_Matches[match.Id] = match;
		} else {
			_Matches.Add(match.Id, match);
		}
	}
	
	private static GK_TBM_Match ParceMatchInfo(string data) {
		string[] MatchData = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);
		return ParceMatchInfo(MatchData, 0);
	}
	
	public static GK_TBM_Match ParceMatchInfo(string[] MatchData, int index) {
		GK_TBM_Match mtach = new GK_TBM_Match();
		
		mtach.Id = MatchData[index];
		mtach.Status = (GK_TurnBasedMatchStatus)  System.Convert.ToInt64(MatchData[index + 1]) ;
		mtach.Message = MatchData[index + 2];
		mtach.CreationTimestamp = DateTime.Parse(MatchData[index + 3]);
		mtach.SetData(MatchData[index + 4]);
		
		string currentPlayerId = MatchData[index + 5];
		
		mtach.Participants =  GameCenterManager.ParseParticipantsData(MatchData, index + 6); 
		
		foreach(GK_TBM_Participant participant in mtach.Participants) {
			participant.SetMatchId(mtach.Id);
		}
		
		mtach.CurrentParticipant = mtach.GetParticipantByPlayerId(currentPlayerId);
		
		return mtach;
	}
	
	
	
	
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------
	
	
}
