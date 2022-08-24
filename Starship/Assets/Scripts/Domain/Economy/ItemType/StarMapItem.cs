using System;
using Galaxy;
using Maths;
using Services.Localization;
using Services.Messenger;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class StarMapItem : IItemType
    {
        [Inject]
        public StarMapItem(StarMap starMap, IMessenger messenger, ILocalization localization, int starId)
        {
            Id = "sm";
            _starMap = starMap;
            _star = _starMap.GetStarById(starId);
            _localization = localization;
            _messenger = messenger;
        }

        public string Id { get; private set; }
        public string Name { get { return _localization.GetString("$StarMapItem"); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return CommonSpriteTable.StarMap; }
        public Price Price { get { return Price.Common(Distance.Credits(_star.Level)); } }
        public Color Color { get { return Color.cyan; } }
        public ItemQuality Quality { get { return ItemQuality.Medium; } }

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
        private const float _radius = 3.2f;

        private readonly StarMap _starMap;
        private readonly ILocalization _localization;
        private readonly IMessenger _messenger;
    }
}
