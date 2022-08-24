using Constructor.Ships.Modification;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Craft
{
    public class ShipModSlot : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _lockIcon;
        [SerializeField] private Toggle _toggle;

        public void Initialize(IShipModification modification, IResourceLocator resourceLocator)
        {
            Modification = modification;
            _icon.gameObject.SetActive(modification != null);
            _lockIcon.gameObject.SetActive(modification == null);

            if (modification != null)
            {
                var sprite = resourceLocator.GetSprite(modification.Type.GetIconId());
                _icon.gameObject.SetActive(sprite != null);
                _icon.sprite = sprite;
                _icon.color = modification.Type.GetColor();
            }
        }

        public IShipModification Modification { get; private set; }
        public bool Selected { get { return _toggle.isOn; } set { _toggle.isOn = value; } }
    }
}
