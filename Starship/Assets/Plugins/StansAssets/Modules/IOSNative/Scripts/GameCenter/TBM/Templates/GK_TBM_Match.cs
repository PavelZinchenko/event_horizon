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

public class GK_TBM_Match  {
	
	public string Id;
	public string Message;
	public GK_TBM_Participant CurrentParticipant;
	public DateTime CreationTimestamp;
	
	public byte[] Data;
	
	public GK_TurnBasedMatchStatus Status;
	public List<GK_TBM_Participant> Participants;
	
	
	
	public void SetData(string val) {
		byte[] decodedFromBase64 = System.Convert.FromBase64String(val);
		Data = decodedFromBase64;
	}
	
	public string UTF8StringData {
		get {
			if(Data != null) {
#if !UNITY_WP8 && !UNITY_WSA
				return System.Text.Encoding.UTF8.GetString(Data);
#else
				return string.Empty;
#endif
			} else {
				return string.Empty;
			}
			
		}
	}
	
	public GK_TBM_Participant GetParticipantByPlayerId(string playerId) {
		
		foreach(GK_TBM_Participant participant in Participants) {
			
			if(participant.Player == null) {
				if(playerId.Length == 0) {
					return participant;
				}
			} else {
				if(playerId.Equals(participant.Player.Id)) {
					return participant;
				}
			}
		}
		
		
		return null;
	}
	
	
	/// <summary>
	/// Return UM_TBM_Participant object of the local player
	/// </summary>
	public GK_TBM_Participant LocalParticipant {
		get {
			foreach(GK_TBM_Participant p in Participants) {
				if(p.Player != null) {
					if(p.PlayerId.Equals(GameCenterManager.Player.Id)) {
						return p;
					}
				}
			}
			return null;
		}
	}
	
	
}
