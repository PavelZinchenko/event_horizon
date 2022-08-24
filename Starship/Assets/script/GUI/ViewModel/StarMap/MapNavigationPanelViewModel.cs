using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ViewModel
{
    public class MapNavigationPanelViewModel :
        MonoBehaviour,
        IEventSystemHandler,
        IPointerClickHandler,
        IDragHandler,
        IBeginDragHandler,
        IEndDragHandler,
        IScrollHandler
    {
        public MoveEvent OnMoveEvent = new MoveEvent();
        public ClickEvent OnClickEvent = new ClickEvent();
        public ZoomEvent OnZoomEvent = new ZoomEvent();

        public void OnPointerClick(PointerEventData data)
        {
            if (!Interactable)
                return;

            if (!data.dragging)
                OnClickEvent.Invoke(Camera.main.ScreenToWorldPoint(data.position));
        }

        public void OnDrag(PointerEventData data)
        {
            _touches[data.pointerId] = data.position;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            OnDrag(data);
            _currentPosition = AveragePosition;
            _touchZoomDistance = 0;

            //Debug.LogWarning("OnBeginDrag: " + data.pointerId + " / " + string.Join(",", _touches));
        }

        public void OnEndDrag(PointerEventData data)
        {
            _touches.Remove(data.pointerId);
            _currentPosition = AveragePosition;
            _touchZoomDistance = 0;

            //Debug.LogWarning("OnEndDrag: " + data.pointerId + " / " + string.Join(",", _touches));
        }

        public void OnScroll(PointerEventData data)
        {
            _zoom -= 0.1f*(data.scrollDelta.y + data.scrollDelta.x);
        }

        public void OnWindowActivated(bool active)
        {
            _touches.Clear();
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

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            _speed *= 1 - Time.deltaTime*5;

            var count = _touches.Count;
            if (count > 0)
            {
                var lastPosition = _currentPosition;
                _currentPosition = AveragePosition;
                var delta = (Vector2) Camera.main.ScreenToWorldPoint(_currentPosition) -
                            (Vector2) Camera.main.ScreenToWorldPoint(lastPosition);
                OnMoveEvent.Invoke(delta);
                var deltaTime = Mathf.Max(0.01f, Time.deltaTime);
                _speed = _speed*0.9f + 0.2f*delta/deltaTime;

                var speedLimit = Camera.main.orthographicSize*10;
                _speed.x = Mathf.Clamp(_speed.x, -speedLimit, speedLimit);
                _speed.y = Mathf.Clamp(_speed.y, -speedLimit, speedLimit);

                if (count > 1)
                {
                    var lastDistance = _touchZoomDistance;
                    _touchZoomDistance = _touches.Values.Sum(item => Vector2.Distance(item, _currentPosition))/count;
                    if (lastDistance > 0.001f)
                        _zoom -= _touchZoomDistance/lastDistance - 1;
                }
            }
            else if (_speed.magnitude > 0.1f)
            {
                OnMoveEvent.Invoke(_speed*Time.deltaTime);
            }

            _timeLeft -= Time.unscaledDeltaTime;
            if (_timeLeft < 0)
            {
                if (Mathf.Abs(_zoom) > 0.001f)
                    OnZoomEvent.Invoke(1f + _zoom*ZoomSpeed);

                _timeLeft = ZoomCooldown;
                _zoom *= 1 - ZoomSpeed;
            }
        }

        private bool Interactable { get { return _canvasGroup ? _canvasGroup.interactable : true; } }

        [Serializable]
        public class MoveEvent : UnityEvent<Vector2>
        {
        }

        [Serializable]
        public class ClickEvent : UnityEvent<Vector2>
        {
        }

        [Serializable]
        public class ZoomEvent : UnityEvent<float>
        {
        }

        private float _timeLeft;
        private Dictionary<int, Vector2> _touches = new Dictionary<int, Vector2>();
        private float _zoom;
        private float _touchZoomDistance;
        private Vector2 _speed;
        private Vector2 _currentPosition;
        private CanvasGroup _canvasGroup;
        private const float ZoomCooldown = 0.02f;
        private const float ZoomSpeed = 0.1f;
    }
}
