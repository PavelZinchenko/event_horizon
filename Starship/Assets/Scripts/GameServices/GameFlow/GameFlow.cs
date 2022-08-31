﻿using GameServices.LevelManager;
using GameServices.Settings;
using Services.Messenger;
using UnityEngine;
using Utils;
using Zenject;

namespace GameServices
{
    public class GameFlow : MonoBehaviour
    {
        [Inject]
        private void Initialize(
            IMessenger messenger, 
            GameSettings gameSettings,
            SceneLoadedSignal sceneLoadedSignal, 
            GamePausedSignal.Trigger gamePausedTrigger)
        {
            _messenger = messenger;
            _sceneLoadedSignal = sceneLoadedSignal;
            _gamePausedTrigger = gamePausedTrigger;
            _runInBackground = gameSettings.RunInBackground;

            _sceneLoadedSignal.Event += OnSceneLoaded;

            UpdateStatus();
        }

        public void Pause()
        {
            if (_gamePaused) return;
            _gamePaused = true;
            UpdateStatus();
        }

        public void Resume()
        {
            if (!_gamePaused)
                return;

            _gamePaused = false;
            UpdateStatus();
        }

        public bool Paused => _gamePaused || _applicationPaused;

        public bool ApplicationPaused => _applicationPaused;

        private void OnApplicationFocus(bool focused)
        {
            if (_runInBackground) return;

            if (_applicationPaused != focused)
                return;

            _applicationPaused = !focused;
            UpdateStatus();
        }

        private void OnApplicationPause(bool paused)
        {
            if (_applicationPaused == paused)
                return;

            _applicationPaused = paused;
            UpdateStatus();
        }

        private void OnSceneLoaded()
        {
            if (_gamePaused)
            {
                _gamePaused = false;
                UpdateStatus();
            }
        }

        private void UpdateStatus()
        {
            Time.timeScale = Paused ? 0.0f : 1.0f;
            Timer.Paused = Paused;

            _messenger.Broadcast(EventType.GamePaused, Paused);
            _gamePausedTrigger.Fire(Paused);
            //Screen.sleepTimeout = Paused ? SleepTimeout.SystemSetting : SleepTimeout.NeverSleep;
        }

        private bool _applicationPaused;
        private bool _gamePaused;

        private bool _runInBackground;
        private IMessenger _messenger;
        private SceneLoadedSignal _sceneLoadedSignal;
        private GamePausedSignal.Trigger _gamePausedTrigger;
    }

    public class GamePausedSignal : SmartWeakSignal<bool>
    {
        public class Trigger : TriggerBase {}
    }
}
