////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.IOSNative.UserNotifications {

	public class TimeIntervalTrigger : NotificationTrigger {

		public int intervalToFire;

		public TimeIntervalTrigger(int secondsInterval) {
			intervalToFire = secondsInterval;
		}

		public override string ToString() {
			var dict = new Dictionary<string, object> ();
			dict.Add ("intervalToFire", this.intervalToFire);
			dict.Add ("repeats", this.repeated);

			return SA.Common.Data.Json.Serialize (dict);
		}


	}


}