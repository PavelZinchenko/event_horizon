using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// This component is responsible for notifying the ListScrollRect when contents size has change so it can refresh
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class ListScrollRectHelper : UIBehaviour
{
	#region Member Variables

	private ListScrollRect					listScrollRect;
	private ListScrollRect.ScrollDirection	scrollDirection;
	private float							lastSize;

	#endregion

	#region Public Methods

	public void Setup(ListScrollRect listScrollRect)
	{
		this.listScrollRect = listScrollRect;
		SetScrollDirection(listScrollRect.ScrollDir);
	}

	public void SetScrollDirection(ListScrollRect.ScrollDirection scrollDirection)
	{
		if (listScrollRect == null)
		{
			Debug.LogError("No ListScrollRect set in ListScrollRectHelper when trying to set the scroll direction.");
			return;
		}

		this.scrollDirection = scrollDirection;

		lastSize = GetSize();
	}

	#endregion

	#region Protected Methods

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();

		if (listScrollRect == null)
		{
			return;
		}

		float size = GetSize();

		if (size != lastSize)
		{
			lastSize = size;

			// Since we are not allowed to do things like SetActive in a OnRectTransformDimensionsChange call, we will
			// set a flag to notify the ListScrolLRect to refresh its content in its next Update loop
			listScrollRect.MarkForRefresh = true;
		}
	}

	#endregion

	#region Private Methods

	private float GetSize()
	{
		// We only care when one of the sizes (width/height) change
		switch (scrollDirection)
		{
		case ListScrollRect.ScrollDirection.Vertical:
			return gameObject.GetComponent<RectTransform>().rect.width;
		case ListScrollRect.ScrollDirection.Horizontal:
			return gameObject.GetComponent<RectTransform>().rect.height;
		}

		return 0f;
	}

	#endregion
}
