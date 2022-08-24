using Constructor;
using Services.Localization;
using UnityEngine;
using UnityEngine.UI;
using Services.Reources;
using Zenject;

namespace Gui.ComponentList
{
    public class ComponentListItem : ComponentListItemBase
    {
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly ILocalization _localization;

        [SerializeField] private Image Icon;
        [SerializeField] private Text NameText;
        [SerializeField] private Text DescriptionText;
        [SerializeField] private Text _quantityText;
        [SerializeField] private Toggle _toggle;

        public override void Initialize(ComponentInfo item, int quantity)
        {
            _component = item;
            Icon.sprite = _resourceLocator.GetSprite(item.Data.Icon);
            Icon.color = item.Data.Color;
            NameText.text = item.GetName(_localization);
            var descriptionText = item.CreateModification().GetDescription(_localization);
            DescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(DescriptionText.text = descriptionText));
            NameText.color = ColorTable.QualityColor(item.ItemQuality);
            _quantityText.text = quantity.ToString();
        }

        public override ComponentInfo Component { get { return _component; } }
        public override bool Selected { get { return _toggle.isOn; } set { _toggle.isOn = value; } }

        private ComponentInfo _component;
    }
}
