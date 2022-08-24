using Combat.Services;
using UnityEngine;
using Zenject;

namespace Combat.Effects
{
    public class TrailEffect : EffectBase
    {
        [Inject] private readonly TrailRendererPool _trailRendererPool;
         
        [SerializeField] private float _duration = 1.0f;

        protected override void OnInitialize() {}

        protected override void OnDispose()
        {
            if (_trailRenderer != null)
                _trailRendererPool.ReleaseTrailRenderer(_trailRenderer);

            _trailRenderer = null;
        }

        protected override void OnGameObjectDestroyed() {}

        protected override void SetColor(Color color)
        {
            if (_trailRenderer != null)
                _trailRenderer.material.color = color;
        }

        protected override void UpdateLife()
        {
            if (Life > 0.01f)
            {
                _lastActivationTime = Time.time;

                if (_trailRenderer == null)
                {
                    _trailRenderer = _trailRendererPool.CreateTrailRenderer(transform, Size, Size*2, Color, _duration);
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

        protected override void OnAfterUpdate()
        {
            if (_trailRenderer == null)
                return;

            var position = (Vector2)transform.position;
            if (Vector2.Distance(_lastPosition, position) > 1.0f)
                _trailRenderer.Clear();

            _lastPosition = position;
        }

        private Vector2 _lastPosition;
        private float _lastActivationTime;
        private TrailRenderer _trailRenderer;
    }
}
