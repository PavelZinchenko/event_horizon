using System;
using System.IO;
using UnityEngine;

namespace GameDatabase.Storage
{
    public class ResourceDatabaseStorage : IDataStorage
    {
        public ResourceDatabaseStorage(string path)
        {
            var info = new DirectoryInfo(path);
            Name = info.Name;
            Id = string.Empty;
            _path = path;
        }

        public void LoadContent(IContentLoader loader)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            foreach (var asset in Resources.LoadAll<TextAsset>(_path))
            {
#if UNITY_EDITOR
                var name = UnityEditor.AssetDatabase.GetAssetPath(asset);
#else
                var name = string.Empty;
#endif
                loader.LoadJson(name, asset.text);
            }
        }

        public string Name { get; }
        public string Id { get; }
        public int SchemaVersion => DatabaseContent.SchemaVersion;
        public bool IsEditable
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        public void UpdateItem(string name, string content)
        {
#if UNITY_EDITOR
            try
            {
                File.WriteAllText(name, content);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
#endif
        }

        private readonly string _path;
    }
}
