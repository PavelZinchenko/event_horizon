using System.Collections.Generic;
using System.Linq;
using Constructor;
using Economy;
using Economy.ItemType;
using Economy.Products;
using GameDatabase;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameServices.Player;
using GameServices.Random;
using UnityEngine;

namespace GameModel
{
    namespace Quests
    {
        public class ArenaInventory : IInventory
        {
            public ArenaInventory(Galaxy.Star star, ItemTypeFactory itemTypeFactory, ProductFactory productFactory, IDatabase database, PlayerSkills playerSkills, IRandom random)
            {
                _star = star;
                _random = random;
                _database = database;
                _productFactory = productFactory;
                _playerSkills = playerSkills;
                _itemTypeFactory = itemTypeFactory;
            }

            public void Refresh() { _items = null; }

            public IEnumerable<IProduct> Items
            {
                get
                {
                    if (_items == null)
                    {
                        var random = _random.CreateRandom(_star.Id);
                        var pricescale = _playerSkills.PriceScale;
                        var extraGoods = _playerSkills.HasMasterTrader ? 1 : 0;
                        var faction = _star.Region.Faction;

                        _items = new List<IProduct>();

                        var componentCount = 4 + extraGoods + random.Next(3);
                        var components = _database.ComponentList.Where(item => item.Availability == Availability.Common || (extraGoods > 0 && item.Availability == Availability.Rare)).OfFaction(faction).LevelLessOrEqual(_star.Level);
                        foreach (var item in components.RandomUniqueElements(componentCount, random))
                        {
                            var itemType = _itemTypeFactory.CreateComponentItem(ComponentInfo.CreateRandomModification(item, random, ModificationQuality.P3));
                            _items.Add(_productFactory.CreateArenaProduct(itemType, Price.Tokens(Mathf.RoundToInt(10 + 10*item.Level*pricescale))));
                        }
                    }

                    return _items;
                }
            }

            private List<IProduct> _items;

            private readonly Galaxy.Star _star;
            private readonly IRandom _random;
            private readonly ItemTypeFactory _itemTypeFactory;
            private readonly ProductFactory _productFactory;
            private readonly PlayerSkills _playerSkills;
            private readonly IDatabase _database;
        }
    }
}
