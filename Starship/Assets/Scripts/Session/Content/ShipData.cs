using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;

namespace Session.Content
{
    public struct ShipData
    {
        public int Id;
        public string Name;
        public long ColorScheme;
        public ObscuredLong Experience;
        public ShipComponentsData Components;
        public ShipModificationsData Modifications;
        public SatelliteData Satellite1;
        public SatelliteData Satellite2;

        public const byte Version = 1;

        public IEnumerable<byte> Serialize()
        {
            yield return Version;
            foreach (var value in Helpers.Serialize(Id))
                yield return value;
            foreach (var value in Helpers.Serialize(Name))
                yield return value;
            foreach (var value in Helpers.Serialize(ColorScheme))
                yield return value;
            foreach (var value in Helpers.Serialize(Experience))
                yield return value;
            foreach (var value in Components.Serialize())
                yield return value;
            foreach (var value in Modifications.Serialize())
                yield return value;
            foreach (var value in Satellite1.Serialize())
                yield return value;
            foreach (var value in Satellite2.Serialize())
                yield return value;
        }

        public static ShipData Deserialize(byte[] buffer, ref int index)
        {
            var version = buffer[index++];
            if (version != Version)
                return Deserialize(buffer, ref index, version);

            var info = new ShipData();
            info.Id = Helpers.DeserializeInt(buffer, ref index);
            info.Name = Helpers.DeserializeString(buffer, ref index);
            info.ColorScheme = Helpers.DeserializeLong(buffer, ref index);
            info.Experience = Helpers.DeserializeLong(buffer, ref index);
            info.Components = ShipComponentsData.Deserialize(buffer, ref index);
            info.Modifications = ShipModificationsData.Deserialize(buffer, ref index);
            info.Satellite1 = SatelliteData.Deserialize(buffer, ref index);
            info.Satellite2 = SatelliteData.Deserialize(buffer, ref index);
            return info;
        }

        public static ShipData Deserialize(byte[] buffer, ref int index, byte version)
        {
            throw new ArgumentException("ShipData: Unknown version - " + version);
        }
    }

    public struct ShipComponentsData
    {
        public IEnumerable<Component> Components
        {
            get { return _components ?? Enumerable.Empty<Component>(); }
            set { _components = value != null ? value.ToArray() : null; }
        }

        public IEnumerable<byte> Serialize()
        {
            yield return Version;
            if (_components == null)
            {
                foreach (var value in Helpers.Serialize(0))
                    yield return value;
                yield break;
            }

            foreach (var value in Helpers.Serialize(_components.Length))
                yield return value;
            foreach (var item in _components)
            {
                foreach (var value in Helpers.Serialize(item.Id))
                    yield return value;
                yield return (byte)item.Quality;
                yield return (byte)item.Modification;
                yield return (byte)item.UpgradeLevel;
                foreach (var value in Helpers.Serialize((short)item.X))
                    yield return value;
                foreach (var value in Helpers.Serialize((short)item.Y))
                    yield return value;
                yield return (byte)item.BarrelId;
                yield return (byte)item.KeyBinding;
                yield return (byte)item.Behaviour;
                yield return item.Locked ? (byte)1 : (byte)0;
            }
        }

        public static ShipComponentsData Deserialize(byte[] buffer, ref int index)
        {
            var version = buffer[index++];
            if (version != Version)
            {
                return Deserialize(buffer, ref index, version);
            }

            var info = new ShipComponentsData();
            var count = Helpers.DeserializeInt(buffer, ref index);
            if (count > 0)
            {
                info._components = new Component[count];
                for (var i = 0; i < count; ++i)
                {
                    info._components[i] = new Component {
                        Id = Helpers.DeserializeInt(buffer, ref index),
                        Quality = (sbyte)buffer[index++],
                        Modification = (sbyte)buffer[index++],
                        UpgradeLevel = (sbyte)buffer[index++],
                        X = Helpers.DeserializeShort(buffer, ref index),
                        Y = Helpers.DeserializeShort(buffer, ref index),
                        BarrelId = (sbyte)buffer[index++],
                        KeyBinding = (sbyte)buffer[index++],
                        Behaviour = (sbyte)buffer[index++],
                        Locked = buffer[index++] == 1
                    };
                }
            }

            return info;
        }

        public static ShipComponentsData Deserialize(byte[] buffer, ref int index, byte version)
        {
            if (version == 1)
            {
                var info = new ShipComponentsData();
                var count = Helpers.DeserializeInt(buffer, ref index);
                if (count > 0)
                {
                    info._components = new Component[count];
                    for (var i = 0; i < count; ++i)
                    {
                        info._components[i] = new Component
                        {
                            Id = Helpers.DeserializeInt(buffer, ref index),
                            Quality = (sbyte)buffer[index++],
                            Modification = (sbyte)buffer[index++],
                            UpgradeLevel = (sbyte)buffer[index++],
                            X = (sbyte)buffer[index++],
                            Y = (sbyte)buffer[index++],
                            BarrelId = (sbyte)buffer[index++],
                            KeyBinding = (sbyte)buffer[index++],
                            Behaviour = (sbyte)buffer[index++],
                            Locked = buffer[index++] == 1
                        };
                    }
                }
                return info;
            }

            throw new ArgumentException("ShipComponentsData: Unknown version - " + version);
        }

        public struct Component
        {
            public int Id;
            public int Quality;
            public int Modification;
            public int UpgradeLevel;
            public int X;
            public int Y;
            public int BarrelId;
            public int KeyBinding;
            public int Behaviour;
            public bool Locked;
        }

        private Component[] _components;

        public const byte Version = 2;
    }

    public struct ShipModificationsData
    {
        public IEnumerable<byte> Layout { get { return _layout ?? Enumerable.Empty<byte>(); } set { _layout = value == null ? null : value.ToArray(); } }

        public IEnumerable<long> Modifications
        {
            get { return _modifications ?? Enumerable.Empty<long>(); }
            set { _modifications = value != null ? value.ToArray() : null; }
        }

        public IEnumerable<byte> Serialize()
        {
            yield return Version;
            foreach (var value in Helpers.Serialize(_layout))
                yield return value;
            foreach (var value in Helpers.Serialize(_modifications))
                yield return value;
        }

        public static ShipModificationsData Deserialize(byte[] buffer, ref int index)
        {
            var version = buffer[index++];
            if (version != Version)
                return Deserialize(buffer, ref index, version);

            var data = new ShipModificationsData();
            data._layout = Helpers.DeserializeByteArray(buffer, ref index, true);
            data._modifications = Helpers.DeserializeLongArray(buffer, ref index, true);
            return data;
        }

        public static ShipModificationsData Deserialize(byte[] buffer, ref int index, byte version)
        {
            throw new ArgumentException("ShipModificationsData: Unknown version - " + version);
        }

        private byte[] _layout;
        private long[] _modifications;

        public const byte Version = 1;
    }

    public struct SatelliteData
    {
        public int Id;
        public ShipComponentsData Components;

        public const byte Version = 1;

        public IEnumerable<byte> Serialize()
        {
            yield return Version;
            foreach (var value in Helpers.Serialize(Id))
                yield return value;
            foreach (var value in Components.Serialize())
                yield return value;
        }

        public static SatelliteData Deserialize(byte[] buffer, ref int index)
        {
            var version = buffer[index++];
            if (version != Version)
                return Deserialize(buffer, ref index, version);
            
            var info = new SatelliteData();
            info.Id = Helpers.DeserializeInt(buffer, ref index);
            info.Components = ShipComponentsData.Deserialize(buffer, ref index);
            return info;
        }

        public static SatelliteData Deserialize(byte[] buffer, ref int index, byte version)
        {
            throw new ArgumentException("SatelliteData: Unknown version - " + version);
        }
    }
}
