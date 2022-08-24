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
using GameServices.Player;
using GameServices.Random;
using Market = Model.Regulations.Market;

namespace GameModel
{
    namespace Quests
	{
		public class FactionInventory : IInventory
		{
			public FactionInventory(Region region, ItemTypeFactory irItemTypeFactory, ProductFactory productFactory, PlayerSkills playerSkills, IRandom random, IDatabase database)
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

			public void Refresh() { _items = null; }

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
				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateFuelItem(), 100 + 100*extraGoods, _starId, Market.FuelRenewalTime, pricescale));
				
				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("f5s1")))), 1, _starId, Market.ShipRenewalTime, pricescale));
				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("f7s1")))), 1, _starId, Market.ShipRenewalTime, pricescale));
				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("fns3")))), 1, _starId, Market.ShipRenewalTime, pricescale));
				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("f0s1")))), 1, _starId, Market.ShipRenewalTime, pricescale));
				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("f1s2")))), 1, _starId, Market.ShipRenewalTime, pricescale));
				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("f2s2")))), 1, _starId, Market.ShipRenewalTime, pricescale));
				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("f4s1")))), 1, _starId, Market.ShipRenewalTime, pricescale));

                if (extraGoods > 0)
			    {
			        _items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("f9s1")))), 1, _starId, Market.ShipRenewalTime, pricescale));
                    _items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("fas3")))), 1, _starId, Market.ShipRenewalTime, pricescale));
                }

			    for (var i = 0; i < 5 + extraGoods; ++i)
                    _items.Add(_productFactory.CreateRandomComponentProduct(_starId, i, _level, Constructor.ComponentQuality.P1, _faction, false, Market.CommonComponentRenewalTime, false, 2f*pricescale));

				//if (Model.Regulations.Time.IsCristmas)
				//	_items.Add(new MarketProduct(new XmaxBoxItem(random.Next(), _starId), 1, _starId, Market.GiftBoxRenewalTime, 1));

				//_items.AddRange(Plugins.SoomlaStoreFacade.Instance.GetAvailableProducts().Select(item => new MarketProduct(item, 1, 0)));
			}

			void CreateItems()
			{
				var random = _random.CreateRandom(_starId);
                var pricescale = _playerSkills.PriceScale;
                var extraGoods = _playerSkills.HasMasterTrader ? 1 : 0;

                _items = new List<IProduct>();
				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateFuelItem(), 100 + 100*extraGoods, _starId, Market.FuelRenewalTime, 2f*pricescale));

				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateResearchItem(_faction), random.Next(1,5) + extraGoods, _starId, 0, pricescale));
				
				var ship = _database.ShipBuildList.OfFaction(_faction).Playable().Common().OfSize(SizeClass.Frigate, extraGoods > 0 ? SizeClass.Battleship : SizeClass.Cruiser).RandomElement(random);
				_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(ship)), 1, _starId, Market.ShipRenewalTime, 3f*pricescale));
				
				for (var i = 0; i < 5 + extraGoods; ++i)
                    _items.Add(_productFactory.CreateRandomComponentProduct(_starId, i, _level, Constructor.ComponentQuality.P1, _faction, false, Market.CommonComponentRenewalTime, false, 2f*pricescale));

				//if (Model.Regulations.Time.IsCristmas && random.Next(3) == 0)
				//	_items.Add(new MarketProduct(new XmaxBoxItem(random.Next(), _starId), 1, _starId, Market.GiftBoxRenewalTime, 1));
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
