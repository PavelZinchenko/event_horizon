//using UnityEngine;
//using System.Collections;

//public class TransparentSprite : BaseObject
//{
//	public Texture2D Texture;
//	public float Size = 1.0f;
//	public float Strength = 1.0f;
//	public float Speed = 0.0f;
//	public Color MaterialColor = Color.white;
//	public Material Material;

//	public override void Update()
//	{
//		var rotation = Rotation;
//		Rotation += Time.time * 360f * Speed;
//		base.Update();
//		Rotation = rotation;

//		var color = Color;
//		color.a *= Lifetime*Strength;
//		gameObject.GetComponent<MeshRenderer>().material.color = color;
//	}

//	public override void Awake()
//	{
//		base.Awake();
//		Primitives.CreateRectangle(gameObject.GetMesh(), Size, Size, 1.0f);
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
//}
