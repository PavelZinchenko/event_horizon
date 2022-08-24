using UnityEngine;
using System.Collections;

public class StarIcon : Square
{
	private void OnStateChanged(Star.State state)
	{
		float scale = 1.0f;

		if (state == Star.State.Map)
			scale = 2.0f;
		else if (state == Star.State.Galaxy)
			scale = 10.0f;

		gameObject.transform.localScale = Vector3.one * Size * scale;
	}
}
