using System;
using System.Collections.Generic;
using System.Linq;
using Constructor;
using Database.Legacy;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using Utils;
using Zenject;
using Helpers = GameModel.Serialization.Helpers;

namespace Session.Content
{
    public class FleetData : ISerializableData
    {
        [Inject]
        public FleetData(IDatabase database, byte[] buffer = null)
        {
            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer, database);

            _ships.DataChangedEvent += OnDataChanged;
            _hangarSlots.DataChangedEvent += OnDataChanged;
        }

        public string FileName => Name;
        public const string Name = "fleet";

        public bool IsChanged { get; private set; } = true;
        public static int CurrentVersion => 5;

        public int ExplorationShipId
        {
            get => _explorationShip;
            set
            {
                IsChanged |= _explorationShip != value;
                _explorationShip = value;
            }
        }

        public IItemCollection<ShipData> Ships => _ships;
        public IItemCollection<HangarSlotInfo> Hangar => _hangarSlots;

        public IEnumerable<byte> Serialize()
        {
            IsChanged = false;

            foreach (var value in Helpers.Serialize(CurrentVersion))
                yield return value;

            foreach (var value in Helpers.Serialize(_ships.Count))
                yield return value;
            foreach (var value in _ships.SelectMany(ship => ship.Serialize()))
                yield return value;

            foreach (var value in Helpers.Serialize(_hangarSlots.Count))
                yield return value;
            foreach (var value in _hangarSlots.SelectMany(slot => slot.Serialize()))
                yield return value;
            foreach (var value in Helpers.Serialize(_explorationShip))
                yield return value;

            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0))
                yield return value;
            foreach (var value in Helpers.Serialize(0))
                yield return value;
            foreach (var value in Helpers.Serialize(0))
                yield return value;
        }

        private void Deserialize(byte[] buffer, IDatabase database)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

            var index = 0;
            var version = Helpers.DeserializeInt(buffer, ref index);

            if (version != CurrentVersion && !TryUpgrade(ref buffer, version, database))
            {
                OptimizedDebug.Log("FleetData: incorrect data version");
                throw new ArgumentException();
            }

            var count = Helpers.DeserializeInt(buffer, ref index);
            for (var i = 0; i < count; ++i)
                _ships.Add(ShipData.Deserialize(buffer, ref index));

            count = Helpers.DeserializeInt(buffer, ref index);
            for (var i = 0; i < count; ++i)
                _hangarSlots.Add(HangarSlotInfo.Deserialize(buffer, ref index));

            _explorationShip = Helpers.DeserializeInt(buffer, ref index);
        }

        private void OnDataChanged()
        {
            IsChanged = true;
        }

        private static bool TryUpgrade(ref byte[] data, int version, IDatabase database)
        {
            if (version == 1)
            {
                data = Upgrade_1_2(data, database).ToArray();
                version = 2;
            }

            if (version == 2)
            {
                data = Upgrade_2_3(data, database).ToArray();
                version = 3;
            }

            if (version == 3)
            {
                data = Upgrade_3_4(data, database).ToArray();
                version = 4;
            }

            if (version == 4)
            {
                data = Upgrade_4_5(data).ToArray();
                version = 5;
            }

            return version == CurrentVersion;
        }

        private static IEnumerable<byte> Upgrade_1_2(byte[] buffer, IDatabase database)
        {
            var index = 0;
            Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(2)) // version
                yield return value;

            var count = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(count))
                yield return value;

            for (var i = 0; i < count; ++i)
            {
                var ship = ShipInfoV1.Deserialize(buffer, ref index).ToShipInfoObsolete(database);
                foreach (var value in ship.Serialize())
                    yield return value;
            }

            for (var i = index; i < buffer.Length; ++i)
                yield return buffer[i];
        }

        private static IEnumerable<byte> Upgrade_2_3(byte[] buffer, IDatabase database)
        {
            var index = 0;
            Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(3)) // version
                yield return value;

            var count = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(count))
                yield return value;

            for (var i = 0; i < count; ++i)
            {
                var ship = ShipInfoObsolete.DeserializeObsolete(buffer, ref index, database);
                foreach (var value in ship.Serialize())
                    yield return value;
            }

            for (var i = index; i < buffer.Length; ++i)
                yield return buffer[i];
        }

        private static IEnumerable<byte> Upgrade_3_4(byte[] buffer, IDatabase database)
        {
            var index = 0;
            Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(4)) // version
                yield return value;

            var count = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(count))
                yield return value;

            for (var i = 0; i < count; ++i)
            {
                var ship = ShipInfoObsolete.Deserialize(buffer, ref index);

                int shipId;
                if (!int.TryParse(ship.Id, out shipId))
                    shipId = LegacyShipNames.GetId(ship.Id).Value;

                int id;
                var satellite1 = database.GetSatellite(int.TryParse(ship.FirstSatellite.Id, out id) ? new ItemId<Satellite>(id) : LegacySatelliteNames.GetId(ship.FirstSatellite.Id));
                var satellite2 = database.GetSatellite(int.TryParse(ship.SecondSatellite.Id, out id) ? new ItemId<Satellite>(id) : LegacySatelliteNames.GetId(ship.SecondSatellite.Id));
                var satelliteComponents1 = ship.FirstSatellite.Components.Select(item => ComponentExtensions.Deserialize(database, item)).ToShipComponentsData();
                var satelliteComponents2 = ship.SecondSatellite.Components.Select(item => ComponentExtensions.Deserialize(database, item)).ToShipComponentsData();

                var shipData = new ShipData
                {
                    Id = shipId,
                    Name = ship.Name,
                    ColorScheme = 0xffffff,
                    Experience = ship.Experience,
                    Components = ship.Components.Select(item => ComponentExtensions.Deserialize(database, item)).ToShipComponentsData(),
                    Modifications = new ShipModificationsData { Modifications = ship.Modifications.Select(item => (item >> 32) == 4 ? (6 << 32) : item ) },
                    Satellite1 = satellite1 != null ? new SatelliteData { Id = satellite1.Id.Value, Components = satelliteComponents1 } : new SatelliteData(),
                    Satellite2 = satellite2 != null ? new SatelliteData { Id = satellite2.Id.Value, Components = satelliteComponents2 } : new SatelliteData(),
                };

                foreach (var value in shipData.Serialize())
                    yield return value;
            }

            for (var i = index; i < buffer.Length; ++i)
                yield return buffer[i];
        }

        private static IEnumerable<byte> Upgrade_4_5(byte[] buffer)
        {
            var index = 0;
            Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(5)) // version
                yield return value;

            for (var i = index; i < buffer.Length; ++i)
                yield return buffer[i];

            foreach (var value in Helpers.Serialize(-1)) // exploration ship
                yield return value;

            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
        }

        private int _explorationShip = -1;
        private readonly ObservableCollection<ShipData> _ships = new ObservableCollection<ShipData>();
        private readonly ObservableCollection<HangarSlotInfo> _hangarSlots = new ObservableCollection<HangarSlotInfo>();

        public struct ShipInfoV1
        {
            public string Id;
            public string Name;
            public ObscuredLong Experience;
            public IEnumerable<long> Components;
            public IEnumerable<long> Modifications;
            public SatelliteInfoV1 FirstSatellite;
            public SatelliteInfoV1 SecondSatellite;

            public IEnumerable<byte> Serialize()
            {
                foreach (var value in Helpers.Serialize(Id))
                    yield return value;
                foreach (var value in Helpers.Serialize(Name))
                    yield return value;
                foreach (var value in Helpers.Serialize(Experience))
                    yield return value;
                foreach (var value in Helpers.Serialize(Components))
                    yield return value;
                foreach (var value in Helpers.Serialize(Modifications))
                    yield return value;
                foreach (var value in FirstSatellite.Serialize())
                    yield return value;
                foreach (var value in SecondSatellite.Serialize())
                    yield return value;
            }

            public ShipInfoObsolete ToShipInfoObsolete(IDatabase database)
            {
                var info = new ShipInfoObsolete();
                info.Id = Id;
                info.Name = Name;
                info.Experience = Experience;
                info.Components = Components.Select(item => ComponentExtensions.DeserializeFromInt64Obsolete(database, item).SerializeObsolete().ToArray());
                info.Modifications = Modifications;
                info.FirstSatellite = FirstSatellite.ToSatelliteInfo(database);
                info.SecondSatellite = SecondSatellite.ToSatelliteInfo(database);
                return info;
            }

            public ShipInfoObsolete ToShipInfo(IDatabase database)
            {
                var info = new ShipInfoObsolete();
                info.Id = Id;
                info.Name = Name;
                info.Experience = Experience;
                info.Components = Components.Select(item => ComponentExtensions.DeserializeFromInt64Obsolete(database, item).Serialize().ToArray());
                info.Modifications = Modifications;
                info.FirstSatellite = FirstSatellite.ToSatelliteInfo(database);
                info.SecondSatellite = SecondSatellite.ToSatelliteInfo(database);
                return info;
            }

            public static ShipInfoV1 Deserialize(byte[] buffer, ref int index)
            {
                var info = new ShipInfoV1();
                info.Id = Helpers.DeserializeString(buffer, ref index);
                info.Name = Helpers.DeserializeString(buffer, ref index);
                info.Experience = Helpers.DeserializeLong(buffer, ref index);
                info.Components = Helpers.DeserializeLongArray(buffer, ref index);
                info.Modifications = Helpers.DeserializeLongArray(buffer, ref index);
                info.FirstSatellite = SatelliteInfoV1.Deserialize(buffer, ref index);
                info.SecondSatellite = SatelliteInfoV1.Deserialize(buffer, ref index);
                return info;
            }
        }

        public struct ShipInfoObsolete
        {
            public string Id;
            public string Name;
            public ObscuredLong Experience;
            public IEnumerable<byte[]> Components;
            public IEnumerable<long> Modifications;
            public SatelliteInfoV2 FirstSatellite;
            public SatelliteInfoV2 SecondSatellite;

            public IEnumerable<byte> Serialize()
            {
                foreach (var value in Helpers.Serialize(Id))
                    yield return value;
                foreach (var value in Helpers.Serialize(Name))
                    yield return value;
                foreach (var value in Helpers.Serialize(Experience))
                    yield return value;
                foreach (var value in Helpers.Serialize(Components))
                    yield return value;
                foreach (var value in Helpers.Serialize(Modifications))
                    yield return value;
                foreach (var value in FirstSatellite.Serialize())
                    yield return value;
                foreach (var value in SecondSatellite.Serialize())
                    yield return value;
            }

            public static ShipInfoObsolete Deserialize(byte[] buffer, ref int index)
            {
                var info = new ShipInfoObsolete();
                info.Id = Helpers.DeserializeString(buffer, ref index);
                info.Name = Helpers.DeserializeString(buffer, ref index);
                info.Experience = Helpers.DeserializeLong(buffer, ref index);
                info.Components = Helpers.DeserializeByteArrays(buffer, ref index);
                info.Modifications = Helpers.DeserializeLongArray(buffer, ref index);
                info.FirstSatellite = SatelliteInfoV2.Deserialize(buffer, ref index);
                info.SecondSatellite = SatelliteInfoV2.Deserialize(buffer, ref index);
                return info;
            }

            public static ShipInfoObsolete DeserializeObsolete(byte[] buffer, ref int index, IDatabase database)
            {
                var ship = Deserialize(buffer, ref index);
                ship.Components = ship.Components.Select(item => ComponentExtensions.DeserializeObsolete(database, item).Serialize().ToArray()).ToArray();
                ship.FirstSatellite.Components = ship.FirstSatellite.Components.Select(item => ComponentExtensions.DeserializeObsolete(database, item).Serialize().ToArray()).ToArray();
                ship.SecondSatellite.Components = ship.SecondSatellite.Components.Select(item => ComponentExtensions.DeserializeObsolete(database, item).Serialize().ToArray()).ToArray();
                return ship;
            }
        }

        public struct SatelliteInfoV1
        {
            public string Id;
            public IEnumerable<long> Components;

            public IEnumerable<byte> Serialize()
            {
                foreach (var value in Helpers.Serialize(Id))
                    yield return value;
                foreach (var value in Helpers.Serialize(Components))
                    yield return value;
            }

            public SatelliteInfoV2 ToSatelliteInfo(IDatabase database)
            {
                var info = new SatelliteInfoV2();
                info.Id = Id;
                info.Components = Components != null ? Components.Select(item => ComponentExtensions.DeserializeFromInt64Obsolete(database, item).SerializeObsolete().ToArray()) : Enumerable.Empty<byte[]>();
                return info;
            }

            public static SatelliteInfoV1 Deserialize(byte[] buffer, ref int index)
            {
                var info = new SatelliteInfoV1();
                info.Id = Helpers.DeserializeString(buffer, ref index);
                info.Components = Helpers.DeserializeLongArray(buffer, ref index);
                return info;
            }
        }

        public struct SatelliteInfoV2
        {
            public string Id;
            public IEnumerable<byte[]> Components;

            public IEnumerable<byte> Serialize()
            {
                foreach (var value in Helpers.Serialize(Id))
                    yield return value;
                foreach (var value in Helpers.Serialize(Components))
                    yield return value;
            }

            public static SatelliteInfoV2 Deserialize(byte[] buffer, ref int index)
            {
                var info = new SatelliteInfoV2();
                info.Id = Helpers.DeserializeString(buffer, ref index);
                info.Components = Helpers.DeserializeByteArrays(buffer, ref index);
                return info;
            }
        }

        public struct HangarSlotInfo
        {
            public int Index;
            public int ShipId;

            public IEnumerable<byte> Serialize()
            {
                foreach (var value in Helpers.Serialize(Index))
                    yield return value;
                foreach (var value in Helpers.Serialize(ShipId))
                    yield return value;
            }

            public static HangarSlotInfo Deserialize(byte[] buffer, ref int index)
            {
                var slotInfo = new HangarSlotInfo();
                slotInfo.Index = Helpers.DeserializeInt(buffer, ref index);
                slotInfo.ShipId = Helpers.DeserializeInt(buffer, ref index);
                return slotInfo;
            }
        }
    }
}
