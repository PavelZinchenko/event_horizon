//using UnityEngine;
//using System.Collections;

//public class ParticleObject : BaseObject
//{
//	public Texture2D Texture;
//	public float Size = 1.0f;
//	public Color MaterialColor = Color.white;
//	public float Alpha = 1.0f;
//	public float MinAlpha = 0.0f;
//	public Material Material;

//	public override void Update()
//	{
//		var temp = Scale;
//		Scale *= (1.5f - Lifetime);
//		base.Update();
//		Scale = temp;
		
//		var color = Color;
//		color.a *= Mathf.Max(MinAlpha, Alpha * Lifetime);
//		gameObject.GetComponent<MeshRenderer>().material.color = color;
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
