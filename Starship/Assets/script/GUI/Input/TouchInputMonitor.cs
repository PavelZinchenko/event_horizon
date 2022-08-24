using UnityEngine;
using System.Collections.Generic;

public abstract class TouchInputMonitor : MonoBehaviour
{
	protected virtual void OnPointerDown (int pointerId, Vector2 position) {}
	protected virtual void OnPointerUp(int pointerId, Vector2 position) {}
	protected virtual void OnPointerClick(int pointerId, Vector2 position) {}
	protected virtual void OnDrag(int pointerId, Vector2 position) {}
	protected virtual void OnBeginDrag(int pointerId, Vector2 position) {}
	protected virtual void OnEndDrag(int pointerId, Vector2 position) {}
	protected virtual void OnMouseScroll(Vector2 delta) {}

	protected Vector2 AveragePosition
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

	protected Dictionary<int, Vector2> Touches { get { return _touches; } }

	protected virtual bool IsAllowedPointerPosition(int poinerId, Vector2 position) { return true; }

	protected virtual void Update()
	{
		var count = Input.touchCount;
		if (count <= 0)
		{
			if (Input.mousePresent)
			{
				var pressed = Input.GetMouseButton(0);
				if (pressed && _mousePressed)
				{
					ProcessPointerDrag(0, Input.mousePosition);
				}
				else if (pressed && !_mousePressed)
				{
					_mousePressed = true;
					ProcessPointerDown(0, Input.mousePosition);
				}
				else if (!pressed && _mousePressed)
				{
					_mousePressed = false;
					ProcessPointerUp(0, Input.mousePosition);
				}
			}

			var scroll = Input.mouseScrollDelta;
			if (scroll.x != 0f || scroll.y != 0f)
			{
				OnMouseScroll(scroll);
			}

			return;
		}

		for (int i = 0; i < count; ++i)
		{
			var touch = Input.GetTouch(i);

			switch (touch.phase)
			{
			case TouchPhase.Began:
				ProcessPointerDown(touch.fingerId, touch.position);
				break;
			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				ProcessPointerUp(touch.fingerId, touch.position);
				break;
			case TouchPhase.Moved:
				ProcessPointerDrag(touch.fingerId, touch.position);
				break;
			}
		}
	}

	private void ProcessPointerDown(int pointerId, Vector2 position)
	{
		if (!IsAllowedPointerPosition(pointerId, position))
		{
			return;
		}

		_clicks[pointerId] = position;
		OnPointerDown(pointerId, position);
	}

	private void ProcessPointerUp(int pointerId, Vector2 position)
	{
		if (_clicks.Remove(pointerId))
		{
			OnPointerClick(pointerId, position);
		}
		
		if (_touches.Remove(pointerId))
		{
			OnEndDrag(pointerId, position);
		}

		OnPointerUp(pointerId, position);
	}

	private void ProcessPointerDrag(int pointerId, Vector2 position)
	{
		if (_touches.ContainsKey(pointerId))
		{
			_touches[pointerId] = position;
			OnDrag(pointerId, position);
			return;
		}
		
		Vector2 startPosition;
		if (!_clicks.TryGetValue(pointerId, out startPosition))
		{
			return;
		}
		
		var dx = startPosition.x - position.x;
		var dy = startPosition.y - position.y;
		if (dx < DragThreshold && dx > -DragThreshold && dy < DragThreshold && dy > -DragThreshold)
		{
			return;
		}
		
		_clicks.Remove(pointerId);
		_touches[pointerId] = position;
		OnBeginDrag(pointerId, position);
	}

	private Dictionary<int, Vector2> _clicks = new Dictionary<int, Vector2>();
	private Dictionary<int, Vector2> _touches = new Dictionary<int, Vector2>();

	private bool _mousePressed;
	private const float DragThreshold = 5f;
}
