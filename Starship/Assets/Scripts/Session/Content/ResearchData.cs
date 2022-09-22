using System;
using System.Collections.Generic;
using System.Linq;
using Database.Legacy;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameModel.Serialization;
using Utils;
using Zenject;

namespace Session.Content
{
    public class ResearchData : ISerializableData
	{
        [Inject]
        public ResearchData(byte[] buffer = null)
		{
			IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "research";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 3; } }
		
		public IEnumerable<int> Technologies { get { return _technologies; } }

		public void AddTechnology(ItemId<Technology> id)
		{
			IsChanged |= _technologies.Add(id.Value);
		}

		public int GetResearchPoints(Faction faction)
		{
			ObscuredInt points;
			return _researchPoints.TryGetValue(faction.Id.Value, out points) ? (int)points : 0;
		}

		public void SetResearchPoints(Faction faction, int points)
		{
			IsChanged = true;
			_researchPoints[faction.Id.Value] = points;
		}

		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;
			
			foreach (var value in Helpers.Serialize(_technologies))
				yield return value;

			foreach (var value in Helpers.Serialize(_researchPoints.Count))
				yield return value;
			foreach (var item in _researchPoints)
			{
				foreach (var value in Helpers.Serialize(item.Key))
					yield return value;
				foreach (var value in Helpers.Serialize((int)item.Value))
					yield return value;
			}
		}
		
		private void Deserialize(byte[] buffer)
		{
			if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

			int index = 0;
			var version = Helpers.DeserializeInt(buffer, ref index);
			if (version != CurrentVersion && !TryUpgrade(ref buffer, version))
			{
				OptimizedDebug.Log("ResearchData: incorrect data version");
                throw new ArgumentException();
            }

			_technologies = new HashSet<int>(Helpers.DeserializeIntArray(buffer, ref index));

			var count = Helpers.DeserializeInt(buffer, ref index);
			for (int i = 0; i < count; ++i)
			{
				var key = Helpers.DeserializeInt(buffer, ref index);
				var value = Helpers.DeserializeInt(buffer, ref index);
				_researchPoints.Add(key, value);
			}

            IsChanged = false;
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
			int index = 0;

			Helpers.DeserializeInt(buffer, ref index);
			var version = 2;
			foreach (var value in Helpers.Serialize(version))
				yield return value;

			var technologies = Helpers.DeserializeStringArray(buffer, ref index);
			foreach (var value in Helpers.Serialize(technologies))
				yield return value;

			var count = Helpers.DeserializeInt(buffer, ref index);
			foreach (var value in Helpers.Serialize(count))
				yield return value;
			for (int i = 0; i < count; ++i)
			{
				var key = Helpers.DeserializeInt(buffer, ref index);
				var exp = Helpers.DeserializeInt(buffer, ref index);

				exp = (int)System.Math.Floor(System.Math.Sqrt(exp/100));

				foreach (var value in Helpers.Serialize(key))
					yield return value;
				foreach (var value in Helpers.Serialize(exp))
					yield return value;					
			}
		}

        private static IEnumerable<byte> Upgrade_2_3(byte[] buffer)
        {
            int index = 0;

            Helpers.DeserializeInt(buffer, ref index);
            var version = 3;
            foreach (var value in Helpers.Serialize(version))
                yield return value;

            var technologies = Helpers.DeserializeStringArray(buffer, ref index);
            foreach (var value in Helpers.Serialize(technologies.Select<string,int>(name => LegacyTechnologyNames.GetId(name).Value)))
                yield return value;

            for (int i = index; i < buffer.Length; ++i)
                yield return buffer[i];
        }

        private HashSet<int> _technologies = new HashSet<int>();
		private Dictionary<int, ObscuredInt> _researchPoints = new Dictionary<int, ObscuredInt>();
	}
}
