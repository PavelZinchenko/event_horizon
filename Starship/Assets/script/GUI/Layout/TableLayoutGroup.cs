using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("Layout/Table Layout Group", 1000)]
public class TableLayoutGroup : LayoutGroup
{
	public float[] Rows = { 1f };
	public float[] Columns = { 1f };
	public float Spacing = 0;

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
	}
	
	public override void CalculateLayoutInputVertical()
	{
	}
	
	public override void SetLayoutHorizontal()
	{
		float width = rectTransform.rect.width - padding.horizontal - Spacing*(Columns.Length-1);
		float total = Columns.Sum();
		float itemWidth = width / total;

		int column = 0;
		float x = padding.left;

		foreach (var rect in rectChildren)
		{
			if (column == Columns.Length)
			{
				column = 0;
				x = padding.left;
			}

			SetChildAlongAxis(rect, 0, x, Columns[column]*itemWidth);

			x += Spacing + Columns[column]*itemWidth;
			column++;
		}
	}
	
	public override void SetLayoutVertical()
	{
		float height = rectTransform.rect.height - padding.vertical - Spacing*(Rows.Length-1);
		float total = Rows.Sum();
		float itemHeight = height / total;

		int row = 0;
		int column = 0;
		float y = padding.top;

		foreach (var rect in rectChildren)
		{
			if (column == Columns.Length)
			{
				column = 0;
				y += Spacing + Rows[row]*itemHeight;
				row++;

				if (row >= Rows.Length) break;
			}

			SetChildAlongAxis(rect, 1, y, Rows[row]*itemHeight);

			column++;
		}
	}
}
