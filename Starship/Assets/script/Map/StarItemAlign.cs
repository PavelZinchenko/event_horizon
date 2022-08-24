using UnityEngine;
using System.Collections;

public class StarItemAlign : MonoBehaviour
{
	public Vector2 Offset = Vector2.zero;
	public float RotationSpeed = 0;
	public bool ShowInNormalState = true;
	public bool ShowInMapState = true;
	public bool ShowInGalaxyState = true;

	void OnEnable()
	{
		var scale = transform.parent.localScale;
		transform.localScale = new Vector3(1/scale.x, 1/scale.y, 1/scale.z);
		transform.rotation = Quaternion.identity;
		gameObject.Move(Offset);
	}

	private void OnStateChanged(Star.State state)
	{
		_state = state;
		var renderer = gameObject.GetComponent<Renderer>();

		switch (state)
		{
		case Star.State.Galaxy:
			renderer.enabled = ShowInGalaxyState;
			break;
		case Star.State.Map:
			renderer.enabled = ShowInMapState;
			break;
		case Star.State.Normal:
			renderer.enabled = ShowInNormalState;
			break;
		default:
			renderer.enabled = false;
			break;
		}

		if (state != Star.State.Normal) 
			transform.localEulerAngles = Vector3.zero;
	}

	void Update()
	{
		if (RotationSpeed != 0 && _state == Star.State.Normal)
		{
			transform.localEulerAngles = new Vector3(0,0,Time.time*RotationSpeed);
		}
	}

	private Star.State _state;
}
