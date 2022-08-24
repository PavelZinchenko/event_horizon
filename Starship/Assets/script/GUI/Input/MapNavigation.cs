using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ViewModel
{
	public class MapNavigation : TouchInputMonitor
	{
		public MoveEvent OnMoveEvent = new MoveEvent();
		public ClickEvent OnClickEvent = new ClickEvent();
		public ZoomEvent OnZoomEvent = new ZoomEvent();

		protected override void OnBeginDrag(int pointerId, Vector2 position)
		{
			_currentPosition = AveragePosition;
			_touchZoomDistance = 0;
		}

		protected override void OnEndDrag(int pointerId, Vector2 position)
		{
			_currentPosition = AveragePosition;
			_touchZoomDistance = 0;
		}

		protected override void OnMouseScroll(Vector2 delta)
		{
			_zoom -= 0.1f*(delta.y + delta.x);
		}

		protected override void OnPointerClick(int pointerId, Vector2 position)
		{
			OnClickEvent.Invoke(Camera.main.ScreenToWorldPoint(position));
		}

		protected override bool IsAllowedPointerPosition(int pointerId, Vector2 position)
		{
#if UNITY_EDITOR
			return !EventSystem.current.IsPointerOverGameObject();
#else
			return !EventSystem.current.IsPointerOverGameObject(pointerId);
#endif
		}

		protected override void Update()
		{
			base.Update();

			_speed *= 1 - Time.deltaTime*5;

			var count = Touches.Count;
			if (count > 0)
			{
				var lastPosition = _currentPosition;
				_currentPosition = AveragePosition;
				var delta = (Vector2)Camera.main.ScreenToWorldPoint(_currentPosition) - (Vector2)Camera.main.ScreenToWorldPoint(lastPosition);
				OnMoveEvent.Invoke(delta);
				_speed = _speed*0.9f + 0.2f*delta/Time.deltaTime;
				
				if (count > 1)
				{
					var lastDistance = _touchZoomDistance;
					_touchZoomDistance = Touches.Values.Sum(item => Vector2.Distance(item, _currentPosition)) / count;
					if (lastDistance > 0.001f)
						_zoom -= _touchZoomDistance/lastDistance - 1;
				}
			}
			else if (_speed.magnitude > 0.1f)
			{
				OnMoveEvent.Invoke(_speed*Time.deltaTime);
			}
			
			if (Mathf.Abs(_zoom) > 0.001f)
			{
				var delta = Time.deltaTime*5;
				OnZoomEvent.Invoke(1f + _zoom * delta);
				_zoom *= 1 - delta;
			}
		}

		private float _zoom;
		private float _touchZoomDistance;
		private Vector2 _speed;
		private Vector2 _currentPosition;

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
	}
}
