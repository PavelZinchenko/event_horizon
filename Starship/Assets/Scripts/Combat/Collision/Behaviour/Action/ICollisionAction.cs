using Combat.Collision.Manager;
using Combat.Component.Unit;

namespace Combat.Collision.Behaviour
{
    public interface ICollisionAction
    {
        void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact);
        void Dispose();
    }
}
