using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SA.Common.Editor
{

    public static class EditorIcon 
    {


        private static Dictionary<string, Texture2D> s_icons = new Dictionary<string, Texture2D>();


        public static Texture2D GetIconAtPath(string path) {

            if (s_icons.ContainsKey(path)) {
                return s_icons[path];
            } else {


#if UNITY_2017
				TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.GUI;
                importer.alphaIsTransparency = true;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.mipmapEnabled = false;
                importer.wrapMode = TextureWrapMode.Clamp;

                var settings =  importer.GetDefaultPlatformTextureSettings();
                settings.compressionQuality = 0;
#endif


				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                s_icons.Add(path, tex);


                return GetIconAtPath(path);
            }
        }
       
    }
}
