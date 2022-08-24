using System.Collections.Generic;
using Economy.Products;
using Services.Gui;
using Services.ObjectPool;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Common;
using ViewModel.Quests;
using Zenject;

namespace Gui.Dialogs
{
    public class DailyRewardDialog : MonoBehaviour
    {
        [Inject] private readonly GameObjectFactory _factory;
        [Inject] private readonly IResourceLocator _resourceLocator;

        public LayoutGroup LayoutGroup;
        public ScrollRect ScrollRect;
        public Text DescriptionText;

        public void InitializeWindow(WindowArgs args)
        {
            var items = new List<IProduct>();
            for (var i = 0; i < args.Count; ++i)
                items.Add(args.Get<IProduct>(i));

                gameObject.SetActive(true);
                LayoutGroup.InitializeElements<RewardItem, IProduct>(items, UpdateItem);
                DescriptionText.text = string.Empty;
                ScrollRect.content.anchoredPosition = Vector2.zero;
        }

        public void OnItemSelected(GameObject item)
        {
            var description = item.GetComponent<IItemDescription>();

            DescriptionText.text = description.Name;
            DescriptionText.color = description.Color;
        }

        public void OnItemDeselected()
        {
            DescriptionText.text = string.Empty;
        }

        private void UpdateItem(RewardItem item, IProduct product)
        {
            item.Initialize(product, _resourceLocator);
            item.GetComponent<Toggle>().isOn = false;
        }

        private void Update()
        {
            ScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(ScrollRect.horizontalNormalizedPosition + Time.deltaTime * 0.01f);
        }
    }
}

