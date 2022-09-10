using System;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using UnityEngine;

namespace Combat.Component.View
{
    [RequireComponent(typeof(LineRenderer))]
    public class LightningView : BaseView
    {
        [SerializeField] private float _alphaScale = 1.0f;
        [SerializeField] private float _startWidth = 1.0f;
        [SerializeField] private float _endWidth = 0.1f;
        [SerializeField] private float _stepSize = 1.0f;
        [SerializeField] private float _bezierStepSize = 5.0f;
        [SerializeField] private float _bezierAmplitude = 5.0f;
        [SerializeField] private float _pinchFactor = 2.0f;
        [SerializeField] private bool _animated = false;
        [SerializeField] private Color _startColor = Color.white;
        [SerializeField] private Color _endColor = Color.white;
        [SerializeField] private Color _baseColor = Color.white;
        [SerializeField] private ColorMode _colorMode = ColorMode.TakeFromOwner;

        public float Thickness { get { return _startWidth; } set { _startWidth = value; } }

        public void Initialize(Color baseColor, ColorMode colorMode)
        {
            _baseColor = baseColor;
            _colorMode = colorMode;
        }

        public override void Dispose()
        {
            _lineLength = 0;
            if (_lineRenderer != null) _lineRenderer.enabled = false;
        }

        public override void UpdateView(float elapsedTime)
        {
            base.UpdateView(elapsedTime);
            if (_animated)
                UpdateLine();
        }

        protected override void OnGameObjectCreated()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;
            _random = new System.Random(GetHashCode());
        }

        protected override void OnGameObjectDestroyed() { }

        protected override void UpdateLife(float life)
        {
            Opacity = Life;
        }

        protected override void UpdatePosition(Vector2 position) { }
        protected override void UpdateRotation(float rotation) { }

        protected override void UpdateSize(float size)
        {
            _objectScale = transform.localScale.z;
            var oldLength = _lineLength;
            _lineLength = size/_objectScale;

            if (!_animated && !Mathf.Approximately(oldLength, _lineLength))
                UpdateLine();
        }

        protected override void UpdateColor(Color color)
        {
            if (!_lineRenderer) return;

            color = _colorMode.Apply(_baseColor, color);
            color.a *= _alphaScale;
            _lineRenderer.startColor = color * _startColor;
            _lineRenderer.endColor = color * _endColor;
        }

        private void UpdateLine()
        {
            if (!_lineRenderer) return;

            if (_lineLength <= 0)
            {
                _lineRenderer.enabled = false;
                return;
            }

            _lineRenderer.enabled = true;
            _lineRenderer.startWidth = _startWidth * _objectScale;
            _lineRenderer.endWidth = _endWidth * _objectScale;

            var steps = 1 + Mathf.FloorToInt(_lineLength/_stepSize);
            _lineRenderer.positionCount = steps + 1;

            _lineRenderer.SetPosition(0, Vector3.zero);

            var p12 = Vector2.zero;
            var p2 = new Vector2(_bezierStepSize, _bezierAmplitude*_random.NextFloatSigned());
            var p3 = new Vector2(2*_bezierStepSize, _bezierAmplitude*_random.NextFloatSigned());
            var p23 = (p2 + p3)/2;

            for (var i = 1; i <= steps; ++i)
            {
                var x = i*_lineLength/steps;

                while (x > p23.x)
                {
                    p2 = p3;
                    p3 = new Vector2(p2.x + _bezierStepSize, _bezierAmplitude*_random.NextFloatSigned());
                    p12 = p23;
                    p23 = (p2 + p3)/2;
                }

                var p = Geometry.Bezier(p12,p2,p23, (x - p12.x)/(p23.x - p12.x));
                var scale = 1f - Mathf.Pow(i/(float)steps, _pinchFactor);

                var position = new Vector3(x, p.y*scale, 0);
                _lineRenderer.SetPosition(i, position);
            }
        }

        private float _lineLength;
        private float _objectScale;

        private System.Random _random;
        private LineRenderer _lineRenderer;
    }
}
