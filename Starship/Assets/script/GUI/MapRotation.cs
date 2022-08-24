using UnityEngine;

public class MapRotation : MonoBehaviour
{
	public float Speed = 1;
	public float Radius = 1;

    void LateUpdate()
	{
		var t = Time.time*Mathf.PI/180;
		gameObject.Move(new Vector2(Radius*Mathf.Sin(t), Radius*Mathf.Cos(t)));
	}
}
