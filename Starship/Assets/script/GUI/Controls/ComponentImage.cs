using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ComponentImage", 1000)]
public class ComponentImage : Image
{
	public void SetDisplayRect(float minX, float minY, float maxX, float maxY)
	{
		_maxX = maxX;
		_minX = minX;
		_maxY = maxY;
		_minY = minY;

		SetVerticesDirty();
	}
		
	protected override void OnPopulateMesh(VertexHelper vertexHelper)
	{
		base.OnPopulateMesh(vertexHelper);
		
		var corner1 = Vector2.zero;
		var corner2 = Vector2.one;
		
		corner1 -= rectTransform.pivot;
		corner2 -= rectTransform.pivot;
		
		corner1.x *= rectTransform.rect.width;
		corner1.y *= rectTransform.rect.height;
		corner2.x *= rectTransform.rect.width;
		corner2.y *= rectTransform.rect.height;
		
		for (int i = 0; i < vertexHelper.currentVertCount; ++i)
		{
			var vertex = new UIVertex();
			vertexHelper.PopulateUIVertex(ref vertex, i);
			var x = (vertex.position.x - corner1.x)/(corner2.x - corner1.x);
			var y = (vertex.position.y - corner1.y)/(corner2.y - corner1.y);

			x = _minX + x*(_maxX - _minX);
			y = _minY + y*(_maxY - _minY);

			vertex.position = new Vector3(corner1.x + x*(corner2.x - corner1.x), corner1.y + y*(corner2.y - corner1.y), vertex.position.y);
			vertexHelper.SetUIVertex(vertex, i);
		}
	}

    private float _minX = 0;
	private float _maxX = 1;
	private float _minY = 0;
	private float _maxY = 1;
}
