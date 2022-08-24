using System.Collections.Generic;
using Economy.Products;
using Gui.Windows;
using Services.Gui;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Exploration
{
    [RequireComponent(typeof(AnimatedWindow))]
    public class LootPanel : MonoBehaviour
    {
        [SerializeField] private float _cooldown = 5;
        [SerializeField] private LayoutGroup _layout;

        [Inject] private readonly IResourceLocator _resourceLocator;

        public void InitializeWindow(WindowArgs args)
        {
            _window = GetComponent<AnimatedWindow>();
            _elapsedTime = 0;

            var items = args.Get<IEnumerable<IProduct>>();
            _layout.InitializeElements<LootPanelItem, IProduct>(items, UpdateItem);
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < _cooldown)
                return;

            _window.Close(WindowExitCode.Ok);
        }

        private void UpdateItem(LootPanelItem item, IProduct data)
        {
            item.Initialize(data, _resourceLocator);
        }

        private float _elapsedTime;
        private AnimatedWindow _window;

    }
}
