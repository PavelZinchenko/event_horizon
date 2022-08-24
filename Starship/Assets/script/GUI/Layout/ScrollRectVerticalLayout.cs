using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("Layout/Scroll Rect Vertical Layout Group", 1000)]
public class ScrollRectVerticalLayout : LayoutGroup
{
	public float Spacing = 0;
	public float Aspect = 1;
	
	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
	}
	
	public override void CalculateLayoutInputVertical()
	{
	}
	
	public override void SetLayoutHorizontal()
	{
		float itemWidth = rectTransform.rect.width - padding.horizontal;
		foreach (var rect in rectChildren)
			SetChildAlongAxis(rect, 0, padding.left, itemWidth);
	}
	
	public override void SetLayoutVertical()
	{
		var count = rectChildren.Count;
		var itemHeight = Aspect*(rectTransform.rect.width - padding.horizontal);
		float height = count*itemHeight + padding.vertical + (count > 0 ? (count-1)*Spacing : 0);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		
		float y = padding.top;
		foreach (var rect in rectChildren)
		{
			SetChildAlongAxis(rect, 1, y, itemHeight);
			y += Spacing + itemHeight;
		}
	}
}
