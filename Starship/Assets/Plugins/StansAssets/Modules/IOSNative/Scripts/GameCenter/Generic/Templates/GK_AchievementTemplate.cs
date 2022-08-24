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

[System.Serializable]
public class GK_AchievementTemplate  {

	//Editor Use Only
	public bool IsOpen = true;

	
	public string Id = string.Empty;
	public string Title = "New Achievement";
	public string Description = string.Empty;
	public float _progress = 0f;

	public Texture2D Texture;



	public float Progress {
		get {
			if(IOSNativeSettings.Instance.UsePPForAchievements) {
				return GameCenterManager.GetAchievementProgress(Id);
			} else {
				return _progress;
			}

		}

		set {
			_progress = value;
		}
	}
}
