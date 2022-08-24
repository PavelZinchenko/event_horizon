using Constructor.Ships.Modification;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Common;

namespace Gui.Craft
{
    public class ShipModListItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _description;
        [SerializeField] private PricePanel _pricePanel;
        [SerializeField] private Selectable _selectable;

        public void Initialize(IShipModification modification, ILocalization localization, IResourceLocator resourceLocator, bool suitable, bool haveMoney)
        {
            Modification = modification;

            _icon.sprite = resourceLocator.GetSprite(modification.Type.GetIconId());
            _icon.color = modification.Type.GetColor();
            _description.text = modification.GetDescription(localization);

            var price = modification.Type.GetInstallPrice();
            _pricePanel.gameObject.SetActive(price.Amount > 0);
            _pricePanel.Initialize(null, price, !haveMoney);

            _selectable.interactable = suitable && haveMoney;
        }

        public IShipModification Modification { get; private set; }
    }
}
