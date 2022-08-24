////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System;
using System.Collections;

namespace SA.Common.Models {
	
	public class ScreenshotMaker : MonoBehaviour {

		//Actions
		public Action<Texture2D> OnScreenshotReady = delegate {};


		public static ScreenshotMaker Create() {
			return  new GameObject("ScreenshotMaker").AddComponent<ScreenshotMaker>();
		}

		void Awake() {
			DontDestroyOnLoad(gameObject);
		}

		public void GetScreenshot() {
			StartCoroutine(SaveScreenshot());
		}

		private IEnumerator SaveScreenshot() {
			
			yield return new WaitForEndOfFrame();
			// Create a texture the size of the screen, RGB24 format
			int width = Screen.width;
			int height = Screen.height;
			Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
			// Read screen contents into the texture
			tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
			tex.Apply();

			if(OnScreenshotReady != null) {
				OnScreenshotReady(tex);
			}

			Destroy(gameObject);

		}
		
	}

}
