using UnityEngine;
using UnityEngine.UI;
using Constructor;
using GameDatabase.Enums;

namespace ViewModel
{
	public class ComponentIconViewModel : MonoBehaviour
	{
		public void SetIcon(Sprite icon, string layout, int size, Color color)
		{
			_icon.sprite = icon;
			_icon.color = color;

			int x0 = size, x1 = 0, y0 = size, y1 = 0;

			for (int i = 0; i < size; ++i)
			{
				for (int j = 0; j < size; ++j)
				{
					if ((CellType)layout[i*size+j] == CellType.Empty)
						continue;
					if (j < x0) x0 = j;
					if (j > x1) x1 = j;
					if (i < y0) y0 = i;
					if (i > y1) y1 = i;
				}
			}

			var x =-0.5f*(size - x0 - x1 - 1)/size;
			var y = 0.5f*(size - y0 - y1 - 1)/size;

			_icon.SetDisplayRect(x, y, x+1, y+1);
		}

		public RectTransform RectTransform 
		{
			get
			{
				if (_rectTransform == null)
					_rectTransform = GetComponent<RectTransform>(); 
				return _rectTransform;
			}
		}

		private RectTransform _rectTransform;

		[SerializeField] ComponentImage _icon;
	}
}
