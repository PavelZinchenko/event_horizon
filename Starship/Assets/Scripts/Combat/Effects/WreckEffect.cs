using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(Renderer))]
    public class WreckEffect : EffectBase
    {
        protected override void OnInitialize() {}
        protected override void OnDispose() {}
        protected override void OnGameObjectDestroyed() {}

        protected override Vector2 Velocity { get { return base.Velocity * Life; } }
        protected override float AngularVelocity { get { return base.AngularVelocity * Life; } }

        protected override void UpdateLife()
        {
            Opacity = 1f - Mathf.Pow(1f - Life, 5.0f);
        }
    }
}
