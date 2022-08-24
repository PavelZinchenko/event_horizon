using System.Collections.Generic;
using Combat.Collision.Manager;
using Combat.Component.Unit;

namespace Combat.Collision.Behaviour
{
    public class BulletCollisionBehaviour : ICollisionBehaviour
    {
        public void Process(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            var count = _actions.Count;
            for (var i = 0; i < count; ++i)
            {
                var action = _actions[i];
                action.Invoke(self, target, collisionData, ref selfImpact, ref targetImpact);
            }
        }

        public void Dispose()
        {
            foreach (var action in _actions)
                action.Dispose();
        }

        public void AddAction(ICollisionAction action)
        {
            _actions.Add(action);
        }

        private readonly List<ICollisionAction> _actions = new List<ICollisionAction>();
    }
}
