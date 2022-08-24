using System.Linq;
using Economy.Products;
using GameModel.Quests;
using GameServices.Economy;
using GameServices.Gui;
using GameServices.Player;
using Services.Audio;
using Services.Gui;
using Services.Messenger;
using UnityEngine;
using ViewModel;
using Zenject;

namespace Gui.StarMap
{
    public class IapStore : MonoBehaviour
    {
        [Inject] private readonly InventoryFactory _inventoryFactory;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly LootGenerator _lootGenerator;
        [Inject] private readonly GuiHelper _guiHelper;

        [SerializeField] private MarketContentFiller ContentFiller;
        [SerializeField] private ListScrollRect ItemList;
        [SerializeField] private AudioClip BuySound;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener(EventType.IapItemsRefreshed, RefreshItems);
            messenger.AddListener(EventType.AdsManagerStatusChanged, RefreshItems);
            messenger.AddListener(EventType.SocialShareCompleted, RefreshItems);
            messenger.AddListener(EventType.RewardedVideoCompleted, OnRewardedVideoCompleted);
        }

        public void InitializeWindow(WindowArgs args)
        {
            _inventory = _inventoryFactory.CreateIapInventory();
            UpdateItems();
        }

        public void BuyButtonClicked(ViewModel.Common.InventoryItem item)
        {
            var product = item.Product;
            if (!product.Price.IsEnough(_playerResources))
                return;

            product.Buy();
            _soundPlayer.Play(BuySound);

            UpdateItems();
        }

        public void UpdateItems()
        {
            ContentFiller.InitializeItems(_inventory.Items, false, true);
            ItemList.RefreshContent();
        }

        private void RefreshItems()
        {
            if (!gameObject.activeSelf)
                return;

            _inventory.Refresh();
            UpdateItems();
        }

        private void OnRewardedVideoCompleted()
        {
            if (!gameObject.activeSelf)
                return;

            var reward = _lootGenerator.GetAdReward().ToArray();
            _guiHelper.ShowLootWindow(reward);
            reward.Consume();
        }

        private IInventory _inventory;
    }
}
