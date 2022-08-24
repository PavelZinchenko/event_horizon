using System.Collections.Generic;
using Economy.Products;
using GameServices.Player;
using Services.ObjectPool;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ViewModel
{
    class MarketContentFiller : MonoBehaviour, IContentFiller
    {
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly GameObjectFactory _gameObjectFactory;
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] Common.InventoryItem _itemPrefab;

        public void OnItemSelected(Common.InventoryItem item)
        {
            _selectedItemIndex = item ? _items.IndexOf(item.Product) : -1;
        }

        public void InitializeItems(IEnumerable<IProduct> items, bool sellMode, bool clearSelection)
        {
            _items.Clear();
            _items.AddRange(items);
            _sellMode = sellMode;

            if (clearSelection)
                _selectedItemIndex = -1;
        }

        public GameObject GetListItem(int index, int itemType, GameObject obj)
        {
            if (obj == null)
            {
                obj = _gameObjectFactory.Create(_itemPrefab.gameObject);
            }

            UpdateItem(obj.GetComponent<Common.InventoryItem>(), _items[index]);
            var toggle = obj.GetComponent<Toggle>();
            if (toggle != null)
                toggle.isOn = index == _selectedItemIndex;

            return obj;
        }

        public int GetItemCount()
        {
            return _items.Count;
        }

        public int GetItemType(int index)
        {
            return 0;
        }

        private void UpdateItem(Common.InventoryItem item, IProduct product)
        {
            item.gameObject.SetActive(true);
            item.Initialize(product, !(_sellMode || product.Price.IsEnough(_playerResources)), _resourceLocator);
        }

        private bool _sellMode;
        private int _selectedItemIndex = 0;
        private readonly List<IProduct> _items = new List<IProduct>();
    }
}
