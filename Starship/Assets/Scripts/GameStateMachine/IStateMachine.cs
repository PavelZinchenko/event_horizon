using Services.Gui;

namespace GameStateMachine
{
    public interface IStateMachine
    {
        StateType ActiveState { get; }

        void LoadState(IGameState state);
        void LoadAdditionalState(IGameState state);
        void UnloadActiveState();
    }
}
