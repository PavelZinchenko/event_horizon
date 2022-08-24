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

	public abstract class NotificationTrigger  {

		public static NotificationTrigger triggerFromDictionary(Dictionary<string, object> triggerDictionary) {
			NotificationTrigger trigger;

			if (triggerDictionary.ContainsKey ("intervalToFire")) {
				trigger = new TimeIntervalTrigger (int.Parse( triggerDictionary ["intervalToFire"].ToString() ));
			} else {
				DateComponents dateComponents = new DateComponents ();

				if (triggerDictionary.ContainsKey ("year")) {
					dateComponents.Year = int.Parse( triggerDictionary ["year"].ToString() );
				}
				if (triggerDictionary.ContainsKey ("month")) {
					dateComponents.Month = int.Parse( triggerDictionary ["month"].ToString() );
				}
				if (triggerDictionary.ContainsKey ("day")) {
					dateComponents.Day = int.Parse( triggerDictionary ["day"].ToString() );
				}
				if (triggerDictionary.ContainsKey ("hour")) {
					dateComponents.Hour = int.Parse( triggerDictionary ["hour"].ToString() );
				}
				if (triggerDictionary.ContainsKey ("minute")) {
					dateComponents.Minute = int.Parse( triggerDictionary ["minute"].ToString() );
				}
				if (triggerDictionary.ContainsKey ("second")) {
					dateComponents.Second = int.Parse( triggerDictionary ["second"].ToString() );
				}
				if (triggerDictionary.ContainsKey ("weekday")) {
					dateComponents.Weekday = int.Parse( triggerDictionary ["weekday"].ToString() );
				}
				if (triggerDictionary.ContainsKey ("quarter")) {
					dateComponents.Quarter = int.Parse( triggerDictionary ["quarter"].ToString() );
				}

				trigger = new CalendarTrigger (dateComponents);
			}

			bool repeats = int.Parse( triggerDictionary ["repeats"].ToString() ) == 1 ? true : false;
			trigger.SetRepeat (repeats);

			return trigger;
		}

		public bool repeated = false;

		public void SetRepeat(bool repeats) {
			this.repeated = repeats;
		}

		public string Type {
			get {
				return this.GetType ().Name;
			}
		}
	}

}
