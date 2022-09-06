using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServices.Player;
using Services.Localization;
using Constructor.Ships;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class ShipItem : IItemType
    {
        [Inject]
        public ShipItem(PlayerFleet playerFleet, ILocalization localization, ItemTypeFactory itemTypeFactory, IShip ship, bool premium = false,
            bool fuzzy = false)
        {
            _localization = localization;
            _playerFleet = playerFleet;
            _ship = ship;
            _premium = premium;
            _fuzzy = fuzzy;
            _itemTypeFactory = itemTypeFactory;
        }

        public int Rank => _ship.Experience.Level;

        public string Id => "sh" + _ship.Id;

        public string Name
        {
            get
            {
                if (_name == null)
                    _name = _localization.GetString(_ship.Name);

                return _name;
            }
        }

        public string Description
        {
            get
            {
                if (_description == null)
                {
                    var sb = new StringBuilder();
                    sb.Append(_localization.GetString("$Level"));
                    sb.Append(_localization.GetString(" "));
                    sb.Append(_localization.GetString(_ship.Experience.Level.ToString()));
                    foreach (var mod in Ship.Model.Modifications)
                    {
                        sb.Append("\n");
                        sb.Append(mod.GetDescription(_localization));
                    }

                    _description = sb.ToString();
                }

                return _description;
            }
        }

        public Sprite GetIcon(IResourceLocator resourceLocator)
        {
            return resourceLocator.GetSprite(_ship.Model.ModelImage);
        }

        public Price Price =>
            _premium
                ? Price.Premium(((int)_ship.Model.Category + 1) * _ship.Model.Layout.CellCount / 5)
                : Price.Common(_ship.Price());

        public Color Color => Color.white;

        public ItemQuality Quality => _ship.Model.Quality();

        public void Consume(int amount)
        {
            for (int i = 0; i < amount; ++i)
                _playerFleet.Ships.Add(new CommonShip(_ship.Model, _ship.Components) { Experience = _ship.Experience });
        }

        public void Withdraw(int amount)
        {
            if (!_fuzzy)
            {
                Strip(_ship);
                _playerFleet.Ships.Remove(_ship);
            }
            else
            {
                var ships = new List<IShip>();
                foreach (var ship in _playerFleet.Ships)
                {
                    if (ship.Id == _ship.Id) ships.Add(ship);
                }

                // Removing ships with lower XP level first
                ships.Sort((a, b) => Math.Sign(a.Experience - b.Experience));
                for (var i = 0; i < amount && i < ships.Count; i++)
                {
                    Strip(ships[i]);
                    _playerFleet.Ships.Remove(ships[i]);
                }
            }
        }

        private void Strip(IShip ship)
        {
            foreach (var component in ship.ComponentCounts(true))
            {
                _itemTypeFactory.CreateComponentItem(component.Key).Consume(component.Value);
            }
            
            for (var i = 0; i < ship.Components.Count; i++)
            {
                var component = ship.Components[i];
                if (!component.Locked) ship.Components.RemoveAt(i);
                i--;
            }
        }

        public int MaxItemsToConsume => int.MaxValue;

        public int MaxItemsToWithdraw
        {
            get
            {
                if (!_fuzzy) return 0;
                var count = 0;
                var id = _ship.Id;
                foreach (var ship in _playerFleet.Ships)
                {
                    if (ship.Id == id) count++;
                }

                return count;
            }
        }

        public IShip Ship => _ship;

        private string _name;
        private string _description;
        private readonly IShip _ship;
        private readonly bool _premium;
        private readonly bool _fuzzy;
        private readonly ItemTypeFactory _itemTypeFactory;
        private readonly PlayerFleet _playerFleet;
        private readonly ILocalization _localization;
    }
}
