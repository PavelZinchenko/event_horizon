//using UnityEngine;
//using System.Collections;
//using CombatSystem.Collision;

//public class Line : BaseObject
//{
//	public Texture2D Texture;
//	public float Length = 1.0f;
//	public float Thickness = 1.0f;
//	public float BorderSize = 0.2f;
//	public Color MaterialColor = Color.white;
//	public float Strength = 1.0f;
//    public bool Fading = true;

//	public override Color Color
//	{
//		get { return base.Color; }
//	    set
//	    {
//	        base.Color = value;
//            UpdateColor();
//	    }
//	}

//	public override float Lifetime
//	{
//		get { return base.Lifetime; }
//	    set
//	    {
//	        base.Lifetime = value;
//            if (Fading)
//                UpdateColor();
//	    }
//	}

//	public override Map GetCollisionMap()
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
//			if (!Mathf.Approximately(Length, value))
//			{
//				Length = value;
//				CreateMesh(gameObject.GetMesh(), Length, Thickness, BorderSize);
//			}
//		}
//	}

//	public override void Awake()
//	{
//		base.Awake();
//		gameObject.CreateDefaultMaterial(Texture, MaterialColor);
//	}

//	public override void OnEnable()
//	{
//		base.OnEnable();
//		var mesh = gameObject.GetMesh();
//		mesh.MarkDynamic();
//		CreateMesh(mesh, Length, Thickness, BorderSize);
//		UpdateColor();
//	}

//	public override void Update()
//	{
//		base.Update();
//		transform.localScale = new Vector3(1f, 0.1f + 0.2f*(1f + Mathf.Sin(50*Time.time*Mathf.PI)) + Lifetime*0.9f, 1f);
//	}

//	public static void CreateMesh(Mesh mesh, float length, float thickness, float borderSize)
//	{
//		mesh.Clear(false);

//		if (length <= 2*borderSize)
//		{
//			mesh.vertices = new Vector3[]
//			{
//				new Vector3 (0, thickness/2, 0),
//				new Vector3 (0, -thickness/2, 0),
//				new Vector3 (length/2, thickness/2, 0),
//				new Vector3 (length/2, -thickness/2, 0),
//				new Vector3 (length/2, thickness/2, 0),
//				new Vector3 (length/2, -thickness/2, 0),
//				new Vector3 (length, thickness/2, 0),
//				new Vector3 (length, -thickness/2, 0),
//			};
//			mesh.triangles = new int[] { 0,1,3, 3,2,0, 4,5,7, 7,6,4 };
//			mesh.uv = new Vector2[]
//			{
//				new Vector2 (0, 1), 
//				new Vector2 (0, 0),
//				new Vector2 (length/2, 1), 
//				new Vector2 (length/2, 0),
//				new Vector2 (1-length/2, 1), 
//				new Vector2 (1-length/2, 0), 
//				new Vector2 (1, 1), 
//				new Vector2 (1, 0), 
//			};
//		}
//		else
//		{
//			mesh.vertices = new Vector3[]
//			{
//				new Vector3 (0, thickness/2, 0),
//				new Vector3 (0, -thickness/2, 0),
//				new Vector3 (borderSize, thickness/2, 0),
//				new Vector3 (borderSize, -thickness/2, 0),
//				new Vector3 (length - borderSize, thickness/2, 0),
//				new Vector3 (length - borderSize, -thickness/2, 0),
//				new Vector3 (length, thickness/2, 0),
//				new Vector3 (length, -thickness/2, 0),
//			};
//			mesh.triangles = new int[] { 0,1,3, 3,2,0, 2,3,5, 5,4,2, 4,5,7, 7,6,4 };
//			mesh.uv = new Vector2[]
//			{
//				new Vector2 (0, 1), 
//				new Vector2 (0, 0),
//				new Vector2 (borderSize, 1), 
//				new Vector2 (borderSize, 0),
//				new Vector2 (1-borderSize, 1), 
//				new Vector2 (1-borderSize, 0), 
//				new Vector2 (1, 1), 
//				new Vector2 (1, 0), 
//			};
//		}
//	}

//	private void UpdateColor()
//	{
//		var color = Color;
//        color.a *= Strength * (Fading ? Lifetime : 1f - Mathf.Pow(1f - Lifetime, 5));
//        var renderer = gameObject.GetComponent<MeshRenderer>();
//		if (renderer != null)
//			renderer.material.color = color;
//	}
//}
