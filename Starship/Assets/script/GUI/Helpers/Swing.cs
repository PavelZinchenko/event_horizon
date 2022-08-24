using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
	[RequireComponent (typeof(RectTransform))]
	public class Swing : MonoBehaviour 
	{
		public float MaxAmplitude;
		public float MaxSpeed;

        void Start()
		{
			_amlitude = (0.1f + 0.9f*Random.value) * MaxAmplitude;
			_speed = (0.1f + 0.9f*Random.value) + MaxSpeed;
			GetComponent<RectTransform>().pivot = new Vector2(Random.value, Random.value);
		}

		void Update() 
		{
			transform.localEulerAngles = new Vector3(0,0,_amlitude*Mathf.Sin(Time.realtimeSinceStartup*_speed));
		}

		private float _amlitude;
		private float _speed;
	}
}