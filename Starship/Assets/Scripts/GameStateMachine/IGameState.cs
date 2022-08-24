namespace GameStateMachine
{
    public interface IGameState
    {
        StateType Type { get; }

        void Load();
        void Unload();

        void Suspend(StateType newState);
        void Resume(StateType lastState);

        void Update(float elapsedTime);
    }
}
