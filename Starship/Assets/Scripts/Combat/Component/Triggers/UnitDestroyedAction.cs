using Services.Messenger;

namespace Combat.Component.Triggers
{
    public class UnitDestroyedAction : IUnitAction
    {
        public UnitDestroyedAction(IMessenger messenger, int unitId)
        {
            _messenger = messenger;
            _unitId = unitId;
        }

        public ConditionType TriggerCondition => ConditionType.OnDestroy;

        public bool TryUpdateAction(float elapsedTime) { return false; }

        public bool TryInvokeAction(ConditionType condition)
        {
            _messenger.Broadcast(EventType.ObjectiveDestroyed, _unitId);
            return false;
        }

        public void Dispose() { }

        private readonly IMessenger _messenger;
        private readonly int _unitId;
    }
}
