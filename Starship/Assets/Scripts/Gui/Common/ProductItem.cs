using Economy.Products;
using Gui.Common;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.StarMap
{
    public class ProductItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
        [SerializeField] private Text _description;
        [SerializeField] private TextValueItem _quantity;

        public IProduct Product { get; private set; }

        public void Initialize(IResourceLocator resourceLocator, IProduct item)
        {
            Product = item;

            _icon.sprite = item.Type.GetIcon(resourceLocator);
            _icon.color = item.Type.Color;
            _name.text = item.Type.Name;
            _name.color = ColorTable.QualityColor(item.Type.Quality);

            if (_description != null)
            {
                _description.text = item.Type.Description;
                _description.color = _name.color;
            }

            if (_quantity != null)
            {
                _quantity.gameObject.SetActive(item.Quantity > 1);
                _quantity.Value = item.Quantity.ToString();
            }
        }
    }
}
