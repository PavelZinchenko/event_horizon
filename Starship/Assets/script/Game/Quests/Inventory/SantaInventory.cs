using System.Collections.Generic;
using System.Linq;
using Constructor;
using Economy;
using Economy.ItemType;
using Economy.Products;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using GameServices.Random;
using GameServices.Research;

namespace GameModel
{
    namespace Quests
    {
        public class SantaInventory : IInventory
        {
            public SantaInventory(Galaxy.Star star, ItemTypeFactory itemTypeFactory, ProductFactory productFactory, Research research, IRandom random, IDatabase database)
            {
                _starId = star.Id;
                _level = star.Level;
                _random = random;
                _itemTypeFactory = itemTypeFactory;
                _productFactory = productFactory;
                _database = database;
                _research = research;
            }

            public void Refresh() { _items = null; }

            public IEnumerable<IProduct> Items
            {
                get
                {
                    if (_items == null)
                    {
                        var santa = _research.Technologies.Get(new ItemId<Technology>(277));
                        var meteor = _research.Technologies.Get(new ItemId<Technology>(281));
                        var firework = _research.Technologies.Get(new ItemId<Technology>(282));
                        var dronebayS = _research.Technologies.Get(new ItemId<Technology>(278));
                        var dronebayM = _research.Technologies.Get(new ItemId<Technology>(279));
                        var dronebayL = _research.Technologies.Get(new ItemId<Technology>(280));
                        var holyCannon = _research.Technologies.Get(new ItemId<Technology>(264));

                        var random = _random.CreateRandom(_starId);

                        _items = new List<IProduct>();

                        if (!_research.IsTechResearched(santa)) _items.Add(_productFactory.CreateSpecial(_itemTypeFactory.CreateBlueprintItem(santa), new Price(1000, Currency.Snowflakes)));
                        if (!_research.IsTechResearched(meteor)) _items.Add(_productFactory.CreateSpecial(_itemTypeFactory.CreateBlueprintItem(meteor), new Price(200, Currency.Snowflakes)));
                        if (!_research.IsTechResearched(dronebayL)) _items.Add(_productFactory.CreateSpecial(_itemTypeFactory.CreateBlueprintItem(dronebayL), new Price(200, Currency.Snowflakes)));
                        if (!_research.IsTechResearched(dronebayM)) _items.Add(_productFactory.CreateSpecial(_itemTypeFactory.CreateBlueprintItem(dronebayM), new Price(150, Currency.Snowflakes)));
                        if (!_research.IsTechResearched(dronebayS)) _items.Add(_productFactory.CreateSpecial(_itemTypeFactory.CreateBlueprintItem(dronebayS), new Price(100, Currency.Snowflakes)));
                        if (!_research.IsTechResearched(holyCannon)) _items.Add(_productFactory.CreateSpecial(_itemTypeFactory.CreateBlueprintItem(holyCannon), new Price(100, Currency.Snowflakes)));
                        if (!_research.IsTechResearched(firework)) _items.Add(_productFactory.CreateSpecial(_itemTypeFactory.CreateBlueprintItem(firework), new Price(50, Currency.Snowflakes)));

                        var componentCount = random.Range(4, 6);
                        var components = _database.ComponentList.LevelLessOrEqual(_level + 50).CommonAndRare().RandomUniqueElements(componentCount, random);
                        foreach (var item in components)
                        {
                            var component = ComponentInfo.CreateRandomModification(item, random, ModificationQuality.P2);
                            var itemType = _itemTypeFactory.CreateComponentItem(component, true);
                            var price = new Price(CurrencyExtensions.PremiumCurrencyAllowed ? itemType.Price.Amount : 1 + itemType.Price.Amount / 500, Currency.Snowflakes);

                            _items.Add(_productFactory.CreateSpecial(itemType, price));
                        }
                    }

                    return _items.Where(item => item.Quantity > 0);
                }
            }

            public int Money { get; private set; }

            private readonly int _starId;
            private readonly int _level;
            private readonly IRandom _random;
            private List<IProduct> _items;
            private readonly ItemTypeFactory _itemTypeFactory;
            private readonly ProductFactory _productFactory;
            private readonly IDatabase _database;
            private readonly Research _research;
        }
    }
}
