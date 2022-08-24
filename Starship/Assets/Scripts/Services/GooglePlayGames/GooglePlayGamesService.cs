#if UNITY_ANDROID

using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using Zenject;

namespace Services.GooglePlayGames
{
    public class GooglePlayGamesService : IInitializable, IDisposable
    {
        public void Initialize()
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .EnableSavedGames()
                .Build();

            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = Debug.isDebugBuild;
            PlayGamesPlatform.Activate();
        }

        public void Dispose()
        {
        }
    }
}

#endif
