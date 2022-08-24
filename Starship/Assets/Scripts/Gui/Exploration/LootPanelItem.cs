using Economy.Products;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Exploration
{
    public class LootPanelItem : MonoBehaviour
    {
        [SerializeField] public Image Icon;
        [SerializeField] public Text Name;
        [SerializeField] public Text Description;
        [SerializeField] public Text Quantity;

        public void Initialize(IProduct product, IResourceLocator resourceLocator)
        {
            Icon.gameObject.SetActive(true);
            Icon.sprite = product.Type.GetIcon(resourceLocator);
            Icon.color = product.Type.Color;
            Name.text = product.Type.Name;
            Name.color = ColorTable.QualityColor(product.Type.Quality);

            if (string.IsNullOrEmpty(product.Type.Description))
            {
                Description.gameObject.SetActive(false);
            }
            else
            {
                Description.gameObject.SetActive(true);
                Description.text = product.Type.Description;
            }

            if (product.Quantity <= 1)
            {
                Quantity.gameObject.SetActive(false);
            }
            else
            {
                Quantity.gameObject.SetActive(true);
                Quantity.text = "✕" + product.Quantity;
            }
        }
    }
}
