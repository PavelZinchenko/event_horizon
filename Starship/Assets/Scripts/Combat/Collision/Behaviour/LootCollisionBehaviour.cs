using Combat.Collision.Manager;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;

namespace Combat.Collision.Behaviour
{
    public class LootCollisionBehaviour : ICollisionBehaviour
    {
        public void Process(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (collisionData.IsNew && target.Type.Side == UnitSide.Player)
                if (target.Type.Class == UnitClass.Ship || target.Type.Class == UnitClass.Shield || target.Type.Class == UnitClass.Limb)
                    selfImpact.Effects |= CollisionEffect.Activate;
        }

        public void Dispose()
        {
        }
    }
}
