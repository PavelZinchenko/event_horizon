using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using Session.Content;
using Helpers = GameModel.Serialization.Helpers;

namespace Constructor
{
	public class IntegratedComponent
	{
		public IntegratedComponent(ComponentInfo component, int x, int y, int barrelId, int keyBinding, int behaviour, bool locked)
		{
			Info = component;
			X = x;
			Y = y;
			BarrelId = barrelId;
		    KeyBinding = keyBinding;
		    Behaviour = behaviour;
		    Locked = locked;
		}

		public override string ToString() { throw new NotSupportedException(); }

		public int KeyBinding { get; set; }
		public bool Locked { get; set; }
        public int Behaviour { get; set; }

		public readonly ComponentInfo Info;
		public readonly int X;
		public readonly int Y;
		public readonly int BarrelId;
	}

    public static class ComponentExtensions
    {
        public static IEnumerable<IntegratedComponent> FromShipComponentsData(this ShipComponentsData data, IDatabase database)
        {
            return FromComponentsData(data.Components, database);
        }

        public static IEnumerable<IntegratedComponent> FromComponentsData(
            this IEnumerable<ShipComponentsData.Component> data, IDatabase database)
        {
            foreach (var item in data)
            {
                var component = database.GetComponent(new ItemId<GameDatabase.DataModel.Component>(item.Id));
                if (component == null)
                {
                    UnityEngine.Debug.LogException(new ArgumentException("Unknown component - " + item.Id));
                    continue;
                }

                var info = new ComponentInfo(component, (ComponentModType)item.Modification, (ModificationQuality)item.Quality, item.UpgradeLevel);
                var x = item.X > -component.Layout.Size ? item.X : 256 + item.X;
                var y = item.Y > -component.Layout.Size ? item.Y : 256 + item.Y;
                yield return new IntegratedComponent(info, x, y, item.BarrelId, item.KeyBinding, item.Behaviour, item.Locked);
            }
        }

        public static ShipComponentsData ToShipComponentsData(this IEnumerable<IntegratedComponent> components)
        {
            var data = new ShipComponentsData
            {
                Components = components.Select<IntegratedComponent, ShipComponentsData.Component>(item => new ShipComponentsData.Component
                {
                    Id = item.Info.Data.Id.Value,
                    Quality = (int)item.Info.ModificationQuality,
                    Modification = (int)item.Info.ModificationType,
                    UpgradeLevel = 0,
                    X = item.X,
                    Y = item.Y,
                    BarrelId = item.BarrelId,
                    KeyBinding = item.KeyBinding,
                    Behaviour = item.Behaviour,
                    Locked = item.Locked,
                })
            };

            return data;
        }

#region Obsolete

        public static IEnumerable<byte> Serialize(this IntegratedComponent component)
        {
            foreach (var value in Helpers.Serialize(component.Info.SerializeToInt64()))
                yield return value;

            yield return (byte)component.X;
            yield return (byte)component.Y;
            yield return (byte)component.BarrelId;
            yield return (byte)component.KeyBinding;
            yield return (byte)component.Behaviour;
            yield return component.Locked ? (byte)1 : (byte)0;
        }

        public static IntegratedComponent Deserialize(IDatabase database, byte[] data/*, ref int index*/)
        {
            var index = 0;
            var component = ComponentInfo.FromInt64(database, Helpers.DeserializeLong(data, ref index));
            var x = (sbyte)data[index++];
            var y = (sbyte)data[index++];
            var barrelId = (sbyte)data[index++];
            var keyBinding = (sbyte)data[index++];
            var mode = (sbyte)data[index++];
            var locked = data[index++] > 0;

            return new IntegratedComponent(component, x, y, barrelId, keyBinding, mode, locked);
        }

        public static IEnumerable<byte> SerializeObsolete(this IntegratedComponent component)
        {
            foreach (var value in Helpers.Serialize(component.Info.SerializeToInt32Obsolete()))
                yield return value;

            yield return (byte)component.X;
            yield return (byte)component.Y;
            yield return (byte)component.BarrelId;
            yield return (byte)component.KeyBinding;
            yield return (byte)component.Behaviour;
            yield return component.Locked ? (byte)1 : (byte)0;
        }

        public static IntegratedComponent DeserializeObsolete(IDatabase database, byte[] data/*, ref int index*/)
        {
            var index = 0;
            var component = ComponentInfo.FromInt32Obsolete(database, Helpers.DeserializeInt(data, ref index));
            var x = (sbyte)data[index++];
            var y = (sbyte)data[index++];
            var barrelId = (sbyte)data[index++];
            var keyBinding = (sbyte)data[index++];
            var mode = (sbyte)data[index++];
            var locked = data[index++] > 0;

            return new IntegratedComponent(component, x, y, barrelId, keyBinding, mode, locked);
        }

        public static string SerializeToStringObsolete(this IntegratedComponent component)
        {
            var builder = new StringBuilder();
            builder.Append(component.Info.ToString());
            builder.Append(_separator);
            builder.Append(component.X);
            builder.Append(_separator);
            builder.Append(component.Y);
            builder.Append(_separator);
            builder.Append(component.BarrelId);
            builder.Append(_separator);
            builder.Append(component.KeyBinding);
            builder.Append(_separator);
            if (component.Locked)
                builder.Append('1');
            builder.Append(_separator);
            builder.Append(component.Behaviour);

            return builder.ToString();
        }

        public static IntegratedComponent DeserializeFromStringObsolete(IDatabase database, string data)
        {
            var parser = new StringParser(data, _separator);

            var info = ComponentInfo.FormString(database, parser.CurrentString);
            var x = parser.MoveNext().CurrentInt;
            var y = parser.MoveNext().CurrentInt;
            var barrelId = parser.MoveNext().CurrentInt;
            var keyBinding = parser.MoveNext().CurrentInt;
            var locked = !string.IsNullOrEmpty(parser.MoveNext().CurrentString);
            var mode = parser.MoveNext().CurrentInt;

            var component = new IntegratedComponent(info, x, y, barrelId, keyBinding, mode, locked);

            return component;
        }

        public static long SerializeToInt64Obsolete(this IntegratedComponent component) // deprecated
        {
            long value = component.Info.SerializeToInt32Obsolete();
            value <<= 8;
            value += (byte)component.X;
            value <<= 8;
            value += (byte)component.Y;
            value <<= 8;
            value += (byte)component.BarrelId;
            value <<= 8;
            value += (byte)component.KeyBinding;
            value <<= 1;
            value += component.Locked ? 1 : 0;
            return value;
        }

        public static IntegratedComponent DeserializeFromInt64Obsolete(IDatabase database, long data)
        {
            var locked = (data & 1) != 0;
            data >>= 1;
            var keyBinding = (sbyte)data;
            data >>= 8;
            var barrelId = (sbyte)data;
            data >>= 8;
            var y = (sbyte)data;
            data >>= 8;
            var x = (sbyte)data;
            data >>= 8;
            var component = ComponentInfo.FromInt32Obsolete(database, (int)data);

            return new IntegratedComponent(component, x, y, barrelId, keyBinding, 0, locked);
        }

#endregion

        public static IntegratedComponent FromDatabase(InstalledComponent serializable)
        {
            var info = new ComponentInfo(serializable.Component, serializable.Modification, serializable.Quality);
            var component = new IntegratedComponent(info, serializable.X, serializable.Y, serializable.BarrelId, serializable.KeyBinding, serializable.Behaviour, serializable.Locked);

            return component;
        }

        private const char _separator = '/';
    }
}
