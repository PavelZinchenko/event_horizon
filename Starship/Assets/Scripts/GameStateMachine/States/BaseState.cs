using GameServices.LevelManager;
using Utils;
using Zenject;

namespace GameStateMachine.States
{
    public abstract class BaseState : IGameState
    {
        protected BaseState(IStateMachine stateMachine, GameStateFactory stateFactory, ILevelLoader levelLoader = null)
        {
            _stateMachine = stateMachine;
            _stateFactory = stateFactory;
            _levelLoader = levelLoader;
        }

        public abstract StateType Type { get; }

        public void Load()
        {
            OptimizedDebug.Log(GetType().Name + ": loaded");

			OnLoad();
            LoadLevel();
        }

        public void Unload()
        {
            OptimizedDebug.Log(GetType().Name + ": unloaded");
            IsActive = false;

            OnUnload();
        }

        public void Suspend(StateType newState)
        {
            IsActive = false;
            OnSuspend(newState);
        }

        public void Resume(StateType oldState)
		{
			OnResume (oldState);
            LoadLevel();
        }

        private void LoadLevel()
        {
            if (RequiredLevel != LevelName.Undefined && RequiredLevel != _levelLoader.Current)
                _levelLoader.LoadLevel(RequiredLevel, OnLevelLoadedCallback, Installer);
            else
            {
                IsActive = true;
                OnActivate();
            }
        }

        public virtual void Update(float elapsedTime) {}

        protected virtual System.Action<DiContainer> Installer => null;

        protected virtual void OnLoad() { }
        protected virtual void OnUnload() { }
        protected virtual void OnSuspend(StateType newState) { }
        protected virtual void OnResume(StateType oldState) { }
        protected virtual void OnLevelLoaded() {}
		protected virtual void OnActivate() {}

        protected virtual LevelName RequiredLevel { get { return LevelName.Undefined; } }

        protected bool IsActive { get; private set; }

        protected IStateMachine StateMachine { get { return _stateMachine; } }
        protected GameStateFactory StateFactory { get { return _stateFactory; } }

		private void OnLevelLoadedCallback()
		{
			OptimizedDebug.Log (GetType ().Name + ": level loaded");
			IsActive = true;
			OnActivate();
			OnLevelLoaded();
		}

        private readonly ILevelLoader _levelLoader;
        private readonly IStateMachine _stateMachine;
        private readonly GameStateFactory _stateFactory;
    }

    public class ExitSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }
}
