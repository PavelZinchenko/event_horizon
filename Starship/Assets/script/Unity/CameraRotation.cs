using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraRotation : MonoBehaviour
{
	void Start()
	{
	}

	void Update()
	{
		var angle = Time.time*Mathf.Deg2Rad / 10;
		GetComponent<Camera>().fieldOfView = 50 + 20*Mathf.Sin(10*angle);
		var x = 360*Mathf.Sin(angle*7);
		var y = 360*Mathf.Cos(angle*5);
		var z = 360*Mathf.Cos(angle*3);

		transform.localEulerAngles = new Vector3(x,y,z);
	}
}
