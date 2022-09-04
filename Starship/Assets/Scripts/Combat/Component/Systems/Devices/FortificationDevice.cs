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
        }

        public override IStatsModification StatsModification => this;

        public bool TryApplyModification(ref Resistance data)
        {
            if (IsEnabled)
            {
                data.Energy = 0.5f + data.Energy * 0.5f;
                data.Heat = 0.5f + data.Heat * 0.5f;
                data.Kinetic = 0.5f + data.Kinetic * 0.5f;
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
    }
}
