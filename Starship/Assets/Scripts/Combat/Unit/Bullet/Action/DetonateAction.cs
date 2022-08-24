using Combat.Collision;

namespace Combat.Component.Bullet.Action
{
    public class DetonateAction : IAction
    {
        public DetonateAction(ConditionType condition = ConditionType.OnDetonate)
        {
            Condition = condition;
        }

        public ConditionType Condition { get; private set; }

        public void Dispose() { }

        public CollisionEffect Invoke()
        {
            return CollisionEffect.Destroy;
        }
    }
}
