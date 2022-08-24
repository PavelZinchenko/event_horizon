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

public class GK_SavedGame  {

	public event Action<GK_SaveDataLoaded> ActionDataLoaded = delegate {};


	private string _Id;
	private string _Name;
	private string _DeviceName;
	private DateTime _ModificationDate;
	private string _OriginalDateString;

	private byte[] _Data = null;
	private bool _IsDataLoaded = false;




	public GK_SavedGame(string id, string name, string device, string dateString) {
		_Id = id;
		_Name = name;
		_DeviceName = device;

		_OriginalDateString = dateString;

		_ModificationDate = DateTime.Now;
		try {
			_ModificationDate  = DateTime.Parse(dateString);
		} catch(Exception ex) {
			Debug.Log ("GK_SavedGame Date parsing issue, cnonsider to parce date byyourself using  _OriginalDateString property");
            Debug.Log(ex.Message);
		}

	}


	//--------------------------------------
	// Public Methods
	//--------------------------------------


	/// <summary>
	/// Loads previously retrieved saved game data.
	/// 
	/// his method loads the saved game data asynchronously. The ActionDataLoaded action contains 
	/// either the saved game data, or an error if no game was loaded.
	/// </summary>
	public void LoadData() {
		ISN_GameSaves.Instance.LoadSaveData(this);
	}



	//--------------------------------------
	// Private Methods
	//--------------------------------------

	/// <summary>
	/// Method for plugin internal use only.
	/// </summary>
	public void GenerateDataLoadEvent(string base64Data) {
		_Data =  System.Convert.FromBase64String(base64Data);
		_IsDataLoaded = true;
		GK_SaveDataLoaded result =  new GK_SaveDataLoaded(this);
		ActionDataLoaded(result);
	}


	/// <summary>
	/// Method for plugin internal use only.
	/// </summary>
	public void GenerateDataLoadFailedEvent(string erorrData) {
		var error = new SA.Common.Models.Error (erorrData);
		GK_SaveDataLoaded result =  new GK_SaveDataLoaded(error);
		ActionDataLoaded(result);
	}



	//--------------------------------------
	// Get / Set
	//--------------------------------------


	/// <summary>
	/// Object unique identifier. (read-only)
	/// </summary>
	public string Id {
		get {
			return _Id;
		}
	}

	/// <summary>
	/// The name of the saved game. (read-only)
	/// 
	/// You can allow users to name their own saved games, or you can create a saved game name automatically.
	/// </summary>
	public string Name {
		get {
			return _Name;
		}
	}

	/// <summary>
	/// The name of the device that created the saved game data. (read-only)
	/// 
	/// The device name is equal to whatever the user has named his or her device. 
	/// For example, “Bob’s iPhone”, “John’s Macbook Pro”.
	/// </summary>
	public string DeviceName {
		get {
			return _DeviceName;
		}
	}

	/// <summary>
	/// The date when the saved game file was modified. (read-only)
	/// </summary>
	public DateTime ModificationDate {
		get {
			return _ModificationDate;
		}
	}

	/// <summary>
	/// Original date string  provided by Apple (read-only)
	/// </summary>
	public string OriginalDateString {
		get {
			return _OriginalDateString;
		}
	}

	/// <summary>
	/// Data of the saved game. Can be used only after was loaded with the LoadData method 
	/// and ActionDataLoaded action was received.
	/// </summary>
	public byte[] Data {
		get {
			return _Data;
		}
	}

	/// <summary>
	/// Inidcates if save data was already loaded.
	/// </summary>
	public bool IsDataLoaded {
		get {
			return _IsDataLoaded;
		}
	}
}
