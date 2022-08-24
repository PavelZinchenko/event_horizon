using System;
using System.Collections.Generic;
using System.Linq;
using Constructor;
using Economy.Products;
using Game;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using GameServices.Gui;
using Services.Localization;
using Services.Reources;
using Zenject;

namespace Economy.ItemType
{
    public class XmaxBoxItem : IItemType
    {
        [Inject]
        public XmaxBoxItem(HolidayManager holidayManager, IDatabase database, ItemTypeFactory itemTypeFactory, ILocalization localization, GuiHelper guiHelper, int seed)
        {
            _seed = seed;
            _database = database;
            _holidayManager = holidayManager;
            _itemTypeFactory = itemTypeFactory;
            _guiHelper = guiHelper;
            _localization = localization;
        }

        public string Id { get { return "xmasbox"; } }
        public string Name { get { return _localization.GetString("$XmasBox"); } }
        public string Description { get { return string.Empty; } }
        public UnityEngine.Sprite GetIcon(IResourceLocator resourceLocator) { return resourceLocator.GetSprite("Textures/GUI/InAppPurchases/xmasbox"); }
        public UnityEngine.Color Color { get { return UnityEngine.Color.white; } }
        public Price Price { get { return Price.Premium(3); } }
        public ItemQuality Quality { get { return ItemQuality.Perfect; } }

        public bool IsEqual(IItemType other) { return false; }

        public void Consume(int amount)
        {
            var loot = Loot(new System.Random(_seed)).ToArray();
            _guiHelper.ShowLootWindow(loot);

            foreach (var item in loot)
                item.Consume();
        }

        public void Withdraw(int amount)
        {
            throw new InvalidOperationException();
        }

        public int MaxItemsToConsume { get { return 1; } }

        public int MaxItemsToWithdraw { get { return 0; } }

        private IEnumerable<IProduct> Loot(System.Random random)
        {
            const int holyCannonId = 213;
            const int santaSmallDronebayId = 219;
            const int santaMediumDronebayId = 220;
            const int santaLargeDronebayId = 215;
            const int fireworkId = 96;

            if (!_holidayManager.IsChristmas)
                yield break;

            var items = new List<Component>();

            var value = random.Next(100);
            if (value < 5)
                items.Add(_database.GetComponent(new ItemId<Component>(santaLargeDronebayId)));
            if (value < 15)
                items.Add(_database.GetComponent(new ItemId<Component>(santaMediumDronebayId)));
            if (value < 30)
                items.Add(_database.GetComponent(new ItemId<Component>(holyCannonId)));
            else if (value < 45)
                items.Add(_database.GetComponent(new ItemId<Component>(santaSmallDronebayId)));
            else if (value < 60)
                items.Add(_database.GetComponent(new ItemId<Component>(fireworkId)));

            items.AddRange(_database.ComponentList.CommonAndRare().RandomUniqueElements(value / 25, random));

            foreach (var item in items)
                yield return new Product(_itemTypeFactory.CreateComponentItem(ComponentInfo.CreateRandomModification(item, random, ModificationQuality.P2)));

            yield return new Product(_itemTypeFactory.CreateCurrencyItem(Currency.Snowflakes), random.Range(15, 25));
        }

        private readonly int _seed;
        private readonly GuiHelper _guiHelper;
        private readonly HolidayManager _holidayManager;
        private readonly ItemTypeFactory _itemTypeFactory;
        private readonly ILocalization _localization;
        private readonly IDatabase _database;
    }
}
