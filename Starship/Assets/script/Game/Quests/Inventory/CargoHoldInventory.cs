using System.Collections.Generic;
using System.Linq;
using Economy.ItemType;
using Economy.Products;
using GameServices.Player;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.Enums;

namespace GameModel
{
    namespace Quests
    {
        public class CargoHoldInventory : IInventory
        {
            public CargoHoldInventory(GameServices.Player.PlayerInventory inventory, PlayerFleet fleet, PlayerResources playerResources, ItemTypeFactory factory, IDatabase database)
            {
                _inventory = inventory;
                _playerResources = playerResources;
                _database = database;
                _fleet = fleet;
                _factory = factory;
            }

            public void Refresh() { }

            public IEnumerable<IProduct> Items
            {
                get
                {
                    foreach (var item in _playerResources.Resources)
                    {
                        var resource = _database.GetQuestItem(item);
                        if (resource != null)
                            yield return new PlayerProduct(_playerResources, _factory.CreateArtifactItem(resource), _playerResources.GetResource(item), _priceScale);
                    }

                    foreach (var item in _inventory.Components.Items)
                        yield return new PlayerProduct(_playerResources, _factory.CreateComponentItem(item.Key), item.Value, _priceScale);
                    foreach (var item in _inventory.Satellites.Items)
                        yield return new PlayerProduct(_playerResources, _factory.CreateSatelliteItem(item.Key), item.Value, _priceScale);

                    var activeShips = new HashSet<IShip>(_fleet.GetAllHangarShips());

                    foreach (var item in _fleet.Ships.Where(ship => ship.Model.Category != ShipCategory.Special && !activeShips.Contains(ship)))
                        yield return new PlayerProduct(_playerResources, _factory.CreateShipItem(item), 1, _priceScale);
                }
            }

            private readonly GameServices.Player.PlayerInventory _inventory;
            private readonly PlayerFleet _fleet;
            private readonly PlayerResources _playerResources;
            private readonly ItemTypeFactory _factory;
            private readonly IDatabase _database;

            private const int _priceScale = 5;
        }
    }
}
