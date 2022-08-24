using System.Collections.Generic;
using Economy.Products;
using Services.Reources;
using UnityEngine.UI;
using UnityEngine;
using Zenject;

namespace ViewModel
{
    class CargoHoldContentFiller : MonoBehaviour, IContentFiller
    {
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] CargoHoldItem ItemPrefab;

        public void OnItemSelected(CargoHoldItem item)
        {
            _selectedItemIndex = item ? _products.IndexOf(item.Product) : -1;
        }

        public void Initialize(IEnumerable<IProduct> products,bool clearSelection = false)
        {
            _products.Clear();
            _products.AddRange(products);

            if (clearSelection)
                _selectedItemIndex = -1;
        }

        public GameObject GetListItem(int index, int itemType, GameObject obj)
        {
            if (obj == null)
            {
                obj = Instantiate(ItemPrefab.gameObject);
            }

            UpdateItem(obj.GetComponent<CargoHoldItem>(), _products[index]);

            var toggle = obj.GetComponent<Toggle>();
            if (toggle != null)
                toggle.isOn = index == _selectedItemIndex;

            return obj;
        }

        public int GetItemCount()
        {
            return _products.Count;
        }

        public int GetItemType(int index)
        {
            return 0;
        }

        private void UpdateItem(CargoHoldItem item, IProduct product)
        {
            item.gameObject.SetActive(true);
            item.Initialize(product, _resourceLocator);
        }

        private int _selectedItemIndex = -1;
        private readonly List<IProduct> _products = new List<IProduct>();
    }
}
