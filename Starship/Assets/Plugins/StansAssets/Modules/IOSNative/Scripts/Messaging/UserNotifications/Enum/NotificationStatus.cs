////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.IOSNative.UserNotifications {
	
	public enum NotificationStatus  {

		// The application does not support this notification type
		NotSupported  = 0,

		// The notification setting is turned off.
		Disabled,

		// The notification setting is turned on.
		Enabled
	}
}