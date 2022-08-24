//using UnityEngine;
//using System.Collections;

//namespace Effects
//{
//	public class CircularEffect : BaseObject
//	{
//		public float Radius = 5.0f;
//		public float Thickness = 2.0f;
//		public int Segments = 64;
//		public Color MaterialColor = Color.white;
//		public float Strength = 1.0f;
//		public Texture2D Texture;

//		public override Color Color
//		{
//			get { return gameObject.GetComponent<MeshRenderer>().material.color; }
//			set { gameObject.GetComponent<MeshRenderer>().material.color = value; }
//		}
		
//		public override void Awake()
//		{
//			base.Awake();
//			Circle.Create(gameObject, Radius, Thickness, Segments, Texture, MaterialColor);
//		}

//		public override void Update()
//		{
//			var temp = Scale;
//			Scale = Scale * (1.0f - Lifetime*Lifetime);
//			base.Update();
//			Scale = temp;

//			var renderer = gameObject.GetComponent<Renderer>();
//			if (renderer != null)
//			{
//				var color = renderer.material.color;
//				color.a = Lifetime*Strength;
//				renderer.material.color = color;
//			}
//		}
//	}
//}