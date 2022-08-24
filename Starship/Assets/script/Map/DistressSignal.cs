using UnityEngine;
using System.Collections;

public class DistressSignal : Square
{
	public float ImpulseInterval = 1;
	public float ScaleMin = 0.2f;
	public float ScaleMax = 1.0f;

	void OnEnable()
	{
		_time = 0;
	}

	private void OnStateChanged(Star.State state)
	{
		gameObject.GetComponent<Renderer>().enabled = state == Star.State.Normal || state == Star.State.Map;
		_isMapState = state == Star.State.Map;
	}

	void Update()
	{
		_time += Time.deltaTime;
		var scale = ScaleMin + (_time % ImpulseInterval)*(ScaleMax - ScaleMin);

		transform.localScale = Vector3.one * Size * (_isMapState ? 2f : 1f) * scale;

		var color = Color;
		color.a *= 1f - scale;
		SetColor(color);
	}

	private float _time;
	private bool _isMapState;
}
