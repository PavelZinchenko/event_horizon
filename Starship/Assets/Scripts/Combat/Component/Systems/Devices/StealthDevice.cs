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
    public class StealthDevice : SystemBase, IDevice, IFeaturesModification, ISystemsModification, IUnitAction
    {
        public StealthDevice(IShip ship, DeviceStats deviceSpec, int keyBinding, bool invulnerability)
            : base(keyBinding, deviceSpec.ControlButtonIcon)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _energyCost = deviceSpec.EnergyConsumption;
            _invulnerability = invulnerability;
        }

        public override float ActivationCost { get { return _energyCost; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && _ship.Stats.Energy.Value >= _energyCost; } }
        
        public override IFeaturesModification FeaturesModification { get { return this; } }
        public bool TryApplyModification(ref FeaturesData data)
        {
            data.Opacity *= _opacity;

            if (_isEnabled)
            {
                //data.Invulnerable |= _invulnerability;
                data.TargetPriority = TargetPriority.None;
            }

            return true;
        }

        public override ISystemsModification SystemsModification { get { return this; } }
        public bool IsAlive { get { return true; } }
        public bool CanActivateSystem(ISystem system) { return true; }
        public void OnSystemActivated(ISystem system)
        {
            var deactivate = true;

            if (system == this)
                deactivate = false;
            else if (system is GhostDevice)
                deactivate = false;
            else if (system is PointDefenseSystem)
                deactivate = false;
            else if (system is EnergyShieldDevice)
                deactivate = false;
            else if (system is RepairSystem)
                deactivate = false;
            else if (system is FortificationDevice)
                deactivate = false;
            else if (system is IDroneBay)
                deactivate = false;

            if (deactivate)
                Deactivate();
        }


        public override IUnitAction UnitAction { get { return this; } }
        public ConditionType TriggerCondition { get { return ConditionType.OnHit; } }
        public bool TryUpdateAction(float elapsedTime) { return false; }
        public bool TryInvokeAction(ConditionType condition)
        {
            if (!_invulnerability)
                Deactivate();
            return false;
        }

        public void Deactivate()
        {
            TimeFromLastUse = 0;

            if (_isEnabled)
            {
                _isEnabled = false;
                InvokeTriggers(ConditionType.OnDeactivate);
            }
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && !_isEnabled && _ship.Stats.Energy.TryGet(_energyCost))
            {
                Activate();
            }
        }

        protected override void OnUpdateView(float elapsedTime)
        {
            var opacity = _isEnabled ? _ship.Type.Side == UnitSide.Player ? 0.4f : 0.05f : 1.0f;
            _opacity = Mathf.MoveTowards(_opacity, opacity, elapsedTime);
        }

        protected override void OnDispose() { }

        private void Activate()
        {
            InvokeTriggers(ConditionType.OnActivate);
            _isEnabled = true;
        }

        private bool _isEnabled;
        private float _opacity = 1.0f;
        private readonly bool _invulnerability;
        private readonly float _energyCost;
        private readonly IShip _ship;
    }
}
