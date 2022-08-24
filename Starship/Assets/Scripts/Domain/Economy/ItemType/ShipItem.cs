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
        public ShipItem(PlayerFleet playerFleet, ILocalization localization, IShip ship, bool premium = false)
        {
            _localization = localization;
            _playerFleet = playerFleet;
            _ship = ship;
            _premium = premium;
        }

        public int Rank { get { return _ship.Experience.Level; } }

        public string Id { get { return "sh" + _ship.Id; } }

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

        public Sprite GetIcon(IResourceLocator resourceLocator) { return resourceLocator.GetSprite(_ship.Model.ModelImage); }
        public Price Price { get { return _premium ? Price.Premium(((int)_ship.Model.Category + 1) * _ship.Model.Layout.CellCount / 5) : Price.Common(_ship.Price()); } }
        public Color Color { get { return Color.white; } }
        public ItemQuality Quality { get { return _ship.Model.Quality(); } }

        public void Consume(int amount)
        {
            for (int i = 0; i < amount; ++i)
                _playerFleet.Ships.Add(new CommonShip(_ship.Model, _ship.Components) { Experience = _ship.Experience });
        }

        public void Withdraw(int amount)
        {
            _playerFleet.Ships.Remove(_ship);
        }

        public int MaxItemsToConsume { get { return int.MaxValue; } }
        public int MaxItemsToWithdraw { get { return 0; } }

        public IShip Ship { get { return _ship; } }

        private string _name;
        private string _description;
        private readonly IShip _ship;
        private readonly bool _premium;
        private readonly PlayerFleet _playerFleet;
        private readonly ILocalization _localization;
    }
}
