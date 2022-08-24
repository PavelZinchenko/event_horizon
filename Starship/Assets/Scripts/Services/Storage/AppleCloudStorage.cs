using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ModestTree;
using SA.Common.Models;
using Services.Localization;
using Zenject;

namespace Services.Storage
{
    public class AppleSavedGame : ISavedGame
    {
        public AppleSavedGame(AppleCloudStorage storage, GK_SavedGame game)
        {
            UnityEngine.Debug.Log("AppleSavedGame: " + game.Id + " / " + game.Name);

            _storage = storage;
            _game = game;
        }

        public string Name { get { return _game.Name; } }
        public DateTime ModificationTime { get { return _game.ModificationDate; } }

        public void Save(IGameData data)
        {
            _storage.Save(Name, data);
        }

        public void Load(IGameData data, string mod)
        {
            _storage.Load(_game, data, mod);
        }

        public void Delete()
        {
            _storage.Delete(Name);
        }

        private readonly GK_SavedGame _game;
        private readonly AppleCloudStorage _storage;
    }

    public class AppleCloudStorage : ICloudStorage, IInitializable, IDisposable
    {
        [Inject] private readonly CloudStorageStatusChangedSignal.Trigger _cloudStorageStatusChangedTrigger;
        [Inject] private readonly CloudSavedGamesReceivedSignal.Trigger _cloudSavedGamesReceivedTrigger;
        [Inject] private readonly CloudOperationFailedSignal.Trigger _cloudOperationFailedTrigger;
        [Inject] private readonly CloudLoadingCompletedSignal.Trigger _cloudLoadingCompletedTrigger;
        [Inject] private readonly CloudSavingCompletedSignal.Trigger _cloudSavingCompletedTrigger;
        [Inject] private readonly ILocalization _localization;

        public void Initialize()
        {
            ISN_GameSaves.ActionSavesFetched += ActionSavesFetched;
            ISN_GameSaves.ActionGameSaved += ActionGameSaved;
            ISN_GameSaves.ActionSaveRemoved += ActionSaveRemoved;
            ISN_GameSaves.ActionSavesResolved += ActionSavesResolved;
        }

        public void Dispose()
        {
            ISN_GameSaves.ActionSavesFetched -= ActionSavesFetched;
            ISN_GameSaves.ActionGameSaved -= ActionGameSaved;
            ISN_GameSaves.ActionSaveRemoved -= ActionSaveRemoved;
            ISN_GameSaves.ActionSavesResolved -= ActionSavesResolved;
        }

        public CloudStorageStatus Status
        {
            get { return _status; }
            private set
            {
                //if (_status == value)
                //    return;

                UnityEngine.Debug.Log("AppleCloudStorage.Status: " + value);
                _status = value;
                _cloudStorageStatusChangedTrigger.Fire(value);
            }
        }

        public IEnumerable<ISavedGame> Games
        {
            get
            {
                if (Status == CloudStorageStatus.NotReady || Status == CloudStorageStatus.Synchronizing)
                    yield break;

                foreach (var game in _games)
                {
                    yield return new AppleSavedGame(this, game);
                }
            }
        }

        public void Save(string filename, IGameData gameData)
        {
            if (Status != CloudStorageStatus.Idle)
                return;

            filename = EscapeFilename(filename);

            UnityEngine.Debug.Log("AppleCloudStorage.Save: " + filename);

            Status = CloudStorageStatus.Saving;
            try
            {
                var data = CloudDataAdapter.Serialize(gameData);
                ISN_GameSaves.Instance.SaveGame(data, filename);
            }
            catch (System.Exception e)
            {
                _cloudOperationFailedTrigger.Fire(e.Message);
                Status = CloudStorageStatus.Idle;
            }
        }

        private static string EscapeFilename(string filename)
        {
            var invalidChars = System.IO.Path.GetInvalidFileNameChars().ToHashSet();
            var sb = new StringBuilder();
            foreach (var c in filename.Where(c => !invalidChars.Contains(c)))
                sb.Append(c);
            return sb.ToString();
        }

        public void Load(GK_SavedGame game, IGameData data, string mod)
        {
            if (Status != CloudStorageStatus.Idle)
                return;

            UnityEngine.Debug.Log("AppleCloudStorage.Load: " + game.Id + " / " + game.Name);

            Status = CloudStorageStatus.Loading;

            try
            {
                _loadingGames.Add(game, new LoadingGameInfo(data, mod));
                game.ActionDataLoaded += ActionDataLoaded;
                game.LoadData();
            }
            catch (Exception e)
            {
                _loadingGames.Remove(game);
                Status = CloudStorageStatus.Idle;
                _cloudOperationFailedTrigger.Fire(e.Message);
            }
        }

        public void Delete(string filename)
        {
            if (Status != CloudStorageStatus.Idle)
                return;

            UnityEngine.Debug.Log("AppleCloudStorage.Delete");
            ISN_GameSaves.Instance.DeleteSavedGame(filename);
        }

        public void Synchronize()
        {
            if (Status != CloudStorageStatus.Idle && Status != CloudStorageStatus.NotReady)
                return;

            UnityEngine.Debug.Log("AppleCloudStorage.Synchronize");
            Status = CloudStorageStatus.Synchronizing;
            ISN_GameSaves.Instance.FetchSavedGames();
        }

        public string LastErrorMessage { get; private set; }

        public bool TryLoadFromCopy(IGameData data, string mod)
        {
            UnityEngine.Debug.Log("AppleCloudStorage.TryLoadFromCopy: not supported");
            return false;
        }

        private void ActionSavesFetched(GK_FetchResult result)
        {
            UnityEngine.Debug.Log("AppleCloudStorage.ActionSavesFetched: " + (result.HasError ? result.Error.Code + " - " + result.Error.Message : "success"));
            SetLastError(result.Error);

            if (result.IsSucceeded)
            {
                _games.Clear();
                _games.AddRange(result.SavedGames);
                Status = CloudStorageStatus.Idle;
                _cloudSavedGamesReceivedTrigger.Fire();
            }
            else
            {
                Status = CloudStorageStatus.NotReady;
            }
        }

        private void ActionGameSaved(GK_SaveResult result)
        {
            UnityEngine.Debug.Log("AppleCloudStorage.ActionGameSaved: " + (result.HasError ? result.Error.Code + " - " + result.Error.Message : "success"));
            SetLastError(result.Error);
            Status = CloudStorageStatus.Idle;

            if (result.IsSucceeded)
            {
                _cloudSavingCompletedTrigger.Fire();
                Synchronize();
            }
            else
            {
                _cloudOperationFailedTrigger.Fire(LastErrorMessage);
            }
        }

        private void ActionSaveRemoved(GK_SaveRemoveResult result)
        {
            UnityEngine.Debug.Log("AppleCloudStorage.ActionSaveRemoved: " + (result.HasError ? result.Error.Code + " - " + result.Error.Message : "success"));
            SetLastError(result.Error);

            if (result.IsSucceeded)
                Synchronize();
        }

        private void ActionDataLoaded(GK_SaveDataLoaded result)
        {
            UnityEngine.Debug.Log("AppleCloudStorage.ActionDataLoaded: " + (result.HasError ? result.Error.Code + " - " + result.Error.Message : "success"));
            SetLastError(result.Error);

            try
            {
                result.SavedGame.ActionDataLoaded -= ActionDataLoaded;

                LoadingGameInfo gameInfo;
                if (!_loadingGames.TryGetValue(result.SavedGame, out gameInfo))
                    throw new InvalidOperationException();

                if (result.IsFailed)
                {
                    _cloudOperationFailedTrigger.Fire(LastErrorMessage);
                    return;
                }
                
                _loadingGames.Remove(result.SavedGame);

                if (new CloudDataAdapter(result.SavedGame.Data).TryLoad(gameInfo.GameData, gameInfo.ModId))
                    _cloudLoadingCompletedTrigger.Fire();
                else
                    _cloudOperationFailedTrigger.Fire(_localization.GetString("BadSavegameError"));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                _cloudOperationFailedTrigger.Fire(e.Message);
            }
            finally
            {
                Status = CloudStorageStatus.Idle;
            }
        }

        private void ActionSavesResolved(GK_SavesResolveResult result)
        {
            UnityEngine.Debug.Log("AppleCloudStorage.ActionSavesResolved: " + (result.HasError ? result.Error.Code + " - " + result.Error.Message : "success"));
            SetLastError(result.Error);
        }

        private void SetLastError(Error error)
        {
            if (error == null)
            {
                LastErrorMessage = string.Empty;
                return;
            }

            var match = new Regex("NSLocalizedDescription=([^,^}]*)").Match(error.Message);
            if (match.Groups.Count > 1)
            {
                LastErrorMessage = match.Groups[1].Value;
                return;
            }

            match = new Regex("Code=([-]?[\\d\\s]*\"[^\"]*\")").Match(error.Message);
            if (match.Groups.Count > 1)
            {
                LastErrorMessage = match.Groups[1].Value;
                return;
            }

            LastErrorMessage = error.Message.Substring(0, 150);
        }

        private CloudStorageStatus _status = CloudStorageStatus.NotReady;
        private readonly List<GK_SavedGame> _games = new List<GK_SavedGame>();
        private readonly Dictionary<GK_SavedGame, LoadingGameInfo> _loadingGames = new Dictionary<GK_SavedGame, LoadingGameInfo>();

        private class LoadingGameInfo
        {
            public LoadingGameInfo(IGameData gameData, string modId)
            {
                GameData = gameData;
                ModId = modId;
            }

            public readonly IGameData GameData;
            public readonly string ModId;
        }
    }
}
