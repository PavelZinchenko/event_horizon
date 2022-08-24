using System;

namespace Combat.Component.Triggers
{
    public interface IUnitAction : IDisposable
    {
        ConditionType TriggerCondition { get; }
        bool TryUpdateAction(float elapsedTime);
        bool TryInvokeAction(ConditionType condition);
    }
}
