using GameDatabase.DataModel;
using GameServices.Player;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class ArtifactItem : IItemType
    {
        [Inject]
        public ArtifactItem(ILocalization localization, QuestItem questItem, PlayerResources playerResources)
        {
            _localization = localization;
            _questItem = questItem;
            _playerResources = playerResources;
        }

        public string Id { get { return "a" + _questItem.Id.Value; } }
        public string Name { get { return _localization.GetString(_questItem.Name); } }
        public string Description { get { return _localization.GetString(_questItem.Description); } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return resourceLocator.GetSprite(_questItem.Icon); }
        public Color Color { get { return _questItem.Color; } }
        public Price Price { get { return Price.Common(_questItem.Price); } }
        public ItemQuality Quality { get { return ItemQuality.High; } }

        public void Consume(int amount)
        {
            _playerResources.AddResource(_questItem.Id, amount);
        }

        public void Withdraw(int amount)
        {
            _playerResources.RemoveResource(_questItem.Id, amount);
        }

        public int MaxItemsToConsume { get { return int.MaxValue; } }

        public int MaxItemsToWithdraw { get { return _playerResources.GetResource(_questItem.Id); } }

        private readonly QuestItem _questItem;
        private readonly ILocalization _localization;
        private readonly PlayerResources _playerResources;
    }
}
