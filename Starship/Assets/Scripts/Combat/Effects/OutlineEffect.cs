using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class OutlineEffect : EffectBase
    {
        [SerializeField] private float _effectSize;
        private static readonly int Thickness = Shader.PropertyToID("_Thickness");

        protected override void OnInitialize()
        {
            Scale = _effectSize;
            Renderer.sprite = transform.parent.GetComponent<SpriteRenderer>().sprite; // TODO: temporary fix
        }

        protected override void OnDispose() {}
        protected override void OnGameObjectDestroyed() {}

        protected override void UpdateLife()
        {
            Opacity = Life;
        }

        protected override void UpdateSize()
        {
            base.UpdateSize();

            var size = transform.lossyScale.z;
            Renderer.material.SetFloat(Thickness, Mathf.Min(5*(size + 10)/10, 10));
        }
        private SpriteRenderer _spriteRenderer;
        // ReSharper disable once Unity.NoNullCoalescing
        private SpriteRenderer Renderer => _spriteRenderer ?? (_spriteRenderer = GetComponent<SpriteRenderer>());
    }
}
