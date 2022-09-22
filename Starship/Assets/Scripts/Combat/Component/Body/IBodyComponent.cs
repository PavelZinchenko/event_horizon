using UnityEngine;

namespace Combat.Component.Body
{
    public interface IBodyComponent : IBody
    {
        void Initialize(IBody parent, Vector2 position, float rotation, float scale, Vector2 velocity,
            float angularVelocity, float weight, bool frozen = false);
    }
}
