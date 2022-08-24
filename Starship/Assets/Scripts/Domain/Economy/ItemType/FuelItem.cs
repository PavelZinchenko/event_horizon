using GameServices.Player;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class FuelItem : IItemType
    {
        [Inject]
        public FuelItem(PlayerResources playerResources, PlayerSkills playerSkills, ILocalization localization)
        {
            _playerResources = playerResources;
            _playerSkills = playerSkills;
            _localization = localization;
        }

        public string Id { get { return "f"; } }
        public string Name { get { return _localization.GetString("$FuelItem"); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.FuelIcon; }
        public Price Price { get { return Price.Common(1); } }
        public Color Color { get { return Color.cyan; } }
        public ItemQuality Quality { get { return ItemQuality.Common; } }

        public void Consume(int amount)
        {
            _playerResources.Fuel += amount;
        }

        public void Withdraw(int amount)
        {
            _playerResources.Fuel -= amount;
        }

        public int MaxItemsToConsume { get { return _playerSkills.MainFuelCapacity - _playerResources.Fuel; } }
        public int MaxItemsToWithdraw { get { return _playerResources.Fuel; } }

        private readonly PlayerResources _playerResources;
        private readonly PlayerSkills _playerSkills;
        private readonly ILocalization _localization;
    }
}
