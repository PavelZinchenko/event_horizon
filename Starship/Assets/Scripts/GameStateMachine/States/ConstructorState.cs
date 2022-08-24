using GameServices.LevelManager;
using Services.Messenger;
using Constructor.Ships;
using Utils;
using Zenject;

namespace GameStateMachine.States
{
    class ConstructorState : BaseState
    {
        [Inject]
        public ConstructorState(
            IStateMachine stateMachine,
            GameStateFactory stateFactory,
            ILevelLoader levelLoader,
            IMessenger messenger,
            IShip ship,
            ExitSignal exitSignal,
            ShipSelectedSignal shipSelectedSignal)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _ship = ship;
            _messenger = messenger;
            _exitSignal = exitSignal;
            _exitSignal.Event += OnExit;
            _shipSelectedSignal = shipSelectedSignal;
            _shipSelectedSignal.Event += OnShipSelected;
        }

        public override StateType Type { get { return StateType.Constructor; } }

        protected override LevelName RequiredLevel { get { return LevelName.Constructor; } }

        protected override void OnLevelLoaded()
        {
            OnShipSelected(_ship);
        }

        private void OnExit()
        {
            if (IsActive)
                StateMachine.UnloadActiveState();
        }

        private void OnShipSelected(IShip ship)
        {
            if (IsActive)
            {
                _ship = ship;
                _messenger.Broadcast(EventType.ConstructorShipChanged, _ship);
            }
        }

        private IShip _ship;
        private readonly ExitSignal _exitSignal;
        private readonly ShipSelectedSignal _shipSelectedSignal;
        private readonly IMessenger _messenger;

        public class Factory : Factory<IShip, ConstructorState> { }
    }

    public class ShipSelectedSignal : SmartWeakSignal<IShip>
    {
        public class Trigger : TriggerBase { }
    }
}
