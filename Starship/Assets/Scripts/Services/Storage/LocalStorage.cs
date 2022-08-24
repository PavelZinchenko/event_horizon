using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameModel.Serialization;
using UnityEngine;

namespace Services.Storage
{
    public class LocalStorage : IDataStorage
    {
        public bool TryLoad(IGameData gameData, string mod)
        {
            _mainFileLoaded = false;
            _currentMod = mod;

            byte[] data;
            if (TryLoadMainFile(out data))
            {
                if (TryDeserializeData(gameData, data))
                {
                    _mainFileLoaded = true;
                    return true;
                }

                TryStoreErrorFile(data);
            }

            return LoadAllBackupFiles().Any(backup => TryDeserializeData(gameData, backup));
        }

        public bool TryImportOriginalSave(IGameData gameData, string mod)
        {
            if (string.IsNullOrEmpty(mod))
                return false;

            _mainFileLoaded = false;
            _currentMod = string.Empty;

            byte[] data;
            if (TryLoadMainFile(out data) && TryDeserializeData(gameData, data, mod) ||
                LoadAllBackupFiles().Any(backup => TryDeserializeData(gameData, backup, mod)))
            {
                _currentMod = mod;
                return true;
            }

            return false;
        }

        // TODO: implement encryption
        public void Save(IGameData gameData)
        {
            try
            {
                if (_currentGameId == gameData.GameId && _currentVersion == gameData.DataVersion)
                {
                    UnityEngine.Debug.Log("LocalStorageBase.Save: Game data not changed: " + gameData.GameId + "/" + gameData.DataVersion);
                    return;
                }

                UnityEngine.Debug.Log("LocalStorageBase.Save: writing data " + gameData.GameId + "/" + gameData.DataVersion);

                if (_mainFileLoaded)
                    TryBackupMainFile();

                var data = new List<byte>();

                const int formatId = 3;
                data.AddRange(Helpers.Serialize(formatId));
                data.AddRange(Helpers.Serialize(gameData.GameId));
                data.AddRange(Helpers.Serialize(gameData.TimePlayed));
                data.AddRange(Helpers.Serialize(gameData.DataVersion));
                data.AddRange(Helpers.Serialize(AppConfig.version));
                data.AddRange(Helpers.Serialize(gameData.ModId));
                data.AddRange(gameData.Serialize());

                var size = (uint)data.Count;
                byte checksumm = 0;
                for (int i = 0; i < size; ++i)
                {
                    checksumm += data[i];
                }

                data.Add(checksumm);

                _currentMod = gameData.ModId;
                _mainFileLoaded = false;

                if (!TrySaveMainFile(data.ToArray()))
                {
                    UnityEngine.Debug.Log("LocalStorageBase.Save: failed");
                    return;
                }

                _currentGameId = gameData.GameId;
                _currentVersion = gameData.DataVersion;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
            }
        }

        protected virtual bool TryLoadMainFile(out byte[] data)
        {
            return TryLoadFile(_savesDir + MainFileName, out data);
        }

        protected virtual IEnumerable<byte[]> LoadAllBackupFiles()
        {
            byte[] data;
            if (TryLoadFile(_savesDir + BackupFileName, out data))
                yield return data;
        }

        protected virtual bool TrySaveMainFile(byte[] data)
        {
            return TrySaveFile(_savesDir + MainFileName, data);
        }

        protected virtual bool TryBackupMainFile()
        {
            return TryCopyFile(_savesDir + MainFileName, _savesDir + BackupFileName);
        }

        protected virtual bool TryStoreErrorFile(byte[] data)
        {
            return TrySaveFile(_savesDir + ErrorFileName, data);
        }

        protected virtual bool TryBackupOldVersion(byte[] data, string version)
        {
            return false;
        }

        protected static bool TryLoadFile(string filename, out byte[] data)
        {
            if (!File.Exists(filename))
            {
                data = null;
                return false;
            }

            try
            {
                data = File.ReadAllBytes(filename);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                data = null;
                return false;
            }
        }

        protected static bool TryCreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        protected static bool TryCopyFile(string source, string destination)
        {
            if (!File.Exists(source))
                return false;

            try
            {
                File.Copy(source, destination, true);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        protected static bool TrySaveFile(string filename, byte[] data)
        {
            try
            {
                File.WriteAllBytes(filename, data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        protected string MainFileName { get { return string.IsNullOrEmpty(_currentMod) ? "savegame" : _currentMod; } }
        protected string BackupFileName { get { return string.IsNullOrEmpty(_currentMod) ? "savegame.bak" : _currentMod + ".bak"; } }
        protected string ErrorFileName { get { return string.IsNullOrEmpty(_currentMod) ? "savegame.err" : _currentMod + ".err"; } }
        protected string DefaultSavesDir { get { return _savesDir; } }

        // TODO: implement encryption
        private bool TryDeserializeData(IGameData gameData, byte[] serializedData, string overrideMod = null)
        {
            try
            {
                _currentGameId = -1;
                
                var size = (uint)(serializedData.Length - 1);
                byte checksumm = 0;
                for (int i = 0; i < size; ++i)
                {
                    checksumm += serializedData[i];
                }

                if (checksumm != serializedData[serializedData.Length - 1])
                {
                    Debug.LogException(new Exception("LocalStorageBase.TryDeserializeData: CheckSumm error - " + checksumm + " " + serializedData[serializedData.Length - 1]));
                    return false;
                }

                int index = 0;
                var formatId = Helpers.DeserializeInt(serializedData, ref index);
                var gameId = Helpers.DeserializeLong(serializedData, ref index);
                var time = Helpers.DeserializeLong(serializedData, ref index);
                var version = formatId >= 1 ? Helpers.DeserializeLong(serializedData, ref index) : 0;
                var gameVersion = formatId >= 2 ? Helpers.DeserializeString(serializedData, ref index) : "old";
                var mod = formatId >= 3 ? Helpers.DeserializeString(serializedData, ref index) : null;

                if (!IsModsEqual(mod, _currentMod))
                {
                    Debug.LogException(new Exception("LocalStorageBase.TryDeserializeData: Invalid mod id"));
                    return false;
                }

                if (gameVersion != AppConfig.version)
                    TryBackupOldVersion(serializedData, gameVersion);

                if (!string.IsNullOrEmpty(overrideMod))
                    mod = overrideMod;

                if (!gameData.TryDeserialize(gameId, time, version, mod, serializedData, index))
                {
                    Debug.LogException(new Exception("LocalStorageBase.TryDeserializeData: Data deserialization failed"));
                    return false;
                }

                _currentGameId = gameId;
                _currentVersion = version;

                UnityEngine.Debug.Log("LocalStorageBase.TryDeserializeData: done - " + gameData.GameId);

                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                return false;
            }
        }

        public static bool IsModsEqual(string mod1, string mod2)
        {
            if (string.IsNullOrEmpty(mod1))
                return string.IsNullOrEmpty(mod2);
            if (string.IsNullOrEmpty(mod2))
                return string.IsNullOrEmpty(mod1);
            return string.Equals(mod1, mod2, StringComparison.OrdinalIgnoreCase);
        }

        private string _currentMod;
        private long _currentGameId;
        private long _currentVersion;
        private bool _mainFileLoaded;

        private readonly string _savesDir = Application.persistentDataPath + "/";
    }
}
