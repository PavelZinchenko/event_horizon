////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;

#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif


namespace SA.IOSNative.Storage {

	public static class AppCache  {

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	
		[DllImport ("__Internal")]
		private static extern void _ISN_CacheSave(string key, string value);


		[DllImport ("__Internal")]
		private static extern void _ISN_CacheRemove(string key);


		[DllImport ("__Internal")]
		private static extern string _ISN_CacheGet(string key);

		#endif



		public static void Save(string key,  Texture2D texture) {
			byte[] data = texture.EncodeToPNG();
			Save (key, data);
		}


		public static void Save(string key, byte[] data) {
			string bytesString = Convert.ToBase64String (data);
			Save (key, bytesString);
		}

		public static void Save (string key, string value) {

			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_CacheSave(key, value);
			#else
			PlayerPrefs.SetString(key, value);
			#endif
		}




		public static Texture2D GetTexture(string key) {
			byte[] data = GetData (key);

			Texture2D 	image = new Texture2D(1, 1);
			image.LoadImage(data);
			image.hideFlags = HideFlags.DontSave;

			return image;
		}

		public static byte[] GetData(string key) {
			string val = GetString (key);
			byte[] decodedFromBase64 = Convert.FromBase64String(val);
			return decodedFromBase64;
		}

		public static string GetString(string key) {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			return  _ISN_CacheGet(key);
			#else
			return PlayerPrefs.GetString(key);
			#endif
		}




		public static void Remove (string key) {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_CacheRemove(key);
			#else
			PlayerPrefs.DeleteKey(key);
			#endif
		}



	
	
	}





}
