using Economy.Products;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;

namespace Exploration
{
	public class ExtractedItem : MonoBehaviour
	{
		public void Initialize(IProduct product, IResourceLocator resourceLocator)
		{
			_icon.sprite = product.Type.GetIcon(resourceLocator);
			_icon.color = product.Type.Color;
			_name.text = product.Type.Name;

			if (product.Quantity > 1)
			{
				_cross.SetActive(true);
				_quantity.gameObject.SetActive(true);
				_quantity.text = product.Quantity.ToString();
			}
			else
			{
				_cross.SetActive(false);
				_quantity.gameObject.SetActive(false);
			}
		}

		[SerializeField]
		private Image _icon;
		[SerializeField]
		private Text _name;
		[SerializeField]
		private Text _quantity;
		[SerializeField]
		private GameObject _cross;
	}
}
