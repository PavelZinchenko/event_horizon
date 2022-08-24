using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasGroupBlink : MonoBehaviour 
	{
		public float Speed = 0.25f;
		
		private void Start()
		{
			_canvasGroup = gameObject.GetComponent<CanvasGroup>();
		}
		
		private void LateUpdate()
		{
			_canvasGroup.alpha = 0.75f + Mathf.Sin(2*Time.realtimeSinceStartup*Mathf.PI*Speed)*0.25f;
		}
		
		private CanvasGroup _canvasGroup;
	}
}