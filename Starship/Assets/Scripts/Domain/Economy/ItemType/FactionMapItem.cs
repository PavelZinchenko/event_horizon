using System;
using Maths;
using Galaxy;
using Services.Localization;
using Services.Messenger;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class FactionMapItem : IItemType
    {
        [Inject]
        public FactionMapItem(StarMap starMap, IMessenger messenger, ILocalization localization, int starId)
        {
            _localization = localization;
            _messenger = messenger;
            _starMap = starMap;
            Id = "fm";
            _star = starMap.GetStarById(starId);
            Price = Price.Common(5 * Distance.Credits(_star.Level));
            Color = _star.Region.Faction.Color;
        }

        public string Id { get; private set; }
        public string Name { get { return _localization.GetString("$StarMapItem"); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.FactionStarMap; }
        public Price Price { get; private set; }
        public Color Color { get; private set; }
        public ItemQuality Quality { get { return ItemQuality.High; } }

        public void Consume(int amount)
        {
            var center = _star.Position;
            var stars = _starMap.GetVisibleStars(center - Vector2.one * _radius, center + Vector2.one * _radius);
            foreach (var item in stars)
                if (Vector2.Distance(item.Position, center) <= _radius)
                    item.SetVisited();

            _messenger.Broadcast(EventType.StarMapChanged);
        }

        public void Withdraw(int amount)
        {
            throw new InvalidOperationException();
        }

        public int MaxItemsToConsume { get { return 1; } }
        public int MaxItemsToWithdraw { get { return 0; } }

        private readonly Galaxy.Star _star;
        private const float _radius = 1.1f;

        private readonly StarMap _starMap;
        private readonly ILocalization _localization;
        private readonly IMessenger _messenger;
    }
}
