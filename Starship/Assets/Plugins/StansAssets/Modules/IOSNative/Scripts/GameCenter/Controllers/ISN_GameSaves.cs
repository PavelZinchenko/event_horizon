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


public class ISN_GameSaves : SA.Common.Pattern.Singleton<ISN_GameSaves> {


	#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _ISN_SaveGame(string data, string name);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_FetchSavedGames();
	
	[DllImport ("__Internal")]
	private static extern void _ISN_DeleteSavedGame(string name);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_ResolveConflictingSavedGames(string saves, string data);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_LoadSaveData(string name);

	#endif

	public static event Action<GK_SaveRemoveResult> ActionSaveRemoved = delegate {};
	public static event Action<GK_SaveResult> ActionGameSaved = delegate {};
	public static event Action<GK_FetchResult> ActionSavesFetched = delegate {};
	public static event Action<GK_SavesResolveResult> ActionSavesResolved = delegate {};


	private static Dictionary<string, GK_SavedGame> _CachedGameSaves =  new Dictionary<string, GK_SavedGame>();

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	/// <summary>
	/// Saves game data under the specified name.
	/// 
	/// This method saves game data asynchronously. When a game is saved, if there is already a saved game with 
	/// the same name, the new saved game data overwrites the old saved game data. If there is no saved game with 
	/// the same name, a new saved game is created. 
	/// 
	///<param name="data">An object that contains the saved game data.</param>
	///<param name="name">A string that identifies the saved game data.</param>
	/// </summary>
	public void SaveGame(byte[] data, string name) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		string bytesString = System.Convert.ToBase64String (data);
		_ISN_SaveGame(bytesString, name);
		#endif

	}
	

	/// <summary>
	/// Retrieves all available saved games.
	/// 
	/// This method deletes saved game files asynchronously. 
	/// same name, a conflict occurs. The app must determine which saved game file is correct and call the
	/// ResolveConflictingSavedGames method. 
	/// 
	/// </summary>
	public void FetchSavedGames() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_FetchSavedGames();
		#endif
	}

	/// <summary>
	/// Deletes a specific saved game file.
	/// This method deletes saved game files asynchronously.
	/// 
	/// <param name="name">A string that identifies the saved game data to be deleted.</param>
	/// </summary>
	public void DeleteSavedGame(string name) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_DeleteSavedGame(name);
		#endif
	}

	/// <summary>
	/// Resolves any conflicting saved games..
	/// 
	/// This method takes an array of GK_SavedGame objects that contain conflicting saved game files and creates a 
	/// new array that contains the resolved conflicts. All saved game conflicts are resolved and added to the
	/// conflicts array in the completion handler. Call this method separately for each set of saved game conflicts. 
	/// For example, if you have multiple saved game files with the name of “savedgame1” and “savedgame2”, you 
	/// need to call this method twice—once with an array containing the GK_SavedGame objects with the “savedgame1
	/// ame and once for the “savedgame2” objects. All saved game conflicts are resolved asynchronously.
	/// 
	/// <param name="conflicts">An list of GK_SavedGame objects containing the conflicting saved games to be deleted.</param>
	/// <param name="data">An object that contains the saved game data.</param>
	/// </summary>
	public void ResolveConflictingSavedGames(List<GK_SavedGame> conflicts, byte[] data) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		string bytesString = System.Convert.ToBase64String (data);

		List<string> savesKeys =  new List<string>();
		foreach(GK_SavedGame save in conflicts) {
			savesKeys.Add(save.Id);
		}

		string savesList =  SA.Common.Data.Converter.SerializeArray(savesKeys.ToArray());

		_ISN_ResolveConflictingSavedGames(savesList, bytesString);
		#endif
	}

	/// <summary>
	/// Method for plugin internal use only.
	/// </summary>
	public void LoadSaveData(GK_SavedGame save) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_LoadSaveData(save.Id);
		#endif
	}

	// --------------------------------------
	// Get / Set
	// --------------------------------------

	// --------------------------------------
	// Native Events
	// --------------------------------------

	public void OnSaveSuccess(string data) {

		GK_SavedGame save = DeserializeGameSave(data);
		GK_SaveResult result = new GK_SaveResult(save);

		ActionGameSaved(result);
	}

	public void OnSaveFailed(string erroData) {
		GK_SaveResult result = new GK_SaveResult(erroData);
		ActionGameSaved(result);
	}

	public void OnFetchSuccess(string data) {
		List<GK_SavedGame> gamesList = new List<GK_SavedGame>();

		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);

		for (int i = 0; i < DataArray.Length; i++) {
			if (DataArray[i] == SA.Common.Data.Converter.DATA_EOF) {
				break;
			}
			
			GK_SavedGame gameSave = DeserializeGameSave(DataArray[i]);
			gamesList.Add(gameSave);
		}
		GK_FetchResult result = new GK_FetchResult(gamesList);
		ActionSavesFetched(result);
	}

	public void OnFetchFailed(string errorData) {
		GK_FetchResult result = new GK_FetchResult(errorData);
		ActionSavesFetched(result);
	}

	public void OnResolveSuccess(string data) {
		List<GK_SavedGame> gamesList = new List<GK_SavedGame>();
		
		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);
		
		for (int i = 0; i < DataArray.Length; i++) {
			if (DataArray[i] == SA.Common.Data.Converter.DATA_EOF) {
				break;
			}
			
			GK_SavedGame gameSave = DeserializeGameSave(DataArray[i]);
			gamesList.Add(gameSave);
		}

		GK_SavesResolveResult result = new GK_SavesResolveResult(gamesList);
		ActionSavesResolved(result);
	}
	
	public void OnResolveFailed(string errorData) {
		GK_SavesResolveResult result = new GK_SavesResolveResult(errorData);
		ActionSavesResolved(result);
	}

	public void OnDeleteSuccess(string name) {
		GK_SaveRemoveResult result =  new GK_SaveRemoveResult(name);
		ActionSaveRemoved(result);
	}

	public void OnDeleteFailed(string data) {
		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);

		string name = DataArray[0];
		string errorData = DataArray[1];

		GK_SaveRemoveResult result =  new GK_SaveRemoveResult(name, errorData);
		ActionSaveRemoved(result);
	}

	private void OnSaveDataLoaded(string data) {
		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);
		
		string key = DataArray[0];
		string base64Data = DataArray[1];

		if(_CachedGameSaves.ContainsKey(key)) {
			_CachedGameSaves[key].GenerateDataLoadEvent(base64Data);
		}
	}

	private void OnSaveDataLoadFailed(string data) {
		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);
		
		string key = DataArray[0];
		string errorData = DataArray[1];
		

		if(_CachedGameSaves.ContainsKey(key)) {
			_CachedGameSaves[key].GenerateDataLoadFailedEvent(errorData);
		}
	}

	// --------------------------------------
	// Private Methods
	// --------------------------------------

	private GK_SavedGame DeserializeGameSave(string serializedData) {
		string[] DataArray = serializedData.Split(SA.Common.Data.Converter.DATA_SPLITTER); 
		
		string Id = DataArray[0];
		string Name = DataArray[1];
		string DeviceName = DataArray[2];
		string ModificationDateString  = DataArray[3];

		GK_SavedGame save =  new GK_SavedGame(Id, Name, DeviceName, ModificationDateString);

		if(_CachedGameSaves.ContainsKey(save.Id)) {
			_CachedGameSaves[save.Id] = save;
		} else {
			_CachedGameSaves.Add(save.Id, save);
		}

		return save;
	}
}
