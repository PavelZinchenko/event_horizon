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

	public class NotificationContent  {

		public NotificationContent() {
		}

		public NotificationContent(Dictionary<string, object> contentDictionary) {
			Title = (string)contentDictionary ["title"];
			Subtitle = (string)contentDictionary ["subtitle"];
			Body = (string)contentDictionary ["body"];
            Sound = (string)contentDictionary["sound"];
			LaunchImageName = (string)contentDictionary ["launchImageName"];
			Badge = int.Parse(contentDictionary ["badge"].ToString());
			UserInfo = (Dictionary<string, object>) SA.Common.Data.Json.Deserialize (contentDictionary ["userInfo"].ToString());
		}
		/// <summary>
		/// A short description of the reason for the alert.
		/// </summary>
		public string Title = string.Empty;

		/// <summary>
		/// A secondary description of the reason for the alert.
		/// </summary>
		public string Subtitle = string.Empty;

		/// <summary>
		/// The message displayed in the notification alert.
		/// </summary>
		public string Body = string.Empty;

        /// <summary>
        /// The notification soound name.
        /// </summary>
        public string Sound = string.Empty;


		/// <summary>
		/// The number to apply to the app’s icon.
		/// </summary>
		public int Badge = 0;

		/// <summary>
		/// The name of the launch image to display when your app is launched in response to the notification
		/// </summary>
		public string LaunchImageName = string.Empty;


		/// <summary>
		/// A dictionary of custom information associated with the notification.
		/// </summary>
		public Dictionary<string, object> UserInfo =  new Dictionary<string, object>();

		public override string ToString() {
			string userInfoString = SA.Common.Data.Json.Serialize (UserInfo);
			return "{" + string.Format ("\"title\" : \"{0}\", " +
                                        "\"subtitle\" : \"{1}\", " +
                                        "\"body\" : \"{2}\", " +
                                        "\"sound\" : \"{3}\", " +
                                        "\"badge\" : {4}, " +
                                        "\"launchImageName\" : \"{5}\", " +
                                        "\"userInfo\" : {6}", 

                                        Title, Subtitle, Body, Sound, Badge, LaunchImageName, userInfoString) + "}";
		}


	}

}
