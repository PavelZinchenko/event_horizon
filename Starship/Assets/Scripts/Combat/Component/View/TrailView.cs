using Combat.Services;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using UnityEngine;
using Zenject;

namespace Combat.Component.View
{
    public class TrailView : BaseView
    {
        [Inject] private readonly TrailRendererPool _trailRendererPool;

        [SerializeField] private float _duration = 1.0f;
        [SerializeField] private float _alpha = 1.0f;
        [SerializeField] private bool _useObjectScale = false;
        [SerializeField] private Color _baseColor = Color.white;
        [SerializeField] private ColorMode _colorMode = ColorMode.TakeFromOwner;

        public void Initialize(Color color, ColorMode colorMode)
        {
            _baseColor = color;
            _colorMode = colorMode;
        }

        public override void Dispose()
        {
            if (_trailRenderer != null)
                _trailRendererPool.ReleaseTrailRenderer(_trailRenderer);

            _trailRenderer = null;
        }

        public override void UpdateView(float elapsedTime)
        {
            base.UpdateView(elapsedTime);

            if (_trailRenderer == null)
                return;

            var position = (Vector2)transform.position;
            if (Vector2.Distance(_lastPosition, position) > 1.0f)
                _trailRenderer.Clear();

            _lastPosition = position;
        }

        protected override void OnGameObjectCreated() { }
        protected override void OnGameObjectDestroyed() { }

        protected override void UpdateLife(float life)
        {
            if (life > 0.01f)
            {
                _lastActivationTime = Time.time;

                if (_trailRenderer == null)
                {
                    _trailRenderer = _trailRendererPool.CreateTrailRenderer(transform, TrailSize, TrailSize*0.5f, _trailColor, _duration);
                }
            }
            else if (Time.time - _lastActivationTime > 0.1f)
            {
                if (_trailRenderer != null)
                {
                    _trailRendererPool.ReleaseTrailRenderer(_trailRenderer);
                    _trailRenderer = null;
                }
            }
        }

        protected override void UpdatePosition(Vector2 position) {}
        protected override void UpdateRotation(float rotation) {}

        protected override void UpdateSize(float size)
        {
            _trailSize = size;
            if (_trailRenderer != null)
            {
                _trailRenderer.startWidth = TrailSize;
                _trailRenderer.endWidth = TrailSize*0.5f;
            }
        }

        protected override void UpdateColor(Color color)
        {
            _trailColor = _colorMode.Apply(_baseColor, color);
            _trailColor.a *= _alpha;
            if (_trailRenderer != null)
                _trailRenderer.material.color = _trailColor;
        }

        private float TrailSize { get { return _useObjectScale ? _trailSize * transform.lossyScale.z : _trailSize; } }

        private Color _trailColor = Color.white;
        private float _trailSize = 1.0f;
        private Vector2 _lastPosition;
        private float _lastActivationTime;
        private TrailRenderer _trailRenderer;
    }
}
