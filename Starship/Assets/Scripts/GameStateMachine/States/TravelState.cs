using Session;
using GameServices.LevelManager;
using GameServices.Player;
using Services.Gui;
using Services.Messenger;
using Zenject;

namespace GameStateMachine.States
{
    class TravelState : BaseState
    {
        [Inject]
        public TravelState(
            IStateMachine stateMachine,
            GameStateFactory gameStateFactory,
            ILevelLoader levelLoader,
            int destination,
            PlayerResources playerResources,
            MotherShip motherShip,
            IGuiManager guiManager,
            ISessionData session,
            IMessenger messenger)
            : base(stateMachine, gameStateFactory, levelLoader)
        {
            _source = session.StarMap.PlayerPosition;
            _destination = destination;
            _playerResources = playerResources;
            _session = session;
            _messenger = messenger;
            _motherShip = motherShip;
            _guiManager = guiManager;
        }

        public override StateType Type { get { return StateType.Travel; } }

        public override void Update(float elapsedTime)
        {
            _progress += elapsedTime / _lifeTime;

            if (_progress < 1f)
            {
                _messenger.Broadcast<int, int, float>(EventType.PlayerShipMoved, _source, _destination, _progress);
            }
            else
            {
                UnityEngine.Debug.Log("FlightState: Finished");

                _session.StarMap.PlayerPosition = _destination;
                StateMachine.UnloadActiveState();
            }
        }

        protected override LevelName RequiredLevel { get { return LevelName.StarMap; } }

        protected override void OnLoad()
        {
            UnityEngine.Debug.Log("FlightState: Started - " + _destination);

            var requiredFuel = _motherShip.CalculateRequiredFuel(_source, _destination);

            if (!_playerResources.TryConsumeFuel(requiredFuel))
            {
				UnityEngine.Debug.Log("FlightState: not enough fuel");
                StateMachine.UnloadActiveState();
                return;
            }

            _lifeTime = _motherShip.CalculateFlightTime(_source, _destination);

            _guiManager.AutoWindowsAllowed = false;
            _guiManager.CloseAllWindows();
        }

        protected override void OnUnload()
        {
            _guiManager.AutoWindowsAllowed = true;
        }

        private float _lifeTime;
        private float _progress;

        private readonly int _source;
        private readonly int _destination;
        private readonly PlayerResources _playerResources;
        private readonly ISessionData _session;
        private readonly IMessenger _messenger;
        private readonly MotherShip _motherShip;
        private readonly IGuiManager _guiManager;

        public class Factory : Factory<int, TravelState> {}
    }
}
