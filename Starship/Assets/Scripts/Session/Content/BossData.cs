using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Utils;
using Zenject;

namespace Session.Content
{
    public class BossData : ISerializableData
	{
        [Inject]
		public BossData(byte[] buffer = null)
		{
			IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "boss";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 2; } }
		
		public long CompletedTime(int id)
		{
			BossInfo info;
			return _completedTime.TryGetValue(id, out info) ? info.LastDefeatTime : 0;
		}
		
		public int DefeatCount(int id)
		{
			BossInfo info;
			return _completedTime.TryGetValue(id, out info) ? info.DefeatCount : 0;
		}
		
		public void SetCompleted(int id)
		{
			IsChanged = true;

			BossInfo info;
			if (!_completedTime.TryGetValue(id, out info))
				info = new BossInfo();
			info.DefeatCount++;
			info.LastDefeatTime = System.DateTime.UtcNow.Ticks;

			_completedTime[id] = info;
		}
		
		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;
			
			foreach (var value in Helpers.Serialize(_completedTime.Count))
				yield return value;
			foreach (var item in _completedTime)
			{
				foreach (var value in Helpers.Serialize(item.Key))
					yield return value;
				foreach (var value in Helpers.Serialize(item.Value.DefeatCount))
					yield return value;
				foreach (var value in Helpers.Serialize(item.Value.LastDefeatTime))
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
				OptimizedDebug.Log("BossData: incorrect data version");
                throw new ArgumentException();
            }

			var count = Helpers.DeserializeInt(buffer, ref index);
			for (int i = 0; i < count; ++i)
			{
				var key = Helpers.DeserializeInt(buffer, ref index);
				var info = new BossInfo();
				info.DefeatCount = Helpers.DeserializeInt(buffer, ref index);
				info.LastDefeatTime = Helpers.DeserializeLong(buffer, ref index);
				_completedTime.Add(key, info);
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
			
			return version == CurrentVersion;
		}

		private static IEnumerable<byte> Upgrade_1_2(byte[] buffer)
		{
			OptimizedDebug.Log("BossData.Upgrade_1_2");
			
			int index = 0;
			
			Helpers.DeserializeInt(buffer, ref index);
			var version = 2;
			foreach (var value in Helpers.Serialize(version))
				yield return value;				

			var completed = Helpers.DeserializeHashSet(buffer, ref index);
			foreach (var value in Helpers.Serialize(completed.Count))
				yield return value;

			var time = System.DateTime.UtcNow.Ticks;
			foreach (var item in completed)
			{
				foreach (var value in Helpers.Serialize(item))
					yield return value;
				foreach (var value in Helpers.Serialize(1)) // DefeatCount
					yield return value;
				foreach (var value in Helpers.Serialize(time))
					yield return value;
			}
		}

		private Dictionary<int,BossInfo> _completedTime = new Dictionary<int, BossInfo>();

		private struct BossInfo
		{
			public int DefeatCount;
			public long LastDefeatTime;
		}
	}
}
