using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[AddComponentMenu("UI/Push Button", 1000)]
public class PushButton : Selectable, ICanvasRaycastFilter
{
	public UnityEvent ButtonPressed = new UnityEvent();
	public UnityEvent ButtonReleased = new UnityEvent();

	public override void OnPointerDown(PointerEventData eventData)
	{
		var oldState = currentSelectionState;
		base.OnPointerDown(eventData);
		Notify(oldState, currentSelectionState);
	}
	
	public override void OnPointerEnter(PointerEventData eventData)
	{
		var oldState = currentSelectionState;
		base.OnPointerEnter(eventData);
		Notify(oldState, currentSelectionState);
	}
	
	public override void OnPointerExit(PointerEventData eventData)
	{
		var oldState = currentSelectionState;
		base.OnPointerExit(eventData);
		Notify(oldState, currentSelectionState);
	}
	
	public override void OnPointerUp(PointerEventData eventData)
	{
		var oldState = currentSelectionState;
		base.OnPointerUp(eventData);
		Notify(oldState, currentSelectionState);
	}
	
	public override void OnSelect(BaseEventData eventData)
	{
		var oldState = currentSelectionState;
		base.OnSelect(eventData);
		Notify(oldState, currentSelectionState);
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		var oldState = currentSelectionState;
		base.OnDeselect(eventData);
		Notify(oldState, currentSelectionState);
	}

	public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		Vector2 pivotToCursorVector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			RectTransform, screenPoint, eventCamera, out pivotToCursorVector);		
		return (pivotToCursorVector.magnitude < RectTransform.rect.width/2);
	}

	private void Notify(SelectionState oldState, SelectionState newState)
	{
		if (newState == SelectionState.Pressed && oldState != SelectionState.Pressed)
			ButtonPressed.Invoke();
		else if (newState != SelectionState.Pressed && oldState == SelectionState.Pressed)
			ButtonReleased.Invoke();
	}

	private RectTransform RectTransform
	{
		get
		{
			if (_rectTransform == null)
				_rectTransform = GetComponent<RectTransform>();
			return _rectTransform;
		}
	}

	private RectTransform _rectTransform;
}
