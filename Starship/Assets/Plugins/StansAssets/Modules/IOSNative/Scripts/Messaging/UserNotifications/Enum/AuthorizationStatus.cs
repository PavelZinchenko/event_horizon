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

	public enum AuthorizationStatus  {

		// The user has not yet made a choice regarding whether the application may post user notifications.
		NotDetermined = 0,

		// The application is not authorized to post user notifications.
		Denied,

		// The application is authorized to post user notifications.
		Authorized
	}
}