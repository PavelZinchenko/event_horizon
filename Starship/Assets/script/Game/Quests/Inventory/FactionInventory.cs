using System;
using System.Collections.Generic;
using System.Linq;
using Constructor.Ships;
using Database.Legacy;
using Economy.ItemType;
using Economy.Products;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using GameServices.Player;
using GameServices.Random;
using Market = Model.Regulations.Market;

namespace GameModel
{
    namespace Quests
    {
        public class FactionInventory : IInventory
        {
            public FactionInventory(Region region, ItemTypeFactory irItemTypeFactory, ProductFactory productFactory,
                PlayerSkills playerSkills, IRandom random, IDatabase database)
            {
                _starId = region.HomeStar;
                _level = region.MilitaryPower;
                _faction = region.Faction;
                _itemTypeFactory = irItemTypeFactory;
                _productFactory = productFactory;
                _random = random;
                _playerSkills = playerSkills;
                _database = database;
            }

            public void Refresh()
            {
                _items = null;
            }

            public IEnumerable<IProduct> Items
            {
                get
                {
                    if (_items == null)
                    {
                        if (_starId == 0)
                            CreateHomeRegionItems();
                        else
                            CreateItems();
                    }

                    return _items.Where(item => item.Quantity > 0).OfType<IProduct>();
                }
            }

            void CreateHomeRegionItems()
            {
                var pricescale = _playerSkills.PriceScale;
                var extraGoods = _playerSkills.HasMasterTrader ? 1 : 0;

                _items = new List<IProduct>();
                _items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateFuelItem(),
                    100 + 100 * extraGoods, _starId, Market.FuelRenewalTime, pricescale));

                TryCreateShipProduct(_items, "f5s1", pricescale);
                TryCreateShipProduct(_items, "f7s1", pricescale);
                TryCreateShipProduct(_items, "fns3", pricescale);
                TryCreateShipProduct(_items, "f0s1", pricescale);
                TryCreateShipProduct(_items, "f1s2", pricescale);
                TryCreateShipProduct(_items, "f2s2", pricescale);
                TryCreateShipProduct(_items, "f4s1", pricescale);

                if (extraGoods > 0)
                {
                    TryCreateShipProduct(_items, "f9s1", pricescale);
                    TryCreateShipProduct(_items, "fas3", pricescale);
                }

                for (var i = 0; i < 5 + extraGoods; ++i)
                {
                    var index = i;
                    TryCreateProduct(() => _productFactory.CreateRandomComponentProduct(_starId, index, _level,
                        Constructor.ComponentQuality.P1, _faction, false, Market.CommonComponentRenewalTime, false,
                        2f * pricescale));
                }
            }

            void CreateItems()
            {
                var random = _random.CreateRandom(_starId);
                var pricescale = _playerSkills.PriceScale;
                var extraGoods = _playerSkills.HasMasterTrader ? 1 : 0;

                _items = new List<IProduct>();
                _items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateFuelItem(),
                    100 + 100 * extraGoods, _starId, Market.FuelRenewalTime, 2f * pricescale));

                _items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateResearchItem(_faction),
                    random.Next(1, 5) + extraGoods, _starId, 0, pricescale));

                var ship = _database.ShipBuildList.OfFaction(_faction).Playable().Common()
                    .OfSize(SizeClass.Frigate, extraGoods > 0 ? SizeClass.Battleship : SizeClass.Cruiser)
                    .RandomElement(random);
                if (ship != null)
                {
                    _items.Add(_productFactory.CreateRenewableMarketProduct(
                        _itemTypeFactory.CreateShipItem(new CommonShip(ship)), 1, _starId, Market.ShipRenewalTime,
                        3f * pricescale));
                }

                for (var i = 0; i < 5 + extraGoods; ++i)
                {
                    var index = i;
                    TryCreateProduct(() => _productFactory.CreateRandomComponentProduct(_starId, index, _level,
                        Constructor.ComponentQuality.P1, _faction, false, Market.CommonComponentRenewalTime, false,
                        2f * pricescale));
                }
            }

            private void TryCreateShipProduct(List<IProduct> list, string legacyId, float pricescale)
            {
                TryCreateShipProduct(list, LegacyShipBuildNames.GetId(legacyId), pricescale);
            }

            private void TryCreateShipProduct(List<IProduct> list, ItemId<ShipBuild> id, float pricescale)
            {
                var build = _database.GetShipBuild(id);
                if (build == null) return;
                _items.Add(_productFactory.CreateRenewableMarketProduct(
                    _itemTypeFactory.CreateShipItem(new CommonShip(build)), 1, _starId, Market.ShipRenewalTime,
                    pricescale));
            }

            /// <summary>
            /// Tries creating a product, and ignores any ValueNotFound errors
            /// </summary>
            /// <param name="provider"></param>
            private void TryCreateProduct(Func<IProduct> provider)
            {
                try
                {
                    _items.Add(provider());
                }
                catch (ValueNotFoundException) { }
            }

            private int _starId;
            private int _level;
            private Faction _faction;
            private List<IProduct> _items;
            private readonly ItemTypeFactory _itemTypeFactory;
            private readonly ProductFactory _productFactory;
            private readonly IRandom _random;
            private readonly PlayerSkills _playerSkills;
            private readonly IDatabase _database;
        }
    }
}
