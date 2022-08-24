using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("Layout/Cell Layout", 1000)]
public class CellLayout : LayoutGroup
{
	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();

		var min = 0f;
		var preferred = 0f;
		var flexible = 0f;
		foreach (var rect in rectChildren)
		{
			min = Mathf.Max(min, LayoutUtility.GetMinSize(rect, 0));
			preferred = Mathf.Max(preferred, LayoutUtility.GetPreferredSize(rect, 0));
			flexible = Mathf.Max(flexible, LayoutUtility.GetFlexibleSize(rect, 0));
		}
		SetLayoutInputForAxis(min + padding.horizontal, preferred + padding.horizontal, flexible + padding.horizontal, 0);
	}
	
	public override void CalculateLayoutInputVertical()
	{
		var min = 0f;
		var preferred = 0f;
		var flexible = 0f;
		foreach (var rect in rectChildren)
		{
			min = Mathf.Max(min, LayoutUtility.GetMinSize(rect, 1));
			preferred = Mathf.Max(preferred, LayoutUtility.GetPreferredSize(rect, 1));
			flexible = Mathf.Max(flexible, LayoutUtility.GetFlexibleSize(rect, 1));
		}
		SetLayoutInputForAxis(min + padding.vertical, preferred + padding.vertical, flexible + padding.vertical, 1);
	}
	
	public override void SetLayoutHorizontal()
	{
		var itemWidth = rectTransform.rect.width - padding.horizontal;
		foreach (var rect in rectChildren)
			SetChildAlongAxis(rect, 0, padding.left, itemWidth);
	}
	
	public override void SetLayoutVertical()
	{
		var itemHeight = rectTransform.rect.height - padding.vertical;
		foreach (var rect in rectChildren)
			SetChildAlongAxis(rect, 1, padding.top, itemHeight);
	}

	protected override void OnEnable()
	{
		_needUpdate = true;
	}

	private void Update()
	{
		if (_needUpdate)
		{
			SetDirty();
			_needUpdate = false;
		}
	}

	private bool _needUpdate;
}
