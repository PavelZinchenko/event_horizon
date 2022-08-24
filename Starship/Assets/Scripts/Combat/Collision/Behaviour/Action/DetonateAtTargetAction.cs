using Combat.Collision.Manager;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;

namespace Combat.Collision.Behaviour.Action
{
    public class DetonateAtTargetAction : ICollisionAction
    {
        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (!collisionData.IsNew)
                return;

            switch (target.Type.Class)
            {
                case UnitClass.Ship:
                case UnitClass.Drone:
                case UnitClass.Missile:
                case UnitClass.Shield:
                case UnitClass.Decoy:
                case UnitClass.SpaceObject:
                case UnitClass.Limb:
                case UnitClass.Loot:
                    selfImpact.Effects |= CollisionEffect.Destroy;
                    break;
            }
        }

        public void Dispose() { }

        private int _hits;
    }
}
