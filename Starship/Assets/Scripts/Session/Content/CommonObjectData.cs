using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Zenject;

namespace Session.Content
{
    public class CommonObjectData : ISerializableData
	{
        [Inject]
		public CommonObjectData(byte[] buffer = null)
		{
			IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "objects";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 3; } }

		public long GetUseTime(int id) 
		{
			long time;
			return _usedTime.TryGetValue(id, out time) ? time : 0L;
		}

		public void SetUseTime(int id, long time)
		{
			_usedTime[id] = time;
			IsChanged = true;
		}

		public int GetIntValue(int id)
		{
			int value;
			return _intValues.TryGetValue(id, out value) ? value : 0;
		}

		public void SetIntValue(int id, int value)
		{
			_intValues[id] = value;
			IsChanged = true;
		}

		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in Helpers.Serialize(CurrentVersion))
				yield return value;
			
			foreach (var value in Helpers.Serialize(_usedTime.Count))
				yield return value;
			foreach (var item in _usedTime)
			{
				foreach (var value in Helpers.Serialize(item.Key))
					yield return value;
				foreach (var value in Helpers.Serialize(item.Value))
					yield return value;
			}

			foreach (var value in Helpers.Serialize(_intValues.Count))
				yield return value;
			foreach (var item in _intValues)
			{
				foreach (var value in Helpers.Serialize(item.Key))
					yield return value;
				foreach (var value in Helpers.Serialize(item.Value))
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
				UnityEngine.Debug.Log("CommonObjectData: incorrect data version");
                throw new ArgumentException();
            }

			var count = Helpers.DeserializeInt(buffer, ref index);
			for (int i = 0; i < count; ++i)
			{
				var id = Helpers.DeserializeInt(buffer, ref index);
				var time = Helpers.DeserializeLong(buffer, ref index);
				_usedTime.Add(id, time);
			}

			count = Helpers.DeserializeInt(buffer, ref index);
			for (int i = 0; i < count; ++i)
			{
				var id = Helpers.DeserializeInt(buffer, ref index);
				var value = Helpers.DeserializeInt(buffer, ref index);
				_intValues.Add(id, value);
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
			foreach (var value in Helpers.Serialize(2))
				yield return value;
			foreach (var value in Helpers.Serialize(0))
				yield return value;				
		}
					
		private static IEnumerable<byte> Upgrade_2_3(byte[] buffer)
		{
			int index = 0;
			Helpers.DeserializeInt(buffer, ref index); // version
			foreach (var value in Helpers.Serialize(3))
				yield return value;

			for (int i = index; i < buffer.Length; ++i)
				yield return buffer[i];

			foreach (var value in Helpers.Serialize(0))
				yield return value;				
		}
		
		private Dictionary<int, long> _usedTime = new Dictionary<int, long>();
		private Dictionary<int, int> _intValues = new Dictionary<int, int>();
	}
}
