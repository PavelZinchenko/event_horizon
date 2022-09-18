using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Constructor.Modification;
using Database.Legacy;
using Economy.ItemType;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using Services.Localization;
using UnityEngine;
using IComponent = Constructor.Component.IComponent;

namespace Constructor
{
    public struct ComponentInfo : IEquatable<ComponentInfo>
    {
        public bool Equals(ComponentInfo other)
        {
            return _modificationType == other._modificationType && _quality == other._quality && Data.Id == other.Data.Id && _level == other._level;
        }

        public static bool operator ==(ComponentInfo c1, ComponentInfo c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(ComponentInfo c1, ComponentInfo c2)
        {
            return !c1.Equals(c2);
        }

        public static implicit operator bool(ComponentInfo info)
        {
            return info._data != null;
        }

        public ComponentInfo(GameDatabase.DataModel.Component data)
        {
            _data = data;
            _modificationType = ComponentModType.Empty;
            _quality = ModificationQuality.N3;
            _level = 0;
        }

        public static ComponentInfo CreateRandom(IDatabase database, int level, Faction faction, System.Random random, bool allowRare, ComponentQuality maxQuality = ComponentQuality.P3)
        {
            var maxLevel = 3*level/2;
            var components = allowRare ? database.ComponentList.CommonAndRare() : database.ComponentList.Common();
            var component = components.OfFaction(faction).LevelLessOrEqual(maxLevel).RandomElement(random);

            if (component == null) throw new ValueNotFoundException("There was no components matching given filters");
            var componentLevel = Mathf.Max(10, component.Level);
            var requiredLevel = Mathf.Max(10, level);
            var componentQuality = ComponentQualityExtensions.FromLevel(requiredLevel, componentLevel).Randomize(random);
            if (componentQuality > maxQuality)
                componentQuality = maxQuality;

            if (componentQuality == ComponentQuality.P0)
                return new ComponentInfo(component);

            var modification = component.Create(100).SuitableModifications.RandomElement(random);
            return new ComponentInfo(component, modification, componentQuality.ToModificationQuality());
        }

        public static IEnumerable<ComponentInfo> CreateRandom(IEnumerable<GameDatabase.DataModel.Component> components, int count, int level, System.Random random, ComponentQuality maxQuality = ComponentQuality.P3)
        {
            foreach (var component in components.RandomElements(count, random))
            {
                var componentLevel = Mathf.Max(10, component.Level);
                var requiredLevel = Mathf.Max(10, level);
                var componentQuality = ComponentQualityExtensions.FromLevel(requiredLevel, componentLevel).Randomize(random);
                if (componentQuality > maxQuality)
                    componentQuality = maxQuality;

                if (componentQuality == ComponentQuality.P0)
                {
                    yield return new ComponentInfo(component);
                }
                else
                {
                    var modification = component.Create(100).SuitableModifications.RandomElement(random);
                    yield return new ComponentInfo(component, modification, componentQuality.ToModificationQuality());
                }
            }
        }

        public bool IsValidModification { get { return _modificationType == ComponentModType.Empty || _data.Create(100).SuitableModifications.Contains(_modificationType); } }

        public static ComponentInfo CreateRandomModification(GameDatabase.DataModel.Component data, System.Random random, ModificationQuality minQuality = ModificationQuality.N3, ModificationQuality maxQuality = ModificationQuality.P3)
        {
            if (minQuality > maxQuality)
                Generic.Swap(ref minQuality, ref maxQuality);

            var quality = (ModificationQuality)random.SquareRange((int)minQuality, (int)maxQuality);
            var modification = data.Create(100).SuitableModifications.RandomElement(random);

            return new ComponentInfo(data, modification, quality);
        }

        public string GetName(ILocalization localization)
        {
            return _level <= 0 ? localization.GetString(Data.Name) : localization.GetString(Data.Name) + " +" + _level;
        }

        public static ComponentInfo FormString(IDatabase _database, string data)
        {
            var parser = new StringParser(data, _separator);

            if (parser.IsLast)
                return new ComponentInfo(_database.GetComponent(LegacyComponentNames.GetId(data)));

            var modification = (ComponentModType)parser.CurrentInt;
            var quality = (ModificationQuality)parser.MoveNext().CurrentInt;
            var component = _database.GetComponent(LegacyComponentNames.GetId(parser.MoveNext().CurrentString));

            return new ComponentInfo(component, modification, quality);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ComponentInfo info))
                return false;
            return Equals(info);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _data?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (int)_modificationType;
                hashCode = (hashCode * 397) ^ (int)_quality;
                hashCode = (hashCode * 397) ^ _level;
                return hashCode;
            }
        }

        
        public int SerializeToInt32Obsolete() // 30 bits used
        {
            #if UNITY_EDITOR
            if (Data.Id.Value < 0 || Data.Id.Value > 0x3fff)
            {
                Debug.LogError("Bad component id - " + Data.Id.Value);
                UnityEngine.Debug.Break();
            }
            if ((int)_modificationType < 0 || (int)_modificationType > 0xff)
            {
                Debug.LogError("Bad modification type " + _modificationType);
                UnityEngine.Debug.Break();
            }
            #endif

            int value = Data.Id.Value & 0x3fff;
            value <<= 8;
            value += (byte)_quality;
            value <<= 8;
            value += (byte)_modificationType;

            return value;
        }

        public long SerializeToInt64()
        {
#if UNITY_EDITOR
            if ((int)_modificationType < 0 || (int)_modificationType > 0xff)
                Debug.Break();
            if ((int)_quality < 0 || (int)_quality > 0xff)
                Debug.Break();
            if ((int)_level < 0 || (int)_level > 0xff)
                Debug.Break();
#endif

            long value = Data.Id.Value;
            value <<= 8;
            value += (byte)_quality;
            value <<= 8;
            value += (byte)_modificationType;
            value <<= 8;
            value += (byte)_level;
            value <<= 8;
            // reserved

            return value;
        }

        public static ComponentInfo FromInt32Obsolete(IDatabase database, int data)
        {
            var modification = (ComponentModType)(byte)(data);
            data >>= 8;
            var quality = (ModificationQuality)(byte)data;
            data >>= 8;
            var component = database.GetComponent(new ItemId<GameDatabase.DataModel.Component>(data));

            return new ComponentInfo(component, modification, quality);
        }

        public static ComponentInfo FromInt64(IDatabase database, long data)
        {
            data >>= 8;
            var level = (byte)data;
            data >>= 8;
            var modification = (ComponentModType)(byte)data;
            data >>= 8;
            var quality = (ModificationQuality)(byte)data;
            data >>= 8;
            var component = database.GetComponent(new ItemId<GameDatabase.DataModel.Component>((int)data));

            return new ComponentInfo(component, modification, quality, level);
        }

        public override string ToString()
        {
            if (_modificationType == ComponentModType.Empty)
                return LegacyComponentNames.GetName(Data.Id);

            var builder = new StringBuilder();
            builder.Append((int)_modificationType);
            builder.Append(_separator);
            builder.Append((int)_quality);
            builder.Append(_separator);
            builder.Append(Data.Id);

            return builder.ToString();
        }

        public IComponent CreateComponent(int shipSize)
        {
            var component = _data.Create(shipSize);
            component.Modification = _modificationType.Create(_quality);
            return component;
        }

        public ComponentModType ModificationType { get { return _modificationType; } }

        public IModification CreateModification()
        {
            return _modificationType.Create(_quality) ?? EmptyModification.Instance;
        }

        public ModificationQuality ModificationQuality { get { return _quality; } }

        public Economy.ItemType.ItemQuality ItemQuality
        {
            get
            {
                if (_modificationType == ComponentModType.Empty)
                    return ItemQuality.Common;

                switch (_quality)
                {
                    case ModificationQuality.N1:
                    case ModificationQuality.N2:
                    case ModificationQuality.N3:
                        return ItemQuality.Low;
                    case ModificationQuality.P1:
                        return ItemQuality.Medium;
                    case ModificationQuality.P2:
                        return ItemQuality.High;
                    case ModificationQuality.P3:
                        return ItemQuality.Perfect;
                    default:
                        throw new InvalidEnumArgumentException("_quality", (int)_quality, typeof(ModificationQuality));
                }
            }
        }

        public GameDatabase.DataModel.Component Data { get { return _data ?? GameDatabase.DataModel.Component.Empty; } }

        public Economy.Price Price
        {
            get
            {
                if (_modificationType == ComponentModType.Empty)
                    return Economy.Price.ComponentPrice(Data);

                return  Economy.Price.ComponentPrice(Data, _quality.PowerMultiplier(0.5f, 0.75f, 0.9f, 1.5f, 2.0f, 3f));
            }
        }

        public Economy.Price PremiumPrice
        {
            get
            {
                if (_modificationType == ComponentModType.Empty)
                    return Economy.Price.ComponentPremiumPrice(Data);

                return Economy.Price.ComponentPremiumPrice(Data, _quality.PowerMultiplier(0.5f, 0.75f, 0.9f, 1.5f, 2.0f, 3f));
            }
        }

        public ComponentInfo(GameDatabase.DataModel.Component data, ComponentModType modification, ModificationQuality quality, int level = 0)
        {
            _data = data;
            _modificationType = modification;
            _quality = quality;
            _level = level < MaxUpgradeLevel ? level : MaxUpgradeLevel;
        }

        private static ModificationQuality FromItemQuality(ItemQuality itemQuality, System.Random random)
        {
            switch (itemQuality)
            {
                case ItemQuality.Perfect:
                    return ModificationQuality.P3;
                case ItemQuality.High:
                    return ModificationQuality.P2;
                case ItemQuality.Medium:
                    return ModificationQuality.P1;
                case ItemQuality.Low:
                    return (ModificationQuality)random.Range((int)ModificationQuality.N3, (int)ModificationQuality.N1);
                default:
                    throw new InvalidEnumArgumentException("itemQuality", (int)itemQuality, typeof(ItemQuality));
            }
        }

        private readonly GameDatabase.DataModel.Component _data;
        private readonly ComponentModType _modificationType;
        private readonly ModificationQuality _quality;
        private readonly int _level;

        private const char _separator = '.';
        public const int MaxUpgradeLevel = 20;
        public static ComponentInfo Empty = new ComponentInfo();
    }
}
