using Combat.Component.Body;
using Combat.Component.Triggers;
using Combat.Component.Unit;
using Combat.Factory;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class ToxicWaste : SystemBase, IDevice, IUnitAction
    {
        public ToxicWaste(IUnit unit, DeviceStats deviceSpec, SpaceObjectFactory objectFactory, float damageMultiplier)
            : base(-1, SpriteId.Empty)
        {
            _unit = unit;
            _color = deviceSpec.Color;
            _damage = deviceSpec.Power*damageMultiplier;
            _lifetime = deviceSpec.Lifetime;
            _size = deviceSpec.Size;
            _objectFactory = objectFactory;
        }

        public override bool CanBeActivated { get { return false; } }

        public override IUnitAction UnitAction { get { return this; } }
        public ConditionType TriggerCondition { get { return ConditionType.OnDestroy; } }
        public bool TryUpdateAction(float elapsedTime) { return false; }
        public bool TryInvokeAction(ConditionType condition)
        {
            _objectFactory.CreateCloud(_unit.Body.WorldPosition(), _unit.Body.WorldVelocity()*0.02f, _lifetime,
                _unit.Body.WorldScale()*_size + 1f, DamageType.Direct, _damage, _unit.Type.Side, _color);

            return false;
        }

        public void Deactivate() { }

        protected override void OnUpdatePhysics(float elapsedTime) { }
        protected override void OnUpdateView(float elapsedTime) { }
        protected override void OnDispose() { }

        private readonly float _size;
        private readonly float _damage;
        private readonly float _lifetime;
        private readonly IUnit _unit;
        private readonly Color _color;
        private readonly SpaceObjectFactory _objectFactory;
    }
}
