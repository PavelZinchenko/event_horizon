using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using UnityEngine;

namespace Combat.Unit
{
    public static class UnitExtensions
    {
        public static IShip GetOwnerShip(this IUnit unit)
        {
            if (unit == null)
                return null;

            var owner = unit.Type.Owner;

            if (owner == null)
                return unit.Type.Class == UnitClass.Ship ? unit as IShip : null;

            while (owner.Type.Owner != null)
                owner = owner.Type.Owner;

            return owner;
        }

        public static bool IsActive(this IUnit unit)
        {
            return unit != null && unit.State == UnitState.Active;
        }

        public static void MoveTowards(this IUnit unit, Vector2 requiredPosition, float requiredRotation, Vector2 parentVelocity, float velocityFactor = 0.75f, float angularVelocityFactor = 3.0f)
        {
            var position = unit.Body.WorldPosition();
            var direction = requiredPosition - position;
            var weightFactor = 1f / (Mathf.Sqrt(1f + unit.Body.Weight) - 1f);

            if (direction.magnitude > 50f) // TODO:
            {
                unit.Body.Move(requiredPosition);
            }
            else
            {
                var requiredVelocity = direction*velocityFactor*weightFactor;
                var velocity = unit.Body.WorldVelocity() - parentVelocity;
                unit.Body.ApplyAcceleration(requiredVelocity - velocity);
            }

            var rotation = unit.Body.WorldRotation();
            var requiredAngularVelocity = Mathf.DeltaAngle(rotation, requiredRotation) * angularVelocityFactor * weightFactor;
            var angularVelocity = unit.Body.AngularVelocity;

            unit.Body.ApplyAngularAcceleration(requiredAngularVelocity - angularVelocity);
        }
    }
}
