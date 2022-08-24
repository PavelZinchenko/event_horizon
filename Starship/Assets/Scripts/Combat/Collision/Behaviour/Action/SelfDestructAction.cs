using Combat.Collision.Manager;
using Combat.Component.Unit;

namespace Combat.Collision.Behaviour.Action
{
    public class SelfDestructAction : ICollisionAction
    {
        public SelfDestructAction(int hitsBeforeDestroy = 1)
        {
            _hits = hitsBeforeDestroy;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (!collisionData.IsNew)
                return;

            _hits--;
            if (_hits <= 0)
                selfImpact.Effects |= CollisionEffect.Destroy;
        }

        public void Dispose() { }

        private int _hits;
    }
}
