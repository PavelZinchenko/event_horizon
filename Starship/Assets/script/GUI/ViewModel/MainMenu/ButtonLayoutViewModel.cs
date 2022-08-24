using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ViewModel
{
	public class ButtonLayoutViewModel : MonoBehaviour
	{
		public Image Icon;
		public Color NormalColor = new Color(0.12f,0.15f,0.12f,1f);
		public Color FocusedColor = new Color(0.15f,0.2f,0.2f,1f);
		public bool CanBeDisabled = false;

		public float Size
		{
			get
			{
				return RectTransform.rect.width;
			}
			set
			{
				RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
				RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
			}
		}

		public Vector2 Position
		{
			get
			{
				return RectTransform.anchoredPosition;
			}
			set
			{
				RectTransform.anchoredPosition = value;
			}
		}

		public bool Focused
		{
			set
			{
				Image.color = value ? FocusedColor : NormalColor;
			}
		}

		private RectTransform RectTransform
		{
			get
			{
				if (_rectTransform == null)
					_rectTransform = GetComponent<RectTransform>();
				return _rectTransform;
			}
		}

		private Image Image
		{
			get
			{
				if (_image == null)
					_image = GetComponent<Image>();
				return _image;
			}
		}

		private Image _image;
		private RectTransform _rectTransform;
	}
}
