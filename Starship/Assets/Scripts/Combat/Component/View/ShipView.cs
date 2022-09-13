using UnityEngine;

namespace Combat.Component.View
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShipView : BaseView
    {
        [SerializeField] private Material HsvMaterial;
        [SerializeField] private Material DefaultMaterial;
        [SerializeField] private SpriteRenderer[] _extraRenderers;

        public override void ApplyHsv(float hue, float saturation)
        {
            if (!HsvMaterial) return;

            if (!_hsvMaterialInstance)
                _hsvMaterialInstance = Instantiate(HsvMaterial);

            _hsvMaterialInstance.SetColor(HsvaAdjust, new Color(hue, saturation, 0));

            Renderer.sharedMaterial = _hsvMaterialInstance;

            if (_extraRenderers != null && _extraRenderers.Length > 0)
                foreach (var item in _extraRenderers)
                    item.sharedMaterial = _hsvMaterialInstance;
        }

        public override void Dispose()
        {
            if (this && DefaultMaterial)
            {
                Renderer.sharedMaterial = DefaultMaterial;
                _renderer = null;

                if (_extraRenderers != null && _extraRenderers.Length > 0)
                    foreach (var item in _extraRenderers)
                        item.sharedMaterial = DefaultMaterial;
            }
        }

        protected override void OnGameObjectCreated() {}

        protected override void OnGameObjectDestroyed()
        {
            if (_hsvMaterialInstance)
                Destroy(_hsvMaterialInstance);
        }

        protected override void UpdateLife(float life) {}
        protected override void UpdatePosition(Vector2 position) {}
        protected override void UpdateRotation(float rotation) {}
        protected override void UpdateSize(float size) {}

        protected override void UpdateColor(Color color)
        {
            Renderer.color = color;
        }

        protected Material _hsvMaterialInstance;
        private static readonly int HsvaAdjust = Shader.PropertyToID("_HSVAAdjust");
        private SpriteRenderer _renderer;
        // ReSharper disable once Unity.NoNullCoalescing
        private SpriteRenderer Renderer => _renderer ?? (_renderer = GetComponent<SpriteRenderer>());
    }
}
