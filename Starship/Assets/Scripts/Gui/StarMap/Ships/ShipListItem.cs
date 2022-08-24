using System.Linq;
using Constructor.Ships;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.StarMap
{
    public class ShipListItem : MonoBehaviour
    {
        [SerializeField] private GameObject _disabledPanel;
        [SerializeField] private Image _icon;
        [SerializeField] private Text _levelText;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _classText;
        [SerializeField] private RectTransform _levelPanel;
        [SerializeField] private ModsPanel _modsPanel;

        public void Initialize(IShip ship, ILocalization localization, IResourceLocator resourceLocator, bool enabled = true)
        {
            Ship = ship;

            _nameText.text = localization.GetString(ship.Name);
            _nameText.color = ColorTable.QualityColor(ship.Model.Quality());
            _levelText.text = ship.Experience.Level > 0 ? ship.Experience.Level.ToString() : "0";
            _classText.text = ship.Model.SizeClass.ToString(localization);

            _icon.sprite = resourceLocator.GetSprite(ship.Model.ModelImage);
            _icon.color = ship.ColorScheme.HsvColor;

            if (_levelPanel != null)
                _levelPanel.gameObject.SetActive(ship.Experience.Level > 0);

            if (_modsPanel != null)
            {
                _modsPanel.gameObject.SetActive(ship.Model.Modifications.Any());
                _modsPanel.Initialize(ship.Model.Modifications, resourceLocator);
            }

            if (_disabledPanel)
                _disabledPanel.gameObject.SetActive(!enabled);
        }

        public IShip Ship { get; private set; }
    }
}