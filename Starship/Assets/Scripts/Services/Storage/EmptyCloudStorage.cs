using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameModel.Serialization;
using UnityEngine;
using Zenject;

namespace Services.Storage
{
    public class EmptyCloudStorage : ICloudStorage
    {
#if UNITY_EDITOR
        [Inject] private readonly LocalStorage _localStorage;
#endif

        public CloudStorageStatus Status { get { return CloudStorageStatus.NotReady; } }
        public IEnumerable<ISavedGame> Games { get { return Enumerable.Empty<ISavedGame>(); } }
        public void Synchronize() {}
        public void Save(string filename, IGameData data) {}

        public bool TryLoadFromCopy(IGameData data, string mod)
        {
#if UNITY_EDITOR
            try
            {
                var bytes = File.ReadAllBytes(_localFileName);
                var cloudData = new CloudDataAdapter(bytes);
                var gameData = new GameDataStub();
                if (!cloudData.TryLoad(gameData, mod)) return false;
                SaveFile(gameData);

                return _localStorage.TryLoad(data, mod);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Failed to load from local copy");
            }
#endif
            return false;
        }

        private void SaveFile(IGameData gameData)
        {
            try
            {
                var data = new List<byte>();

                const int formatId = 0;
                data.AddRange(Helpers.Serialize(formatId));
                data.AddRange(Helpers.Serialize(gameData.GameId));
                data.AddRange(Helpers.Serialize(gameData.TimePlayed));
                data.AddRange(gameData.Serialize());

                var size = (uint)data.Count;
                byte checksumm = 0;
                for (int i = 0; i < size; ++i)
                {
                    checksumm += data[i];
                }

                data.Add(checksumm);

                File.WriteAllBytes(Application.persistentDataPath + "/savegame", data.ToArray());
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
            }
        }

        public string LastErrorMessage { get { return string.Empty; } }

        private readonly string _localFileName = UnityEngine.Application.persistentDataPath + "/savegame.cloud";
    }
}
