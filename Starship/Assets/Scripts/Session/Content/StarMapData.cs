using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Utils;
using Zenject;

namespace Session.Content
{
    public class StarMapData : ISerializableData
	{
		public enum Occupant
		{
			Unknown = 0,
			Empty,
			Passive,
			Agressive,
        }
     
        [Inject]   
        public StarMapData(PlayerPositionChangedSignal.Trigger playerPositionChangedTrigger, NewStarExploredSignal.Trigger newStarExploredTrigger, byte[] buffer = null)
        {
            _playerPositionChangedTrigger = playerPositionChangedTrigger;
            _newStarExploredTrigger = newStarExploredTrigger;
            IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "starmap";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 7; } }

        public void Reset()
        {
            _playerPosition = 0;
            LastPlayerPosition = 0;
            _stardata.Clear();
            _bookmarks.Clear();
            _planetdata.Clear();
            IsChanged = true;
        }

		public int PlayerPosition 
		{
			get { return _playerPosition; } 
			set
			{
			    if (value == _playerPosition)
                    return;

			    IsChanged = true;
			    LastPlayerPosition = _playerPosition;
			    _playerPosition = value;
			    SetVisited(value);

			    _playerPositionChangedTrigger.Fire(_playerPosition);
			} 
		}

		public int LastPlayerPosition { get; private set; }

		public float MapScaleFactor
		{
			get { return _mapScaleFactor; } 
			set
			{
				_mapScaleFactor = value;
				//IsChanged = true;
			} 
		}

		public float StarScaleFactor
		{
			get { return _starScaleFactor; } 
			set
			{
				_starScaleFactor = value;
				//IsChanged = true;
			} 
		}
		
		public bool IsVisited(int starId)
		{
			return _stardata.ContainsKey(starId);
		}

		public int VisitedStarsCount => _stardata.Count;
        public int FurthestVisitedStar => _stardata.Keys.Max();

        public uint GetPlanetData(int starId, int planetId)
        {
            var key = (((long)starId) << 32) + planetId;
            return _planetdata.TryGetValue(key, out var value) ? (uint)value : 0;
        }

		public void SetPlanetData(int starId, int planetId, uint value)
		{
			var key = (((long)starId) << 32) + planetId;
			_planetdata[key] = (int)value;
			IsChanged = true;
        }

		public void SetVisited(int starId)
		{
		    if (IsVisited(starId))
                return;

		    _stardata.Add(starId, 0);
		    IsChanged = true;
		}

        public Occupant GetEnemy(int starId)
		{
			if (!_stardata.TryGetValue(starId, out var value))
				return Occupant.Unknown;

			return (Occupant)value;
		}

		public void SetEnemy(int starId, Occupant enemy)
		{
		    if (_stardata.TryGetValue(starId, out var oldValue) && oldValue == (int)enemy)
                return;

		    IsChanged = true;
            _stardata[starId] = (int)enemy;

		    if (oldValue == (int)Occupant.Agressive || oldValue == (int)Occupant.Unknown)
		        if (enemy == Occupant.Empty || enemy == Occupant.Passive)
		            _newStarExploredTrigger.Fire(starId);
        }

		public string GetBookmark(int starId)
		{
			string value;
			return _bookmarks.TryGetValue(starId, out value) ? value : string.Empty;
		}

        public bool HasBookmark(int starId)
        {
            return _bookmarks.ContainsKey(starId) && !string.IsNullOrEmpty(_bookmarks[starId]);
        }

        public void SetBookmark(int starId, string value)
		{
			if (string.IsNullOrEmpty(value))
				IsChanged |= _bookmarks.Remove(starId);
			else
			{
                IsChanged = true;
				_bookmarks[starId] = value;
			}
		}
		
		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;

			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;

			foreach (var value in BitConverter.GetBytes(PlayerPosition))
				yield return value;

			foreach (var value in BitConverter.GetBytes(LastPlayerPosition))
				yield return value;

			foreach (var value in BitConverter.GetBytes(MapScaleFactor))
				yield return value;

			foreach (var value in BitConverter.GetBytes(StarScaleFactor))
				yield return value;                
            
			foreach (var value in Helpers.Serialize(_stardata))
				yield return value;

			foreach (var value in Helpers.Serialize(_planetdata))
				yield return value;

			foreach (var value in Helpers.Serialize(_bookmarks.Count))
				yield return value;
			foreach (var item in _bookmarks)
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
				UnityEngine.Debug.Log("StarMapData: incorrect data version");
                throw new ArgumentException();
            }

			_playerPosition = Helpers.DeserializeInt(buffer, ref index);
			LastPlayerPosition = Helpers.DeserializeInt(buffer, ref index);
			_mapScaleFactor = Helpers.DeserializeFloat(buffer, ref index);
			_starScaleFactor = Helpers.DeserializeFloat(buffer, ref index);
			_stardata = Helpers.DeserializeDictionary(buffer, ref index);
			_planetdata = Helpers.DeserializeDictionaryLongInt(buffer, ref index);

			var count = Helpers.DeserializeInt(buffer, ref index);
			for (int i = 0; i < count; ++i)
			{
				var key = Helpers.DeserializeInt(buffer, ref index);
				var value = Helpers.DeserializeString(buffer, ref index);
				_bookmarks.Add(key, value);
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
            
			if (version == 3)
			{
				data = Upgrade_3_4(data).ToArray();
                version = 4;
            }
            
			if (version == 4)
			{
				data = Upgrade_4_5(data).ToArray();
				version = 5;
			}

            if (version == 5 || version == 6)
            {
                data = Upgrade_5_7(data).ToArray();
                version = 7;
            }

            return version == CurrentVersion;
		}

		private static IEnumerable<byte> Upgrade_1_2(byte[] buffer)
		{
			int index = 0;
			
			Helpers.DeserializeInt(buffer, ref index);
			foreach (var value in Helpers.Serialize(2)) // version
				yield return value;				
			
			Helpers.DeserializeInt(buffer, ref index); // playerPosition
			foreach (var value in Helpers.Serialize(0))
				yield return value;				
			
			Helpers.DeserializeInt(buffer, ref index); // lastPlayerPosition
			foreach (var value in Helpers.Serialize(0))
				yield return value;
			
			foreach (var value in Helpers.Serialize(1.0f)) // mapScaleFactor
				yield return value;
			
			var lockedStars = Helpers.DeserializeHashSet(buffer, ref index);
			var unlockedStars = Helpers.DeserializeHashSet(buffer, ref index);
			var stardata = new Dictionary<int, long>();
			var random = new System.Random();
			foreach (var id in lockedStars)
				stardata[id] = 0;
			foreach (var id in unlockedStars)
				stardata[id] = DateTime.UtcNow.Ticks - random.Next(1000000);
			
			foreach (var value in Helpers.Serialize(stardata.Count))
				yield return value;
			foreach (var item in stardata)
			{
				foreach (var value in Helpers.Serialize(item.Key))
					yield return value;
				foreach (var value in Helpers.Serialize(item.Value))
					yield return value;
			}
			
			foreach (var value in Helpers.Serialize((int)0))
				yield return value;
			foreach (var value in Helpers.Serialize((int)0))
				yield return value;
		}

		private static IEnumerable<byte> Upgrade_2_3(byte[] buffer)
		{
			int index = 0;
			
			Helpers.DeserializeInt(buffer, ref index);
			foreach (var value in Helpers.Serialize(3)) // version
				yield return value;				
			
			Helpers.DeserializeInt(buffer, ref index); // playerPosition
			foreach (var value in Helpers.Serialize(0))
				yield return value;				
			
			Helpers.DeserializeInt(buffer, ref index); // lastPlayerPosition
			foreach (var value in Helpers.Serialize(0))
				yield return value;
			
			var mapScaleFactor = Helpers.DeserializeFloat(buffer, ref index);
			foreach (var value in Helpers.Serialize(mapScaleFactor))
				yield return value;
			
			foreach (var value in Helpers.Serialize(0)) // stardata
				yield return value;
			
			foreach (var value in Helpers.Serialize((int)0))
				yield return value;
			foreach (var value in Helpers.Serialize((int)0))
				yield return value;
		}
		
		private static IEnumerable<byte> Upgrade_3_4(byte[] buffer)
		{
			int index = 0;

			Helpers.DeserializeInt(buffer, ref index);
			foreach (var value in Helpers.Serialize(4)) // version
				yield return value;				

			var playerPosition = Helpers.DeserializeInt(buffer, ref index);
			var lastPlayerPosition = Helpers.DeserializeInt(buffer, ref index);
			var mapScaleFactor = Helpers.DeserializeFloat(buffer, ref index);

			var starData = new Dictionary<int, long>();
			var count = Helpers.DeserializeInt(buffer, ref index);
			for (int i = 0; i < count; ++i)
			{
				var key = Helpers.DeserializeInt(buffer, ref index);
				var value = Helpers.DeserializeLong(buffer, ref index);
                starData.Add(key, value);
            }
            
            var starScaleFactor = Helpers.DeserializeFloat(buffer, ref index);
            
			foreach (var value in BitConverter.GetBytes(playerPosition))
				yield return value;				
			foreach (var value in BitConverter.GetBytes(lastPlayerPosition))
				yield return value;				
			foreach (var value in BitConverter.GetBytes(mapScaleFactor))
				yield return value;				
			foreach (var value in BitConverter.GetBytes(starScaleFactor))
				yield return value;                
			
			foreach (var value in Helpers.Serialize(starData.Count))
				yield return value;
			
			foreach (var item in starData)
			{
				foreach (var value in Helpers.Serialize(item.Key))
                    yield return value;

				var data = item.Value > 0 ? Occupant.Empty : Occupant.Unknown;
                foreach (var value in Helpers.Serialize((int)data))
                    yield return value;
            }
            
            foreach (var value in Helpers.Serialize(0))
                yield return value;
		}

		private static IEnumerable<byte> Upgrade_4_5(byte[] buffer)
		{
			foreach (var value in buffer)
				yield return value;
			foreach (var value in Helpers.Serialize(0))
				yield return value;
		}

        private static IEnumerable<byte> Upgrade_5_7(byte[] buffer)
        {
            int index = 0;

            Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(7)) // version
                yield return value;

            var playerPosition = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(playerPosition))
                yield return value;

            var lastPlayerPosition = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(lastPlayerPosition))
                yield return value;

            var mapScaleFactor = Helpers.DeserializeFloat(buffer, ref index);
            foreach (var value in Helpers.Serialize(mapScaleFactor))
                yield return value;

            var starScaleFactor = Helpers.DeserializeFloat(buffer, ref index);
            foreach (var value in Helpers.Serialize(starScaleFactor))
                yield return value;

            var stardata = Helpers.DeserializeDictionary(buffer, ref index);
            foreach (var value in Helpers.Serialize(stardata))
                yield return value;

            Helpers.DeserializeDictionaryLongInt(buffer, ref index); // planetdata
            foreach (var value in Helpers.Serialize(0))
                yield return value;

            for (int i = index; i < buffer.Length; ++i)
                yield return buffer[i];
        }

        private int _playerPosition;
		private float _mapScaleFactor;
		private float _starScaleFactor;
		private Dictionary<int, int> _stardata = new Dictionary<int, int>();
		private Dictionary<long, int> _planetdata = new Dictionary<long, int>();
		private Dictionary<int, string> _bookmarks = new Dictionary<int, string>();

        private readonly PlayerPositionChangedSignal.Trigger _playerPositionChangedTrigger;
	    private readonly NewStarExploredSignal.Trigger _newStarExploredTrigger;
	}

    public class PlayerPositionChangedSignal : SmartWeakSignal<int> { public class Trigger : TriggerBase { } }
    public class NewStarExploredSignal : SmartWeakSignal<int> { public class Trigger : TriggerBase { } }
}
