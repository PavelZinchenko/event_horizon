using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Systems.DroneBays;
using Combat.Component.Triggers;
using Combat.Component.Unit.Classification;
using GameDatabase.DataModel;
using UnityEngine;
using IUnitAction = Combat.Component.Triggers.IUnitAction;

namespace Combat.Component.Systems.Devices
{
    public class StealthDevice : ContinuouslyActivatedDevice, IFeaturesModification, ISystemsModification, IUnitAction
    {
        public StealthDevice(IShip ship, DeviceStats deviceSpec, int keyBinding, bool invulnerability)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship, deviceSpec.Lifetime, 0, deviceSpec.EnergyConsumption)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _invulnerability = invulnerability;
        }

        public override IFeaturesModification FeaturesModification => this;

        public bool TryApplyModification(ref FeaturesData data)
        {
            data.Opacity *= _opacity;

            if (IsEnabled)
            {
                //data.Invulnerable |= _invulnerability;
                data.TargetPriority = TargetPriority.None;
            }

            return true;
        }

        public override ISystemsModification SystemsModification => this;
        public bool IsAlive => true;

        public bool CanActivateSystem(ISystem system)
        {
            return true;
        }

        public void OnSystemActivated(ISystem system)
        {
            var remainActive =
                system == this
                || system is GhostDevice
                || system is PointDefenseSystem
                || system is EnergyShieldDevice
                || system is RepairSystem
                || system is FortificationDevice
                || system is IDroneBay;

            if (!remainActive)
                Deactivate();
        }


        public override IUnitAction UnitAction => this;
        public ConditionType TriggerCondition => ConditionType.OnHit;

        public bool TryUpdateAction(float elapsedTime)
        {
            return false;
        }

        public bool TryInvokeAction(ConditionType condition)
        {
            if (!_invulnerability)
                Deactivate();
            return false;
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            // Stealth device does not gets deactivated when button is released,
            // it can only deactivate when triggered, or when lifetime runs out 
            if (!IsEnabled && Active && CanBeActivated)
            {
                if (Activate())
                {
                    IsEnabled = true;
                }
            } else if (IsEnabled && !RemainActive(elapsedTime))
            {
                Deactivate();
            }
        }

        protected override void OnUpdateView(float elapsedTime)
        {
            var opacity = IsEnabled ? Ship.Type.Side == UnitSide.Player ? 0.4f : 0.05f : 1.0f;
            _opacity = Mathf.MoveTowards(_opacity, opacity, elapsedTime);
        }

        protected override void OnDispose() { }

        private float _opacity = 1.0f;
        private readonly bool _invulnerability;
    }
}
