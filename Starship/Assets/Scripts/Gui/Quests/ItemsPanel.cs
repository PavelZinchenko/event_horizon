using System.Linq;
using Domain.Quests;
using Economy.Products;
using UnityEngine;
using UnityEngine.UI;
using Services.ObjectPool;
using Services.Reources;
using Zenject;

namespace Gui.Quests
{
    public class ItemsPanel : MonoBehaviour
    {
        [Inject] private readonly GameObjectFactory _factory;
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private LayoutGroup _layoutGroup;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Text _descriptionText;

        public void Initialize(ILoot loot)
        {
            if (loot != null && loot.Items.Any())
            {
                gameObject.SetActive(true);
                _layoutGroup.InitializeElements<ViewModel.Common.RewardItem, IProduct>(loot.Items.Select(item => (IProduct)new Product(item.Type, item.Quantity)), UpdateItem);
                _descriptionText.text = string.Empty;
                _scrollRect.content.anchoredPosition = Vector2.zero;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void OnItemSelected(ViewModel.Common.RewardItem item)
        {
            _descriptionText.text = item.Name;
            _descriptionText.color = item.Color;
        }

        public void OnItemDeselected()
        {
            _descriptionText.text = string.Empty;
        }

        private void UpdateItem(ViewModel.Common.RewardItem item, IProduct product)
        {
            item.Initialize(product, _resourceLocator);
            item.GetComponent<Toggle>().isOn = false;
        }

        private void Update()
        {
            _scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(_scrollRect.horizontalNormalizedPosition + Time.deltaTime * 0.01f);
        }
    }
}
