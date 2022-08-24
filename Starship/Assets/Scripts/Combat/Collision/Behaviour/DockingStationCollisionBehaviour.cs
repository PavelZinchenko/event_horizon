using Combat.Collision.Manager;
using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Unit;

namespace Combat.Collision.Behaviour
{
    public class DockingStationCollisionBehaviour : ICollisionBehaviour
    {
        public virtual void Process(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            var ship = target as IShip;
            if (ship == null && target.Type.Class == UnitClass.Shield)
                ship = target.GetOwnerShip();

            if (ship == null || ship.Type.Side != UnitSide.Player || ship.Type.Class != UnitClass.Ship || ship.Type.Owner != null)
                return;

            selfImpact.Effects |= CollisionEffect.Activate;
            if (ship.Engine.Throttle < 0.1f)
                ship.Body.ApplyAcceleration(-ship.Body.Velocity*collisionData.TimeInterval*3f);
        }

        public virtual void Dispose()
        {
        }
    }
}
