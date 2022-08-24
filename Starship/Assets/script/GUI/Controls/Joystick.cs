using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[AddComponentMenu("UI/Joystick", 1000)]
public class Joystick : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public PressedEvent Pressed = new PressedEvent();
	public UnityEvent Released = new UnityEvent();

	[Serializable]
	public class PressedEvent : UnityEvent<Vector2>
	{
	}

	public void OnDrag(PointerEventData data)
	{
		if (!_pressed || data.pointerId != _touchId)
				return;

		ProcessPointerClick(data, false);
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (!_pressed && ProcessPointerClick(data, true))
		{
			_pressed = true;
			_touchId = data.pointerId;
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		if (data.pointerId == _touchId)
		{
			_pressed = false;
			Released.Invoke();
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		var rectTransform = GetComponent<RectTransform>();
		var corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);
		_radius = Vector2.Distance(corners[0], corners[1])/2;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		transform.localEulerAngles = new Vector3(0,0,90);
		_pressed = false;
	}

	private void OnApplicationPause(bool paused)
	{
		_pressed = false;
	}	

	private bool ProcessPointerClick(PointerEventData data, bool checkDistance)
	{
		var direction = (data.position - (Vector2)transform.position)/_radius;
		if (checkDistance && direction.sqrMagnitude > 1)
			return false;

		_direction = direction;
		transform.localEulerAngles = new Vector3(0,0,RotationHelpers.Angle(direction));
		Pressed.Invoke(_direction);
		return true;
	}

	private float _radius;
	private int _touchId;
	private bool _pressed;
	private Vector2 _direction;
}
