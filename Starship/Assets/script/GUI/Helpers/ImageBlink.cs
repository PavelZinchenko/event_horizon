using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
	[RequireComponent (typeof (Image))]
	public class ImageBlink : MonoBehaviour 
	{
		public float Speed = 0.25f;

		private void Start()
		{
			_image = gameObject.GetComponent<Image>();
		}

		private void LateUpdate()
		{
			if (_image.canvasRenderer.GetAlpha() < 0.1f)
				return;

			_image.canvasRenderer.SetAlpha(0.75f + Mathf.Sin(2*Time.realtimeSinceStartup*Mathf.PI*Speed)*0.25f);
        }
        
		private Image _image;
    }
}