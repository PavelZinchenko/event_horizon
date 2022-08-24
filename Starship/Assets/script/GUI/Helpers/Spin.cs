using UnityEngine;

namespace Gui
{
	public class Spin : MonoBehaviour 
	{
		public float Speed = 1.0f;

	    void Start()
	    {
	        _offset = new System.Random(GetHashCode()).Next(360);
	    }

		void Update() 
		{
			transform.localEulerAngles = new Vector3(0,0,Time.realtimeSinceStartup*360*Speed + _offset);
		}

	    private float _offset;
	}
}