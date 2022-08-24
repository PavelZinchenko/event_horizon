using Combat.Collision;
using Combat.Component.Unit;
using Combat.Factory;

namespace Combat.Component.Bullet.Action
{
    public class CreateGravitationAction : IAction
    {
        public CreateGravitationAction(IUnit unit, SpaceObjectFactory factory, float radius, float power, float lifetime, ConditionType condition = ConditionType.None)
        {
            _factory = factory;
            _unit = unit;
            _power = power;
            _radius = radius;
            _lifetime = lifetime;
            Condition = condition;
        }

        public ConditionType Condition { get; private set; }

        public void Dispose() { }

        public CollisionEffect Invoke()
        {
            _factory.CreateGravitation(_unit, _radius, _lifetime, _power);
            return CollisionEffect.None;
        }

        private readonly float _power;
        private readonly float _radius;
        private readonly float _lifetime;
        private readonly IUnit _unit;
        private readonly SpaceObjectFactory _factory;
    }
}
