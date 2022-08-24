////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCenterInvitations : SA.Common.Pattern.Singleton<GameCenterInvitations> {


	public static event Action<GK_Player, GK_InviteRecipientResponse> ActionInviteeResponse = delegate {};

	public static event Action<GK_MatchType, GK_Invite> ActionPlayerAcceptedInvitation = delegate {};
	public static event Action<GK_MatchType, string[], GK_Player[]> ActionPlayerRequestedMatchWithRecipients = delegate {};



	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	public void Init() {
		//empty method just to create GameCenterInvitations,
		//will be called on Game Center initialization
	}



	// --------------------------------------
	// Native Events
	// --------------------------------------

	private void OnInviteeResponse(string data) {
		ISN_Logger.Log("OnInviteeResponse");
		string[] DataArray = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		GK_Player player = GameCenterManager.GetPlayerById(DataArray[0]);
		GK_InviteRecipientResponse responce = (GK_InviteRecipientResponse) Convert.ToInt32(DataArray[1]);

		ActionInviteeResponse(player, responce);
	}


	// --------------------------------------
	// RTM
	// --------------------------------------

	private void OnPlayerAcceptedInvitation_RTM(string data) {
		ISN_Logger.Log("OnPlayerAcceptedInvitation_RTM");
		GK_Invite invite = new GK_Invite(data);
		ActionPlayerAcceptedInvitation(GK_MatchType.RealTime, invite);
	}

	private void OnPlayerRequestedMatchWithRecipients_RTM(string data) {
		ISN_Logger.Log("OnPlayerRequestedMatchWithRecipients_RTM");
		string[] playersIds = SA.Common.Data.Converter.ParseArray(data);
		List<GK_Player> players = new List<GK_Player>();
		foreach(string playerId in playersIds) {
			players.Add(GameCenterManager.GetPlayerById(playerId));
		}

		ActionPlayerRequestedMatchWithRecipients(GK_MatchType.RealTime, playersIds, players.ToArray());
	}


	// --------------------------------------
	// TBM
	// --------------------------------------


	private void OnPlayerAcceptedInvitation_TBM(string data) {
		ISN_Logger.Log("OnPlayerAcceptedInvitation_TBM");
		GK_Invite invite = new GK_Invite(data);
		ActionPlayerAcceptedInvitation(GK_MatchType.TurnBased, invite);
	}
	
	private void OnPlayerRequestedMatchWithRecipients_TBM(string data) {
		ISN_Logger.Log("OnPlayerRequestedMatchWithRecipients_TBM");
		string[] playersIds = SA.Common.Data.Converter.ParseArray(data);
		List<GK_Player> players = new List<GK_Player>();
		foreach(string playerId in playersIds) {
			players.Add(GameCenterManager.GetPlayerById(playerId));
		}
		
		ActionPlayerRequestedMatchWithRecipients(GK_MatchType.RealTime, playersIds, players.ToArray());
	}
}
