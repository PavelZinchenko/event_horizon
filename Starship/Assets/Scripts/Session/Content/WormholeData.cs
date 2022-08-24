using System;
using System.Collections.Generic;
using GameModel.Serialization;
using Zenject;

namespace Session.Content
{
    public class WormholeData : ISerializableData
	{
        [Inject]
		public WormholeData(byte[] buffer = null)
		{
			IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "wormholes";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 1; } }
		
		public int GetTarget(int source)
		{
			int target;
			return _routes.TryGetValue(source, out target) ? target : -1;
		}
		
		public void SetTarget(int source, int target)
		{
			if (_routes.ContainsKey(source) || _routes.ContainsKey(target))
				throw new System.InvalidOperationException();

			_routes[source] = target;
			_routes[target] = source;
			IsChanged = true;
		}
		
		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;

			var keys = new HashSet<int>();
			foreach (var item in _routes)
				if (!keys.Contains(item.Value))
					keys.Add(item.Key);

			foreach (var value in BitConverter.GetBytes(keys.Count))
				yield return value;

			foreach (var item in keys)
			{
				foreach (var value in BitConverter.GetBytes(item))
					yield return value;
				foreach (var value in BitConverter.GetBytes(_routes[item]))
					yield return value;
			}
		}
		
		private void Deserialize(byte[] buffer)
		{
			if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

			int index = 0;
			var version = Helpers.DeserializeInt(buffer, ref index);
			if (version != CurrentVersion)
			{
				UnityEngine.Debug.Log("WormholeData: incorrect data version");
                throw new ArgumentException();
            }

			var count = Helpers.DeserializeInt(buffer, ref index);
			for (var i = 0; i < count; ++i)
			{
				var key = Helpers.DeserializeInt(buffer, ref index);
				var value = Helpers.DeserializeInt(buffer, ref index);
				_routes[key] = value;
				_routes[value] = key;
			}

            IsChanged = false;
		}
		
		private Dictionary<int, int> _routes = new Dictionary<int, int>();
	}
}
