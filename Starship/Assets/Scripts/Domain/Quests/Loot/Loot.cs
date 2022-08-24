using System;
using System.Collections.Generic;
using System.Linq;
using Constructor;
using Constructor.Ships;
using Economy;
using Economy.ItemType;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Extensions;
using Zenject;

namespace Domain.Quests
{
    public interface ILoot
    {
        IEnumerable<LootItem> Items { get; }
        bool CanBeRemoved { get; }
    }

    public static class LootExtensions
    {
        public static void Consume(this ILoot loot)
        {
            foreach (var item in loot.Items)
                item.Type.Consume(item.Quantity);
        }

        public static void Remove(this ILoot loot)
        {
            foreach (var item in loot.Items)
                item.Type.Withdraw(item.Quantity);
        }
    }

    public class EmptyLoot : ILoot
    {
        public IEnumerable<LootItem> Items { get { return Enumerable.Empty<LootItem>(); } }
        public bool CanBeRemoved { get { return true; } }
        public static readonly EmptyLoot Instance = new EmptyLoot();
    }

    public struct LootItem
    {
        public LootItem(IItemType type, int quantity = 1)
        {
            Type = type;
            Quantity = quantity;
        }

        public readonly IItemType Type;
        public readonly int Quantity;
    }

    public class Loot : ILoot, ILootContentFactory<IEnumerable<LootItem>>
    {
        public Loot(LootModel loot, QuestContext context, ItemTypeFactory itemTypeFactory, /*StarData starData,*/ IDatabase database)
        {
            _itemTypeFactory = itemTypeFactory;
            _database = database;
            _loot = loot;
            _context = context;
            _random = new Random(context.Seed);
        }

        public bool CanBeRemoved
        {
            get
            {
                if (_items == null)
                    Initialize();

                foreach (var item in _items)
                    if (item.Type.MaxItemsToWithdraw < item.Quantity)
                        return false;

                return true;
            }
        }

        public IEnumerable<LootItem> Items
        {
            get
            {
                if (_items == null)
                    Initialize();

                return _items;
            }
        }

        private void Initialize()
        {
            _items = new List<LootItem>();
            foreach (var item in _loot.Loot.Create(this))
                _items.Add(item);
        }

        #region ILootItemFactory

        public IEnumerable<LootItem> Create(LootContent_None content)
        {
            return Enumerable.Empty<LootItem>();
        }

        public IEnumerable<LootItem> Create(LootContent_SomeMoney content)
        {
            var amount = (int)(Maths.Distance.Credits((int)(_context.Level * content.ValueRatio)));
            amount = 9 * amount / 10 + _random.Next(1 + amount / 5);
            yield return new LootItem(_itemTypeFactory.CreateCurrencyItem(Currency.Credits), amount);
        }

        public IEnumerable<LootItem> Create(LootContent_Fuel content)
        {
            var amount = _random.Range(content.MinAmount, content.MaxAmount);
            yield return new LootItem(_itemTypeFactory.CreateFuelItem(), amount);
        }

        public IEnumerable<LootItem> Create(LootContent_Money content)
        {
            var amount = _random.Range(content.MinAmount, content.MaxAmount);
            yield return new LootItem(_itemTypeFactory.CreateCurrencyItem(Currency.Credits), amount);
        }

        public IEnumerable<LootItem> Create(LootContent_Stars content)
        {
            var amount = _random.Range(content.MinAmount, content.MaxAmount);
            yield return new LootItem(Price.Premium(amount).GetProduct(_itemTypeFactory).Type, amount);
        }

        public IEnumerable<LootItem> Create(LootContent_StarMap content)
        {
            yield return new LootItem(_itemTypeFactory.CreateStarMapItem(_context.StarId));
        }

        public IEnumerable<LootItem> Create(LootContent_RandomComponents content)
        {
            var level = Math.Max(0, (int)(_context.Level * content.ValueRatio));
            var amount = _random.Range(content.MinAmount, content.MaxAmount);
            var factionFilter = new FactionFilter(content.Factions, level, _context.Faction);
            var all = _database.ComponentList.Common().LevelLessOrEqual(level * 3 / 2).Where(item => factionFilter.IsSuitable(item.Faction));
            var components = ComponentInfo.CreateRandom(all, amount, level, _random);
            return components.Select(item => new LootItem(_itemTypeFactory.CreateComponentItem(item)));
        }

        public IEnumerable<LootItem> Create(LootContent_RandomItems content)
        {
            var amount = _random.Range(content.MinAmount, content.MaxAmount);
            var itemCount = content.Items.Count;

            if (itemCount == 0)
                yield break;

            var totalWeight = 0f;
            for (var i = 0; i < itemCount; ++i)
                totalWeight += content.Items[i].Weight;

            if (totalWeight < 0.0001f)
                foreach (var item in content.Items.RandomUniqueElements(amount, itemCount, _random).SelectMany(item => item.Loot.Create(this)))
                    yield return item;

            var itemsLeft = amount;
            foreach (var item in content.Items)
            {
                if (itemsLeft <= 0)
                    yield break;

                if (_random.NextFloat() * totalWeight >= item.Weight * itemsLeft)
                    continue;

                itemsLeft--;
                totalWeight = totalWeight > item.Weight ? totalWeight - item.Weight : 0f;

                foreach (var lootItem in item.Loot.Create(this))
                    yield return lootItem;
            }
        }

        public IEnumerable<LootItem> Create(LootContent_AllItems content)
        {
            return content.Items.SelectMany(item => item.Loot.Create(this));
        }

        public IEnumerable<LootItem> Create(LootContent_ItemsWithChance content)
        {
            foreach (var item in content.Items)
            {
                if (_random.NextFloat() > item.Weight)
                    continue;

                foreach (var lootItem in item.Loot.Create(this))
                    yield return lootItem;
            }
        }

        public IEnumerable<LootItem> Create(LootContent_QuestItem content)
        {
            var amount = _random.Range(content.MinAmount, content.MaxAmount);
            yield return new LootItem(_itemTypeFactory.CreateArtifactItem(content.QuestItem), amount);
        }

        public IEnumerable<LootItem> Create(LootContent_Ship content)
        {
            yield return new LootItem(_itemTypeFactory.CreateShipItem(new CommonShip(content.ShipBuild)));
        }

        public IEnumerable<LootItem> Create(LootContent_EmptyShip content)
        {
            yield return new LootItem(_itemTypeFactory.CreateShipItem(new CommonShip(content.Ship, Enumerable.Empty<IntegratedComponent>())));
        }

        public IEnumerable<LootItem> Create(LootContent_Component content)
        {
            var amount = _random.Range(content.MinAmount, content.MaxAmount);
            yield return new LootItem(_itemTypeFactory.CreateComponentItem(new ComponentInfo(content.Component)), amount);
        }

        #endregion

        private List<LootItem> _items;
        private readonly Random _random;
        private readonly QuestContext _context;
        private readonly LootModel _loot;
        private readonly IDatabase _database;
        private readonly ItemTypeFactory _itemTypeFactory;

        public class Factory : Factory<LootModel, QuestContext, Loot> { }
    }
}
