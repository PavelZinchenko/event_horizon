using GameServices.Player;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class SnowflakesItem : IItemType
    {
        [Inject]
        public SnowflakesItem(PlayerResources playerResources, ILocalization localization)
        {
            _localization = localization;
            _playerResources = playerResources;
        }

        public string Id { get { return "snowflake"; } }
        public string Name { get { return _localization.GetString("$Snowflakes"); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.SnowflakesIcon; }
        public Color Color { get { return ColorTable.SnowflakesColor; } }
        public Price Price { get { return new Price(1, Currency.Snowflakes); } }
        public ItemQuality Quality { get { return ItemQuality.Common; } }

        public bool IsEqual(IItemType other) { return other.GetType() == GetType(); }

        public void Consume(int amount)
        {
            _playerResources.Snowflakes += amount;
        }

        public void Withdraw(int amount)
        {
            _playerResources.Snowflakes -= amount;
        }

        public int MaxItemsToConsume { get { return int.MaxValue; } }

        public int MaxItemsToWithdraw { get { return _playerResources.Snowflakes; } }

        private readonly PlayerResources _playerResources;
        private readonly ILocalization _localization;
    }
}
