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

public class GameCenter_RTM : SA.Common.Pattern.Singleton<GameCenter_RTM> {


	
	#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE


	[DllImport ("__Internal")]
	private static extern  void _ISN_RTM_FindMatch(int minPlayers, int maxPlayers, string msg, string invitations);

	[DllImport ("__Internal")]
	private static extern  void _ISN_RTM_FindMatchWithNativeUI(int minPlayers, int maxPlayers, string msg, string invitations);
	
	[DllImport ("__Internal")]
	private static extern  void _ISN_RTM_SetPlayerGroup(int group);
	
	[DllImport ("__Internal")]
	private static extern  void _ISN_RTM_SetPlayerAttributes(int attributes);
	
	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_StartMatchWithInviteID(string inviteId, bool useNativeUI);

	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_CancelPendingInviteToPlayerWithId(string playerId);
	
	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_CancelMatchSeartch();
	
	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_FinishMatchmaking ();
	
	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_QueryActivity ();

	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_QueryPlayerGroupActivity(int group);
	
	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_StartBrowsingForNearbyPlayers();
	
	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_StopBrowsingForNearbyPlayers ();
	
	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_Rematch();

	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_Disconnect();
	
	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_SendData(string data, string playersIds, int dataMode);
	
	[DllImport ("__Internal")]
	private static extern  void ISN_RTM_SendDataToAll(string data, int dataMode);


	#endif
	

	private GK_RTM_Match _CurrentMatch = null;
	private Dictionary<string, GK_Player> _NearbyPlayers = new Dictionary<string, GK_Player>();

	public static event Action<GK_RTM_MatchStartedResult> ActionMatchStarted = delegate {};
	public static event Action<SA.Common.Models.Error> ActionMatchFailed = delegate {};
	//public static event Action<GK_RTM_Match> ActionMatchInfoUpdated = delegate {};


	//[Route("GK_Player", Name = "EditVehicle")]
	public static event Action<GK_Player, bool> ActionNearbyPlayerStateUpdated = delegate {};
	public static event Action<GK_RTM_QueryActivityResult> ActionActivityResultReceived = delegate {};

	public static event Action<SA.Common.Models.Error> ActionDataSendError = delegate {};
	public static event Action<GK_Player, byte[]> ActionDataReceived = delegate {};
	public static event Action<GK_Player, GK_PlayerConnectionState, GK_RTM_Match> ActionPlayerStateChanged = delegate {};
	public static event Action<GK_Player> ActionDiconnectedPlayerReinvited = delegate {};


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	


	public void FindMatch(int minPlayers, int maxPlayers, string msg = "", string[] playersToInvite = null) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_RTM_FindMatch(minPlayers, maxPlayers, msg, SA.Common.Data.Converter.SerializeArray(playersToInvite));
		#endif
	}
	
	public void FindMatchWithNativeUI(int minPlayers, int maxPlayers, string msg = "", string[] playersToInvite = null) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_RTM_FindMatchWithNativeUI(minPlayers, maxPlayers, msg, SA.Common.Data.Converter.SerializeArray(playersToInvite));
		#endif
	}

	public void SetPlayerGroup(int group) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_RTM_SetPlayerGroup(group);
		#endif
	}
	
	
	public void SetPlayerAttributes(int attributes)  {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_RTM_SetPlayerAttributes(attributes);
		#endif
	}

	public void StartMatchWithInvite(GK_Invite invite, bool useNativeUI) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		ISN_RTM_StartMatchWithInviteID(invite.Id,  useNativeUI);
		#endif
	}

	public void CancelPendingInviteToPlayer(GK_Player player) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		ISN_RTM_CancelPendingInviteToPlayerWithId(player.Id);
		#endif
	}

	public void CancelMatchSearch() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		ISN_RTM_CancelMatchSeartch();
		#endif
	}

	public void FinishMatchmaking() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		ISN_RTM_FinishMatchmaking();
		#endif
	}


	public void QueryActivity() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		ISN_RTM_QueryActivity();
		#endif
	}

	public void QueryPlayerGroupActivity(int group) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		ISN_RTM_QueryPlayerGroupActivity(group);
		#endif
	}

	public void StartBrowsingForNearbyPlayers() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		ISN_RTM_StartBrowsingForNearbyPlayers();
		#endif
	}

	public void StopBrowsingForNearbyPlayers() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		ISN_RTM_StopBrowsingForNearbyPlayers();
		#endif
	}

	public void Rematch() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		ISN_RTM_Rematch();
		#endif
	}

	public void Disconnect() {
		_CurrentMatch = null;
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		ISN_RTM_Disconnect();
		#endif
	}
	
	public void SendDataToAll(byte[] data, GK_MatchSendDataMode dataMode) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		string bytesString = System.Convert.ToBase64String (data);
		ISN_RTM_SendDataToAll(bytesString, (int) dataMode);
		#endif
	}

	public void SendData(byte[] data, GK_MatchSendDataMode dataMode, params GK_Player[] players) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		string bytesString = System.Convert.ToBase64String (data);

		List<string> playersIds =  new List<string>();
		foreach(GK_Player player in players) {
			playersIds.Add(player.Id);
		}

		ISN_RTM_SendData(bytesString, SA.Common.Data.Converter.SerializeArray(playersIds.ToArray()), (int) dataMode);
		#endif
	}

	

	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public GK_RTM_Match CurrentMatch {
		get {
			return _CurrentMatch;
		}
	}

	public List<GK_Player> NearbyPlayersList {
		get {
			List<GK_Player> playersList  = new List<GK_Player>();
			foreach(KeyValuePair<string, GK_Player> pair in _NearbyPlayers) {
				playersList.Add(pair.Value);
			}
			
			return playersList;
		}
	}

	public Dictionary<string, GK_Player> NearbyPlayers {
		get {
			return _NearbyPlayers;
		}
	}



	//--------------------------------------
	//  EVENTS
	//--------------------------------------


	private void OnMatchStartFailed(string errorData) {
		GK_RTM_MatchStartedResult result =  new GK_RTM_MatchStartedResult(errorData);
		ActionMatchStarted(result);
	}

	private void OnMatchStarted(string matchData) {
		GK_RTM_Match match = ParseMatchData(matchData);

		GK_RTM_MatchStartedResult result =  new GK_RTM_MatchStartedResult(match);
		ActionMatchStarted(result);
	}


	private void OnMatchFailed(string errorData) {
		_CurrentMatch = null;
		SA.Common.Models.Error error =  new SA.Common.Models.Error(errorData);
		ActionMatchFailed(error);
	}

	private void OnNearbyPlayerInfoReceived(string data) {
		string[] DataArray = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		string playerId = DataArray[0];
		GK_Player player = GameCenterManager.GetPlayerById(playerId);


		bool reachable = Convert.ToBoolean(DataArray[1]);
		if(reachable)  {
			if(!_NearbyPlayers.ContainsKey(player.Id)) {
				_NearbyPlayers.Add(player.Id, player);
			}
		} else {
			if(_NearbyPlayers.ContainsKey(player.Id)) {
				_NearbyPlayers.Remove(player.Id);
			}
		}

		ActionNearbyPlayerStateUpdated(player, reachable);
	}


	private void OnQueryActivity(string data) {
		int activity = Convert.ToInt32(data);
		GK_RTM_QueryActivityResult result =  new GK_RTM_QueryActivityResult(activity);
		ActionActivityResultReceived(result);
	}


	private void OnQueryActivityFailed(string errorData) {
		GK_RTM_QueryActivityResult result =  new GK_RTM_QueryActivityResult(errorData);
		ActionActivityResultReceived(result);
	}

	//this event will update current match info
	//event always called right before OnMatchPlayerStateChanged
	private void OnMatchInfoUpdated(string matchData) {
		GK_RTM_Match match =  ParseMatchData(matchData);
		if(match.Players.Count == 0 && match.ExpectedPlayerCount == 0) {
			_CurrentMatch = null;
		}
	}


	private void OnMatchPlayerStateChanged(string data) {

		if(_CurrentMatch == null) {
			return;
		}

		string[] DataArray = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);
		string playerId = DataArray[0];
		GK_Player player = GameCenterManager.GetPlayerById(playerId);



		GK_PlayerConnectionState state = (GK_PlayerConnectionState) Convert.ToInt32(DataArray[1]);
		ActionPlayerStateChanged(player, state, CurrentMatch);
	}

	private void OnDiconnectedPlayerReinvited(string playerId) {
		GK_Player player = GameCenterManager.GetPlayerById(playerId);
		ActionDiconnectedPlayerReinvited(player);
	}

	private void OnMatchDataReceived(string data) {
		string[] DataArray = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);
		string playerId = DataArray[0];
		GK_Player player = GameCenterManager.GetPlayerById(playerId);

		byte[] decodedFromBase64 = System.Convert.FromBase64String(DataArray[1]);
		ActionDataReceived(player, decodedFromBase64);
	}

	private void OnSendDataError(string errorData) {
		SA.Common.Models.Error error =  new SA.Common.Models.Error(errorData);
		ActionDataSendError(error);
	}

	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private GK_RTM_Match ParseMatchData(string matchData) {
		GK_RTM_Match match =  new GK_RTM_Match(matchData);
		_CurrentMatch = match;

		return match;
	}
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
