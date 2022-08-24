using UnityEngine;
using UnityEngine.UI;

namespace ViewModel
{
	public class CreditsPanelViewModel : MonoBehaviour
	{
		public RectTransform Panel;
		public RectTransform Content;
		public float TimeInSeconds = 120f;

		private void Start()
		{
			if (!_startTime.HasValue)
				_startTime = Time.time;
		}

		private void Update()
		{
			var deltaTime = (Time.time - _startTime.Value) / TimeInSeconds; 
			deltaTime -= Mathf.Floor(deltaTime);
			var panelHeight = Panel.rect.height;
			var contentHeight = Content.sizeDelta.y;
			Content.anchoredPosition = new Vector2(0,(contentHeight+2*panelHeight)*deltaTime - panelHeight);
		}

		private static float? _startTime;
	}
}
