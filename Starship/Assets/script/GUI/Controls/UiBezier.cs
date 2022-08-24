using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[AddComponentMenu("UI/Bezier", 1000)]
public class UiBezier : MaskableGraphic
{
	public float Width;
	public bool RightToLeft;
	public Texture Texture;
	
	public override Texture mainTexture { get { return Texture != null ? Texture : base.mainTexture; } }
	
	public void SetPoints(Vector2 v1, Vector2 v2)
	{
		gameObject.SetActive(true);
		
		var transform = GetComponent<RectTransform>();
        transform.pivot = Vector2.zero;

        RightToLeft = v2.x < v1.x != v2.y < v1.y;
        if (v1.x > v2.x) { var temp = v1.x; v1.x = v2.x; v2.x = temp; }
        if (v1.y > v2.y) { var temp = v1.y; v1.y = v2.y; v2.y = temp; }

        transform.anchoredPosition = v1;
        transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v2.x - v1.x);
        transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, v2.y - v1.y);
    }

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
	{
		var corner1 = Vector2.zero;
		var corner2 = Vector2.one;
		
		corner1 -= rectTransform.pivot;
		corner2 -= rectTransform.pivot;
		
		corner1.x *= rectTransform.rect.width;
		corner1.y *= rectTransform.rect.height;
		corner2.x *= rectTransform.rect.width;
		corner2.y *= rectTransform.rect.height;
		
		if (RightToLeft)
			Generic.Swap(ref corner1.x, ref corner2.x);
		
		var center = (corner1 + corner2) / 2;
		var control1 = new Vector2(corner1.x, (corner1.y * 2 + corner2.y) / 3);
		var control2 = new Vector2(corner2.x, (corner2.y * 2 + corner1.y) / 3);
		
		vertexHelper.Clear();
		BuildPath(vertexHelper, corner1, control1, center);
		BuildPath(vertexHelper, center, control2, corner2);
	}

	private void BuildPath(VertexHelper vertexHelper, Vector2 start, Vector2 control, Vector2 end)
	{
		var dir = (control - start).normalized;
		var left = new Vector2(-dir.y, dir.x);
		var p0 = start;
		
		int count = Mathf.RoundToInt(Vector2.Distance(start, end) / Width);
		if (count < 10) count = 10;
		
		var step = 1.0f / count;
		
		var index = vertexHelper.currentVertCount;
		for (int i = 1; i <= count; ++i)
		{
			var t = i * step;
			
			vertexHelper.AddVert(p0 + left * Width / 2, color, new Vector2(t, 0));
			vertexHelper.AddVert(p0 - left * Width / 2, color, new Vector2(t, 1));
			
			var p = Geometry.Bezier(start, control, end, t);
			dir = (p - p0).normalized;
			left.x = -dir.y;
			left.y = dir.x;
			
			vertexHelper.AddVert(p - left * Width / 2, color, new Vector2(t + step, 1));
			vertexHelper.AddVert(p + left * Width / 2, color, new Vector2(t + step, 0));
			
			vertexHelper.AddTriangle(index + 0, index + 1, index + 2);
			vertexHelper.AddTriangle(index + 2, index + 3, index + 0);
			index += 4;
			
			p0 = p;
		}
	}
}
