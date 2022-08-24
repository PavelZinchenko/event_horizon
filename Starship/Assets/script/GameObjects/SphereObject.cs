//using UnityEngine;
//using System.Collections;

//public class SphereObject : BaseObject
//{
//	public Texture2D Texture;
//	public float Size = 1.0f;
//	public Color MaterialColor = Color.white;
//	public Material Material;

//	public int DetailLevel = 6;
//	public float NoiseFrequency = 5f;
//	public float NoisePower = 0.2f;
//	public float RotationSpeed = 1.0f;

//	public override Color Color
//	{
//		get { return gameObject.GetComponent<MeshRenderer>().material.color; }
//		set { gameObject.GetComponent<MeshRenderer>().material.color = value*MaterialColor; }
//	}

//	public override void Awake()
//	{
//		base.Awake();
//		Primitives.CreateSphere(gameObject.GetMesh(), Size, DetailLevel, NoiseFrequency, NoisePower);
//		rotation = new Vector3(Random.Range(0, 360), Random.Range(0, 360), RotationSpeed*Random.Range(20.5f, 60f));
		
//		if (Material == null)
//			gameObject.CreateDefaultMaterial(Texture, MaterialColor);
//		else
//			gameObject.CreateMaterial(Material, Texture, MaterialColor);
//	}

//	public override void Update()
//	{
//		gameObject.Move(Position);
//		transform.localEulerAngles = new Vector3(rotation.x, rotation.y, rotation.z*Time.time);
//		transform.localScale = Vector3.one*Scale;
//	}

//	private Vector3 rotation;
//}
