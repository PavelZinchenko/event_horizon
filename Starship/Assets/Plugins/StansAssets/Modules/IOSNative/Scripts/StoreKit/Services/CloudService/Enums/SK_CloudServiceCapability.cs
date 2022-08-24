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

public enum SK_CloudServiceCapability  {

	None                           = 0,
	MusicCatalogPlayback           = 1 << 0,
	AddToCloudMusicLibrary         = 1 << 8
}
