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

public class GK_FriendRequest {

	private int _Id;

	private List<string> _PlayersIds =  new List<string>();
	private List<string> _Emails =  new List<string>();



	public GK_FriendRequest() {
		_Id = SA.Common.Util.IdFactory.NextId;
	}


	/// <summary>
	/// Adds recipients based on their email addresses..
	/// 
	/// If you do not add at least once recipient, the recipients 
	/// field is selected when the view controller is presented so 
	/// that the player can type a list of recipients. 
	/// Adding more players than defined by the
	/// 
	/// <param name="emailAddresses">A string that identifies the saved game data to be deleted.</param>
	/// </summary>
	public void addRecipientsWithEmailAddresses(params string[] emailAddresses) {
		foreach(string email in emailAddresses) {
			if(!_Emails.Contains(email)) {
				_Emails.Add(email);
			}
		}
	}


	/// <summary>
	/// Adds recipients based on their Game Center player identifiers.
	/// 
	/// If you do not add at least once recipient, the recipients 
	/// field is selected when the view controller is presented 
	/// so that the player can type a list of recipients.
	/// 
	/// <param name="players">An array with one or more GK_Player objects.</param>
	/// </summary>
	public void  addRecipientPlayers(params GK_Player[] players) {
		foreach(GK_Player p in players) {
			if(!_PlayersIds.Contains(p.Id)) {
				_PlayersIds.Add(p.Id);
			}
		}
	}

	/// <summary>
	/// Start's Friend Request view controller
	/// </summary>
	public void Send() {
		GameCenterManager.SendFriendRequest(this, _Emails, _PlayersIds);
	}




	public int Id {
		get {
			return _Id;
		}
	}
}
