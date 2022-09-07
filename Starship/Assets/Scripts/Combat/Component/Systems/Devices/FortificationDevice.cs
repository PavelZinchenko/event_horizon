using Combat.Collision;
using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Stats;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class FortificationDevice : ContinuouslyActivatedDevice, IFeaturesModification, IStatsModification
    {
        public FortificationDevice(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship, deviceSpec.Lifetime, deviceSpec.EnergyConsumption)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _activeColor = deviceSpec.Color;
            _power = deviceSpec.Power;
            if (_power == 0) _power = 0.5f;
        }

        public override IStatsModification StatsModification => this;

        public bool TryApplyModification(ref Resistance data)
        {
            if (IsEnabled)
            {
                var multiplier = _power > 1 ? 0 : 1 - _power;
                data.Energy = _power + data.Energy * multiplier;
                data.Heat = _power + data.Heat * multiplier;
                data.Kinetic = _power + data.Kinetic * multiplier;
            }

            return true;
        }

        public override IFeaturesModification FeaturesModification => this;

        public bool TryApplyModification(ref FeaturesData data)
        {
            data.Color *= _color;
            return true;
        }

        protected override void OnUpdateView(float elapsedTime)
        {
            var color = IsEnabled ? _activeColor : Color.white;
            _color = Color.Lerp(_color, color, 5 * elapsedTime);
        }

        protected override void OnDispose() { }

        private Color _color = Color.white;
        private readonly Color _activeColor;
        private readonly float _power;
    }
}
