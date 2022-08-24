using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Line", 1000)]
public class UiLine : MaskableGraphic
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
		{
			var temp = corner1.x;
			corner1.x = corner2.x;
			corner2.x = temp;
		}
		
		var dir = (corner2 - corner1).normalized;
		var left = new Vector2(-dir.y, dir.x);
		
		vertexHelper.Clear();
		vertexHelper.AddVert(corner1 + left*Width/2, color, new Vector2(0,0));
		vertexHelper.AddVert(corner1 - left*Width/2, color, new Vector2(0,1));
		vertexHelper.AddVert(corner2 - left*Width/2, color, new Vector2(1,1));
		vertexHelper.AddVert(corner2 + left*Width/2, color, new Vector2(1,0));
		vertexHelper.AddTriangle(0, 1, 2);
		vertexHelper.AddTriangle(2, 3, 0);
	}
}
