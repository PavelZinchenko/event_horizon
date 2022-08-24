using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Storage;
using GameDatabase.Model;
using UnityEngine;
using Utils;
using Zenject;

namespace GameDatabase
{
    public partial class Database : IInitializable
    {
        public Database(GameDatabaseLoadedSignal.Trigger dataLoadedTrigger)
        {
            _dataLoadedTrigger = dataLoadedTrigger;
            _defaultStorage = new ResourceDatabaseStorage(DefaultPath);
        }

        public void Initialize()
        {
            ScanForMods();
        }

        public void ScanForMods()
        {
            _mods.Clear();

#if UNITY_ANDROID && !UNITY_EDITOR
            var path = Application.persistentDataPath + "/Mods/";
#elif UNITY_STANDALONE || UNITY_EDITOR
            var path = Application.dataPath + "/../Mods/";
#else
            var path = string.Empty;
            return;
#endif
            try
            {
                var info = new DirectoryInfo(path);
                foreach (var fileInfo in info.GetFiles("*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        var mod = new FileDatabaseStorage(fileInfo.FullName);
                        _mods.Add(new ModInfo(mod.Name, mod.Id, fileInfo.FullName));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("invalid mod file - " + fileInfo.FullName);
                        Debug.LogException(e);
                    }
                }

                foreach (var directoryInfo in info.GetDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        var mod = new FolderDatabaseStorage(directoryInfo.FullName);
                        _mods.Add(new ModInfo(mod.Name, mod.Id, directoryInfo.FullName));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("invalid database - " + directoryInfo.FullName);
                        Debug.LogException(e);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading mods - " + e.Message);
            }
        }

        public void LoadDefault()
        {
            try
            {
                Load();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public bool TryLoad(string id, out string error)
        {
            try
            {
                Clear();
                error = string.Empty;

                if (string.IsNullOrEmpty(id))
                {
                    Load();
                    return true;
                }

                var index = _mods.FindIndex(item => item.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
                if (index < 0)
                {
                    error = "Mod " + id + " is not found";
                    return false;
                }

                var path = _mods[index].Path;
                if (File.Exists(path))
                    Load(new FileDatabaseStorage(path));
                else if (Directory.Exists(path))
                    Load(new FolderDatabaseStorage(path));
                else
                    throw new FileNotFoundException(path);

                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Database.TryLoad() Error: " + e.Message);
                error = e.Message;
                return false;
            }
        }

        protected void Load(IDataStorage storage = null)
        {
            Clear();
            _storage = null;

            _content = new DatabaseContent(_jsonSerializer, storage);

            if (_content.DatabaseSettings == null || !_content.DatabaseSettings.UnloadOriginalDatabase)
                _content.LoadParent(_defaultStorage);

            _factionMap.Add(Faction.Neutral.Id.Value, Faction.Neutral);
            Loader.Load(this, _content);
            
            _storage = storage;
            _dataLoadedTrigger.Fire();
        }

        public IEnumerable<ModInfo> AvailableMods => _mods;

        public string Name => DatabaseSettings.ModName ?? Storage.Name;
        public string Id => Storage.Id;
        public bool IsEditable => Storage.IsEditable;

#region this functionality should be moved to database editor

        public void SaveShipBuild(ItemId<ShipBuild> id)
        {
            var serializedBuild = _content.GetShipBuild(id.Value);
            if (serializedBuild == null)
            {
                Debug.LogError("SaveShipBuild: not found " + id);
                return;
            }

            var build = GetShipBuild(id);
            serializedBuild.Components = build.Components.Select(item => item.Serialize()).ToArray();
            Storage.UpdateItem(serializedBuild.FileName, _jsonSerializer.ToJson(serializedBuild));
        }

        public void SaveSatelliteBuild(ItemId<SatelliteBuild> id)
        {
            var serializedBuild = _content.GetSatelliteBuild(id.Value);
            if (serializedBuild == null)
            {
                Debug.LogError("SaveSatelliteBuild: not found " + id);
                return;
            }

            var build = GetSatelliteBuild(id);
            serializedBuild.Components = build.Components.Select(item => item.Serialize()).ToArray();
            Storage.UpdateItem(serializedBuild.FileName, _jsonSerializer.ToJson(serializedBuild));
        }

#endregion

        private IDataStorage Storage => _storage ?? _defaultStorage;

        private IDataStorage _storage;
        private DatabaseContent _content;
        private readonly IDataStorage _defaultStorage;
        private readonly IJsonSerializer _jsonSerializer = new UnityJsonSerializer();
        private readonly GameDatabaseLoadedSignal.Trigger _dataLoadedTrigger;
        private readonly List<ModInfo> _mods = new List<ModInfo>();
        private const string DefaultPath = "Database";
    }

    public class GameDatabaseLoadedSignal : SmartWeakSignal { public class Trigger : TriggerBase { } }
}
