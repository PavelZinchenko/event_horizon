using System.Collections.Generic;
using System.Linq;
using Constructor;
using Economy.Products;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameServices.Economy;
using GameServices.Player;
using GameServices.Random;

namespace GameModel
{
    namespace Quests
	{
		public class MerchantInventory : IInventory
		{
			public MerchantInventory(int starId, int level, ProductFactory productFactory, LootGenerator lootGenerator, PlayerSkills playerSkills, IRandom random)
			{
				_starId = starId;
				_level = level;
			    _lootGenerator = lootGenerator;
			    _random = random;
			    _productFactory = productFactory;
			    _playerSkills = playerSkills;
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

						_items.Add(_productFactory.CreateMarketProduct(_lootGenerator.Factory.CreateFuelItem(), random.Next(10,100) + extraGoods*30, 2f*pricescale));

                        _items.AddRange(_lootGenerator.GetRandomComponents(_level - 25, random.Next(2, 6) + extraGoods, Faction.Undefined, _random.Seed + _starId, false, extraGoods > 0 ? Constructor.ComponentQuality.P1 : ComponentQuality.P0)
							.Select(item => _productFactory.CreateMarketProduct(item, 1, 2f*pricescale)));

						var factionMap = _lootGenerator.Factory.TryCreateFactionMapItem(_starId);
					    _items.Add(factionMap != null
					        ? _productFactory.CreateMarketProduct(factionMap, 1, 2f*pricescale)
					        : _productFactory.CreateMarketProduct(_lootGenerator.Factory.CreateStarMapItem(_starId), 1, 2f*pricescale));

					    //if (Model.Regulations.Time.IsCristmas && random.Next(3) == 0)
						//	_items.Add(_productFactory.CreateMarketProduct(new XmaxBoxItem(random.Next()), 1, 1));
					}
					
					return _items.Where(item => item.Quantity > 0);
				}
			}

            //private Faction _faction;
            private List<IProduct> _items;
            private readonly int _starId;
			private readonly int _level;
		    private readonly LootGenerator _lootGenerator;
		    private readonly IRandom _random;
		    private readonly ProductFactory _productFactory;
		    private readonly PlayerSkills _playerSkills;
		}
	}
}
