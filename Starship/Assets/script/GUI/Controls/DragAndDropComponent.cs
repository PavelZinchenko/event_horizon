using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragAndDropComponent : DragAndDrop
{
	public RectTransform StartPosition;
	public Vector2 RectSize = new Vector2(128,128);

	public PositionChangedEvent OnPositionChanged = new PositionChangedEvent();
	public PositionChangedEvent OnComponentReleased = new PositionChangedEvent();

	[Serializable]
	public class PositionChangedEvent : UnityEvent<Vector2> {}

	protected override void OnPressed(Vector2 position)
	{
		if (!Interactable) return;

		RectTransform.SetParent(Container);
		RectTransform.anchorMin = Vector2.zero;
		RectTransform.anchorMax = Vector2.zero;

		var scale = RectTransformHelpers.GetScreenSizeScale(Container);
		position.x *= scale.x;
		position.y *= scale.y;
		_offset = new Vector2(0, 0/*RectSize.y/3*/);

		RectTransform.offsetMin = position + _offset - RectSize/2;
		RectTransform.offsetMax = position + _offset + RectSize/2;

		_icon.enabled = true;
	}

	protected override void OnReleased(Vector2 position)
	{
		if (!Interactable) return;

		RectTransform.SetParent(StartPosition);
		RectTransform.anchorMin = Vector2.zero;
		RectTransform.anchorMax = Vector2.one;
		RectTransform.offsetMin = Vector2.zero;
		RectTransform.offsetMax = Vector2.zero;

		_icon.enabled = false;
		OnComponentReleased.Invoke(position + _offset);
	}

	protected override void OnMove(Vector2 position)
	{
		OnPositionChanged.Invoke(position + _offset);
	}

	protected override void CheckBounds() {}

	[SerializeField]
	private Graphic _icon;

	private Vector2 _offset;
}
