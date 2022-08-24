using Combat.Domain;
using Services.Gui;
using Constructor.Ships;
using Domain.Quests;
using Game.Exploration;
using GameModel.Quests;
using Zenject;

namespace GameStateMachine.States
{
    public class GameStateFactory
    {
        [Inject] private readonly MainMenuState.Factory _mainMenuStateFactory;
        [Inject] private readonly StarMapState.Factory _starMapStateFactory;
        [Inject] private readonly TravelState.Factory _flightStateFactory;
        [Inject] private readonly QuestState.Factory _questStateFactory;
        [Inject] private readonly RetreatState.Factory _retreatStateFactory;
        [Inject] private readonly SkillTreeState.Factory _skillTreeStateFactory;
        [Inject] private readonly ConstructorState.Factory _constructorStateStateFactory;
        [Inject] private readonly DialogState.Factory _dialogStateFactory;
        [Inject] private readonly TestingState.Factory _testingStateFactory;
        [Inject] private readonly EhopediaState.Factory _echopediaStateFactory;
        [Inject] private readonly CombatState.Factory _combatSceneStateFactory;
        [Inject] private readonly DailyRewardState.Factory _dailyRewardStateFactory;
        [Inject] private readonly AnnouncementState.Factory _announcementStateFactory;
        [Inject] private readonly CombatRewardState.Factory _combatRewardStateFactory;
        [Inject] private readonly ExplorationState.Factory _explorationStateFactory;

        public IGameState CreateStarMapState()
        {
            return _starMapStateFactory.Create();
        }

        public IGameState CreateMainMenuState()
        {
            return _mainMenuStateFactory.Create();
        }

        public IGameState CreateAnnouncementState()
        {
            return _announcementStateFactory.Create();
        }

        public IGameState CreateDaylyRewardState()
        {
            return _dailyRewardStateFactory.Create();
        }

        public IGameState CreateTravelState(int destination)
        {
            return _flightStateFactory.Create(destination);
        }

        public IGameState CreateRetreatState()
        {
            return _retreatStateFactory.Create();
        }

        public IGameState CreateQuestState(IUserInteraction userInteraction)
        {
            return _questStateFactory.Create(userInteraction);
        }

        public IGameState CreateSkillTreeState()
        {
            return _skillTreeStateFactory.Create();
        }

        public IGameState CreateConstructorState(IShip ship)
        {
            return _constructorStateStateFactory.Create(ship);
        }

        public IGameState CreateDialogState(string windowName, WindowArgs args, System.Action<WindowExitCode> onExitAction = null)
        {
            return _dialogStateFactory.Create(windowName, args, onExitAction);
        }

        public IGameState CreateEchopediaState()
        {
            return _echopediaStateFactory.Create();
        }

        public IGameState CreateTestingState()
        {
            return _testingStateFactory.Create();
        }

        public IGameState CreateCombatState(ICombatModel combatModel, System.Action<ICombatModel> onCompleteAction)
        {
            return _combatSceneStateFactory.Create(combatModel, onCompleteAction);
        }

        public IGameState CreateCombatRewardState(IReward reward)
        {
            return _combatRewardStateFactory.Create(reward);
        }

        public IGameState CreateExplorationState(Planet planet)
        {
            return _explorationStateFactory.Create(planet);
        }
    }
}
