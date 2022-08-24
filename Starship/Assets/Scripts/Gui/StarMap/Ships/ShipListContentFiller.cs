using System.Collections.Generic;
using System.Linq;
using Constructor.Ships;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.StarMap
{
    class ShipListContentFiller : MonoBehaviour, IContentFiller
    {
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] ShipListItem ItemPrefab;

        public IShip SelectedShip { get; set; }

        public int SelectedShipIndex => _ships.FindIndex(item => item.Key == SelectedShip);

        public void InitializeShips(IEnumerable<KeyValuePair<IShip, bool>> ships)
        {
            _ships.Clear();
            _ships.AddRange(ships);
        }

        public void InitializeShips(IEnumerable<IShip> ships)
        {
            _ships.Clear();
            _ships.AddRange(ships.Select(ship => new KeyValuePair<IShip, bool>(ship, true)));
        }

        public GameObject GetListItem(int index, int itemType, GameObject obj)
        {
            if (obj == null)
            {
                obj = Instantiate(ItemPrefab.gameObject);
            }

            UpdateItem(obj.GetComponent<ShipListItem>(), _ships[index]);
            return obj;
        }

        public int GetItemCount()
        {
            return _ships.Count;
        }

        public int GetItemType(int index)
        {
            return 0;
        }

        private void UpdateItem(ShipListItem item, KeyValuePair<IShip, bool> ship)
        {
            item.gameObject.SetActive(true);
            item.Initialize(ship.Key, _localization, _resourceLocator, ship.Value);

            var toggle = item.GetComponent<Toggle>();
            if (toggle != null)
                toggle.isOn = SelectedShip == ship.Key;
        }

        private readonly List<KeyValuePair<IShip, bool>> _ships = new List<KeyValuePair<IShip, bool>>();
    }
}
