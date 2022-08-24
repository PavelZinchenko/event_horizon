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

public class GK_RTM_Match  {


	private int _ExpectedPlayerCount;
	private List<GK_Player> _Players =  new List<GK_Player>();


	public GK_RTM_Match(string matchData) {
		string[] MatchData = matchData.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);
		_ExpectedPlayerCount = Convert.ToInt32(MatchData[0]);

		string[] playersIds =  SA.Common.Data.Converter.ParseArray(MatchData[1]);
		foreach(string playerId in playersIds) {
			GK_Player player = GameCenterManager.GetPlayerById(playerId);
			_Players.Add(player);
		}
	}


	public int ExpectedPlayerCount {
		get {
			return _ExpectedPlayerCount;
		}
	}

	public List<GK_Player> Players {
		get {
			return _Players;
		}
	}
}
