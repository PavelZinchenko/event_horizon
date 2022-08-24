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
#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class GameCenterManager : MonoBehaviour {
	
	//Actions
	public static event Action<SA.Common.Models.Result> OnAuthFinished  = delegate{};

	public static event Action<GK_LeaderboardResult> OnScoreSubmitted = delegate{};
	public static event Action<GK_LeaderboardResult> OnScoresListLoaded = delegate{};
	public static event Action<GK_LeaderboardResult> OnLeadrboardInfoLoaded = delegate {};


	public static event Action<SA.Common.Models.Result> OnLeaderboardSetsInfoLoaded = delegate{};


	public static event Action<SA.Common.Models.Result> OnAchievementsReset = delegate{};
	public static event Action<SA.Common.Models.Result> OnAchievementsLoaded  = delegate{};
	public static event Action<GK_AchievementProgressResult> OnAchievementsProgress  = delegate{};




	public static event Action OnGameCenterViewDismissed = delegate{};
	public static event Action<SA.Common.Models.Result> OnFriendsListLoaded = delegate{};
	public static event Action<GK_UserInfoLoadResult> OnUserInfoLoaded  = delegate{};
	public static event Action<GK_PlayerSignatureResult> OnPlayerSignatureRetrieveResult = delegate{};




	#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _initGameCenter();
	
	[DllImport ("__Internal")]
	private static extern void _showLeaderboard(string leaderboardId, int timeSpan);

	[DllImport ("__Internal")]
	private static extern void _reportScore (string score, string leaderboardId, string context);


	[DllImport ("__Internal")]
	private static extern void _showLeaderboards ();


	[DllImport ("__Internal")]
	private static extern void _ISN_loadLeaderboardInfo (string leaderboardId, int requestId);

	[DllImport ("__Internal")]
	private static extern void _ISN_loadLeaderboardScore (string leaderboardId, int timeSpan, int collection, int from, int to);
	
	[DllImport ("__Internal")]
	private static extern void _showAchievements();

	[DllImport ("__Internal")]
	private static extern void _resetAchievements();
	

	[DllImport ("__Internal")]
	private static extern void _submitAchievement(float percent, string achievementId, bool isCompleteNotification);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_issueLeaderboardChallenge(string leaderboardId, string message, string playerIds);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueLeaderboardChallengeWithFriendsPicker(string leaderboardId, string message);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueAchievementChallenge(string leaderboardId, string message, string playerIds);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueAchievementChallengeWithFriendsPicker(string leaderboardId, string message);

	[DllImport ("__Internal")]
	private static extern void _ISN_RetrieveFriends();

	[DllImport ("__Internal")]
	private static extern void _ISN_loadGKPlayerData(string playerId);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_loadGKPlayerPhoto(string playerId, int size);

	[DllImport ("__Internal")]
	private static extern void _ISN_getSignature();

	[DllImport ("__Internal")]
	private static extern void _ISN_loadLeaderboardSetInfo();
	
	[DllImport ("__Internal")]
	private static extern void _ISN_loadLeaderboardsForSet(string setId);
	
	[DllImport ("__Internal")]
	private static extern  void _ISN_ShowNotificationBanner (string title, string message);
	
	[DllImport ("__Internal")]
	private static extern  void _ISN_LoadAchievements ();

	[DllImport ("__Internal")]
	private static extern bool _ISN_GK_IsUnderage();

	[DllImport ("__Internal")]
	private static extern bool _ISN_GK_IsAuthenticated();



	[DllImport ("__Internal")]
	private static extern void _ISN_GK_SendFriendRequest(int id, string emails, string players);



	#endif


	private  static bool _IsInitialized = false;
	private  static bool _IsAchievementsInfoLoaded = false;
	

	private static Dictionary<string, GK_Player> _players =  new Dictionary<string, GK_Player>();
	private static List<string> _friendsList = new List<string>();


	private static List<GK_LeaderboardSet> _LeaderboardSets = new List<GK_LeaderboardSet>();
	private static Dictionary<int, GK_FriendRequest> _FriendRequests = new Dictionary<int, GK_FriendRequest>();


	private static GK_Player _player = null;


	private const string ISN_GC_PP_KEY = "ISN_GameCenterManager";

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	

	[System.Obsolete("init is deprecated, please use Init instead.")]
	public static void init() {
		Init();
	}

	public static void Init() {
		
		if(_IsInitialized) {
			return;
		}
		
		_IsInitialized = true;
		
		GameCenterInvitations.Instance.Init();
		
		GameObject go =  new GameObject("GameCenterManager");
		go.AddComponent<GameCenterManager>();
		DontDestroyOnLoad(go);


		foreach(GK_Leaderboard leaderboard in Leaderboards) {
			leaderboard.Refresh();
		}
		
		
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_initGameCenter();
		#endif
		
	}


	public static void RetrievePlayerSignature() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_getSignature();
		#endif
	}


	public static void ShowGmaeKitNotification (string title, string message) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_ShowNotificationBanner (title, message);
		#endif
	}



	public static void RegisterAchievement(string achievementId) {

		GK_AchievementTemplate tpl = new GK_AchievementTemplate ();
		tpl.Id = achievementId;

		RegisterAchievement(tpl);
	}

	public static void RegisterAchievement(GK_AchievementTemplate achievement) {
		bool isContains = false;
	
		int replaceIndex = 0;
		foreach(GK_AchievementTemplate tpl in Achievements) {
			if(tpl.Id.Equals(achievement.Id)) {
				isContains = true;
				replaceIndex = Achievements.IndexOf(tpl);
				break;
			}
		}
		
		if(isContains) {
			Achievements[replaceIndex] = achievement;
		} else {
			Achievements.Add(achievement);
		}
	}




	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	

	public static void ShowLeaderboard(string leaderboardId) {
		ShowLeaderboard(leaderboardId, GK_TimeSpan.ALL_TIME);
	}


	public static void ShowLeaderboard(string leaderboardId, GK_TimeSpan timeSpan) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
			_showLeaderboard(leaderboardId, (int) timeSpan);
		#endif
	}

	public static void ShowLeaderboards() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
			_showLeaderboards ();
		#endif
	}
	

	public static void ReportScore(long score, string leaderboardId, long context = 0) {
		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			ISN_Logger.Log("unity reportScore: " + leaderboardId);

		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_reportScore(score.ToString(),  leaderboardId, context.ToString());
		#endif
	}


	public static void ReportScore(double score, string leaderboardId) {
		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			ISN_Logger.Log("unity reportScore double: " + leaderboardId);
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		long s = System.Convert.ToInt64(score * 100);
		_reportScore(s.ToString(),  leaderboardId, "0");
		#endif
	}



	public static void RetrieveFriends() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_RetrieveFriends();
		#endif
	}

	[System.Obsolete("LoadUsersData is deprecated, please use LoadGKPlayerInfo instead.")]
	public static void LoadUsersData(string[] UIDs) {
		LoadGKPlayerInfo(UIDs[0]);
	}
	
	public static void LoadGKPlayerInfo(string playerId) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_loadGKPlayerData(playerId);
		#endif
	}
	
	public static void LoadGKPlayerPhoto(string playerId, GK_PhotoSize size) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_loadGKPlayerPhoto(playerId, (int) size);
		#endif
	}

	
	[System.Obsolete("LoadCurrentPlayerScore is deprecated, please use LoadLeaderboardInfo instead.")]
	public static void LoadCurrentPlayerScore(string leaderboardId, GK_TimeSpan timeSpan = GK_TimeSpan.ALL_TIME, GK_CollectionType collection = GK_CollectionType.GLOBAL)  {
		LoadLeaderboardInfo(leaderboardId);
	}




	public static void LoadLeaderboardInfo(string leaderboardId)  {



		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		int requestId = SA.Common.Util.IdFactory.NextId;
		GK_Leaderboard leaderboard = GetLeaderboard(leaderboardId);
		leaderboard.CreateScoreListener(requestId, false);

		_ISN_loadLeaderboardInfo(leaderboardId, requestId);
		#endif
	}



	private IEnumerator LoadLeaderboardInfoLocal(string leaderboardId) {
		yield return new WaitForSeconds(4f);
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		int requestId = SA.Common.Util.IdFactory.NextId;
		GK_Leaderboard leaderboard = GetLeaderboard(leaderboardId);
		leaderboard.CreateScoreListener(requestId, true);

		_ISN_loadLeaderboardInfo(leaderboardId, requestId);
		#endif
	}




	public static void LoadScore(string leaderboardId, int startIndex, int length, GK_TimeSpan timeSpan = GK_TimeSpan.ALL_TIME, GK_CollectionType collection = GK_CollectionType.GLOBAL) {


		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_loadLeaderboardScore(leaderboardId, (int) timeSpan, (int) collection, startIndex, length);
		#endif

	}


	public static void IssueLeaderboardChallenge(string leaderboardId, string message, string playerId) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_issueLeaderboardChallenge(leaderboardId, message, playerId);
		#endif
	}

	public static void IssueLeaderboardChallenge(string leaderboardId, string message, string[] playerIds) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
			string ids = "";
			int len = playerIds.Length;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					ids += ",";
				}
				
				ids += playerIds[i];
			}

		_ISN_issueLeaderboardChallenge(leaderboardId, message, ids);
		#endif
	}


	public static void IssueLeaderboardChallenge(string leaderboardId, string message) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_issueLeaderboardChallengeWithFriendsPicker(leaderboardId, message);
		#endif
	}




	public static void IssueAchievementChallenge(string achievementId, string message, string playerId) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_issueAchievementChallenge(achievementId, message, playerId);
		#endif
	}

	public static void LoadLeaderboardSetInfo() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_loadLeaderboardSetInfo();
		#endif
	}

	public static void LoadLeaderboardsForSet(string setId) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_loadLeaderboardsForSet(setId);
		#endif
	}



	public static void LoadAchievements() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_LoadAchievements();
		#endif
	}


	public static void IssueAchievementChallenge(string achievementId, string message, string[] playerIds) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
			string ids = "";
			int len = playerIds.Length;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					ids += ",";
				}
				
				ids += playerIds[i];
			}
			
		_ISN_issueAchievementChallenge(achievementId, message, ids);
		#endif
	}



	public static void IssueAchievementChallenge(string achievementId, string message) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_ISN_issueAchievementChallengeWithFriendsPicker(achievementId, message);
		#endif
	}


	public static void ShowAchievements() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
			_showAchievements();
		#endif
	}

	public static void ResetAchievements() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
			_resetAchievements();

			foreach(GK_AchievementTemplate tpl in Achievements) {
				tpl.Progress = 0f;
			}
		#endif

		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			ResetStoredProgress();
		}
	}


	public static void SubmitAchievement(float percent, string achievementId) {
		SubmitAchievement (percent, achievementId, true);
	}

	public static void SubmitAchievementNoCache(float percent, string achievementId) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
		_submitAchievement(percent, achievementId, false);
		#endif
	}

	public static void SubmitAchievement(float percent, string achievementId, bool isCompleteNotification) {

		if(Application.internetReachability == NetworkReachability.NotReachable) {
			ISN_CacheManager.SaveAchievementRequest(achievementId, percent);
		}


		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			SaveAchievementProgress(achievementId, percent);
		}


		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
			_submitAchievement(percent, achievementId, isCompleteNotification);
		#endif
	}




	public static float GetAchievementProgress(string id) {
		float progress = 0f;

		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			progress = GetStoredAchievementProgress(id);
		} else {
			GK_AchievementTemplate achievement = GetAchievement(id);
			progress = achievement.Progress;
		}

		return progress;
	}


	public static GK_AchievementTemplate GetAchievement(string achievementId) {
		foreach(GK_AchievementTemplate achievement in Achievements) {
			if(achievement.Id.Equals(achievementId)) {
				return achievement;
			}
		}

		GK_AchievementTemplate new_achievement =  new GK_AchievementTemplate();
		new_achievement.Id = achievementId;
		Achievements.Add(new_achievement);

		return new_achievement;
	}



	public static GK_Leaderboard GetLeaderboard(string id) {

		foreach(GK_Leaderboard leaderboard in Leaderboards) {
			if(leaderboard.Id.Equals(id)) {
				return leaderboard;
			}
		}

		GK_Leaderboard new_leaderboard = new GK_Leaderboard(id);
		Leaderboards.Add(new_leaderboard);

		return new_leaderboard;
	}


	public static GK_Player GetPlayerById(string playerID) {
		if(_players.ContainsKey(playerID)) {
			//ISN_Logger.Log("Returning player object for id: " + playerID);
			return _players[playerID];
		} else {
			//ISN_Logger.Log("player not found with id: " + playerID);
			return null;
		}
	}


	
	//--------------------------------------
	//  Friends Request
	//--------------------------------------

	public static void SendFriendRequest(GK_FriendRequest request, List<string> emails, List<string> players) {

		_FriendRequests.Add(request.Id, request);

		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE

		string EmailsList = SA.Common.Data.Converter.SerializeArray(emails.ToArray());
		string PlayersList = SA.Common.Data.Converter.SerializeArray(players.ToArray());

		_ISN_GK_SendFriendRequest(request.Id, EmailsList, PlayersList);
		#endif


	}
	


	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public static List<GK_AchievementTemplate> Achievements {
		get {
			return IOSNativeSettings.Instance.Achievements;
		}
	}


	public static List<GK_Leaderboard> Leaderboards {
		get {
			return IOSNativeSettings.Instance.Leaderboards;
		}
	}



	public static Dictionary<string, GK_Player> Players {
		get {
			return _players;
		}
	}
	

	public static GK_Player Player {
		get {
			return _player;
		}
	}


	public static bool IsInitialized {
		get {
			return _IsInitialized;
		}
	}

	public static List<GK_LeaderboardSet> LeaderboardSets {
		get {
			return _LeaderboardSets;
		}
	}

	public static bool IsPlayerAuthenticated {
		get {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
			return _ISN_GK_IsAuthenticated();
			#else
			return false;
			#endif
		}
	}

	public static bool IsAchievementsInfoLoaded {
		get {
			return _IsAchievementsInfoLoaded;
		}
	}

	public static List<string> FriendsList {
		get {
			return _friendsList;
		}
	}



	public static bool IsPlayerUnderage {
		get {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && GAME_CENTER_ENABLED) || SA_DEBUG_MODE
			return _ISN_GK_IsUnderage();
			#else
			return false;
			#endif
		}
	}


	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnLoaderBoardInfoRetrivedFail(string data) {
		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);

		string leaderboardId 			= DataArray[0];
//		GK_TimeSpan timeSpan 			= (GK_TimeSpan) System.Convert.ToInt32(DataArray[1]);
//		GK_CollectionType collection 	= (GK_CollectionType) System.Convert.ToInt32(DataArray[2]);
		int requestId 					= System.Convert.ToInt32(DataArray[3]);
		string errorData 				= DataArray[4];



		GK_Leaderboard board = GetLeaderboard(leaderboardId);
		board.ReportLocalPlayerScoreUpdateFail(errorData, requestId);
	}


	private void OnLoaderBoardInfoRetrived(string data) {

		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);


		string leaderboardId 			= DataArray[0];
		GK_TimeSpan timeSpan 			= (GK_TimeSpan) System.Convert.ToInt32(DataArray[1]);
		GK_CollectionType collection 	= (GK_CollectionType) System.Convert.ToInt32(DataArray[2]);
		int requestId 					= System.Convert.ToInt32(DataArray[3]);

		long scoreVal 			= System.Convert.ToInt64(DataArray[4]);
		int rank 						= System.Convert.ToInt32(DataArray[5]);
		int context 				= System.Convert.ToInt32(DataArray[6]);
		int maxRange 			=  System.Convert.ToInt32(DataArray[7]);
		string title 				= DataArray[8];
		string describtion 	= DataArray[9];





		GK_Leaderboard board = GetLeaderboard(leaderboardId);
		board.UpdateMaxRange(maxRange);
		board.Info.Title = title;
		board.Info.Description = describtion;


		GK_Score score =  new GK_Score(scoreVal, rank, context, timeSpan, collection, leaderboardId, Player.Id);
		board.ReportLocalPlayerScoreUpdate(score, requestId);

	}

	
	public void onScoreSubmittedEvent(string data) {

		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);
		
		string leaderboardId = DataArray[0];
//		long submittedScore = System.Convert.ToInt64(DataArray[1]);
	
	
		StartCoroutine(LoadLeaderboardInfoLocal(leaderboardId));
	}

	

	
	public void onScoreSubmittedFailed(string data) {

		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);
		
		string leaderboardId = DataArray[0];
//		long submittedScore = System.Convert.ToInt64(DataArray[1]);
		string errorData = DataArray[2];

		GK_Leaderboard board = GetLeaderboard(leaderboardId);
		GK_LeaderboardResult result =  new GK_LeaderboardResult(board, new SA.Common.Models.Error(errorData));
		OnScoreSubmitted(result);
	}




	private void OnLeaderboardScoreListLoaded(string data) {


		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);



		string leaderboardId = DataArray[0];
		GK_TimeSpan timeSpan = (GK_TimeSpan) System.Convert.ToInt32(DataArray[1]);
		GK_CollectionType collection = (GK_CollectionType) System.Convert.ToInt32(DataArray[2]);

		GK_Leaderboard board = GetLeaderboard(leaderboardId);


		
		
		for(int i = 3; i < DataArray.Length; i+=4) {
			string playerId = DataArray[i];
			long scoreVal = System.Convert.ToInt64 (DataArray[i + 1]); 
			int rank = System.Convert.ToInt32(DataArray[i + 2]);
			int context = System.Convert.ToInt32(DataArray[i + 3]);
			GK_Score score =  new GK_Score(scoreVal, rank, context,timeSpan, collection, leaderboardId, playerId);
			board.UpdateScore(score);


			if(Player != null) {
				if(Player.Id.Equals(playerId)) {
					board.UpdateCurrentPlayerScore(score);
				}
			}
		}
		


		GK_LeaderboardResult result = new GK_LeaderboardResult (board);
		OnScoresListLoaded (result);


	}

	private void OnLeaderboardScoreListLoadFailed(string data) {

		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);

		string leaderboardId = DataArray[0];
		//GK_TimeSpan timeSpan = (GK_TimeSpan) System.Convert.ToInt32(DataArray[1]);
		//GK_CollectionType collection = (GK_CollectionType) System.Convert.ToInt32(DataArray[2]);

		string errorData = DataArray[3];

		GK_Leaderboard board = GetLeaderboard(leaderboardId);


		GK_LeaderboardResult result = new GK_LeaderboardResult (board, new SA.Common.Models.Error(errorData));
		OnScoresListLoaded (result);

	}



	private void onAchievementsReset(string array) {

		SA.Common.Models.Result result = new SA.Common.Models.Result ();
		OnAchievementsReset (result);

	}

	private void onAchievementsResetFailed(string errorData) {
		SA.Common.Models.Result result = new SA.Common.Models.Result (new SA.Common.Models.Error(errorData));
		OnAchievementsReset (result);
	}


	private void onAchievementProgressChanged(string array) {
		string[] data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER);



		GK_AchievementTemplate tpl =  GetAchievement(data[0]);
		tpl.Progress = System.Convert.ToSingle(data [1]) ;

		GK_AchievementProgressResult result = new GK_AchievementProgressResult(tpl);
		SaveLocalProgress (tpl);

		OnAchievementsProgress(result);
	}


	private void onAchievementsLoaded(string array) {

		SA.Common.Models.Result result = new SA.Common.Models.Result ();
		if(array.Equals(string.Empty)) {
			OnAchievementsLoaded (result);
			return;
		}

		string[] data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER);


		for(int i = 0; i < data.Length; i+=3) {
			GK_AchievementTemplate tpl =  GetAchievement(data[i]);
			tpl.Description 	= data[i + 1];
			tpl.Progress 		= System.Convert.ToSingle(data[i + 2]);
			SaveLocalProgress (tpl);
		}

		_IsAchievementsInfoLoaded = true;
		OnAchievementsLoaded (result);
	}


	private void onAchievementsLoadedFailed(string errorData) {
		SA.Common.Models.Result result = new SA.Common.Models.Result (new SA.Common.Models.Error(errorData));
		OnAchievementsLoaded (result);
	}


	private void onAuthenticateLocalPlayer(string  array) {
		string[] data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		_player = new GK_Player (data[0], data [1], data [2]);


		ISN_CacheManager.SendAchievementCachedRequest();

		SA.Common.Models.Result result;
		if(IsPlayerAuthenticated) {
			result = new SA.Common.Models.Result ();
		} else {
			result = new SA.Common.Models.Result (new SA.Common.Models.Error ());
		}

		 
		OnAuthFinished (result);
	}
	
	
	private void onAuthenticationFailed(string  errorData) {

		SA.Common.Models.Result result = new SA.Common.Models.Result(new SA.Common.Models.Error(errorData));
		OnAuthFinished (result);
	}


	private void OnUserInfoLoadedEvent(string array) {
		ISN_Logger.Log("OnUserInfoLoadedEvent");

		string[] data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		string playerId = data[0];
		string alias = data[1];
		string displayName = data[2];


		GK_Player p =  new GK_Player(playerId, displayName, alias);


		if(_players.ContainsKey(playerId)) {
			_players[playerId] = p;
		} else {
			_players.Add(playerId, p);
		}

		if(p.Id == _player.Id) {
			_player = p;
		}

		ISN_Logger.Log("Player Info loaded, for player with id: " + p.Id);

		GK_UserInfoLoadResult result = new GK_UserInfoLoadResult (p);
		OnUserInfoLoaded (result);

	}    
	
	private void OnUserInfoLoadFailedEvent(string playerId) {
		
		GK_UserInfoLoadResult result = new GK_UserInfoLoadResult (playerId);
		OnUserInfoLoaded (result);
	}


	private void OnUserPhotoLoadedEvent(string array) {
		string[] data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER);
		
		string playerId = data[0];
		GK_PhotoSize size = (GK_PhotoSize) System.Convert.ToInt32(data[1]);
		string encodedImage = data[2];

		GK_Player player = GetPlayerById(playerId);
		if(player != null) {
			player.SetPhotoData(size, encodedImage);
		}

	}

	private void OnUserPhotoLoadFailedEvent(string data) {
		string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);

		string playerId = DataArray[0];
		GK_PhotoSize size = (GK_PhotoSize) System.Convert.ToInt32(DataArray[1]);
		string errorData = DataArray[2];

		GK_Player player = GetPlayerById(playerId);
		if(player != null) {
			player.SetPhotoLoadFailedEventData(size, errorData);
		}
		
	}


	private void OnFriendListLoadedEvent(string data) {


		string[] fl;
		fl = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		_friendsList.Clear ();

		for(int i = 0; i < fl.Length; i++) {
			_friendsList.Add(fl[i]);
		}

		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			ISN_Logger.Log("Friends list loaded, total friends: " + _friendsList.Count);


		SA.Common.Models.Result result = new SA.Common.Models.Result ();
		OnFriendsListLoaded (result);

	}

	private void OnFriendListLoadFailEvent(string errorData) {
		SA.Common.Models.Result result = new SA.Common.Models.Result (new SA.Common.Models.Error(errorData));
		OnFriendsListLoaded (result);
	}


	private void OnGameCenterViewDismissedEvent(string data) {
		OnGameCenterViewDismissed();
	}



	private void VerificationSignatureRetrieveFailed(string array) {

		SA.Common.Models.Error error =  new SA.Common.Models.Error(array);
	

		GK_PlayerSignatureResult res =  new GK_PlayerSignatureResult(error);
		OnPlayerSignatureRetrieveResult(res);

	}

	private void VerificationSignatureRetrieved(string array) {
		string[] data;
		data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		GK_PlayerSignatureResult res =  new GK_PlayerSignatureResult(data[0], data[1], data[2], data[3]);
		OnPlayerSignatureRetrieveResult(res);
	}



	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private void SaveLocalProgress(GK_AchievementTemplate tpl) {
		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			SaveAchievementProgress(tpl.Id, tpl.Progress);
		}
	}


	private static void ResetStoredProgress() {
		foreach(GK_AchievementTemplate t in Achievements) {
			PlayerPrefs.DeleteKey(ISN_GC_PP_KEY + t.Id);
		}
	}

	private static void SaveAchievementProgress(string achievementId, float progress) {

		float currentProgress =  GetStoredAchievementProgress(achievementId);
		if(progress > currentProgress) {
			PlayerPrefs.SetFloat(ISN_GC_PP_KEY + achievementId, Mathf.Clamp(progress, 0f, 100f));
		}
	}

	private static float GetStoredAchievementProgress(string achievementId) {
		float v = 0f;
		if(PlayerPrefs.HasKey(ISN_GC_PP_KEY + achievementId)) {
			v = PlayerPrefs.GetFloat(ISN_GC_PP_KEY + achievementId);
		} 

		return v;
	}

	private void ISN_OnLBSetsLoaded(string array) {

		string[] data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		for(int i = 0; i+2  < data.Length; i+=3) {
			GK_LeaderboardSet lbSet =  new GK_LeaderboardSet();
			lbSet.Title = data[i];
			lbSet.Identifier = data[i + 1];
			lbSet.GroupIdentifier = data[i + 2];
			LeaderboardSets.Add(lbSet);
		}


		SA.Common.Models.Result res =  new SA.Common.Models.Result();
		OnLeaderboardSetsInfoLoaded(res);
	}

	private void ISN_OnLBSetsLoadFailed(string array) {
		SA.Common.Models.Result res =  new SA.Common.Models.Result(new SA.Common.Models.Error());
		OnLeaderboardSetsInfoLoaded(res);
	}


	private void ISN_OnLBSetsBoardsLoadFailed(string identifier) {
		foreach(GK_LeaderboardSet lb in LeaderboardSets) {
			if(lb.Identifier.Equals(identifier)) {
				lb.SendFailLoadEvent();
				return;
			}
		}
	}


	private void ISN_OnLBSetsBoardsLoaded(string array) {


		string[] data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER);

		string identifier = data[0];

		foreach(GK_LeaderboardSet lb in LeaderboardSets) {
			if(lb.Identifier.Equals(identifier)) {

				for(int i = 1; i < data.Length; i+=3) {
					GK_LeaderBoardInfo info =  new GK_LeaderBoardInfo();
					info.Title = data[i];
					info.Description = data[i + 1];
					info.Identifier = data[i + 2];
					lb.AddBoardInfo(info);
				}

				lb.SendSuccessLoadEvent();

				return;
			}
		}
		

	}

	
	//--------------------------------------
	// Internal
	//--------------------------------------

	public static void DispatchLeaderboardUpdateEvent(GK_LeaderboardResult result, bool isInternal) {
		if(isInternal) {
			OnScoreSubmitted(result);
		} else {
			OnLeadrboardInfoLoaded(result);
		}
	}

	//--------------------------------------
	// UTILS
	//--------------------------------------

	public static List<GK_TBM_Participant>  ParseParticipantsData(string[] data, int index ) {
		
		List<GK_TBM_Participant> Participants =  new List<GK_TBM_Participant>();
		
		for(int i = index; i < data.Length; i += 5) {
			if(data[i] == SA.Common.Data.Converter.DATA_EOF) {
				break;
			}
			
			GK_TBM_Participant p = ParseParticipanData(data, i);
			Participants.Add(p);
			
		}
		
		return Participants;
	}


	public static GK_TBM_Participant ParseParticipanData(string[] data, int index ) {
		GK_TBM_Participant participant =  new GK_TBM_Participant(data[index], data[index + 1], data[index + 2], data[index + 3], data[index + 4]);
		return participant;
	}


	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
