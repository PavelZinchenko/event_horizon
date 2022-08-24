using System;
using System.Collections.Generic;
using Combat.Domain;
using GameDatabase.DataModel;
using GameServices.Player;
using Services.InternetTime;
using Session;
using UnityEngine.Assertions;

namespace Domain.Quests
{
    public interface IRequirementCache
    {
        INodeRequirements Get(Requirement requirement, QuestContext context);
    }

    public interface IEnemyCache
    {
        ICombatModelBuilder Get(Fleet enemy, QuestContext context);
    }

    public interface ILootCache
    {
        ILoot Get(LootModel model, QuestContext context);
    }

    public struct Key<T> : IEquatable<Key<T>>
        where T : class
    {
        public Key(QuestContext context, T item)
        {
            Assert.IsNotNull(item);

            StarId = context.StarId;
            Seed = context.Seed;
            Item = item;
        }

        public bool Equals(Key<T> other)
        {
            return StarId == other.StarId && Seed == other.Seed && ReferenceEquals(Item, other.Item);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Key<T> && Equals((Key<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StarId;
                hashCode = (hashCode * 397) ^ Seed;
                hashCode = (hashCode * 397) ^ Item.GetHashCode();
                return hashCode;
            }
        }

        public readonly int StarId;
        public readonly int Seed;
        public readonly T Item;
    }

    public class RequirementCache : IRequirementCache
    {
        public RequirementCache(MotherShip motherShip, ISessionData session, ILootCache lootCache, GameTime gameTime)
        {
            _motherShip = motherShip;
            _lootCache = lootCache;
            _session = session;
            _gameTime = gameTime;
        }

        public INodeRequirements Get(Requirement requirement, QuestContext context)
        {
            if (requirement == null) return null;

            var key = new Key<Requirement>(context, requirement);

            INodeRequirements value;
            if (!_cache.TryGetValue(key, out value))
            {
                var builder = new RequirementsBuilder(context, _lootCache, _motherShip, _session, _gameTime);
                value = builder.Build(requirement);
                _cache.Add(key, value);
            }

            return value;
        }

        private readonly Dictionary<Key<Requirement>, INodeRequirements> _cache = new Dictionary<Key<Requirement>, INodeRequirements>();
        private readonly MotherShip _motherShip;
        private readonly ILootCache _lootCache;
        private readonly ISessionData _session;
        private readonly GameTime _gameTime;
    }

    public class EnemyCache : IEnemyCache
    {
        public EnemyCache(FleetFactory factory)
        {
            _factory = factory;
        }

        public ICombatModelBuilder Get(Fleet enemy, QuestContext context)
        {
            if (enemy == null) return null;

            var key = new Key<Fleet>(context, enemy);

            ICombatModelBuilder value;
            if (!_cache.TryGetValue(key, out value))
            {
                value = _factory.CreateCombatModelBuilder(enemy, context);
                _cache.Add(key, value);
            }

            return value;
        }

        private readonly Dictionary<Key<Fleet>, ICombatModelBuilder> _cache = new Dictionary<Key<Fleet>, ICombatModelBuilder>();
        private readonly FleetFactory _factory;
    }

    public class LootCache : ILootCache
    {
        public LootCache(Loot.Factory factory)
        {
            _factory = factory;
        }

        public ILoot Get(LootModel loot, QuestContext context)
        {
            if (loot == null) return null;

            var key = new Key<LootModel>(context, loot);

            ILoot value;
            if (!_cache.TryGetValue(key, out value))
            {
                value = _factory.Create(loot, context);
                _cache.Add(key, value);
            }

            return value;
        }

        private readonly Dictionary<Key<LootModel>, ILoot> _cache = new Dictionary<Key<LootModel>, ILoot>();
        private readonly Loot.Factory _factory;
    }
}
