using System;
using DataModel.Technology;
using GameServices.Research;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class BlueprintItem : IItemType
    {
        [Inject]
        public BlueprintItem(ITechnology technology, Research research, ILocalization localization)
        {
            _technology = technology;
            _localization = localization;
            _research = research;
        }

        public string Id { get { return "blueprint_" + _technology.Id; } }
        public string Name { get { return _localization.GetString("$Blueprint", _technology.GetName(_localization)); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.Blueprint; }
        public Color Color { get { return Color.white; } }
        public Price Price { get { return Price.Common(_technology.GetCraftPrice(CraftItemQuality.Common).Credits*3); } }
        public ItemQuality Quality { get { return ItemQuality.Medium; } }

        public void Consume(int amount)
        {
            _research.ResearchTechForFree(_technology);
        }

        public void Withdraw(int amount)
        {
            throw new InvalidOperationException();
        }

        public int MaxItemsToConsume { get { return 1; } }

        public int MaxItemsToWithdraw { get { return 0; } }

        private readonly ITechnology _technology;
        private readonly ILocalization _localization;
        private readonly Research _research;
    }
}
