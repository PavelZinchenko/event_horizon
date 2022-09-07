using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class GhostDevice : ContinuouslyActivatedDevice, IFeaturesModification, ISystemsModification
    {
        public GhostDevice(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship, deviceSpec.Lifetime, deviceSpec.EnergyConsumption)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _activeColor = deviceSpec.Color;
        }

        public override float ActivationCost => EnergyCostInitial;

        public override bool CanBeActivated =>
            base.CanBeActivated && (IsEnabled || Ship.Stats.Energy.Value >= EnergyCostInitial);

        public override IFeaturesModification FeaturesModification => this;

        public bool TryApplyModification(ref FeaturesData data)
        {
            data.Color *= _color;

            if (IsEnabled)
                data.Invulnerable = true;

            return true;
        }

        public override ISystemsModification SystemsModification => this;
        public bool IsAlive => true;

        public bool CanActivateSystem(ISystem system)
        {
            return !IsEnabled || system == this || system is StealthDevice;
        }

        public void OnSystemActivated(ISystem system) { }

        protected override void OnUpdateView(float elapsedTime)
        {
            var color = IsEnabled ? _activeColor : Color.white;
            _color = Color.Lerp(_color, color, 5 * elapsedTime);
        }

        protected override void OnDispose() { }

        private Color _color = Color.white;
        private readonly Color _activeColor;
    }
}
