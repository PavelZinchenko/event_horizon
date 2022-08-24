//using UnityEngine;
//using System.Collections;

//public class SpriteObject : BaseObject
//{
//	public Texture2D Texture;
//	public float Size = 1.0f;
//	public Color MaterialColor = Color.white;
//	public float Alpha = 1.0f;
//	public Material Material;

//	public override Color Color
//	{
//		get { return gameObject.GetComponent<MeshRenderer>().material.color; }
//		set 
//		{
//			if (!_initialized)
//				Initialize();

//			value.a *= Alpha;
//			gameObject.GetComponent<MeshRenderer>().material.color = value; 
//		}
//	}

//	public override void Awake()
//	{
//		base.Awake();
//		if (!_initialized)
//			Initialize();
//	}

//	private void Initialize()
//	{
//		Primitives.CreateRectangle(gameObject.GetMesh(), Size, Size, 1);
		
//		if (Material == null)
//			gameObject.CreateDefaultMaterial(Texture, MaterialColor);
//		else
//			gameObject.CreateMaterial(Material, Texture, MaterialColor);

//		_initialized = true;
//	}

//	private bool _initialized;
//}
