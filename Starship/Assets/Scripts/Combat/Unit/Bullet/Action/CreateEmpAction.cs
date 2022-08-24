using Combat.Collision;
using Combat.Component.Unit;
using Combat.Factory;
using GameDatabase.Enums;

namespace Combat.Component.Bullet.Action
{
    public class CreateEmpAction : IAction
    {
        public CreateEmpAction(IUnit unit, SpaceObjectFactory factory, DamageType damageType, float damage, float shieldDamage, float energyDrain, float radius, ConditionType condition = ConditionType.OnDetonate)
        {
            _factory = factory;
            _unit = unit;
            _damageType = damageType;
            _damage = damage;
            _shieldDamage = shieldDamage;
            _energyDrain = energyDrain;
            _radius = radius;
            Condition = condition;
        }

        public ConditionType Condition { get; private set; }

        public void Dispose() { }

        public CollisionEffect Invoke()
        {
            var position = _unit.GetHitPoint();
            _factory.CreateEmp(position, _radius, _damageType, _damage, _shieldDamage, _energyDrain, _unit.Type.Side, _unit.View.Color);
            return CollisionEffect.None;
        }

        private readonly DamageType _damageType;
        private readonly float _damage;
        private readonly float _radius;
        private readonly float _shieldDamage;
        private readonly float _energyDrain;
        private readonly IUnit _unit;
        private readonly SpaceObjectFactory _factory;
    }
}
