using System;
using GameServices.GameManager;
using GameStateMachine;
using Session;
using Zenject;

namespace Services.InternetTime
{
    public class GameTime : ITickable
    {
        public GameTime(
            ISessionData session, 
            SessionDataLoadedSignal sessionDataLoadedSignal, 
            SessionAboutToSaveSignal sessionAboutToSaveSignal,
            GameStateChangedSignal gameStateChangedSignal)
        {
            _session = session;
            _sessionAboutToSaveSignal = sessionAboutToSaveSignal;
            _sessionDataLoadedSignal = sessionDataLoadedSignal;
            _gameStateChangedSignal = gameStateChangedSignal;

            _sessionAboutToSaveSignal.Event += OnSessionAboutToSave;
            _sessionDataLoadedSignal.Event += OnSessionDataLoaded;
            _gameStateChangedSignal.Event += OnGameStateChanged;
        }

        public long TotalPlayTime => _totalPlayTime;

        public void Tick()
        {
            var now = DateTime.Now.Ticks;
            if (!_paused &&_lastUpdateTime < now && now - _lastUpdateTime < TimeSpan.TicksPerMinute)
                _totalPlayTime += now - _lastUpdateTime;

            _lastUpdateTime = now;
        }

        private void OnSessionAboutToSave()
        {
            if (_session.Game.TotalPlayTime > _totalPlayTime)
            {
                UnityEngine.Debug.LogException(new InvalidOperationException());
                return;
            }

            _session.Game.TotalPlayTime = _totalPlayTime;
        }

        private void OnSessionDataLoaded()
        {
            _totalPlayTime = _session.Game.TotalPlayTime;
            _lastUpdateTime = DateTime.Now.Ticks;
        }

        private void OnGameStateChanged(StateType type)
        {
            switch (type)
            {
                case StateType.MainMenu:
                case StateType.DailyReward:
                case StateType.Dialog:
                case StateType.Ehopedia:
                case StateType.Constructor:
                case StateType.Initialization:
                case StateType.SkillTree:
                case StateType.Testing:
                    _paused = true;
                    break;
                default:
                    _paused = false;
                    break;
            }
        }

        private bool _paused = false;
        private long _totalPlayTime;
        private long _lastUpdateTime;
        private readonly ISessionData _session;
        private readonly SessionDataLoadedSignal _sessionDataLoadedSignal;
        private readonly SessionAboutToSaveSignal _sessionAboutToSaveSignal;
        private readonly GameStateChangedSignal _gameStateChangedSignal;
    }
}
