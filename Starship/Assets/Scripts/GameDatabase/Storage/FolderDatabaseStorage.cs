using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GameDatabase.Storage
{
    public class FolderDatabaseStorage : IDataStorage
    {
        public FolderDatabaseStorage(string path)
        {
            var info = new DirectoryInfo(path);
            Name = info.Name;
            Id = info.Name;

            var modInfo = info.GetFiles("mod", SearchOption.AllDirectories).FirstOrDefault();
            if (modInfo != null)
            {
                var data = File.ReadAllText(modInfo.FullName).Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 2 && data[1].IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                {
                    Name = data[0];
                    Id = data[1];
                }
            }

            _path = path;
        }

        public void LoadContent(IContentLoader loader)
        {
            var info = new DirectoryInfo(_path);
            var itemCount = 0;
            foreach (var fileInfo in info.GetFiles("*", SearchOption.AllDirectories))
            {
                var file = fileInfo.FullName;
                if (fileInfo.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || 
                    fileInfo.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || 
                    fileInfo.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    var binaryData = File.ReadAllBytes(file);
                    loader.LoadImage(fileInfo.Name, binaryData);
                }
                else if (fileInfo.Extension.Equals(".wav", StringComparison.OrdinalIgnoreCase))
                {
                    var audioData = File.ReadAllBytes(file);
                    loader.LoadAudioClip(Path.GetFileNameWithoutExtension(file), audioData);
                }
                else if (fileInfo.Extension.Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    var xmlData = File.ReadAllText(file);
                    loader.LoadLocalization(Path.GetFileNameWithoutExtension(file), xmlData);
                }
                else if (fileInfo.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
                {
                    var data = File.ReadAllText(file);
                    loader.LoadJson(file, data);
                    itemCount++;
                }
            }

            if (itemCount == 0)
                throw new FileNotFoundException("Invalid database - ", _path);
        }

        public string Name { get; }
        public string Id { get; }
        public int SchemaVersion => DatabaseContent.SchemaVersion;
        public bool IsEditable => true;

        public void UpdateItem(string name, string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            if (!name.StartsWith(_path))
                name = Path.Combine(_path, name);
            
            try
            {
                File.WriteAllText(name, content);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private readonly string _path;
    }
}
