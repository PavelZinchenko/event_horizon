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

public class GK_RTM_QueryActivityResult : SA.Common.Models.Result {

	private int _Activity = 0;
	
	public GK_RTM_QueryActivityResult(int activity):base() {
		_Activity = activity;
	}
	
	public GK_RTM_QueryActivityResult(string errorData):base(new SA.Common.Models.Error(errorData)) {
	}
	
	
	public int Activity {
		get {
			return _Activity;
		}
	}
}
