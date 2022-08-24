using GameDatabase.DataModel;
using Services.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Ehopedia
{
    public class FactionListItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _background;
        [SerializeField] private Toggle _toggle;
        [SerializeField] private Text _name;

        public void Initialize(Faction faction, bool unlocked, ILocalization localization)
        {
            Faction = faction;
            var color = faction.Color;
            _icon.color = color;
            if (_background) _background.color = new Color(color.R, color.G, color.B, 0.5f);
            _name.text = unlocked ? localization.GetString(faction.Name) : "???";
            _toggle.isOn = false;
            _toggle.interactable = unlocked;
        }

        public Faction Faction { get; private set; }
    }
}