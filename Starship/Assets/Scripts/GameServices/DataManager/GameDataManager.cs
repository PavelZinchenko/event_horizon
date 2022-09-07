using System;
using GameDatabase;
using GameServices.Gui;
using Session;
using GameServices.LevelManager;
using GameServices.Settings;
using Services.Localization;
using Services.Storage;
using UnityEngine;
using Utils;
using Zenject;

namespace GameServices.GameManager
{
    public class GameDataManager : MonoBehaviour, IGameDataManager
    {
        [Inject] private readonly GuiHelper _guiHelper;
        
        [Inject]
        private void Initialize(
            IDataStorage localStorage,
            ICloudStorage cloudStorage,
            ILocalization localization,
            ILevelLoader levelLoader,
            ISessionData sessionData,
            GameSettings gameSettings,
            IDatabase database,
            SceneLoadedSignal sceneLoadedSignal,
            SessionAboutToSaveSignal.Trigger sessionAboutToSaveTrigger)
        {
            _database = database;
            _localStorage = localStorage;
            _cloudStorage = cloudStorage;
            _localization = localization;
            _levelLoader = levelLoader;
            _sessionData = sessionData;
            _gameSettings = gameSettings;
            _sceneLoadedSignal = sceneLoadedSignal;
            _sessionAboutToSaveTrigger = sessionAboutToSaveTrigger;

            _sceneLoadedSignal.Event += OnLevelLoaded;
        }

        ~GameDataManager()
        {
            UnityEngine.Debug.Log("GameDataManager: destructor");
        }

        public void LoadMod(string id = null, bool force = false)
        {
            if (_database.Id.Equals(id, StringComparison.OrdinalIgnoreCase) && !force)
                return;

            string error;

            try
            {
                SaveSession();

                if (!_database.TryLoad(id, out error))
                    throw new Exception(error);

                if (_localStorage.TryLoad(_sessionData, _database.Id))
                    UnityEngine.Debug.Log("Saved game loaded");
                else if (_database.IsEditable && _localStorage.TryImportOriginalSave(_sessionData, _database.Id))
                    UnityEngine.Debug.Log("Original saved game imported");
                else
                    _sessionData.CreateNewGame(_database.Id);

                _gameSettings.ActiveMod = _database.Id;
                _guiHelper.ShowMessage(_localization.GetString("$DatabaseLoaded"));
            }
            catch (Exception e)
            {
                _guiHelper.ShowConfirmation(_localization.GetString("$DatabaseLoadingError", e.Message), () => { });
                _database.LoadDefault();
                if (!_localStorage.TryLoad(_sessionData, string.Empty))
                    _sessionData.CreateNewGame(string.Empty);
            }
        }

        public void ReloadMod()
        {
            LoadMod(_database.Id, true);
        }

        public void CreateNewGame()
        {
            _sessionData.CreateNewGame(_database.Id);
        }

        public void SaveGameToCloud(string filename)
        {
            UnityEngine.Debug.Log("GameDataManager.SaveGameToCloud: " + filename);
            _localStorage.Save(_sessionData);
            _cloudStorage.Save(filename, _sessionData);
        }

        public void SaveGameToCloud(ISavedGame game)
        {
            UnityEngine.Debug.Log("GameDataManager.SaveGameToCloud: " + game.Name);
            _sessionAboutToSaveTrigger.Fire();
            _localStorage.Save(_sessionData);
            game.Save(_sessionData);
        }

        public void LoadGameFromCloud(ISavedGame game)
        {
            UnityEngine.Debug.Log("GameDataManager.LoadGameFromCloud: " + game.Name);
            game.Load(_sessionData, _database.Id);
        }

        public void LoadGameFromLocalCopy()
        {
            UnityEngine.Debug.Log("GameDataManager.LoadGameFromLocalCopy");
            _cloudStorage.TryLoadFromCopy(_sessionData, _database.Id);
        }

        private void SaveSession()
        {
            if (!_sessionData.IsGameStarted)
                return;

            UnityEngine.Debug.Log("SessionData.TimePlayed = " + _sessionData.TimePlayed);
            UnityEngine.Debug.Log("GameStartTime = " + _sessionData.Game.GameStartTime);

            _sessionAboutToSaveTrigger.Fire();
            _localStorage.Save(_sessionData);
        }

        //private void SaveDatabaseToCloud()
        //{
        //    _autoSaveTime = Time.time;

        //    if (!_sessionData.IsGameStarted)
        //        return;

        //    _sessionAboutToSaveTrigger.Fire();
        //    _localStorage.Save(_sessionData);
        //    _cloudStorage.QuickSave("autosave", _localization.GetString("$AutoSave"), _sessionData);
        //}

        private void Start()
        {
            _autoSaveTime = Time.time;
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
                SaveSession();
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (!focusStatus)
                SaveSession();
        }

        private void OnDisable()
        {
            SaveSession();
            Cleanup();
        }

        private void OnLevelLoaded()
        {
            if (_levelLoader.Current == LevelName.StarMap)
            {
                SaveSession();
            }
            //else if (Time.time - _autoSaveTime > 30f && _gameSettings.AutoSave &&
            //    _levelLoader.Current == LevelName.MainMenu)
            //{
            //    SaveDatabaseToCloud();
            //}

            Cleanup();
        }

        private void Cleanup()
        {
            UnityEngine.Debug.Log("Cleanup");
            Resources.UnloadUnusedAssets();
            System.GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        private IDataStorage _localStorage;
        private ICloudStorage _cloudStorage;
        private ILocalization _localization;
        private ILevelLoader _levelLoader;
        private ISessionData _sessionData;
        private GameSettings _gameSettings;
        private SceneLoadedSignal _sceneLoadedSignal;
        private SessionAboutToSaveSignal.Trigger _sessionAboutToSaveTrigger;
        private IDatabase _database;

        private float _autoSaveTime;
    }

    public class SessionAboutToSaveSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }
}
