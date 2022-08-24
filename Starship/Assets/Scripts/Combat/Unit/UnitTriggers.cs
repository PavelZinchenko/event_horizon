using System;
using System.Collections.Generic;
using Combat.Component.Triggers;

namespace Combat.Unit
{
    public class UnitTriggers : IDisposable
    {
        public void Add(IUnitEffect effect)
        {
            if (effect == null)
                return;

            if (effect.TriggerCondition == ConditionType.None)
                _activeEffects.Add(effect);
            else
                _effects.Add(effect);

            _resources.Add(effect);
        }

        public void Add(IUnitAction action)
        {
            if (action == null)
                return;

            if (action.TriggerCondition == ConditionType.None)
                _activeTriggers.Add(action);
            else
                _triggers.Add(action);

            _resources.Add(action);
        }

        public void UpdatePhysics(float elapsedTime)
        {
            var needCleanup = false;
            for (var i = 0; i < _activeTriggers.Count; ++i)
            {
                if (!_activeTriggers[i].TryUpdateAction(elapsedTime))
                {
                    needCleanup = true;
                    _activeTriggers[i] = null;
                }
            }

            if (needCleanup)
            {
                _activeTriggers.RemoveAll(item => item == null);
            }
        }

        public void UpdateView(float elapsedTime)
        {
            var needCleanup = false;
            for (var i = 0; i < _activeEffects.Count; ++i)
            {
                if (!_activeEffects[i].TryUpdateEffect(elapsedTime))
                {
                    needCleanup = true;
                    _activeEffects[i] = null;
                }
            }

            if (needCleanup)
            {
                _activeEffects.RemoveAll(item => item == null);
            }
        }

        public void Invoke(ConditionType condition)
        {
            foreach (var effect in _effects)
                if (effect.TriggerCondition.Contains(condition) && effect.TryInvokeEffect(condition) && !_activeEffects.Contains(effect))
                    _activeEffects.Add(effect);
            foreach (var action in _triggers)
                if (action.TriggerCondition.Contains(condition) && action.TryInvokeAction(condition) && !_activeTriggers.Contains(action))
                    _activeTriggers.Add(action);
        }

        public void Dispose()
        {
            foreach (var item in _resources)
                item.Dispose();
            _resources.Clear();
        }

        private readonly List<IUnitEffect> _effects = new List<IUnitEffect>();
        private readonly List<IUnitAction> _triggers = new List<IUnitAction>();
        private readonly List<IUnitEffect> _activeEffects = new List<IUnitEffect>();
        private readonly List<IUnitAction> _activeTriggers = new List<IUnitAction>();
        private readonly List<IDisposable> _resources = new List<IDisposable>();
    }
}
