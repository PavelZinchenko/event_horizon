using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ViewModel
{
    public class Zoom : MonoBehaviour, IEventSystemHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IScrollHandler
    {
        [SerializeField] private float _minValue = 0.4f;
        [SerializeField] private float _maxValue = 1.0f;
        [SerializeField] private bool _keepPosition = true;
        [SerializeField] private Transform _target;
        [SerializeField] private Transform[] _additionalTargets;

        public float MinValue { get { return _minValue; } }
        public float MaxValue { get { return _maxValue; } }

        public float Value
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = Mathf.Clamp(value, _minValue, _maxValue);
                UpdateScale();
            }
        }

        public void OnDrag(PointerEventData data)
        {
            _touches[data.pointerId] = data.position;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            OnDrag(data);
            _touchZoomDistance = 0;
        }

        public void OnEndDrag(PointerEventData data)
        {
            _touches.Remove(data.pointerId);
            _touchZoomDistance = 0;
        }

        public void OnScroll(PointerEventData data)
        {
            _zoom -= 0.1f * (data.scrollDelta.y + data.scrollDelta.x);
        }

        private Vector2 AveragePosition
        {
            get
            {
                Vector2 position = Vector2.zero;
                var count = _touches.Count;
                if (count > 0)
                {
                    foreach (var item in _touches.Values)
                        position += item;
                    position /= _touches.Count;
                }

                return position;
            }
        }

        private void OnEnable()
        {
            if (!_target)
                _target = transform;
            _scale = _target.localScale.z;
            _touches.Clear();
        }

        private void Update()
        {
            var count = _touches.Count;
            if (count > 1)
            {
                var averagePosition = AveragePosition;
                var lastDistance = _touchZoomDistance;
                _touchZoomDistance = _touches.Values.Sum(item => Vector2.Distance(item, averagePosition)) / count;
                if (lastDistance > 0.001f)
                    _zoom -= _touchZoomDistance / lastDistance - 1;
            }

            if (Mathf.Abs(_zoom) > 0.001f)
            {
                var delta = Time.unscaledDeltaTime * 5;
                UpdateZoom(1f + _zoom * delta);
                _zoom *= 1 - delta;
            }
        }

        private void UpdateZoom(float value)
        {
            var currentScale = _target.localScale.z;
            if (Mathf.Abs(currentScale - _scale) > 0.01f)
            {
                _zoom = 0;
                _scale = currentScale;
                return;
            }

            var oldScale = _scale;
            _scale = Mathf.Clamp(_scale / value, _minValue, _maxValue);

            if (_keepPosition)
            {
                var position = _target.localPosition;
                _target.localPosition = position * (_scale / oldScale);
            }

            UpdateScale();
        }

        private void UpdateScale()
        {
            if (float.IsNaN(_scale))
                _scale = _maxValue;

            var scaleVector = Vector3.one * _scale;
            _target.localScale = scaleVector;

            if (_additionalTargets != null)
                foreach (var target in _additionalTargets)
                    target.localScale = scaleVector;
        }

        private readonly Dictionary<int, Vector2> _touches = new Dictionary<int, Vector2>();
        private float _scale = 1.0f;
        private float _zoom;
        private float _touchZoomDistance;
    }
}
