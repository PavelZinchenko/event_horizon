using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("Layout/Random Layout Group", 1000)]
public class RandomLayoutGroup : LayoutGroup
{
	public float MinSize;
	public float MaxSize;
	public int Seed;

	public override void CalculateLayoutInputHorizontal()
	{
		_random = new System.Random(Seed);
		base.CalculateLayoutInputHorizontal();
	}
	
	public override void CalculateLayoutInputVertical()
	{
	}
	
	public override void SetLayoutHorizontal()
	{
		float width = rectTransform.rect.width - padding.horizontal;

		foreach (var rect in rectChildren)
		{
			Random.seed = rect.GetHashCode();
			float size = MinSize + (MaxSize - MinSize) * Random.value;
			float position = Mathf.Max(0, width - size) * (float)_random.NextDouble();
			SetChildAlongAxis(rect, 0, padding.left + position, size);
		}
	}
	
	public override void SetLayoutVertical()
	{
		float height = rectTransform.rect.height - padding.vertical;

		foreach (var rect in rectChildren)
		{
			Random.seed = rect.GetHashCode();
			float size = MinSize + (MaxSize - MinSize) * Random.value;
			float position = Mathf.Max(0, height - size) * (float)_random.NextDouble();
			SetChildAlongAxis(rect, 1, padding.top + position, size);
		}
	}

	private System.Random _random = new System.Random();
}
