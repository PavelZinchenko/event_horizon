using GameDatabase.DataModel;
using GameServices.Player;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class SatelliteItem : IItemType
    {
        [Inject]
        public SatelliteItem(PlayerInventory inventory, ILocalization localization, Satellite satellite, bool premium = false)
        {
            _localization = localization;
            _inventory = inventory;
            _premium = premium;
            Satellite = satellite;
        }

        public string Id { get { return "sat" + Satellite.Id; } }
        public string Name { get { return _localization.GetString(Satellite.Name); } }
        public string Description { get { return string.Empty; } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return resourceLocator.GetSprite(Satellite.ModelImage); }
        public Color Color { get { return Color.white; } }

        public Price Price { get { return Economy.Price.SatellitePrice(Satellite, _premium); } }
        public Currency Currency { get { return _premium ? Currency.Stars : Currency.Credits; } }
        public ItemQuality Quality { get { return ItemQuality.Common; } }

        public void Consume(int amount)
        {
            _inventory.Satellites.Add(Satellite, amount);
        }

        public void Withdraw(int amount)
        {
            _inventory.Satellites.Remove(Satellite, amount);
        }

        public Satellite Satellite { get; private set; }

        public int MaxItemsToConsume { get { return int.MaxValue; } }
        public int MaxItemsToWithdraw { get { return _inventory.Satellites.GetQuantity(Satellite); } }

        private readonly bool _premium;
        private readonly PlayerInventory _inventory;
        private readonly ILocalization _localization;
    }
}
