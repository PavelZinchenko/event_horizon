////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using System;
using UnityEngine;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;

namespace SA.Common.Util
{

    public static class General
    {



        //--------------------------------------
        // Delays
        //--------------------------------------


        public static void Invoke(float time, Action callback, string name = "") {

            var i = SA.Common.Models.Invoker.Create(name);
            i.StartInvoke(callback, time);
        }


        //--------------------------------------
        // Parsing
        //--------------------------------------

        public static T ParseEnum<T>(string value) {
            try {
                T val = (T)Enum.Parse(typeof(T), value, true);
                return val;
            } catch (Exception ex) {
                Debug.LogWarning("Enum Parsing failed: " + ex.Message);
                return default(T);
            }
        }



        //--------------------------------------
        // Time
        //--------------------------------------


        /// <summary>
        /// Current UTC timestamp format
        /// </summary>
        public static Int32 CurrentTimeStamp {
            get {
                return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }


        private static string[] _rfc3339Formats = new string[0];
        private const string Rfc3339Format = "yyyy-MM-dd'T'HH:mm:ssK";
        private const string MinRfc339Value = "0001-01-01T00:00:00Z";


        /// <summary>
        /// Converts DateTime to Rfc3339 formated string
        /// </summary>
        public static string DateTimeToRfc3339(DateTime dateTime) {
            if (dateTime == DateTime.MinValue) {
                return MinRfc339Value;
            } else {
                return dateTime.ToString(Rfc3339Format, DateTimeFormatInfo.InvariantInfo);
            }
        }


        public static DateTime ConvertFromUnixTimestamp(long timestamp) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static long ConvertToUnixTimestamp(DateTime date) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (long)diff.TotalSeconds;
        }

        public static string[] DateTimePatterns {
            get {
                if (_rfc3339Formats.Length > 0) {
                    return _rfc3339Formats;
                }
                _rfc3339Formats = new string[11];

                // Rfc3339DateTimePatterns
                _rfc3339Formats[0] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK";
                _rfc3339Formats[1] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffffK";
                _rfc3339Formats[2] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffK";
                _rfc3339Formats[3] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffK";
                _rfc3339Formats[4] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";
                _rfc3339Formats[5] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffK";
                _rfc3339Formats[6] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fK";
                _rfc3339Formats[7] = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

                // Fall back patterns
                _rfc3339Formats[8] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK"; // RoundtripDateTimePattern
                _rfc3339Formats[9] = DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern;
                _rfc3339Formats[10] = DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern;

                return _rfc3339Formats;
            }
        }


        public static bool TryParseRfc3339(string s, out DateTime result) {

            bool wasConverted = false;
            result = DateTime.Now;

            if (!string.IsNullOrEmpty(s)) {
                DateTime parseResult;
                if (DateTime.TryParseExact(s, DateTimePatterns, DateTimeFormatInfo.InvariantInfo,
                    DateTimeStyles.AdjustToUniversal, out parseResult)) {
                    result = DateTime.SpecifyKind(parseResult, DateTimeKind.Utc);
                    result = result.ToLocalTime();
                    wasConverted = true;
                }
            }
            return wasConverted;
        }



        //--------------------------------------
        // Sequrity
        //--------------------------------------


        /// <summary>
        /// HMAC SHA256 hex key 
        /// </summary>
        public static string HMAC(string key, string data) {

#if UNITY_WSA || UNITY_WSA_10_0 || WINDOWS_UWP
            throw new System.NotSupportedException("Platform is not supported"); 
#else
            var keyByte = ASCIIEncoding.UTF8.GetBytes(key);
            using (var hmacsha256 = new HMACSHA256(keyByte)) {
                hmacsha256.ComputeHash(ASCIIEncoding.UTF8.GetBytes(data));

                byte[] buff = hmacsha256.Hash;
                string sbinary = "";

                for (int i = 0; i < buff.Length; i++)
                    sbinary += buff[i].ToString("X2"); /* hex format */
                return sbinary.ToLower();

            }
#endif
		}


		public static void CleanupInstallation() {

#if UNITY_EDITOR
			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.VERSION_INFO_PATH);

			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.SETTINGS_PATH);
			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.SETTINGS_REMOVE_PATH);


			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.ANDROID_DESTANATION_PATH);
			SA.Common.Util.Files.DeleteFolder(SA.Common.Config.IOS_DESTANATION_PATH);


			UnityEditor.AssetDatabase.Refresh ();
#endif

		}

	}

}
