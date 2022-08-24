using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Layout/Scroll Rect Horizontal Layout Group", 1000)]
public class ScrollRectHorizontalLayout : LayoutGroup
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

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        var height = (int)rectTransform.rect.height;
        if (height != _height)
        {
            _height = height;
            SetLayoutVertical();
            SetLayoutHorizontal();
        }
    }

    public override void SetLayoutHorizontal()
	{
		var count = rectChildren.Count;
        var height = rectTransform.rect.height;
        var itemWidth = Aspect*(height - padding.vertical);
		var width = count*itemWidth + padding.horizontal + (count > 0 ? (count-1)*Spacing : 0);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        
        float x = padding.left;
		foreach (var rect in rectChildren)
		{
			SetChildAlongAxis(rect, 0, x, itemWidth);
			x += Spacing + itemWidth;
		}
	}
	
	public override void SetLayoutVertical()
	{
		float itemHeight = rectTransform.rect.height - padding.vertical;
		foreach (var rect in rectChildren)
			SetChildAlongAxis(rect, 1, padding.top, itemHeight);
    }

    private int _height = -1;
}
