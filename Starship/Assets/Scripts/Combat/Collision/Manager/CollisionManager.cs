using Combat.Component.Unit;
using Combat.Unit;

namespace Combat.Collision.Manager
{
    public class CollisionManager : ICollisionManager
    {
        public void OnCollision(IUnit first, IUnit second, CollisionData collision)
        {
            ProcessCollision(first, second, collision);
        }

        //public void OnCollisionExit(IUnit first, IUnit second, CollisionData collision)
        //{
        //}

        //public void OnCollisionStay(IUnit first, IUnit second, CollisionData collision)
        //{
        //    ProcessCollision(first, second, collision);
        //}

        private void ProcessCollision(IUnit first, IUnit second, CollisionData collisionData)
        {
            var behaviour = first.CollisionBehaviour;
            if (behaviour == null)
                return;

            if (!first.IsActive() || !second.IsActive())
                return;

            var selfImpact = new Impact();
            var targetImpact = new Impact();

            behaviour.Process(first, second, collisionData, ref selfImpact, ref targetImpact);

            first.OnCollision(selfImpact, second, collisionData);
            second.OnCollision(targetImpact, first, collisionData);
        }
    }
}
