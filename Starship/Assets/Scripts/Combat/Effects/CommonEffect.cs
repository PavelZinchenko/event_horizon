using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(Renderer))]
    public class CommonEffect : EffectBase
    {
        [SerializeField] private bool _lifeAffectsOpacity = true;
        [SerializeField] private bool _lifeAffectsSize = false;

        protected override void OnInitialize() {}
        protected override void OnDispose() {}
        protected override void OnGameObjectDestroyed() {}

        protected override void UpdateLife()
        {
            if (_lifeAffectsOpacity)
                Opacity = Life;
            if (_lifeAffectsSize)
                Scale = Life;
        }
    }
}
