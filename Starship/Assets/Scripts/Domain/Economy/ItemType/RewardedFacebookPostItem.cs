using System;
using Services.Localization;
using Services.Reources;
using Services.Social;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class RewardedFacebookPostItem : IItemType
    {
        [Inject]
        public RewardedFacebookPostItem(IFacebookFacade facebookFacade, ILocalization localization)
        {
            _facebookFacade = facebookFacade;
            _localization = localization;
        }

        public string Id { get { return "facebook"; } }
        public string Name { get { return _localization.GetString("$FacebookItemDescription"); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.RewardedFacebookPost; }
        public Color Color { get { return Color.white; } }
        public Price Price { get { return new Price(0, Currency.None); } }
        public ItemQuality Quality { get { return ItemQuality.Common; } }

        public void Consume(int amount)
        {
            _facebookFacade.Share();
        }

        public void Withdraw(int amount)
        {
            throw new InvalidOperationException();
        }

        public int MaxItemsToConsume { get { return 1; } }

        public int MaxItemsToWithdraw { get { return 0; } }

        private readonly IFacebookFacade _facebookFacade;
        private readonly ILocalization _localization;
    }
}
