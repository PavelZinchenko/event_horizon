using GameModel.Quests;
using GameServices.Player;
using Services.Audio;
using Services.Gui;
using UnityEngine;
using ViewModel;
using Zenject;

namespace Gui.StarMap
{
    public class SpecialStore : MonoBehaviour
    {
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly PlayerResources _playerResources;

        [SerializeField] private MarketContentFiller ContentFiller;
        [SerializeField] private ListScrollRect ItemList;
        [SerializeField] private AudioClip BuySound;

        public void InitializeWindow(WindowArgs args)
        {
            _inventory = args.Get<IInventory>();
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
            _inventory.Refresh();
            ContentFiller.InitializeItems(_inventory.Items/*.OrderBy(item => item.Type.Id)*/, false, true);
            ItemList.RefreshContent();
        }

        private IInventory _inventory;
    }
}
