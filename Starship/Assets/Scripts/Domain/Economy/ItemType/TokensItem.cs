using GameServices.Player;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class TokensItem : IItemType
    {
        [Inject]
        public TokensItem(PlayerResources playerResources, ILocalization localization)
        {
            _playerResources = playerResources;
            _localization = localization;
        }

        public string Id { get { return "token"; } }
        public string Name { get { return _localization.GetString("$TokenCurrency"); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.TokenCurrencyIcon; }
        public Color Color { get { return ColorTable.TokensColor; } }
        public Price Price { get { return Price.Tokens(1); } }
        public ItemQuality Quality { get { return ItemQuality.Perfect; } }

        public void Consume(int amount)
        {
            _playerResources.Tokens += amount;
        }

        public void Withdraw(int amount)
        {
            _playerResources.Tokens -= amount;
        }

        public int MaxItemsToConsume { get { return int.MaxValue; } }

        public int MaxItemsToWithdraw { get { return _playerResources.Tokens; } }

        private readonly PlayerResources _playerResources;
        private readonly ILocalization _localization;
    }
}
