using Combat.Collision.Manager;
using Combat.Component.Unit;
using UnityEngine;

namespace Combat.Collision.Behaviour.Action
{
    public class RollAction : ICollisionAction
    {
        public RollAction(float power)
        {
            _power = power;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (!collisionData.IsNew)
                return;

            var angularVelocity = target.Body.AngularVelocity;

            var power = _power / Mathf.Sqrt(target.Body.Weight);

            target.Body.ApplyAngularAcceleration(Mathf.Sign(angularVelocity) * power * 180f);
        }

        public void Dispose() { }

        private readonly float _power;
    }
}
