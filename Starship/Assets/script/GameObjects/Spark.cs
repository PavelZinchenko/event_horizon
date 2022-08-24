//using UnityEngine;
//using System.Collections;

//public class Spark : BaseObject
//{
//	public Texture2D Texture;
//	public float Size = 1.0f;
//	public Color MaterialColor = Color.white;
//	public Material Material;
//	public float Alpha = 1.0f;
	
//	public override void Update()
//	{
//		base.Update();
//		var color = Color;
//		color.a *= Alpha*(1 + Random.value*Random.value*Random.value);
//        gameObject.GetComponent<MeshRenderer>().material.color = color; 
//	}
	
//	public override void Awake()
//	{
//		base.Awake();
//		Primitives.CreateRectangle(gameObject.GetMesh(), Size, Size, 1);
		
//		if (Material == null)
//			gameObject.CreateDefaultMaterial(Texture, MaterialColor);
//		else
//			gameObject.CreateMaterial(Material, Texture, MaterialColor);
//	}
//}
