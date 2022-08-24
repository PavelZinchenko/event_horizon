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

[System.Serializable]
public class GK_LeaderBoardInfo  {
	public string Title = "New Leaderboard";
	public string Description = string.Empty;
	public string Identifier = string.Empty;

	public Texture2D Texture;

	public int MaxRange = 0;
}
