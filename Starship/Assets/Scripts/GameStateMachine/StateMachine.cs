using System;
using System.Collections.Generic;
using System.Linq;
using GameStateMachine.States;
using Utils;
using Zenject;

namespace GameStateMachine
{
    public class StateMachine : IStateMachine, IInitializable, IDisposable, ITickable
    {
#if EDITOR_MODE
        [Inject] private readonly EditorInitializationState.Factory _initializationStateFactory;
#else
        [Inject] private readonly InitializationState.Factory _initializationStateFactory;
#endif
        [Inject] private readonly GameStateChangedSignal.Trigger _gameStateChangedTrigger;

        public void Initialize()
        {
            OptimizedDebug.Log("StateMachine.Initialize");
            LoadState(_initializationStateFactory.Create());
        }

        public void Dispose()
        {
            while (_states.Any())
                _states.Pop().Unload();
        }

        public void Tick()
        {
            if (_states.Any())
                _states.Peek().Update(UnityEngine.Time.deltaTime);
        }

        public StateType ActiveState { get { return _states.Any() ? _states.Peek().Type : StateType.Undefined; } }

        public void UnloadActiveState()
        {
            UnloadState();

            if (ActiveState == StateType.Undefined)
                UnityEngine.Application.Quit();
        }

        public void LoadState(IGameState state)
        {
            if (_states.Any())
                _states.Pop().Unload();

            _states.Push(state);
            state.Load();

            _gameStateChangedTrigger.Fire(state.Type);
        }

        public void LoadAdditionalState(IGameState state)
        {
            if (_states.Any())
                _states.Peek().Suspend(state.Type);

            _states.Push(state);
            state.Load();

            _gameStateChangedTrigger.Fire(state.Type);
        }

        private void UnloadState()
        {
            var lastState = _states.Pop();
            lastState.Unload();

            if (_states.Any())
            {
                var state = _states.Peek();
                state.Resume(lastState.Type);

                _gameStateChangedTrigger.Fire(state.Type);
            }
        }

        private readonly Stack<IGameState> _states = new Stack<IGameState>();
    }

    public class GameStateChangedSignal : SmartWeakSignal<StateType>
    {
        public class Trigger : TriggerBase { }
    }
}
