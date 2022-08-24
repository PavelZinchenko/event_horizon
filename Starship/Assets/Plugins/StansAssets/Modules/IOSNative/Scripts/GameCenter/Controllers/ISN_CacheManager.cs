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

public class ISN_CacheManager : MonoBehaviour {

	private const string DATA_SPLITTER = "|";
	private const string ACHIEVEMENT_SPLITTER = "&";

	private const string GA_DATA_CACHE_KEY = "ISN_Cache";
	
	public static void SaveAchievementRequest(string achievementId, float percent) {

		if (!IOSNativeSettings.Instance.UseGCRequestCaching) {
			return;
		}

		string data = SavedData;
		string achievementData = achievementId + ACHIEVEMENT_SPLITTER + percent.ToString();


		if(data != string.Empty) {
			data = data + DATA_SPLITTER + achievementData;
		} else {
			data = achievementData;
		}
		
		SavedData = data;
	}
	
	public static void SendAchievementCachedRequest() {
		
		string data = SavedData;
		if(data != string.Empty) {
			string[] requests = data.Split(DATA_SPLITTER [0]);
			foreach(string request in requests) {
				string[] achievementData = request.Split(ACHIEVEMENT_SPLITTER[0]);
				GameCenterManager.SubmitAchievementNoCache(System.Convert.ToSingle(achievementData[1]), achievementData[0]);
			}
			
		} 
		
		Clear();
	}
	
	
	public static void Clear() {
		PlayerPrefs.DeleteKey(GA_DATA_CACHE_KEY);
	}
	
	public static string SavedData {
		get {
			if(PlayerPrefs.HasKey(GA_DATA_CACHE_KEY)) {
				return PlayerPrefs.GetString(GA_DATA_CACHE_KEY);
			} else {
				return string.Empty;
			}
		}
		
		set {
			PlayerPrefs.SetString(GA_DATA_CACHE_KEY, value);
		}
	}

}
