using DataModel.Technology;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameServices.Player;
using GameServices.Research;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Craft
{
    public class CraftPricePanel : MonoBehaviour
    {
        [Inject] private readonly PlayerResources _resources;
        [Inject] private readonly Research _research;

        [SerializeField] private GameObject _starsPanel;
        [SerializeField] private Text _starsText;
        [SerializeField] private GameObject _creditsPanel;
        [SerializeField] private Text _creditsText;
        [SerializeField] private GameObject _techsPanel;
        [SerializeField] private Text _techsText;
        [SerializeField] private Image _techsImage;
        [SerializeField] private Color _enoughColor;
        [SerializeField] private Color _notEnoughColor;

        public void Initialize(CraftingPrice price, Faction faction)
        {
            _creditsPanel.SetActive(price.Credits > 0);
            _creditsText.text = Economy.Price.PriceToString(price.Credits);
            var enoughMoney = _resources.Money >= price.Credits;
            _creditsText.color = enoughMoney ? _enoughColor : _notEnoughColor;
            _starsPanel.SetActive(price.Stars > 0);
            _starsText.text = Economy.Price.PriceToString(price.Stars);
            var enoughStars = _resources.Stars >= price.Stars;
            _starsText.color = enoughStars ? _enoughColor : _notEnoughColor;
            _techsPanel.SetActive(price.Techs > 0);
            _techsText.text = Economy.Price.PriceToString(price.Techs);
            var enoughTechs = _research.GetAvailablePoints(faction) >= price.Techs;
            _techsText.color = enoughTechs ? _enoughColor : _notEnoughColor;
            _techsImage.color = faction.Color;

            HaveEnoughResources = enoughTechs && enoughMoney && enoughStars;
        }

        public bool HaveEnoughResources { get; private set; }
    }
}
