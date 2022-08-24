using Economy.Products;
using UnityEngine;
using UnityEngine.UI;
using GameModel;
using Services.Reources;
using ViewModel.Quests;

namespace ViewModel
{
	namespace Common
	{
		public class RewardItem : MonoBehaviour, IItemDescription
        {
			[SerializeField] Image Icon;
            [SerializeField] Image Background;
            [SerializeField] Text InfoText;
			[SerializeField] GameObject InfoPanel;

		    public string Name { get; private set; }
		    public Color Color { get; private set; }

		    public void Initialize(IProduct item, IResourceLocator resourceLocator)
			{
				Name = item.Type.Name;
		        Color = ColorTable.QualityColor(item.Type.Quality);

                Icon.sprite = item.Type.GetIcon(resourceLocator);
				Icon.color = item.Type.Color;
			    Background.color = Color.Lerp(Color.black, Color, 0.5f);

				if (item.Quantity > 1)
				{
					InfoText.text = item.Quantity.ToString();
					InfoPanel.gameObject.SetActive(true);
				}
				else
				{
					InfoPanel.SetActive(false);
				}
			}
		}
	}
}
