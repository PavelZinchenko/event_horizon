using System;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices.Research;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class ResearchItem : IItemType
    {
        [Inject]
        public ResearchItem(ILocalization localization, Research research, Faction faction)
        {
            _research = research;
            _localization = localization;
            _faction = faction;
        }

        public string Id { get { return "r" + _faction.Id.Value; } }
        public string Name { get { return _localization.GetString("$AlienTechnology"); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.TechIcon; }
        public Color Color { get { return _faction.Color; } }
        public Price Price { get { return Price.Premium(1); } }
        public ItemQuality Quality { get { return ItemQuality.Common; } }

        public void Consume(int amount)
        {
            _research.AddResearchPoints(_faction, amount);
        }

        public void Withdraw(int amount)
        {
            throw new InvalidOperationException();
        }

        public int MaxItemsToConsume { get { return int.MaxValue; } }

        public int MaxItemsToWithdraw { get { return 0; } }

        private readonly Faction _faction;
        private readonly ILocalization _localization;
        private readonly Research _research;
    }
}
