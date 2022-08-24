using UnityEngine;
using UnityEngine.UI;

namespace ViewModel
{
	public class TextFieldViewModel : MonoBehaviour
	{
		public Text Label;
		public Text Value;

		public RectTransform RectTransform
		{
			get
			{
				if (_rectTransform == null)
					_rectTransform = GetComponent<RectTransform>();
				return _rectTransform;
			}
		}

		public Color Color
		{
			set
			{
				Label.color = new Color(value.r, value.g, value.b, Label.color.a);
				Value.color = new Color(value.r, value.g, value.b, Value.color.a);
			}
		}

		private RectTransform _rectTransform;
	}
}
