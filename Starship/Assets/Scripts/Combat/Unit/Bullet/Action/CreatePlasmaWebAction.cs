using Combat.Collision;
using Combat.Component.Unit;
using Combat.Factory;
using GameDatabase.Enums;

namespace Combat.Component.Bullet.Action
{
    public class CreatePlasmaWebAction : IAction
    {
        public CreatePlasmaWebAction(IUnit unit, SpaceObjectFactory factory, DamageType damageType, float damage, float radius, float deceleration, float lifetime, ConditionType condition = ConditionType.OnDetonate)
        {
            _factory = factory;
            _unit = unit;
            _damageType = damageType;
            _damage = damage;
            _radius = radius;
            _lifetime = lifetime;
            _deceleration = deceleration;
            Condition = condition;
        }

        public ConditionType Condition { get; private set; }

        public void Dispose() { }

        public CollisionEffect Invoke()
        {
            _factory.CreateWeb(_unit.GetHitPoint(), _radius, _lifetime, _damageType, _damage, _deceleration, _unit.Type.Side, _unit.View.Color);
            return CollisionEffect.None;
        }

        private readonly DamageType _damageType;
        private readonly float _damage;
        private readonly float _radius;
        private readonly float _lifetime;
        private readonly float _deceleration;
        private readonly IUnit _unit;
        private readonly SpaceObjectFactory _factory;
    }
}
