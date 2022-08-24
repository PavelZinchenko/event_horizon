using Combat.Component.Unit;

namespace Combat.Collision.Manager
{
    public interface ICollisionManager
    {
        void OnCollision(IUnit first, IUnit second, CollisionData collision);
        //void OnCollisionExit(IUnit first, IUnit second, CollisionData collision);
        //void OnCollisionStay(IUnit first, IUnit second, CollisionData collision);
    }
}
