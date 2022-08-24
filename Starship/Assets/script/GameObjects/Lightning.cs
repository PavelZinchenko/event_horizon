//using UnityEngine;
//using System.Linq;
//using System.Collections.Generic;
//using CombatSystem.Collision;

//public class Lightning : BaseObject
//{
//	public Texture2D Texture;
//	public float Length = 1.0f;
//	public float Thickness = 1.0f;
//	public float BorderSize = 0.2f;
//	public int Steps = 10;
//	public float Spread = 10.0f;
//	public Color MaterialColor = Color.white;
//	public float Strength = 1.0f;
//	public Material Material;
//    public bool Fading = true;

//    public override Map GetCollisionMap()
//	{
//		return null;
//	}	
	
//	public override float Scale
//	{
//		get
//		{
//			return 1.0f;
//		}
//		set
//		{
//			if (!Mathf.Approximately(Length, value) && !DEBUG)
//			{
//				Length = value;
//				var mesh = gameObject.GetMesh();
//				mesh.MarkDynamic();
//				CreateMesh(mesh, Length, Thickness, BorderSize, Spread*Length, Steps);
//			}
//		}
//	}
	
//	public override void Awake()
//	{
//		base.Awake();
//		Color = MaterialColor;

//		if (Material == null)
//			gameObject.CreateDefaultMaterial(Texture, Color);
//		else
//		{
//			var renderer = gameObject.GetComponent<MeshRenderer>();
//			renderer = gameObject.AddComponent<MeshRenderer>();
//			renderer.material = Material;
//			renderer.material.mainTexture = Texture;
//			renderer.material.color = Color;
//		}
//	}
	
//	public override void OnEnable()
//	{
//		base.OnEnable();
//		gameObject.GetMesh().MarkDynamic();
//	}
	
//	public override void Update()
//	{
//		base.Update();

//		if (Lifetime > 0)
//			CreateMesh(gameObject.GetMesh(), Length, Thickness, BorderSize, Spread*Length, Steps);

//		var color = Color;
//        color.a *= Strength * (Fading ? Lifetime : 1f - Mathf.Pow(1f - Lifetime,5));
//        gameObject.GetComponent<Renderer>().material.color = color;
//	}
	
//	private void CreateMesh(Mesh mesh, float length, float thickness, float borderSize, float spread, int steps)
//	{
//		mesh.Clear(false);

//		if (length < 2*borderSize)
//			return;

//		var target = Vector2.right * length;
//		var keys = 2 + Mathf.FloorToInt(length/thickness/4);

//		var verticesCount = keys*steps*2 + 6;
//		var segments = verticesCount/2 - 1;

//		if (_vertices.Length != verticesCount)
//		{
//			_vertices = new Vector3[verticesCount];
//			_uv = new Vector2[verticesCount];
//			_triangles = new int[segments*6];
//		}

//		Vector2 p1 = Vector2.zero;
//		Vector2 p2 = Vector2.zero;
//		Vector2 p3 = Random.insideUnitCircle + new Vector2(length/keys,0);

//		var index = 0;
//		_vertices[index++] = new Vector2(-borderSize,-thickness/2);
//		_vertices[index++] = new Vector2(-borderSize,thickness/2);
//		_vertices[index++] = new Vector2(0,-thickness/2);
//		_vertices[index++] = new Vector2(0,thickness/2);

//        Vector2 p = Vector2.zero;
//		float rotation = 0;
//		for (int i = 1; i <= keys; ++i)
//		{
//			var x = (i+1)*length/keys;
//			p1 = p2;
//			p2 = p3;
//            p3 = new Vector2(x, 0) + spread*Random.insideUnitCircle * Mathf.Sin(x*Mathf.PI/length);
//			var p12 = i == 1 ? Vector2.zero : Vector2.Lerp(p1,p2,0.5f);
//			var p23 = i == keys ? target : Vector2.Lerp(p2,p3,0.5f);

//			for (int j = 1; j <= steps; j++)
//			{
//				var old = p;
//				p = Geometry.Bezier(p12,p2,p23,(float)j/steps);
//				rotation = RotationHelpers.Angle(p - old);
//				_vertices[index++] = p + RotationHelpers.Transform(new Vector2(0,-thickness/2), rotation);
//				_vertices[index++] = p + RotationHelpers.Transform(new Vector2(0,thickness/2), rotation);
//            }
//        }

//		_vertices[index++] = target + RotationHelpers.Transform(new Vector2(borderSize,-thickness/2), rotation);
//		_vertices[index++] = target + RotationHelpers.Transform(new Vector2(borderSize,thickness/2), rotation);
//		mesh.vertices = _vertices;

//		index = 0;
//		_uv[index++] = new Vector2 (0, 1);
//		_uv[index++] = new Vector2 (0, 0);
//		for (var i = 1; i < segments; ++i)
//		{
//			_uv[index++] = new Vector2 (borderSize + (1.0f-2*borderSize)*(float)i/segments, 1);
//			_uv[index++] = new Vector2 (borderSize + (1.0f-2*borderSize)*(float)i/segments, 0);
//		}
//		_uv[index++] = new Vector2 (1, 1);
//		_uv[index++] = new Vector2 (1, 0);
//		mesh.uv = _uv;

//		index = 0;
//		for (var i = 0; i < segments; ++i)
//		{
//			_triangles[index++] = i*2;
//			_triangles[index++] = i*2+1;
//			_triangles[index++] = i*2+3;
//			_triangles[index++] = i*2+3;
//			_triangles[index++] = i*2+2;
//			_triangles[index++] = i*2;
//		}
//		mesh.triangles = _triangles;
//	}

//	private Vector3[] _vertices = new Vector3[0];
//	private Vector2[] _uv = new Vector2[0];
//	private int[] _triangles = new int[0];
//}
