using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class InfinityStone : SystemBase, IDevice, IFeaturesModification
    {
        public InfinityStone(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _activeColor = deviceSpec.Color;
            _lifetime = deviceSpec.Lifetime;
        }

        public void Deactivate() { }

        public override IFeaturesModification FeaturesModification { get { return this; } }
        
        public bool TryApplyModification(ref FeaturesData data)
        {
            data.Color *= _color;

            if (_invulnerabilityTime > 0)
                data.Invulnerable = true;

            return true;
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            _invulnerabilityTime -= elapsedTime;

            if (Active && CanBeActivated)
                Activate();
        }

        public override void OnEvent(SystemEventType eventType)
        {
            if (eventType == SystemEventType.DamageTaken && !_ship.Stats.IsAlive && CanBeActivated)
                Activate();
        }

        private void Activate()
        {
            InvokeTriggers(ConditionType.OnActivate);
            TimeFromLastUse = 0;
            _ship.Stats.Energy.Get(-_ship.Stats.Energy.MaxValue);
            _ship.Stats.Armor.Get(-_ship.Stats.Armor.MaxValue);
            _ship.Stats.Shield.Get(-_ship.Stats.Shield.MaxValue);
            _invulnerabilityTime = _lifetime;
        }

        protected override void OnUpdateView(float elapsedTime)
        {
            var color = _invulnerabilityTime > 0 ? _activeColor : Color.white;
            _color = Color.Lerp(_color, color, 5 * elapsedTime);
        }

        protected override void OnDispose() { }

        private float _invulnerabilityTime;
        private Color _color = Color.white;
        private readonly float _lifetime;
        private readonly Color _activeColor;
        private readonly IShip _ship;
    }
}
