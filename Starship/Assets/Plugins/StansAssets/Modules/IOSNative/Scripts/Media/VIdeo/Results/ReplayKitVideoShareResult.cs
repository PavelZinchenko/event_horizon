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

public class ReplayKitVideoShareResult  {

	private string[] _Sources =  new string[0];


	public ReplayKitVideoShareResult(string[] sourcesArray) {
		_Sources = sourcesArray;
	}
	

	public string[] Sources {
		get {
			return _Sources;
		}
	}
}
