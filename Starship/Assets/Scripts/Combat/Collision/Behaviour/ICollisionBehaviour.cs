using System;
using Combat.Collision.Manager;
using Combat.Component.Unit;

namespace Combat.Collision.Behaviour
{
    public interface ICollisionBehaviour : IDisposable
    {
        void Process(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact);
    }
}
