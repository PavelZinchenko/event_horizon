using UnityEngine;
using UnityEngine.UI;

namespace ViewModel
{
	public class HomeStarPanelViewModel : MonoBehaviour
	{
		public RectTransform Container;
		public RectTransform HomePanel;
		public Text DistanceText;

		public bool Visible
		{
			get	{ return HomePanel.gameObject.activeSelf; }
			set { HomePanel.gameObject.SetActive(value); }
		}

		public void SetDistance(Vector2 distance)
		{
			DistanceText.text = distance.magnitude.ToString("N1");
			var size = Container.rect.size/2;
			var scale = Mathf.Min(Mathf.Abs(size.x / distance.x), Mathf.Abs(size.y / distance.y));
			distance = distance*scale;
			HomePanel.anchoredPosition = -distance;
		}
	}
}
