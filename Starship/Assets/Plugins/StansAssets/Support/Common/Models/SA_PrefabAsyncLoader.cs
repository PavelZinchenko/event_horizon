////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using System;


namespace SA.Common.Models {

	public class PrefabAsyncLoader : MonoBehaviour {
		
		private string PrefabPath;
		public event Action<GameObject> ObjectLoadedAction = delegate {};
			 

		public static PrefabAsyncLoader Create() {
		
			PrefabAsyncLoader loader =  new GameObject("PrefabAsyncLoader").AddComponent<PrefabAsyncLoader>();

			return loader;
		}

		void Awake() {
			DontDestroyOnLoad(gameObject);
		}


		public void LoadAsync(string name) {
			PrefabPath = name;
			StartCoroutine(Load());
		}



		private IEnumerator Load() {
			ResourceRequest request = Resources.LoadAsync(PrefabPath);

			yield return request;

			if(request.asset == null) {
				Debug.LogWarning("Prefab not found at path: "  + PrefabPath);
				ObjectLoadedAction(null);
			} else {
				GameObject loadedObject =   UnityEngine.Object.Instantiate (request.asset) as GameObject;
				ObjectLoadedAction(loadedObject);
			}


			Destroy(gameObject);

		}

	}

}
