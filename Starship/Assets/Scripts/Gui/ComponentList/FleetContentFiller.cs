using System;
using System.Collections.Generic;
using Constructor;
using Constructor.Ships;
using GameStateMachine.States;
using Services.Localization;
using Services.ObjectPool;
using Services.Reources;
using UnityEngine;
using UnityEngine.Events;
using ViewModel;
using Zenject;

namespace Gui.ComponentList
{
    public class FleetContentFiller : MonoBehaviour, IContentFiller
    {
        [Inject] private readonly GameObjectFactory _gameObjectFactory;
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly ShipSelectedSignal.Trigger _shipSelectedTrigger;

        [SerializeField] private ShipInfoViewModel _shipInfoPrefab;

        [SerializeField] private ItemSelectedEvent _itemSelectedEvent = new ItemSelectedEvent();

        [Serializable]
        public class ItemSelectedEvent : UnityEvent<ComponentInfo> { }

        private void Awake()
        {
            _shipInfoPrefab.gameObject.SetActive(false);
        }

        public GameObject GetListItem(int index, int itemType, GameObject obj)
        {
            if (obj == null)
            {
                obj = _gameObjectFactory.Create(_shipInfoPrefab.gameObject);
            }

            var item = obj.GetComponent<ShipInfoViewModel>();
            var ship = _ships[index];
            UpdateShip(item, ship);

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

        public void SetShips(IEnumerable<IShip> ships)
        {
            _ships.Clear();
            _ships.AddRange(ships);
        }

        private void UpdateShip(ShipInfoViewModel item, IShip ship)
        {
            item.Icon.sprite = _resourceLocator.GetSprite(ship.Model.IconImage) ?? _resourceLocator.GetSprite(ship.Model.ModelImage);
            item.Icon.color = ship.ColorScheme.HsvColor;
            item.Icon.rectTransform.localScale = 1.4f*ship.Model.IconScale*Vector3.one;
            item.NameText.text = _localization.GetString(ship.Name);
            item.SetLevel(ship.Experience.Level);
            item.ClassText.text = ship.Model.SizeClass.ToString(_localization);
            item.Button.onClick.RemoveAllListeners();
            item.Button.onClick.AddListener(() => ShipButtonClicked(ship));
        }

        private void ShipButtonClicked(IShip ship)
        {
            _shipSelectedTrigger.Fire(ship);
        }
        
        private ComponentInfo _selectedItem;
        private readonly List<IShip> _ships = new List<IShip>();
    }
}
