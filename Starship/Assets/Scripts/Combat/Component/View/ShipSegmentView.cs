using UnityEngine;

namespace Combat.Component.View
{
    public sealed class ShipSegmentView : BaseView
    {
        [SerializeField] private Material HsvMaterial;
        [SerializeField] private Material DefaultMaterial;

        [SerializeField] private GameObject _normalView;
        [SerializeField] private GameObject _wreckView;

        [SerializeField] private SpriteRenderer[] _renderers;

        protected override void OnGameObjectCreated()
        {
            _activeView = _normalView;
            _normalView.SetActive(true);
            _wreckView.SetActive(false);
        }

        public override void ApplyHsv(float hue, float saturation)
        {
            if (!HsvMaterial) return;

            if (!_hsvMaterialInstance)
                _hsvMaterialInstance = Instantiate(HsvMaterial);

            _hsvMaterialInstance.SetColor("_HSVAAdjust", new Color(hue, saturation, 0));

            if (_renderers != null && _renderers.Length > 0)
                foreach (var item in _renderers)
                    item.sharedMaterial = _hsvMaterialInstance;
        }

        public override void Dispose()
        {
            if (this && DefaultMaterial && _renderers != null && _renderers.Length > 0)
                foreach (var item in _renderers)
                    item.sharedMaterial = DefaultMaterial;
        }

        protected override void OnGameObjectDestroyed()
        {
            if (_hsvMaterialInstance)
                Destroy(_hsvMaterialInstance);
        }

        protected override void UpdateLife(float life)
        {
            if (life > 0 && _activeView != _normalView)
            {
                _wreckView.SetActive(false);
                _normalView.SetActive(true);
                _activeView = _normalView;
            }

            if (life <= 0 && _activeView != _wreckView)
            {
                _wreckView.SetActive(true);
                _normalView.SetActive(false);
                _activeView = _wreckView;
            }
        }

        protected override void UpdatePosition(Vector2 position) {}
        protected override void UpdateRotation(float rotation) {}
        protected override void UpdateSize(float size) {}
        protected override void UpdateColor(Color color) {}

        private GameObject _activeView;
        private Material _hsvMaterialInstance;
    }
}
