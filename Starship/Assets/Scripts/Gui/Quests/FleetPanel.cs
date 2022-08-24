using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Constructor.Ships;
using Model.Military;
using Services.Reources;
using ViewModel;
using Zenject;

namespace Gui.Quests
{
    public class FleetPanel : MonoBehaviour
    {
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private LayoutGroup _shipsLayoutGroup;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private ViewModel.Quests.ThreatPanel _threatPanel;

        public void Initialize(IFleet fleet)
        {
            if (fleet != null && fleet.Ships.Any())
            {
                gameObject.SetActive(true);
                UpdateShips(fleet.Ships);
                _threatPanel.Initialize(fleet.Power);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void UpdateShips(IEnumerable<IShip> ships)
        {
            if (_shipsLayoutGroup)
                _shipsLayoutGroup.InitializeElements<ShipInfoViewModel, IShip>(ships, UpdateShip);
        }

        private void UpdateShip(ShipInfoViewModel item, IShip ship)
        {
            item.gameObject.SetActive(true);
            item.Icon.sprite = _resourceLocator.GetSprite(ship.Model.IconImage) ?? _resourceLocator.GetSprite(ship.Model.ModelImage);
            item.Icon.rectTransform.localScale = Vector3.one * ship.Model.IconScale;
            item.SetLevel(ship.Experience.Level);
            item.SetClass(ship.ExtraThreatLevel);
        }

        private void Start()
        {
            if (_scrollRect)
                _scrollRect.horizontalNormalizedPosition = 0;
        }

        private void Update()
        {
            if (_scrollRect)
                _scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(_scrollRect.horizontalNormalizedPosition + Time.deltaTime * 0.01f);
        }
    }
}
