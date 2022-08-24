using System;

namespace Combat.Component.Triggers
{
    public interface IUnitEffect : IDisposable
    {
        ConditionType TriggerCondition { get; }
        bool TryUpdateEffect(float elapsedTime);
        bool TryInvokeEffect(ConditionType condition);
    }
}
