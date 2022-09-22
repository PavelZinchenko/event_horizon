using System;
using System.Collections.Generic;
using System.Linq;
using Database.Legacy;
using Utils;
using Zenject;
using Helpers = GameModel.Serialization.Helpers;

namespace Session.Content
{
    public class InventoryData : ISerializableData
    {
        [Inject]
        public InventoryData(byte[] buffer = null)
        {
            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "inventory2";

        public bool IsChanged { get { return _isChanged || _components.IsDirty || _satellites.IsDirty; } set { _isChanged = value; } }
        public static int CurrentVersion { get { return 3; } }

        public IGameItemCollection<long> Components { get { return _components; } }
        public IGameItemCollection<int> Satellites { get { return _satellites; } }

        public IEnumerable<byte> Serialize()
        {
            _isChanged = false;
            _components.IsDirty = false;
            _satellites.IsDirty = false;

            foreach (var value in Helpers.Serialize(CurrentVersion))
                yield return value;

            foreach (var value in Helpers.Serialize(_components.Count))
                yield return value;
            foreach (var item in _components.Items)
            {
                foreach (var value in Helpers.Serialize(item.Key))
                    yield return value;
                foreach (var value in Helpers.Serialize(item.Value))
                    yield return value;
            }

            foreach (var value in Helpers.Serialize(_satellites.Count))
                yield return value;
            foreach (var item in _satellites.Items)
            {
                foreach (var value in Helpers.Serialize(item.Key))
                    yield return value;
                foreach (var value in Helpers.Serialize(item.Value))
                    yield return value;
            }

            foreach (var value in Helpers.Serialize(0))
                yield return value;
            foreach (var value in Helpers.Serialize(0))
                yield return value;
        }

        private void Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

            int index = 0;
            var version = Helpers.DeserializeInt(buffer, ref index);

            if (version != CurrentVersion && !TryUpgrade(ref buffer, version))
            {
                OptimizedDebug.Log("InventoryData: incorrect data version");
                throw new ArgumentException();
            }

            var count = Helpers.DeserializeInt(buffer, ref index);
            for (int i = 0; i < count; ++i)
            {
                var key = Helpers.DeserializeLong(buffer, ref index);
                var value = Helpers.DeserializeInt(buffer, ref index);
                if (value >= 100000)
                    continue;

                _components.Add(key, value);
            }

            count = Helpers.DeserializeInt(buffer, ref index);
            for (int i = 0; i < count; ++i)
            {
                var key = Helpers.DeserializeInt(buffer, ref index);
                var value = Helpers.DeserializeInt(buffer, ref index);
                if (value >= 100000)
                    continue;

                _satellites.Add(key, value);
            }

            Helpers.DeserializeInt(buffer, ref index); // reserved
            Helpers.DeserializeInt(buffer, ref index);

            _isChanged = false;
            _components.IsDirty = false;
            _satellites.IsDirty = false;
        }

        private static bool TryUpgrade(ref byte[] data, int version)
        {
            if (version == 1)
            {
                data = Upgrade_1_2(data).ToArray();
                version = 2;
            }

            if (version == 2)
            {
                data = Upgrade_2_3(data).ToArray();
                version = 3;
            }

            return version == CurrentVersion;
        }

        private static IEnumerable<byte> Upgrade_1_2(byte[] buffer)
        {
            OptimizedDebug.Log("InventoryData.Upgrade_1_2");

            var index = 0;
            Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(2)) // version
                yield return value;

            var count = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(count))
                yield return value;
            for (int i = 0; i < count; ++i)
            {
                var key = Helpers.DeserializeInt(buffer, ref index);
                foreach (var value in Helpers.Serialize(key))
                    yield return value;
                var amount = Helpers.DeserializeInt(buffer, ref index);
                foreach (var value in Helpers.Serialize(amount))
                    yield return value;
            }

            count = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(count))
                yield return value;
            for (int i = 0; i < count; ++i)
            {
                var key = Helpers.DeserializeString(buffer, ref index);
                var id = LegacySatelliteNames.GetId(key);
                foreach (var value in Helpers.Serialize(id.Value))
                    yield return value;

                var amount = Helpers.DeserializeInt(buffer, ref index);
                foreach (var value in Helpers.Serialize(amount))
                    yield return value;
            }

            foreach (var value in Helpers.Serialize(0))
                yield return value;
            foreach (var value in Helpers.Serialize(0))
                yield return value;
        }

        private static IEnumerable<byte> Upgrade_2_3(byte[] buffer)
        {
            OptimizedDebug.Log("InventoryData.Upgrade_2_3");

            var index = 0;
            Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(3)) // version
                yield return value;

            var count = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(count))
                yield return value;
            for (var i = 0; i < count; ++i)
            {
                var key = FromInt32ToInt64(Helpers.DeserializeInt(buffer, ref index));
                foreach (var value in Helpers.Serialize(key))
                    yield return value;
                var amount = Helpers.DeserializeInt(buffer, ref index);
                foreach (var value in Helpers.Serialize(amount))
                    yield return value;
            }

            for (var i = index; i < buffer.Length; ++i)
                yield return buffer[i];
        }

        internal static long FromInt32ToInt64(int data)
        {
            var modification = (byte)data; data >>= 8;
            var quality = (byte)data; data >>= 8;
            var component = data;

            long result = component; result <<= 8;
            result += quality; result <<= 8;
            result += modification; result <<= 8;
            result += /*level*/0; result <<= 8;

            return result;
        }


        private bool _isChanged = true;
        private readonly GameItemCollection<long> _components = new GameItemCollection<long>();
        private readonly GameItemCollection<int> _satellites = new GameItemCollection<int>();
    }
}
