using System;
using System.Linq;
using GameServices.Player;
using Services.Localization;
using Constructor.Ships;
using GameDatabase;
using Gui.StarMap;
using Services.Reources;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Gui.ShipService
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
            _shipListContentFiller.SelectedShip = null;
            _shipListContentFiller.InitializeShips(_playerFleet.Ships.OrderBy(ship => ship.Id.Value));
            _shipList.RefreshContent();
        }

        public void OnItemSelected(ShipListItem ship)
        {
            _shipSelectedEvent.Invoke(ship.Ship);
        }
    }
}
