using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(Renderer))]
    public class FlickerEffect : EffectBase
    {
        [SerializeField] private bool _lifeAffectsSize;
        [SerializeField] private bool _randomSize;
        [SerializeField] private bool _randomOpacity;

        protected override void OnInitialize() { }
        protected override void OnDispose() { }
        protected override void OnGameObjectDestroyed() { }

        protected override void UpdateLife() { }

        protected override void OnBeforeUpdate()
        {
            if (_randomSize || _lifeAffectsSize)
            {
                var size = _randomSize ? (0.5f + Random.value) : 1.0f;
                Scale = _lifeAffectsSize ? Life * size : size;
            }

            if (_randomOpacity)
                Opacity = (0.5f + Random.value);
        }

    }
}
