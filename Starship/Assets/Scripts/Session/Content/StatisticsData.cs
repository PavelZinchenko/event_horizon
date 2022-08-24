using System;
using System.Collections.Generic;
using System.Linq;
using Database.Legacy;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameModel.Serialization;
using Zenject;

namespace Session.Content
{
    public class StatisticsData : ISerializableData
	{
        [Inject]
		public StatisticsData(byte[] buffer = null)
		{
			IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "craft";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 4; } }

        public void UnlockShip(ItemId<Ship> id)
        {
            IsChanged |= _unlockedShips.Add(id.Value);
        }

        public IEnumerable<ItemId<Ship>> UnlockedShips { get { return _unlockedShips.Select(id => new ItemId<Ship>(id)); } }

        public int DefeatedEnemies
        {
            get { return _defeatedEnemies; }
            set
            {
                if (_defeatedEnemies == value)
                    return;

                IsChanged = true;
                _defeatedEnemies = value;
            }
        }

        public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;
            foreach (var value in Helpers.Serialize(_unlockedShips))
                yield return value;
            foreach (var value in Helpers.Serialize(_defeatedEnemies))
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
				UnityEngine.Debug.Log("StatisticsData: incorrect data version");
                throw new ArgumentException();
            }

            _unlockedShips = new HashSet<int>(Helpers.DeserializeIntArray(buffer, ref index));
			_defeatedEnemies = Helpers.DeserializeInt(buffer, ref index);

            IsChanged = false;
		}

		private static bool TryUpgrade(ref byte[] data, int version)
		{
			if (version < 3)
			{
				data = Upgrade_3().ToArray();
				version = 3;
			}

            if (version == 3)
            {
                data = Upgrade_3_4(data).ToArray();
                version = 4;
            }

            return version == CurrentVersion;
		}

		
		private static IEnumerable<byte> Upgrade_3()
		{
			UnityEngine.Debug.Log("StatisticsData.Upgrade_3");
			
			int index = 0;
			
			var version = 3;
			foreach (var value in Helpers.Serialize(version))
				yield return value;

            foreach (var value in Helpers.Serialize(0))
                yield return value;
            foreach (var value in Helpers.Serialize(0))
                yield return value;
		}

        private static IEnumerable<byte> Upgrade_3_4(byte[] buffer)
        {
            UnityEngine.Debug.Log("StatisticsData.Upgrade_3_4");

            int index = 0;

            Helpers.DeserializeInt(buffer, ref index);
            var version = 4;
            foreach (var value in Helpers.Serialize(version))
                yield return value;

            var ships = Helpers.DeserializeStringArray(buffer, ref index).Select(item => LegacyShipBuildNames.GetId(item).Value);
            foreach (var value in Helpers.Serialize(ships))
                yield return value;
           
            var defeatedEnemies = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(defeatedEnemies))
                yield return value;

            foreach (var value in Helpers.Serialize(0))
                yield return value;
            foreach (var value in Helpers.Serialize(0))
                yield return value;
        }

        private HashSet<int> _unlockedShips = new HashSet<int>();
        private int _defeatedEnemies;
	}
}
