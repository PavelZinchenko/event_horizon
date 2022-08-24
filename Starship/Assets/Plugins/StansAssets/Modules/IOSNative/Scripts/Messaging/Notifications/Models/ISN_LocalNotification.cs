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
using System;

public class ISN_LocalNotification  {


	private int _Id = 0;
	private DateTime _Date;
	private string _Message = string.Empty;
	private bool _UseSound = true;
	private int _Badges = 0;
	private string _Data = string.Empty;

	private string _SoundName = "";
	private const string DATA_SPLITTER = "|||";


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public ISN_LocalNotification(DateTime time, string message, bool useSound = true) {


		_Id = SA.Common.Util.IdFactory.NextId;
		_Date = time;
		_Message = message;
		_UseSound = useSound;

	}

	public ISN_LocalNotification(string serializaedNotificationData) {

		try {
			string[] nodes = serializaedNotificationData.Split(new string[] { DATA_SPLITTER }, StringSplitOptions.None);
			
			
			_Id = System.Convert.ToInt32(nodes[0]);
			_UseSound = System.Convert.ToBoolean(nodes[1]);
			_Badges =  System.Convert.ToInt32(nodes[2]);
			_Data = nodes[3];
			_SoundName = nodes[4];
			_Date = new DateTime(System.Convert.ToInt64(nodes[5]));
		} catch(Exception ex) {
			ISN_Logger.Log("Failed to deserialize the ISN_LocalNotification object");
			ISN_Logger.Log(ex.Message);
		}

	}


	//--------------------------------------
	// Internal use only
	//--------------------------------------

	public void SetId(int id) {
		_Id = id;
	}

	//--------------------------------------
	// Public Methods
	//--------------------------------------

	public void SetData(string data) {
		_Data = data;
	}

	public void SetSoundName(string soundName) {
		_SoundName = soundName;
	}


	public void SetBadgesNumber(int badges) {
		_Badges = badges;
	}

	public void Schedule() {
		ISN_LocalNotificationsController.Instance.ScheduleNotification(this);
	}


	//--------------------------------------
	// Get / Set
	//--------------------------------------


	public int Id {
		get {
			return _Id;
		}
	}

	public DateTime Date {
		get {
			return _Date;
		}
	}

	public bool IsFired {
		get {
			if(System.DateTime.Now.Ticks > Date.Ticks) {
				return true;
			} else {
				return false;
			}
		}
	}

	public string Message {
		get {
			return _Message;
		}
	}

	public bool UseSound {
		get {
			return _UseSound;
		}
	}

	public int Badges {
		get {
			return _Badges;
		}
	}

	public string Data {
		get {
			return _Data;
		}
	}

	public string SoundName {
		get {
			return _SoundName;
		}
	}

	public string SerializedString {
		get {
			return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes( 
			                                                                        Id.ToString() + DATA_SPLITTER + 
			                                                                        UseSound.ToString() + DATA_SPLITTER + 
			                                                                        Badges.ToString() + DATA_SPLITTER + 
			                                                                        Data + DATA_SPLITTER + 
			                                                                        SoundName + DATA_SPLITTER + 
			                                                                        Date.Ticks.ToString() 			                                                                     
			                                                                        ));
		}
	}
}
