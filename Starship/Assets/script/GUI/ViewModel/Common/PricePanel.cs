using UnityEngine;
using UnityEngine.UI;
using GameModel;
using Economy;
using Economy.ItemType;

namespace ViewModel
{
	namespace Common
	{
		public class PricePanel : MonoBehaviour
		{
			[SerializeField] private Text PriceText;
			[SerializeField] private Image CurrencyIcon;
			[SerializeField] private Image BackgroundImage;
			[SerializeField] private Color NotEnoughMoneyColor = new Color(1,0,0,0.75f);

		    public void Initialize(Currency currency)
		    {
		        Initialize(null, new Price(0, currency), false);
		        PriceText.text = "???";
		    }

		    public void Initialize(Price price, bool haveMoney = true)
		    {
		        Initialize(null, price, !haveMoney);
		    }

			public void Initialize(IItemType item, Price price, bool notEnoughMoney = false)
			{
			    if (price.Currency == Currency.None)
			    {
			        gameObject.SetActive(false);
			        return;
			    }

                gameObject.SetActive(true);
				PriceText.text = Price.PriceToString(price.Amount);
				
				switch (price.Currency)
				{
				case Currency.Credits:
					CurrencyIcon.sprite = CommonSpriteTable.CreditsIcon;
					CurrencyIcon.color = ColorTable.CreditsColor;
					break;
				case Currency.Stars:
					CurrencyIcon.sprite = CommonSpriteTable.StarCurrencyIcon;
					CurrencyIcon.color = ColorTable.PremiumItemColor;
					break;
				case Currency.Tokens:
					CurrencyIcon.sprite = CommonSpriteTable.TokenCurrencyIcon;
					CurrencyIcon.color = ColorTable.TokensColor;
					break;
				case Currency.Snowflakes:
				    CurrencyIcon.sprite = CommonSpriteTable.SnowflakesIcon;
				    CurrencyIcon.color = ColorTable.SnowflakesColor;
				    break;
				case Currency.Money:
					CurrencyIcon.sprite = CommonSpriteTable.ShopIcon;
					CurrencyIcon.color = ColorTable.PremiumItemColor;
					var iapProduct = item as Services.IAP.IIapItem;
					PriceText.text = iapProduct != null ? iapProduct.PriceText : string.Empty;
					break;
                case Currency.None:
                    CurrencyIcon.gameObject.SetActive(false);
                    PriceText.gameObject.SetActive(false);
                    break;
				}

				if (BackgroundImage != null)
				{
					var color = CurrencyIcon.color;
					color.a *= 0.5f;
					BackgroundImage.color = notEnoughMoney ? NotEnoughMoneyColor : color;
				}
			}
		}
	}
}
