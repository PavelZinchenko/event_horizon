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

public class GK_AchievementProgressResult : SA.Common.Models.Result {


	private GK_AchievementTemplate _tpl;

	public GK_AchievementProgressResult(GK_AchievementTemplate tpl):base() {
		_tpl = tpl;
	}


	public GK_AchievementTemplate info {
		get {
			return _tpl;
		}
	}

	public GK_AchievementTemplate Achievement {
		get {
			return _tpl;
		}
	}
}
