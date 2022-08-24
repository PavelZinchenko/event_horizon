using UnityEngine;

namespace Combat.Effects
{
    public class EmptyEffect : IEffect
    {
        public bool Visible { get { return false; } set {} }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Size { get; set; }
        public Color Color { get; set; }
        public float Life { get; set; }
        public bool IsAlive { get; private set; }
        public void Run(float lifetime, Vector2 velocity, float angularVelocity) {}
        public void Dispose() {}
    }
}
