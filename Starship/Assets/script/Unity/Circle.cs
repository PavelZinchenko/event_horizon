 using UnityEngine;
using System.Collections.Generic;

public class Circle : MonoBehaviour
{
	public float Thickness = 2.0f;
	public int Segments = 64;
	public Color Color = Color.white;
	public float Rotation = 4.0f;
	public Texture2D Texture;

	public static void Create(GameObject parent, float radius, float thickness, int segments, Texture2D texture, Color color)
	{
		var mesh = parent.GetMesh();
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		var aspect = (float)texture.height / (float)texture.width;
		var length = 2*Mathf.PI*radius;
		float step = Mathf.Round(aspect*length/thickness)/segments;

		for (var i = 0; i <= segments; ++i)
		{
			var alpha = i*2*Mathf.PI / segments;
			var x = Mathf.Cos(alpha);
			var y = Mathf.Sin(alpha);
			vertices.Add(new Vector3(x,y,0));
			x = (1.0f-thickness/radius)*Mathf.Cos(alpha);
			y = (1.0f-thickness/radius)*Mathf.Sin(alpha);
			vertices.Add(new Vector3(x,y,0));
			uv.Add(new Vector2(i*step,1));
			uv.Add(new Vector2(i*step,0));
		}

		for (var i = 0; i < segments; ++i)
		{
			var p1 = i*2;
			var p2 = i*2 + 1;
			var p3 = i*2 + 2;
			var p4 = i*2 + 3;
			triangles.Add(p2);
			triangles.Add(p1);
			triangles.Add(p3);
			triangles.Add(p3);
			triangles.Add(p4);
			triangles.Add(p2);
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uv.ToArray();

		parent.CreateDefaultMaterial(texture, color);
	}

	protected virtual void Start()
	{
		Create(gameObject, transform.localScale.z, Thickness, Segments, Texture, Color);
	}
	
	protected virtual void Update()
	{
		transform.localEulerAngles = new Vector3(0,0,Rotation*Time.realtimeSinceStartup);
	}	
}
