using System;
using Services.IAP;
using Services.Reources;
using Session;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class PurchasedStarsItem : IItemType
    {
        [Inject]
        public PurchasedStarsItem(ISessionData session, InAppPurchaseCompletedSignal.Trigger iapCompletedTrigger)
        {
            _session = session;
            _iapCompletedTrigger = iapCompletedTrigger;
        }

        public string Id { get { return "iapstar"; } }
        public string Name { get { return "stars"; } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.StarCurrencyIcon; }
        public Color Color { get { return ColorTable.PremiumItemColor; } }
        public Price Price { get { return Price.Common(15000); } }
        public ItemQuality Quality { get { return ItemQuality.Perfect; } }

        public void Consume(int amount)
        {
            _session.Purchases.PurchasedStars += amount;
            _session.Resources.Stars += amount;
            _iapCompletedTrigger.Fire();
        }

        public void Withdraw(int amount)
        {
            throw new InvalidOperationException();
        }

        public int MaxItemsToConsume { get { return int.MaxValue; } }

        public int MaxItemsToWithdraw { get { return 0; } }

        private readonly ISessionData _session;
        private readonly InAppPurchaseCompletedSignal.Trigger _iapCompletedTrigger;
    }

    public class SupporterPackItem : IItemType
    {
        [Inject]
        public SupporterPackItem(IInAppPurchasing purchasing, InAppPurchaseCompletedSignal.Trigger iapCompletedTrigger)
        {
            _purchasing = purchasing;
            _iapCompletedTrigger = iapCompletedTrigger;
        }

        public string Id { get { return "iap_pack1"; } }
        public string Name { get { return "supporter pack"; } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.ShopIcon; }
        public Color Color { get { return ColorTable.PremiumItemColor; } }
        public Price Price { get { return Price.Common(15000); } }
        public ItemQuality Quality { get { return ItemQuality.Perfect; } }

        public void Consume(int amount)
        {
            _purchasing.ProcessPurchase(ProductIds.SupporterPack_Id);
            _iapCompletedTrigger.Fire();
        }

        public void Withdraw(int amount)
        {
            throw new InvalidOperationException();
        }

        public int MaxItemsToConsume { get { return 1; } }

        public int MaxItemsToWithdraw { get { return 0; } }

        private readonly IInAppPurchasing _purchasing;
        private readonly InAppPurchaseCompletedSignal.Trigger _iapCompletedTrigger;
    }
}
