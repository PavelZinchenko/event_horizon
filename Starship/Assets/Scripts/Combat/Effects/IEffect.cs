using UnityEngine;

namespace Combat.Effects
{
    public interface IEffect
    {
        bool Visible { get; set; }
        Vector2 Position { get; set; }
        float Rotation { get; set; }
        float Size { get; set; }
        Color Color { get; set; }
        float Life { get; set; }

        bool IsAlive { get; }
        void Run(float lifetime , Vector2 velocity, float angularVelocity);
        void Dispose();
    }
}
