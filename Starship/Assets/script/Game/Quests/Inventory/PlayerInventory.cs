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
		public class PlayerInventory : IInventory
		{
		    public PlayerInventory(GameServices.Player.PlayerInventory inventory, PlayerFleet fleet, PlayerResources playerResources, ItemTypeFactory factory, IDatabase database)
		    {
		        _inventory = inventory;
                _playerResources = playerResources;
		        _fleet = fleet;
		        _factory = factory;
		        _database = database;
		    }

			public void Refresh() {}

			public IEnumerable<IProduct> Items
			{
				get
				{
				    foreach (var item in _playerResources.Resources)
				    {
				        var resource = _database.GetQuestItem(item);
				        if (resource == null || resource.Price == 0) continue;
				        var quantity = _playerResources.GetResource(item);
				        yield return new PlayerProduct(_playerResources, _factory.CreateArtifactItem(resource), quantity);
				    }

					foreach (var item in _inventory.Components.Items)
						yield return new PlayerProduct(_playerResources, _factory.CreateComponentItem(item.Key), item.Value);
					foreach (var item in _inventory.Satellites.Items)
						yield return new PlayerProduct(_playerResources, _factory.CreateSatelliteItem(item.Key), item.Value);

					var activeShips = new HashSet<IShip>(_fleet.GetAllHangarShips());
					foreach (var item in _fleet.Ships.Where(ship => ship.Model.Category != ShipCategory.Special && !activeShips.Contains(ship)))
						yield return new PlayerProduct(_playerResources, _factory.CreateShipItem(item));
				}
			}

		    private readonly GameServices.Player.PlayerInventory _inventory;
		    private readonly IDatabase _database;
            private readonly PlayerFleet _fleet;
            private readonly PlayerResources _playerResources;
            private readonly ItemTypeFactory _factory;
        }
	}
}
