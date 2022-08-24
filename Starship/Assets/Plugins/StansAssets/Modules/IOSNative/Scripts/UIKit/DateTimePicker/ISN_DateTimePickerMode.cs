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


namespace SA.IOSNative.UIKit {
	
	public enum DateTimePickerMode  {
		
		Time = 1, // Displays hour, minute, and optionally AM/PM designation depending on the locale setting (e.g. 6 | 53 | PM)
		Date = 2, // Displays month, day, and year depending on the locale setting (e.g. November | 15 | 2007)
		DateAndTime = 3, // Displays date, hour, minute, and optionally AM/PM designation depending on the locale setting (e.g. Wed Nov 15 | 6 | 53 | PM)
		CountdownTimer = 4 // Displays hour and minute (e.g. 1 | 53)
	}
}