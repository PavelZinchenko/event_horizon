using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.Model;

namespace Combat.Component.Systems.Devices
{
    public abstract class ContinuouslyActivatedDevice : SystemBase, IDevice
    {
        protected ContinuouslyActivatedDevice(int keyBinding, SpriteId controlButtonIcon, IShip ship,
            float maxActivationTime, float energyCostPerSecond, float energyCostInitial = 0) : base(keyBinding,
            controlButtonIcon)
        {
            _maxActivationTime = maxActivationTime;
            if (_maxActivationTime == 0) _maxActivationTime = float.MaxValue;
            Ship = ship;
            EnergyCostPerSecond = energyCostPerSecond;
            EnergyCostInitial = energyCostInitial;
        }

        public override float ActivationCost => EnergyCostInitial;

        public override bool CanBeActivated =>
            base.CanBeActivated && (IsEnabled || Ship.Stats.Energy.Value >= EnergyCostInitial);

        public virtual void Deactivate()
        {
            if (!IsEnabled)
                return;

            IsEnabled = false;
            TimeFromLastUse = 0;
            InvokeTriggers(ConditionType.OnDeactivate);
        }

        protected virtual bool RemainActive(float elapsedTime)
        {
            if (EnergyCostPerSecond != 0 && !Ship.Stats.Energy.TryGet(EnergyCostPerSecond * elapsedTime)) return false;
            if (_timeLeft <= 0) return false;
            _timeLeft -= elapsedTime;
            return true;
        }

        protected virtual bool Activate()
        {
            if (EnergyCostInitial != 0 && !Ship.Stats.Energy.TryGet(EnergyCostInitial)) return false;
            _timeLeft = _maxActivationTime;
            InvokeTriggers(ConditionType.OnActivate);
            return true;
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            // If not active or can't be activated, device is disabled and function exits
            if (!Active || !CanBeActivated)
            {
                if (IsEnabled)
                {
                    Deactivate();
                }

                return;
            }

            // If not enabled, try to activate
            if (!IsEnabled)
            {
                if (!Activate()) return;
                IsEnabled = true;
            }
            // Otherwise, if RemainActive method returns false, deactivate
            else if (!RemainActive(elapsedTime))
            {
                Deactivate();
            }
        }

        protected bool IsEnabled;
        protected readonly IShip Ship;
        protected readonly float EnergyCostPerSecond;
        protected readonly float EnergyCostInitial;

        private float _timeLeft;
        private readonly float _maxActivationTime;
    }
}
