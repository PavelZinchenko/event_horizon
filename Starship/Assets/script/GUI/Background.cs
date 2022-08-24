using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class Background : MonoBehaviour
{
	public Color Color = Color.white;
	public float RotationMax = 5;
	public float ScaleMax = 0.1f;
	public float ColorBurst = 0.5f;
	public LensFlare LensFlare;

	void Start()
	{
		_scaleCorrection = Mathf.Sqrt(Screen.width*Screen.width + Screen.height*Screen.height) *
			Mathf.Cos( Mathf.Atan2(Screen.height, Screen.width) - RotationMax*Mathf.Deg2Rad) / Screen.width;
	
		var renderer = GetComponent<SpriteRenderer>();
		var sprite = renderer.sprite;
		var pixelsPerUnit = sprite.pixelsPerUnit;
		var width = sprite.texture.width;
		var height = sprite.texture.height;
		_scaleVector = new Vector3(pixelsPerUnit/width, pixelsPerUnit/height, 1.0f);
	}

	void LateUpdate()
	{
		var scale = _scaleCorrection + ScaleMax*(1 + Mathf.Sin(Time.time/13));

		var alpha = 0.75f + ColorBurst*(1+Mathf.Pow(Mathf.Sin(Time.time/5), 5));
		var flarePower = 0.75f + ColorBurst*(1+Mathf.Pow(Mathf.Sin(Time.time/5), 5));

		var angle = RotationMax*Mathf.Sin(Time.time/17);

		transform.localScale = _scaleVector * 2*Camera.main.orthographicSize*Camera.main.aspect * scale;
		transform.localEulerAngles = new Vector3(0,0,angle);
		GetComponent<SpriteRenderer>().material.color = new Color(alpha,alpha,alpha,1);
		LensFlare.brightness = flarePower;
	}

	private Vector2 _scaleVector;
	private float _scaleCorrection;
}
