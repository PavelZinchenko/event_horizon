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

	public class DateComponents {

		public int? Year;
		public int? Month;
		public int? Day;
		public int? Hour;
		public int? Minute;
		public int? Second;
		public int? Weekday;
		public int? Quarter;
	}

	public class CalendarTrigger : NotificationTrigger {

		DateComponents ComponentsOfDateToFire;

		public CalendarTrigger(DateComponents dateComponents) {
			ComponentsOfDateToFire = dateComponents;
		}

		public override string ToString() {
			var dict = new Dictionary<string, object> ();

			if (ComponentsOfDateToFire.Year != null) {
				dict.Add ("year", ComponentsOfDateToFire.Year);
			}
			if (ComponentsOfDateToFire.Month != null) {
				dict.Add ("month", ComponentsOfDateToFire.Month);
			}
			if (ComponentsOfDateToFire.Day != null) {
				dict.Add ("day", ComponentsOfDateToFire.Day);
			}
			if (ComponentsOfDateToFire.Hour != null) {
				dict.Add ("hour", ComponentsOfDateToFire.Hour);
			}
			if (ComponentsOfDateToFire.Minute != null) {
				dict.Add ("minute", ComponentsOfDateToFire.Minute);
			}
			if (ComponentsOfDateToFire.Second != null) {
				dict.Add ("second", ComponentsOfDateToFire.Second);
			}
			if (ComponentsOfDateToFire.Weekday != null) {
				dict.Add ("weekday", ComponentsOfDateToFire.Weekday);
			}
			if (ComponentsOfDateToFire.Quarter != null) {
				dict.Add ("quarter", ComponentsOfDateToFire.Quarter);
			}

			dict.Add ("repeats", this.repeated);

			return SA.Common.Data.Json.Serialize (dict);
		}

	}

}
