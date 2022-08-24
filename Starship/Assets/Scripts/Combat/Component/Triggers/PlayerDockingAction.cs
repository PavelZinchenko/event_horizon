using Services.Messenger;

namespace Combat.Component.Triggers
{
    public class PlayerDockingAction : IUnitAction
    {
        public PlayerDockingAction(IMessenger messenger, int stationId)
        {
            _stationId = stationId;
            _messenger = messenger;
        }

        public ConditionType TriggerCondition => ConditionType.OnActivate;

        public bool TryUpdateAction(float elapsedTime)
        {
            if (--_counter > 0)
                return true;

            _messenger.Broadcast(EventType.PlayerShipUndocked, _stationId);
            return false;
        }

        public bool TryInvokeAction(ConditionType condition)
        {
            const int maxValue = 5;

            if (_counter <= 0)
                _messenger.Broadcast(EventType.PlayerShipDocked, _stationId);

            _counter = maxValue;
            return true;
        }

        public void Dispose() { }

        private int _counter;
        private readonly int _stationId;
        private readonly IMessenger _messenger;
    }
}
