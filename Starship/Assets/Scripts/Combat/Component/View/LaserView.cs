using GameDatabase.Enums;
using GameDatabase.Extensions;
using UnityEngine;

namespace Combat.Component.View
{
    [RequireComponent(typeof(LineRenderer))]
    public class LaserView : BaseView
    {
        [SerializeField] private float _alphaScale = 1.0f;
        [SerializeField] private float _thickness = 1.0f;
        [SerializeField] private float _borderSize = 0.2f;
        [SerializeField] private Color _startColor = Color.white;
        [SerializeField] private Color _endColor = Color.white;
        [SerializeField] private Color _baseColor = Color.white;
        [SerializeField] private ColorMode _colorMode = ColorMode.TakeFromOwner;
        
        public void Initialize(Color baseColor, ColorMode colorMode)
        {
            _baseColor = baseColor;
            _colorMode = colorMode;
        }

        public override void Dispose() {}

        public float BorderSize { get { return _borderSize; } set { _borderSize = value; } }
        public float Thickness { get { return _thickness; } set { _thickness = value; } }

        protected override void OnGameObjectCreated()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 4;
            _lineRenderer.enabled = false;
        }

        protected override void OnGameObjectDestroyed() {}

        protected override void UpdateLife(float life)
        {
            Opacity = Life;
        }

        protected override void UpdatePosition(Vector2 position) {}
        protected override void UpdateRotation(float rotation) {}

        protected override void UpdateSize(float size)
        {
            var scale = transform.localScale.z;
            UpdateLine(size/scale, _thickness*scale);
        }

        protected override void UpdateColor(Color color)
        {
            if (!_lineRenderer) return;

            color = _colorMode.Apply(_baseColor, color);
            color.a *= _alphaScale;
            _lineRenderer.startColor = color * _startColor;
            _lineRenderer.endColor = color * _endColor;
        }

        private void UpdateLine(float size, float thikness)
        {
            if (!_lineRenderer) return;

            if (size < 2*_borderSize)
            {
                _lineRenderer.enabled = false;
                return;
            }

            _lineRenderer.enabled = true;
            _lineRenderer.startWidth = thikness;
            _lineRenderer.endWidth = thikness;
            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, new Vector3(_borderSize,0,0));
            _lineRenderer.SetPosition(2, new Vector3(size - _borderSize,0,0));
            _lineRenderer.SetPosition(3, new Vector3(size, 0, 0));
        }

        private LineRenderer _lineRenderer;
    }
}
