using UnityEngine;

public static class TouchInput
{
	static TouchInput()
	{
		Input.simulateMouseWithTouches = false;
	}

	public static int TouchCount
	{
		get
		{
			var count = Input.touchCount;
			if (count > 0)
				return count;

			return IsMouseActive ? 1 : 0;
		}
	}

	public static TouchInfo GetTouchInfo(int id)
	{
		if (id < Input.touchCount)
			return new TouchInfo(Input.GetTouch(id));

		UpdateMouseState();

		State state;
		if (_mouseButtonPressed && _lastMouseButtonPressed)
			state = State.Drag;
		else if (_mouseButtonPressed)
			state = State.Down;
		else
			state = State.Up;

		return new TouchInfo(0, Input.mousePosition, state);
	}

	private static void UpdateMouseState()
	{
		if (_lastFrame == Time.frameCount)
			return;

		_lastFrame = Time.frameCount;

		_lastMouseButtonPressed = _mouseButtonPressed;
		_mouseButtonPressed = Input.GetMouseButton(0);
	}

	private static bool IsMouseActive
	{
		get
		{
			UpdateMouseState();
			return _lastMouseButtonPressed || _mouseButtonPressed; 
		}
	}

	public enum State
	{
		Down,
		Drag,
		Up,
	}
	
	public struct TouchInfo
	{
		public TouchInfo(Touch touch)
		{
			id = touch.fingerId;
			position = touch.position;
			if (touch.phase == TouchPhase.Began)
				state = State.Down;
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				state = State.Up;
			else
				state = State.Drag;
		}

		public TouchInfo(int id, Vector2 position, State state)
		{
			this.id = id;
			this.position = position;
			this.state = state;
		}

		public readonly int id;
		public readonly Vector2 position;
		public readonly State state;
	}

	private static int _lastFrame = -1;
	private static bool _mouseButtonPressed = false;
	private static bool _lastMouseButtonPressed = false;
}