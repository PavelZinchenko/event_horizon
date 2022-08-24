using System;
using System.IO;
using Ionic.Zlib;

namespace GameDatabase.Storage
{
    public class FileDatabaseStorage : IDataStorage
    {
        // TODO: implement encryption
        public FileDatabaseStorage(string filename)
        {
            var data = File.ReadAllBytes(filename);

            var size = (uint)(data.Length - 1);
            byte checksumm = 0;
            for (var i = 0; i < size; ++i)
            {
                checksumm += data[i];
            }

            if (checksumm != data[size])
                throw new Exception($"FileDatabaseStorage: CheckSumm error - {checksumm} {data[data.Length - 1]}");

            _content = ZlibStream.UncompressBuffer(data);

            var index = 0;
            Name = GameModel.Serialization.Helpers.DeserializeString(_content, ref index);
            Id = GameModel.Serialization.Helpers.DeserializeString(_content, ref index);

            SchemaVersion = 1;
            if (_content[index] == 0)
            {
                index++;
                SchemaVersion = GameModel.Serialization.Helpers.DeserializeInt(_content, ref index);
            }

            _startIndex = index;
        }

        public string Name { get; }
        public string Id { get; }
        public int SchemaVersion { get; }
        public bool IsEditable => false;

        public void UpdateItem(string id, string content)
        {
            throw new InvalidOperationException("FileDatabaseStorage.UpdateItem is not supported");
        }

        public void LoadContent(IContentLoader loader)
        {
            var index = _startIndex;
            var itemCount = 0;
            while (_content[index] != 0)
            {
                var type = _content[index++];

                if (type == 1) // json
                {
                    var fileContent = GameModel.Serialization.Helpers.DeserializeString(_content, ref index);
                    loader.LoadJson(string.Empty, fileContent);
                    itemCount++;
                }
                else if (type == 2) // image
                {
                    var key = GameModel.Serialization.Helpers.DeserializeString(_content, ref index);
                    var image = GameModel.Serialization.Helpers.DeserializeByteArray(_content, ref index);
                    loader.LoadImage(key, image);
                }
                else if (type == 3) // localization
                {
                    var key = GameModel.Serialization.Helpers.DeserializeString(_content, ref index);
                    var text = GameModel.Serialization.Helpers.DeserializeString(_content, ref index);
                    loader.LoadLocalization(key, text);
                }
                else if (type == 4) // wav audioClip
                {
                    var key = GameModel.Serialization.Helpers.DeserializeString(_content, ref index);
                    var audioClip = GameModel.Serialization.Helpers.DeserializeByteArray(_content, ref index);
                    loader.LoadAudioClip(key, audioClip);
                }
            }

            if (itemCount == 0)
                throw new FileNotFoundException("Invalid database - ", Name);
        }

        private readonly byte[] _content;
        private readonly int _startIndex;
    }
}
