using System.Linq;
using System.Collections.Generic;
using Economy.ItemType;
using Economy.Products;
using UnityEngine;
using UnityEngine.UI;
using GameModel;
using Services.Reources;

namespace ViewModel
{
	namespace Common
	{
		public class InventoryItem : MonoBehaviour
		{
			[SerializeField] private Image Icon;
			[SerializeField] private Text NameText;
            [SerializeField] private Text DescriptionText;
            [SerializeField] private GameObject QuantityPanel;
			[SerializeField] private Text QuantityText;
			[SerializeField] private RectTransform OutOfStockPanel;
			[SerializeField] private GameObject RankPanel;
			[SerializeField] private Text RankText;
			[SerializeField] private PricePanel PricePanel;
		    [SerializeField] private Selectable Selectable;

			public void Initialize(IProduct item, bool notEnoughMoney, IResourceLocator resourceLocator)
			{
				Product = item;
				Icon.sprite = item.Type.GetIcon(resourceLocator);
				Icon.color = item.Type.Color;
				NameText.text = item.Type.Name;
                DescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(DescriptionText.text = item.Type.Description));
                NameText.color = DescriptionText.color = ColorTable.QualityColor(item.Type.Quality);

                if (PricePanel != null)
                    PricePanel.Initialize(item.Type, item.Price, notEnoughMoney);

			    if (QuantityPanel != null)
			    {
			        QuantityText.text = item.Quantity.ToString();
			        QuantityPanel.gameObject.SetActive(item.Quantity > 1);
			    }

                if (OutOfStockPanel != null)
			        OutOfStockPanel.gameObject.SetActive(!notEnoughMoney && item.Quantity <= 0);

			    if (Selectable != null)
			        Selectable.interactable = !notEnoughMoney;

				InitShip(item.Type as ShipItem);
			}

			public IProduct Product { get; private set; }

			private void InitShip(ShipItem ship)
			{
                if (RankPanel == null)
                    return;
			    
				if (ship == null || ship.Rank < 1)
				{
					RankPanel.gameObject.SetActive(false);
				}
				else
				{
					RankPanel.gameObject.SetActive(true);
					RankText.text = ship.Rank.ToString();
				}
			}
		}
	}
}
