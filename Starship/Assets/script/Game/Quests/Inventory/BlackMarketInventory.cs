using System;
using System.Collections.Generic;
using System.Linq;
using Constructor;
using Constructor.Ships;
using Economy;
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
		public class BlackMarketInventory : IInventory
		{
			public BlackMarketInventory(Galaxy.Star star, ItemTypeFactory itemTypeFactory, ProductFactory productFactory, PlayerSkills playerSkills, IRandom random, IDatabase database)
			{
				_starId = star.Id;
				_level = star.Level;
			    _random = random;
			    _itemTypeFactory = itemTypeFactory;
			    _productFactory = productFactory;
			    _playerSkills = playerSkills;
			    _database = database;
				
				Money = 10000;
			}

			public void Refresh() { _items = null; }

			public IEnumerable<IProduct> Items
			{
				get
				{
					if (_items == null)
					{
						var random = _random.CreateRandom(_starId);
					    var pricescale = _playerSkills.PriceScale;
					    var extraGoods = _playerSkills.HasMasterTrader ? 1 : 0;

						_items = new List<IProduct>();
						_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateFuelItem(), 100 + extraGoods*50, _starId, Market.FuelRenewalTime, 5f*pricescale));

						foreach(var id in _database.FactionList.Visible().Where(faction => faction.WanderingShipsDistance <= _level).RandomUniqueElements(random.Next(2, 5 + extraGoods), random))
							_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateResearchItem(id), random.Next(1,5), _starId, Market.TechRenewalTime, pricescale));

                        if (CurrencyExtensions.PremiumCurrencyAllowed)
    						_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateCurrencyItem(Currency.Stars), random.Next(5, 15 + 5*extraGoods), _starId, Market.StarsRenewalTime, 2f*pricescale));

                        foreach (var ship in _database.ShipBuildList.Playable().NormalShips().Where(item => item.Ship.Faction.WanderingShipsDistance <= _level).RandomUniqueElements(random.Next(extraGoods + 1, 5), random))
							_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateShipItem(new CommonShip(ship), true), 1, _starId, Market.ShipRenewalTime, pricescale));

						var componentCount = random.Next(4, 7);
						for (var i = 0; i < componentCount; ++i)
						{
							var index = i;
							TryCreateProduct(() => _productFactory.CreateRandomComponentProduct(_starId, index, _level + 75,
								ComponentQuality.P3, Faction.Undefined, true, Market.RareComponentRenewalTime, true,
								2f * pricescale));
						}

                        if (extraGoods > 0)
	                        TryCreateProduct(() => _productFactory.CreateRandomComponentProduct(_starId, componentCount, _level + 75, ComponentQuality.P3, Faction.Undefined, true, Market.RareComponentRenewalTime, false, 5f*pricescale));

                        foreach (var item in _database.SatelliteList.Where(item => item.SizeClass != SizeClass.Titan).RandomUniqueElements(random.Next(extraGoods + 3), random))
							_items.Add(_productFactory.CreateRenewableMarketProduct(_itemTypeFactory.CreateSatelliteItem(item, true), 1, _starId, Market.SatelliteRenewalTime, pricescale));

						//if (Model.Regulations.Time.IsCristmas && random.Next(3) == 0)
						//	_items.Add(_productFactory.CreateRenewableMarketProduct(new XmaxBoxItem(random.Next(), _starId), 1, _starId, Market.GiftBoxRenewalTime, 1));
					}
					
					return _items.Where(item => item.Quantity > 0);
				}
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
			
			public int Money { get; private set; }
			
			private readonly int _starId;
			private readonly int _level;
			private readonly IRandom _random;
			private List<IProduct> _items;
		    private readonly ItemTypeFactory _itemTypeFactory;
		    private readonly ProductFactory _productFactory;
		    private readonly PlayerSkills _playerSkills;
		    private readonly IDatabase _database;
		}
	}
}
