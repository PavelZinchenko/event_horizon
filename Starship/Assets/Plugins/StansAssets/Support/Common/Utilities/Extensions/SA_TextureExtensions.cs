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


namespace SA.Common.Extensions {

	public static class SA_TextureExtensions  {
		public static Sprite ToSprite(this Texture texture) {
			return Sprite.Create(texture as Texture2D, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f)); 
		}
	}

}
