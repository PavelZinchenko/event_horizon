using Combat.Component.Bullet;
using UnityEngine;

namespace Combat.Component.Controller
{
    public class AuraController : IController
    {
        public AuraController(IBullet bullet, float radius, float time)
        {
            _bullet = bullet;
            _radius = radius;
            _time = time;
        }

        public void Dispose() {}

        public void UpdatePhysics(float elapsedTime)
        {
            var lifetime = _bullet.Lifetime.Value;
            _power = Mathf.Clamp01(_power + elapsedTime/_time);
            if (_power > lifetime)
                _power = lifetime;

            _bullet.Body.SetSize(2*_radius*_power);
        }

        private float _power;
        private readonly IBullet _bullet;
        private readonly float _radius;
        private readonly float _time;
    }
}
