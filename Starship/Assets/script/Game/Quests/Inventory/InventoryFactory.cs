using System.Collections.Generic;
using Domain.Quests;
using Economy.ItemType;
using Economy.Products;
using Galaxy;
using Game;
using GameDatabase;
using GameDatabase.DataModel;
using GameServices.Economy;
using GameServices.Player;
using GameServices.Random;
using GameServices.Research;
using Zenject;

namespace GameModel.Quests
{
    public class InventoryFactory
    {
        [Inject] private readonly StarMap _starMap;
        [Inject] private readonly ItemTypeFactory _itemTypeFactory;
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly GameServices.Player.PlayerInventory _playerInventory;
        [Inject] private readonly PlayerFleet _playerFleet;
        [Inject] private readonly LootGenerator _lootGenerator;
        [Inject] private readonly IRandom _random;
        [Inject] private readonly ProductFactory _productFactory;
        [Inject] private readonly PlayerSkills _playerSkills;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly HolidayManager _holidayManager;
        [Inject] private readonly Research _research;

        public IInventory CreateBlackMarketInventory(int starId)
        {
            return new BlackMarketInventory(_starMap.GetStarById(starId), _itemTypeFactory, _productFactory, _playerSkills, _random, _database);
        }

        public IInventory CreateBlackMarketPlayerInventory(int starId)
        {
            return new BlackMarketPlayerInventory(_playerResources, _itemTypeFactory);
        }

        public IInventory CreateFactionInventory(Region region)
        {
            return new FactionInventory(region, _itemTypeFactory, _productFactory, _playerSkills, _random, _database);
        }

        public IInventory CreateMerchantInventory(int starId, int level, Faction faction)
        {
            return new MerchantInventory(starId, level, _productFactory, _lootGenerator, _playerSkills, _random);
        }

        public IInventory CreateQuestInventory(ILoot items)
        {
            return new QuestInventory(items, _productFactory, _playerSkills);
        }

        public IInventory CreatePlayerInventory()
        {
            return new PlayerInventory(_playerInventory, _playerFleet, _playerResources, _itemTypeFactory, _database);
        }

        public IInventory CreateCargoHoldInventory()
        {
            return new CargoHoldInventory(_playerInventory, _playerFleet, _playerResources, _itemTypeFactory, _database);
        }

        public IInventory CreateArenaInventory(Galaxy.Star star)
        {
            return new ArenaInventory(star, _itemTypeFactory, _productFactory, _database, _playerSkills, _random);
        }

        public IInventory CreateInventory(IEnumerable<IProduct> items = null)
        {
            return new Inventory(items);
        }

        public IInventory CreateSantaInventory(int starId)
        {
            return new SantaInventory(_starMap.GetStarById(starId), _itemTypeFactory, _productFactory, _research, _random, _database);
        }
    }
}
