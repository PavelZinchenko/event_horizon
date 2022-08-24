//using UnityEngine;
//using System.Collections;

//public class RollingSprite : BaseObject
//{
//	public Texture2D Texture;
//	public float Size = 1.0f;
//	public float Speed = 1.0f;
//	public Color MaterialColor = Color.white;
//	public Material Material;
    
//	public float Alpha = 1.0f;
	
//	public override Color Color
//	{
//		get { return gameObject.GetComponent<MeshRenderer>().material.color; }
//		set 
//		{
//			value.a *= Alpha;
//			gameObject.GetComponent<MeshRenderer>().material.color = value; 
//		}
//	}

//	public override void Update()
//	{
//		var rotation = Rotation;
//		Rotation += Time.time * 360f * Speed;
//		base.Update();
//		Rotation = rotation;
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
	
//	private float _activationTime;
//}
