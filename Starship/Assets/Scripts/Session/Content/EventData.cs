using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Utils;
using Zenject;

namespace Session.Content
{
    public class EventData : ISerializableData
	{
        [Inject]
		public EventData(byte[] buffer = null)
		{
			IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "events";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 3; } }

		public long CompletedTime(int starId)
		{
			long value;
			return _completedTime.TryGetValue(starId, out value) ? value : 0;
		}
		
		public void Complete(int starId)
		{
			_completedTime[starId] = System.DateTime.UtcNow.Ticks;
			IsChanged = true;
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
				OptimizedDebug.Log("EventData: incorrect data version");
                throw new ArgumentException();
            }

			var count = Helpers.DeserializeInt(buffer, ref index);
			for (int i = 0; i < count; ++i)
			{
				var key = Helpers.DeserializeInt(buffer, ref index);
				var value = Helpers.DeserializeLong(buffer, ref index);
				_completedTime.Add(key, value);
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
			foreach (var value in Helpers.Serialize(2)) // version
				yield return value;
			
			foreach (var value in Helpers.Serialize(0)) // randomEventId
				yield return value;
		}

		private static IEnumerable<byte> Upgrade_2_3(byte[] buffer)
		{
			foreach (var value in Helpers.Serialize(3)) // version
				yield return value;
			
			foreach (var value in Helpers.Serialize(0)) // completedTime
				yield return value;
		}
		
		private Dictionary<int, long> _completedTime = new Dictionary<int, long>();
	}
}
