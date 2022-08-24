using Combat.Collision;
using Combat.Component.Body;
using Combat.Component.Unit;
using Combat.Factory;
using GameDatabase.Enums;

namespace Combat.Component.Bullet.Action
{
    public class CreateCloudAction : IAction
    {
        public CreateCloudAction(IUnit unit, SpaceObjectFactory factory, DamageType damageType, float damage, float radius, float lifetime, ConditionType condition = ConditionType.OnDetonate)
        {
            _factory = factory;
            _unit = unit;
            _damageType = damageType;
            _damage = damage;
            _radius = radius;
            _lifetime = lifetime;
            Condition = condition;
        }

        public ConditionType Condition { get; private set; }

        public void Dispose() { }

        public CollisionEffect Invoke()
        {
            _factory.CreateCloud(_unit.GetHitPoint(), _unit.Body.WorldVelocity()*0.01f, _lifetime, _radius, _damageType, _damage, _unit.Type.Side, _unit.View.Color);
            return CollisionEffect.None;
        }

        private readonly DamageType _damageType;
        private readonly float _damage;
        private readonly float _radius;
        private readonly float _lifetime;
        private readonly IUnit _unit;
        private readonly SpaceObjectFactory _factory;
    }
}
