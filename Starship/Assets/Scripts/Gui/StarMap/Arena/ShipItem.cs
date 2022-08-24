using Constructor.Ships;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Multiplayer
{
    public class ShipItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
        [SerializeField] private Color _defaultColor = Color.black;
        [SerializeField] private GameObject _notAllowedPanel;
        [SerializeField] private GameObject _allowedPanel;

        public void Initialize(IShip ship, bool isAllowed, IResourceLocator resourceLocator, ILocalization localization)
        {
            Ship = ship;

            _icon.sprite = resourceLocator.GetSprite(ship.Model.ModelImage);
            _icon.color = ship.ColorScheme.Type == ShipColorScheme.SchemeType.Default ? _defaultColor : ship.ColorScheme.HsvColor;
            _name.text = localization.GetString(ship.Name);
            _notAllowedPanel.SetActive(!isAllowed);
            _allowedPanel.SetActive(isAllowed);
        }

        public IShip Ship { get; private set; }
    }
}
