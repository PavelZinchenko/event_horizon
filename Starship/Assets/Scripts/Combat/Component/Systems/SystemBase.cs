using Combat.Component.Features;
using Combat.Component.Engine;
using Combat.Component.Stats;
using Combat.Component.Triggers;
using Combat.Unit;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Systems
{
    public abstract class SystemBase : ISystem
    {
        protected SystemBase(int keyBinding, SpriteId controlButtonIcon)
        {
            _controlButtonIcon = controlButtonIcon;
            _keyBinding = keyBinding;
            Enabled = true;
            TimeFromLastUse = 60f;
        }

        public bool Enabled { get; set; }
        public bool Active { get; set; }

        public virtual float ActivationCost { get { return 0; } }
        public virtual bool CanBeActivated { get { return Enabled && TimeFromLastUse >= MaxCooldown; } }
        public virtual float Cooldown { get { return !Enabled ? 1.0f : MaxCooldown > 0 ? Mathf.Clamp01((MaxCooldown - TimeFromLastUse)/MaxCooldown) : 0f; } }

        public int KeyBinding { get { return _keyBinding; } }
        public SpriteId ControlButtonIcon { get { return _controlButtonIcon; } }

        public virtual IEngineModification EngineModification { get { return null; } }
        public virtual IFeaturesModification FeaturesModification { get { return null; } }
        public virtual ISystemsModification SystemsModification { get { return null; } }
        public virtual IStatsModification StatsModification { get { return null; } }
        public virtual IUnitAction UnitAction { get { return null; } }

        public void UpdatePhysics(float elapsedTime)
        {
            TimeFromLastUse += elapsedTime;
            OnUpdatePhysics(elapsedTime);
            _triggers.UpdatePhysics(elapsedTime);
        }

        public void UpdateView(float elapsedTime)
        {
            OnUpdateView(elapsedTime);
            _triggers.UpdateView(elapsedTime);
        }

        public virtual void OnEvent(SystemEventType eventType) { }

        public void Dispose()
        {
            _triggers.Dispose();
            OnDispose();
        }

        public void AddTrigger(IUnitEffect effect)
        {
            _triggers.Add(effect);
        }

        public void AddTrigger(IUnitAction action)
        {
            _triggers.Add(action);
        }

        protected abstract void OnUpdateView(float elapsedTime);
        protected abstract void OnUpdatePhysics(float elapsedTime);
        protected abstract void OnDispose();

        protected float TimeFromLastUse { get; set; }
        protected float MaxCooldown { get; set; }

        protected void InvokeTriggers(ConditionType condition)
        {
            _triggers.Invoke(condition);
        }

        private readonly int _keyBinding;
        private readonly SpriteId _controlButtonIcon;
        private readonly UnitTriggers _triggers = new UnitTriggers();
    }
}
