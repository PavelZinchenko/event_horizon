using Combat.Scene;
using UnityEngine;

namespace Combat.Effects
{
    public class ShakeEffect : IEffect
    {
        public ShakeEffect(IScene scene, Transform parent)
        {
            _scene = scene;
            _parent = parent;
        }

        public bool Visible
        {
            get
            {
                return _active;
            }
            set
            {
                if (_active == value) return;

                if (value)
                {
                    var distance = Vector2.Distance(_parent.position, _scene.ViewPoint) + 20;
                    var power = _parent.localScale.z * Size;
                    _scene.Shake(Mathf.Min(2.0f * power / Mathf.Sqrt(distance), 5f));
                }

                _active = value;
            }
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Size { get; set; }
        public Color Color { get; set; }
        public float Life { get; set; }
        public bool IsAlive { get { return true; } }

        public void Run(float lifetime, Vector2 velocity, float angularVelocity)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose() {}

        private bool _active;
        private readonly float _power;
        private readonly Transform _parent;
        private readonly IScene _scene;
    }
}
