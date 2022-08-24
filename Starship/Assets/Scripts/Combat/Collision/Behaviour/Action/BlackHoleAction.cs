using Combat.Collision.Manager;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Unit.Object;

namespace Combat.Collision.Behaviour.Action
{
    public class BlackHoleAction : ICollisionAction
    {
        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (target.Type.Class == UnitClass.Loot)
            {
                //var requiredVelocity = self.Body.Position.Direction(target.Body.Position).normalized * 15f;
                //target.Body.ApplyAcceleration(requiredVelocity - target.Body.Velocity);
            }
            else if (target is Asteroid)
            {
                //targetImpact.Effects |= CollisionEffect.Destroy;
                selfImpact.Effects |= CollisionEffect.Destroy;
            }
            else if (target.Type.Class == UnitClass.Ship || target.Type.Class == UnitClass.SpaceObject ||
                     target.Type.Class == UnitClass.Shield || target.Type.Class == UnitClass.Limb)
            {
                selfImpact.Effects |= CollisionEffect.Destroy;
                target.Body.ApplyAcceleration(-target.Body.Velocity);
            }
            else
            {
                targetImpact.Effects |= CollisionEffect.Destroy;
            }
        }

        public void Dispose() {}
    }
}
