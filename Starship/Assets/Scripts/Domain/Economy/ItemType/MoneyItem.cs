using GameServices.Player;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class MoneyItem : IItemType
    {
        [Inject]
        public MoneyItem(PlayerResources playerResources, ILocalization localization)
        {
            _playerResources = playerResources;
            _localization = localization;
        }

        public string Id { get { return "m"; } }
        public string Name { get { return _localization.GetString("$Credits"); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.CreditsIcon; }
        public Price Price { get { return Price.Common(1); } }
        public Color Color { get { return Color.green; } }
        public ItemQuality Quality { get { return ItemQuality.Common; } }

        public void Consume(int amount)
        {
            _playerResources.Money += amount;
        }

        public void Withdraw(int amount)
        {
            _playerResources.Money -= amount;
        }

        public int MaxItemsToConsume { get { return int.MaxValue; } }
        public int MaxItemsToWithdraw { get { return _playerResources.Money; } }

        private readonly PlayerResources _playerResources;
        private readonly ILocalization _localization;
    }
}
