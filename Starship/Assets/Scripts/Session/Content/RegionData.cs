using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameModel;
using UnityEngine;
using Utils;
using Zenject;
using Helpers = GameModel.Serialization.Helpers;

namespace Session.Content
{
    public class RegionData : ISerializableData
	{
        [Inject]
        public RegionData(int gameSeed, byte[] buffer = null)
		{
			IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer, gameSeed);
        }

        public string FileName { get { return Name; } }
        public const string Name = "regions";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 3; } }

	    public void Reset()
	    {
            _factions.Clear();
            _defeatedFleetCount.Clear();
	        IsChanged = true;
	    }

        public int ExploredRegionCount => _factions.Count;

        public IEnumerable<int> Regions => _factions.Keys;

        public ItemId<Faction> GetRegionFactionId(int regionId)
	    {
	        int factionId;
	        return _factions.TryGetValue(regionId, out factionId) ? new ItemId<Faction>(factionId) : Faction.Undefined.Id;
	    }

	    public void SetRegionFactionId(int regionId, ItemId<Faction> factionId)
	    {
            _factions[regionId] = factionId.Value;
	    }

		public int GetDefeatedFleetCount(int regionId)
		{
			int value;
			if (!_defeatedFleetCount.TryGetValue(regionId, out value))
				return 0;
			return value > 0 ? value : 0;
		}

		public bool IsRegionCaptured(int regionId)
		{
			int value;
			return _defeatedFleetCount.TryGetValue(regionId, out value) && value < 0;
		}

	    public IEnumerable<ItemId<Faction>> GetCapturedFactions()
	    {
	        foreach (var item in _defeatedFleetCount)
	        {
	            if (item.Value >= 0) continue;

	            int faction;
	            if (!_factions.TryGetValue(item.Key, out faction)) continue;
	            yield return new ItemId<Faction>(faction);
	        }
	    }

        public void SetRegionCaptured(int regionId, bool caputured)
		{
			IsChanged = true;
			_defeatedFleetCount[regionId] = caputured ? -1 : 0;
		}

	    public IEnumerable<int> DiscoveredRegions { get { return _defeatedFleetCount.Keys; }  }

		public void SetDefeatedFleetCount(int regionId, int count)
		{
			int value;
			if (_defeatedFleetCount.TryGetValue(regionId, out value) && value < 0)
				return;

			IsChanged = true;
			_defeatedFleetCount[regionId] = count > 0 ? count : 0;
		}

		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;

		    foreach (var value in Helpers.Serialize(_defeatedFleetCount))
		        yield return value;
		    foreach (var value in Helpers.Serialize(_factions))
		        yield return value;

            foreach (var value in Helpers.Serialize(0)) // reserved
		        yield return value;
		    foreach (var value in Helpers.Serialize(0))
		        yield return value;
		    foreach (var value in Helpers.Serialize(0))
		        yield return value;
		}

        private void Deserialize(byte[] buffer, int gameSeed)
		{
			if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

			int index = 0;
			var version = Helpers.DeserializeInt(buffer, ref index);
			if (version != CurrentVersion && !TryUpgrade(ref buffer, version, gameSeed))
			{
				OptimizedDebug.Log("RegionData: incorrect data version");
                throw new ArgumentException();
            }

            _defeatedFleetCount = Helpers.DeserializeDictionary(buffer, ref index);
		    _factions = Helpers.DeserializeDictionary(buffer, ref index);

            IsChanged = false;
		}

		private static bool TryUpgrade(ref byte[] data, int version, int gameSeed)
		{
			if (version == 1)
			{
				data = Upgrade_1_2(data).ToArray();
				version = 2;
			}

		    if (version == 2)
		    {
		        data = Upgrade_2_3(data, gameSeed).ToArray();
		        version = 3;
		    }

			return version == CurrentVersion;
		}
		
		private static IEnumerable<byte> Upgrade_1_2(byte[] buffer)
		{
			foreach (var value in Helpers.Serialize(2)) // version
				yield return value;
			foreach (var value in Helpers.Serialize(0)) // defeatedFleetCount
				yield return value;
		}

	    private static IEnumerable<byte> Upgrade_2_3(byte[] buffer, int gameSeed)
	    {
	        var index = 0;
	        Helpers.DeserializeInt(buffer, ref index);
	        var defeatedFleetCount = Helpers.DeserializeDictionary(buffer, ref index);
            //var factions = defeatedFleetCount.Keys.ToDictionary(item => item, item => Faction.Undefined.Id.Value);

	        var factions = new List<KeyValuePair<int, int>>
	        {
	            new KeyValuePair<int, int>(1, 0),
	            new KeyValuePair<int, int>(2, 0),
	            new KeyValuePair<int, int>(6, 0),
	            new KeyValuePair<int, int>(2, 0),
	            new KeyValuePair<int, int>(1, 10),
	            new KeyValuePair<int, int>(1, 10),
	            new KeyValuePair<int, int>(2, 10),
	            new KeyValuePair<int, int>(6, 10),
	            new KeyValuePair<int, int>(3, 20),
	            new KeyValuePair<int, int>(3, 20),
	            new KeyValuePair<int, int>(3, 20),
	            new KeyValuePair<int, int>(6, 20),
	            new KeyValuePair<int, int>(8, 30),
	            new KeyValuePair<int, int>(8, 30),
	            new KeyValuePair<int, int>(8, 30),
	            new KeyValuePair<int, int>(1, 30),
	            new KeyValuePair<int, int>(7, 40),
	            new KeyValuePair<int, int>(4, 40),
	            new KeyValuePair<int, int>(7, 40),
	            new KeyValuePair<int, int>(2, 40),
	            new KeyValuePair<int, int>(5, 50),
	            new KeyValuePair<int, int>(4, 50),
	            new KeyValuePair<int, int>(5, 50),
	            new KeyValuePair<int, int>(6, 50),
	            new KeyValuePair<int, int>(9, 60),
	            new KeyValuePair<int, int>(4, 60),
	            new KeyValuePair<int, int>(9, 60),
	            new KeyValuePair<int, int>(9, 60),
	            new KeyValuePair<int, int>(11, 100),
	            new KeyValuePair<int, int>(11, 150),
	            new KeyValuePair<int, int>(11, 200),
	            new KeyValuePair<int, int>(11, 250),
	            new KeyValuePair<int, int>(10, 150),
	            new KeyValuePair<int, int>(10, 200),
	            new KeyValuePair<int, int>(10, 250),
	            new KeyValuePair<int, int>(10, 300),
	            new KeyValuePair<int, int>(12, 250),
	            new KeyValuePair<int, int>(12, 250),
	            new KeyValuePair<int, int>(12, 250),
	            new KeyValuePair<int, int>(12, 300)
	        };

            var regionFactions = defeatedFleetCount.Keys.ToDictionary(regionId => regionId, regionId =>
	        {
                var homeStar = RegionLayout.GetRegionHomeStar(regionId);
	            var level = Mathf.RoundToInt(StarLayout.GetStarPosition(homeStar, gameSeed).magnitude);
	            var random = new System.Random(homeStar + gameSeed);
                var faction = factions.Where(item => item.Value <= level).RandomElements(1, random).First().Key;
                return faction;
	        });


	        foreach (var value in Helpers.Serialize(3)) // version
	            yield return value;
            foreach (var value in Helpers.Serialize(defeatedFleetCount))
	            yield return value;
	        foreach (var value in Helpers.Serialize(regionFactions))
	            yield return value;
	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
	        foreach (var value in Helpers.Serialize(0))
	            yield return value;
	    }




        private Dictionary<int, int> _defeatedFleetCount = new Dictionary<int, int>();
        private Dictionary<int, int> _factions = new Dictionary<int, int>();
	}
}
