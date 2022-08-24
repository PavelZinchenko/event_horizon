#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using System.IO;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Services.Account;
using Services.Localization;
using UnityEngine;
using Zenject;

namespace Services.Storage
{
    public class GoogleSavedGame : ISavedGame
    {
        public GoogleSavedGame(GoogleCloudStorage storage, ISavedGameMetadata game)
        {
            UnityEngine.Debug.Log("GoogleSavedGame: " + game.Filename + " / " + game.Description);

            _storage = storage;
            _game = game;
        }

        public string Name { get { return _game.Description; } }
        public DateTime ModificationTime { get { return _game.LastModifiedTimestamp; } }

        public void Save(IGameData data)
        {
            _storage.Save(_game.Filename, _game.Description, data);
        }

        public void Load(IGameData data, string mod)
        {
            _storage.Load(_game, data, mod);
        }

        public void Delete()
        {
            _storage.Delete(_game);
        }

        private readonly ISavedGameMetadata _game;
        private readonly GoogleCloudStorage _storage;
    }


    public class GoogleCloudStorage : ICloudStorage
    {
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly CloudStorageStatusChangedSignal.Trigger _statusChangedTrigger;
        [Inject] private readonly CloudOperationFailedSignal.Trigger _cloudOperationFailedTrigger;
        [Inject] private readonly CloudLoadingCompletedSignal.Trigger _cloudLoadingCompletedTrigger;
        [Inject] private readonly CloudSavingCompletedSignal.Trigger _cloudSavingCompletedTrigger;
        [Inject] private readonly CloudSavedGamesReceivedSignal.Trigger _cloudSavedGamesReceivedTrigger;
        [Inject] private readonly IAccount _account;

        [Inject]
        public GoogleCloudStorage(AccountStatusChangedSignal accountStatusChangedSignal)
        {
            _accountStatusChangedSignal = accountStatusChangedSignal;
            _accountStatusChangedSignal.Event += OnAccountStatusChanged;
        }

        public CloudStorageStatus Status 
        {
            get
            {
                return _status;
            }
            private set
            {
                //if (_status == value)
                //    return;

                UnityEngine.Debug.Log("GoogleDataStorage.Status: " + value);

                _status = value;
                _statusChangedTrigger.Fire(value);
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
                    yield return new GoogleSavedGame(this, game);
                }
            }
        }

        public void Synchronize()
        {
            if (Status != CloudStorageStatus.Idle && Status != CloudStorageStatus.NotReady)
                return;

            if (_account.Status != Account.Status.Connected)
            {
                LastErrorMessage = StatusToString(SavedGameRequestStatus.AuthenticationError);
                Status = CloudStorageStatus.NotReady;
                _account.SignIn();
                return;
            }

            UnityEngine.Debug.Log("GoogleDataStorage.Synchronize");
            Status = CloudStorageStatus.Synchronizing;
            PlayGamesPlatform.Instance.SavedGame.FetchAllSavedGames(DataSource.ReadCacheOrNetwork, OnSavesFetched);
        }


        public void Save(string description, IGameData gameData)
        {
            var filename = "savegame." + System.DateTime.UtcNow.Ticks;
            Save(filename, description, gameData);
        }

        public void Save(string filename, string description, IGameData gameData)
        {
            if (Status != CloudStorageStatus.Idle)
                return;

            UnityEngine.Debug.Log("GoogleDataStorage.Save: " + filename);

            try
            {
                Status = CloudStorageStatus.Saving;

                var data = CloudDataAdapter.Serialize(gameData);

                PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, (status, metadata) =>
                {
                    try
                    {
                        if (status != SavedGameRequestStatus.Success)
                        {
                            UnityEngine.Debug.Log("GoogleDataStorage.Save - Failed: " + status);
                            _cloudOperationFailedTrigger.Fire(status.ToString());
                            Status = CloudStorageStatus.Idle;
                            return;
                        }

                        var builder = new SavedGameMetadataUpdate.Builder();
                        builder = builder
                            .WithUpdatedPlayedTime(TimeSpan.FromTicks(gameData.TimePlayed))
                            .WithUpdatedDescription(description);

                        var updatedMetadata = builder.Build();
                        PlayGamesPlatform.Instance.SavedGame.CommitUpdate(metadata, updatedMetadata, data, OnGameSaved);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log("GoogleDataStorage.Save - Failed: " + status);

                        UnityEngineThread.Execute(() =>
                        {
                            _cloudOperationFailedTrigger.Fire(e.Message);
                            Status = CloudStorageStatus.Idle;
                        });
                    }
                });
            }
            catch (System.Exception e)
            {
                _cloudOperationFailedTrigger.Fire(e.Message);
                Status = CloudStorageStatus.Idle;
            }
        }

        public void Load(ISavedGameMetadata game, IGameData gameData, string mod)
        {
            if (Status != CloudStorageStatus.Idle)
                return;

            UnityEngine.Debug.Log("GoogleDataStorage.Load: " + game.Filename + " / " + game.Description);

            try
            {
                Status = CloudStorageStatus.Loading;

                PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(game.Filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, (status, metadata) =>
                {
                    try
                    {
                        if (status != SavedGameRequestStatus.Success)
                        {
                            UnityEngine.Debug.Log("GoogleDataStorage.Load - Failed: " + status);
                            _cloudOperationFailedTrigger.Fire(StatusToString(status));
                            Status = CloudStorageStatus.Idle;
                            return;
                        }

                        PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(metadata, (status2, data) =>
                        {
                            try
                            {
                                if (status2 != SavedGameRequestStatus.Success)
                                {
                                    UnityEngine.Debug.Log("GoogleDataStorage.Load - Failed: " + status2);
                                    _cloudOperationFailedTrigger.Fire(StatusToString(status2));
                                    Status = CloudStorageStatus.Idle;
                                    return;
                                }

                                try
                                {
                                    File.WriteAllBytes(_localFileName, data);
                                }
                                catch (Exception e)
                                {
                                    UnityEngine.Debug.Log("Failed to create local copy of cloud save");
                                }

                                if (new CloudDataAdapter(data).TryLoad(gameData, mod))
                                {
                                    UnityEngine.Debug.Log("GoogleDataStorage.Load - Success");
                                    _cloudLoadingCompletedTrigger.Fire();
                                }
                                else
                                {
                                    UnityEngine.Debug.Log("GoogleDataStorage.Load - Failed");
                                    _cloudOperationFailedTrigger.Fire(_localization.GetString("$BadSavegameError"));
                                }

                                Status = CloudStorageStatus.Idle;
                            }
                            catch (Exception e)
                            {
                                UnityEngine.Debug.Log("GoogleDataStorage.Load - Failed: " + e.Message);
                                UnityEngineThread.Execute(() =>
                                {
                                    _cloudOperationFailedTrigger.Fire(e.Message);
                                    Status = CloudStorageStatus.Idle;
                                });
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log("GoogleDataStorage.Load - Failed: " + e.Message);
                        Status = CloudStorageStatus.Idle;
                        _cloudOperationFailedTrigger.Fire(e.Message);
                    }
                });

            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("GoogleDataStorage.Load - Failed: " + e.Message);
                Status = CloudStorageStatus.Idle;
                _cloudOperationFailedTrigger.Fire(e.Message);
            }
        }

        public void Delete(ISavedGameMetadata game)
        {
            if (Status != CloudStorageStatus.Idle)
                return;

            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(game.Filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime,
                (status, metadata) =>
                {
                    if (status != SavedGameRequestStatus.Success)
                    {
                        UnityEngine.Debug.Log("GoogleDataStorage.Delete - Failed: " + StatusToString(status));
                        _cloudOperationFailedTrigger.Fire(StatusToString(status));
                        return;
                    }

                    PlayGamesPlatform.Instance.SavedGame.Delete(metadata);
                    Synchronize();
                });
        }

        public bool TryLoadFromCopy(IGameData data, string mod)
        {
            try
            {
                var bytes = File.ReadAllBytes(_localFileName);
                var cloudData = new CloudDataAdapter(bytes);
                return cloudData.TryLoad(data, mod);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Failed to load from local copy");
            }

            return false;
        }

        public string LastErrorMessage { get; private set; }

        /*public void QuickSave(string filename, string description, IGameData gameData)
        {
            if (_status != CloudStorageStatus.Idle)
                return;
            if (gameData.GameId == _lastGameId && gameData.DataVersion == _lastGameVersion)
            {
                UnityEngine.Debug.Log("QuickSave: Game data not changed: " + gameData.GameId);
                return;
            }

            Status = CloudStorageStatus.Saving;

            try
            {
                var data = CloudDataAdapter.Serialize(gameData);

                var savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, (status2, game2) => 
                {
                    if (status2 == SavedGameRequestStatus.Success) 
                    {
                        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
                        builder = builder
                            .WithUpdatedPlayedTime(System.TimeSpan.FromTicks(gameData.TimePlayed))
                            .WithUpdatedDescription(description);

                        SavedGameMetadataUpdate updatedMetadata = builder.Build();
                        savedGameClient.CommitUpdate(game2, updatedMetadata, data, (status3, game3) => 
                        {
                            if (status3 == SavedGameRequestStatus.Success)
                            {
                                _lastGameId = gameData.GameId;
                                _lastGameVersion = gameData.DataVersion;
                                _cloudSavingCompletedTrigger.Fire();
                            }
                            else
                                _cloudOperationFailedTrigger.Fire(status3.ToString());

                            Status = CloudStorageStatus.Idle;
                        });
                    }
                    else
                    {
                        _cloudOperationFailedTrigger.Fire(status2.ToString());
                        Status = CloudStorageStatus.Idle;
                    }
                });
            }
            catch (System.Exception e)
            {
                _cloudOperationFailedTrigger.Fire(e.Message);
                Status = CloudStorageStatus.Idle;
            }
        }*/

        private void OnSavesFetched(SavedGameRequestStatus status, List<ISavedGameMetadata> games)
        {
            try
            {
                UnityEngine.Debug.Log("GoogleDataStorage.OnSavesFetched: " + status);
                LastErrorMessage = StatusToString(status);

                if (status == SavedGameRequestStatus.Success)
                {
                    _games.Clear();
                    _games.AddRange(games);
                    Status = CloudStorageStatus.Idle;
                    _cloudSavedGamesReceivedTrigger.Fire();
                }
                else
                {
                    Status = CloudStorageStatus.NotReady;
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                UnityEngineThread.Execute(() => Status = CloudStorageStatus.NotReady);
            }
        }

        private void OnAccountStatusChanged(Status accountStatus)
        {
            if (Status == CloudStorageStatus.NotReady && accountStatus == Account.Status.Connected)
                Synchronize();
        }

        private void OnGameSaved(SavedGameRequestStatus status, ISavedGameMetadata metadata)
        {
            try
            {
                UnityEngine.Debug.Log("GoogleDataStorage.OnGameSaved: " + status);
                Status = CloudStorageStatus.Idle;

                if (status == SavedGameRequestStatus.Success)
                {
                    _cloudSavingCompletedTrigger.Fire();
                    Synchronize();
                }
                else
                {
                    _cloudOperationFailedTrigger.Fire(StatusToString(status));
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                UnityEngineThread.Execute(() => Status = CloudStorageStatus.Idle);
            }
        }

        private string StatusToString(SavedGameRequestStatus status)
        {
            if (status == SavedGameRequestStatus.Success)
                return string.Empty;
            if (status == SavedGameRequestStatus.AuthenticationError)
                return _localization.GetString("$GoogleSingInRequired");

            return _localization.GetString("$InternalError") + " (" + status + ")";
        }

        private readonly List<ISavedGameMetadata> _games = new List<ISavedGameMetadata>();
        private readonly string _localFileName = UnityEngine.Application.persistentDataPath + "/savegame.cloud";
        private CloudStorageStatus _status = CloudStorageStatus.NotReady;
        private readonly AccountStatusChangedSignal _accountStatusChangedSignal;
    }
}

#endif