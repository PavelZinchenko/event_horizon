using System;
using System.IO;
using UnityEngine;

namespace Utils
{
    public static class ScreenshotUtils
    {
        public static Texture2D TakeScreenshot()
        {
            var screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            return screenshot;
        }

        public static void MakeTextureFullyOpaque(Texture2D texture)
        {
            var pixels = texture.GetPixels32();

            for (var i = 0; i < pixels.Length; i++)
            {
                var color32 = pixels[i];
                color32.a = 255;
                pixels[i] = color32;
            }

            texture.SetPixels32(pixels);
        }
    }
}
