//using UnityEngine;
//using System.Collections;

//public class PulseSprite : BaseObject
//{
//	public Texture2D Texture;
//	public float Size = 1.0f;
//	public float Strength = 1.0f;
//	public Color MaterialColor = Color.white;

//	public override void OnEnable()
//	{
//		_activationTime = Time.time;
//		base.OnEnable();
//	}

//	public override void Update()
//	{
//		base.Update();
		
//		var color = Color;
//		var time = Time.time - _activationTime;
//		time = 1 - 2*Mathf.Abs(time - Mathf.Floor(time) - 0.5f);
//		var alpha = 1f + Strength*Mathf.Pow(time,5);

//		color.a *= alpha;
//		gameObject.GetComponent<MeshRenderer>().material.color = color;
//	}

//	public override void Awake()
//	{
//		base.Awake();
//		Primitives.CreateRectangle(gameObject.GetMesh(), Size, Size, 1);
//		gameObject.CreateDefaultMaterial(Texture, MaterialColor);
//	}

//	private float _activationTime;
//}
