using System;
using System.Collections.Generic;
using System.Linq;
using GameServices.Player;
using Services.Localization;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.Enums;
using Gui.StarMap;
using Services.Reources;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Gui.Exploration
{
    public class FleetPanel : MonoBehaviour
    {
        [Inject] private readonly PlayerFleet _playerFleet;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private ListScrollRect _shipList;
        [SerializeField] private ShipListContentFiller _shipListContentFiller;

        [SerializeField] private ShipSelectedEvent _shipSelectedEvent = new ShipSelectedEvent();

        [Serializable]
        public class ShipSelectedEvent : UnityEvent<IShip> { }

        public void Initialize()
        {
            _shipListContentFiller.SelectedShip = _playerFleet.ExplorationShip;
            _shipListContentFiller.InitializeShips(_playerFleet.Ships.Where(IsShipAllowed).OrderBy(ship => ship.Id.Value));
            _shipList.RefreshContent();

            var selectedIndex = _shipListContentFiller.SelectedShipIndex;
            if (selectedIndex >= 0)
                _shipList.ScrollToListItem(selectedIndex);
        }

        public void OnItemSelected(ShipListItem ship)
        {
            _shipListContentFiller.SelectedShip = ship.Ship;
            _shipSelectedEvent.Invoke(ship.Ship);
        }

        private static bool IsShipAllowed(IShip ship)
        {
            if (ship.Model.SizeClass == SizeClass.Frigate) return true;
            //if (ship.Id.Value == 103) return true; // wormship
            //if (ship.Id.Value == 78) return true; // scavenger

            return false;
        }
    }
}
