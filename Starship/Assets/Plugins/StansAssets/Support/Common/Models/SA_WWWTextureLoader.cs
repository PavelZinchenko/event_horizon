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
using System.Collections.Generic;
using System.Collections;


namespace SA.Common.Models {

	public class WWWTextureLoader : MonoBehaviour {


		public static Dictionary<string, Texture2D> LocalCache =  new Dictionary<string, Texture2D>();


		private string _url;

		public event Action<Texture2D> OnLoad = delegate{}; 

		public static WWWTextureLoader Create() {
			return new GameObject("WWWTextureLoader").AddComponent<WWWTextureLoader>();
		}

		void Awake() {
			DontDestroyOnLoad(gameObject);
		}

		public void LoadTexture(string url) {
			_url = url;
			if(LocalCache.ContainsKey(_url)) {
				OnLoad(LocalCache[_url]);
				Destroy(gameObject);
				return;
			}
			StartCoroutine(LoadCoroutin());
		}


		private IEnumerator LoadCoroutin () {
			// Start a download of the given URL
			WWW www = new WWW (_url);

			// Wait for download to complete
			yield return www;

			if(www.error == null) {
				UpdateLocalCache(_url, www.texture);
				OnLoad(www.texture);

			} else {
				OnLoad(null);
			}

			Destroy(gameObject);

		}

		private static void UpdateLocalCache(string url, Texture2D image) {
			if(!LocalCache.ContainsKey(url)) {
				LocalCache.Add(url, image);
			}

		}

	}

}
