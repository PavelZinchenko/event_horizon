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

public class GK_Invite  {

	private string _Id;
	private GK_Player _Sender;
	private int _PlayerGroup;
	private int _PlayerAttributes;



	public GK_Invite(string inviteData) {
		string[] DataArray = inviteData.Split(SA.Common.Data.Converter.DATA_SPLITTER); 
		_Id = DataArray[0];
		_Sender = GameCenterManager.GetPlayerById(DataArray[1]);

		_PlayerGroup = System.Convert.ToInt32(DataArray[2]);
		_PlayerAttributes = System.Convert.ToInt32(DataArray[3]);
	}



	public string Id {
		get {
			return _Id;
		}
	}

	public GK_Player Sender {
		get {
			return _Sender;
		}
	}

	public int PlayerGroup {
		get {
			return _PlayerGroup;
		}
	}

	public int PlayerAttributes {
		get {
			return _PlayerAttributes;
		}
	}
}
