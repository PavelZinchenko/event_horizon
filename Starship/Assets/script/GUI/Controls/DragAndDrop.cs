using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public interface IDragAndDropInitializer
{
	bool TryInitializeDragAndDrop(GameObject sender, PointerEventData data);
}

[AddComponentMenu("UI/DragAndDrop", 1000)]
public class DragAndDrop : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public RectTransform Container;
	public bool Interactable = true;
	public MonoBehaviour Initializer;

	public void OnDrag(PointerEventData data)
	{
		if (!_pressed || data.pointerId != _touchId)
			return;

		var delta = data.position - _position;
		_position = data.position;

		delta.x *= _scalex;
		delta.y *= _scaley;

		RectTransform.anchoredPosition += delta;
		CheckBounds();

		OnMove(data.position);
	}
	
	public void OnPointerDown(PointerEventData data)
	{
		if (_pressed || !Interactable)
			return;

		var initializer = Initializer as IDragAndDropInitializer;
		if (initializer != null && !initializer.TryInitializeDragAndDrop(gameObject, data))
			return;

		_pressed = true;
		_touchId = data.pointerId;
		_position = data.position;
		OnPressed(_position);
	}
    
	public void OnPointerUp(PointerEventData data)
	{
		if (_pressed && data.pointerId == _touchId)
		{
			_pressed = false;
			OnReleased(data.position);
		}
	}

	protected virtual void OnReleased(Vector2 position) {}
	protected virtual void OnPressed(Vector2 position) {}
	protected virtual void OnMove(Vector2 position) {}

	protected override void OnRectTransformDimensionsChange()
	{
		var corners = new Vector3[4];
		RectTransform.GetWorldCorners(corners);
		var width = Vector2.Distance(corners[1], corners[2]);
		var height = Vector2.Distance(corners[0], corners[1]);
		_scalex = RectTransform.rect.width / width;
		_scaley = RectTransform.rect.height / height;
		CheckBounds();
    }

	protected virtual void CheckBounds()
	{
		var container = Container.rect;
		var rect = RectTransform.rect;
		var min = rect.min += RectTransform.anchoredPosition;
		var max = rect.max += RectTransform.anchoredPosition;

		if (rect.width <= 0 || rect.height <= 0 || container.width <= 0 || container.height <= 0)
			return;

		if (min.x < 0)
		{
			var position = RectTransform.anchoredPosition;
			position.x -= min.x;
			RectTransform.anchoredPosition = position;
		}
		else if (max.x > container.width)
		{
			var position = RectTransform.anchoredPosition;
			position.x -= max.x - container.width;
			RectTransform.anchoredPosition = position;
        }

        if (min.y < 0)
		{
			var position = RectTransform.anchoredPosition;
			position.y -= min.y;
			RectTransform.anchoredPosition = position;
        }
		else if (max.y > container.height)
		{
			var position = RectTransform.anchoredPosition;
			position.y -= max.y - container.height;
			RectTransform.anchoredPosition = position;
        }
    }
    
	protected RectTransform RectTransform
	{
		get
		{
			if (_rectTransform == null)
				_rectTransform = GetComponent<RectTransform>();
			return _rectTransform;
		}
	}
	
	private int _touchId;
	private bool _pressed;
	private Vector2 _position;
	private float _scalex;
	private float _scaley;
	private RectTransform _rectTransform;
}
