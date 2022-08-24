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

namespace SA.Common.Util {


	public static class Screen  {

		public static void TakeScreenshot(Action<Texture2D> callback) {
			var maker = SA.Common.Models.ScreenshotMaker.Create();
			maker.OnScreenshotReady += callback;
			maker.GetScreenshot();
		}


	}

}

