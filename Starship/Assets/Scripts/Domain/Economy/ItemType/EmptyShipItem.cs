using System;
using System.Linq;
using Constructor;
using Constructor.Ships;
using GameServices.Player;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class EmptyShipItem : IItemType
    {
        [Inject]
        public EmptyShipItem(ILocalization localization, PlayerFleet playerFleet, IShipModel ship)
        {
            _localization = localization;
            _playerFleet = playerFleet;
            _ship = ship;
        }

        public string Id { get { return "es" + _ship.Id; } }
        public string Name { get { return _localization.GetString(_ship.OriginalName); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return resourceLocator.GetSprite(_ship.ModelImage); }
        public Price Price { get { return Price.Common(_ship.Layout.CellCount * _ship.Layout.CellCount); } }
        public Color Color { get { return ColorTable.QualityColor(Quality); } }
        public ItemQuality Quality { get { return _ship.Quality(); } }

        public void Consume(int amount)
        {
            for (int i = 0; i < amount; ++i)
                _playerFleet.Ships.Add(new CommonShip(_ship, Enumerable.Empty<IntegratedComponent>()));
        }

        public void Withdraw(int amount)
        {
            throw new InvalidOperationException();
        }

        public int MaxItemsToConsume { get { return int.MaxValue; } }
        public int MaxItemsToWithdraw { get { return 0; } }

        private readonly IShipModel _ship;
        private readonly PlayerFleet _playerFleet;
        private readonly ILocalization _localization;
    }
}
