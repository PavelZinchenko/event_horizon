using System.Collections.Generic;
using GameDatabase;
using GameServices;
using GameServices.Random;
using Services.Messenger;
using Session;
using Utils;
using Zenject;

namespace GameModel
{
	public class RegionMap : GameServiceBase
	{
	    [Inject]
	    public RegionMap(
            IRandom random, 
            ISessionData session,
            IDatabase database,
            BaseCapturedSignal.Trigger baseCapturedTrigger, 
            RegionFleetDefeatedSignal.Trigger regionFleetDefeatedTrigger, 
            SessionDataLoadedSignal dataLoadedSignal, 
            SessionCreatedSignal sessionCreatedSignal)
            : base(dataLoadedSignal, sessionCreatedSignal)
	    {
	        _random = random;
	        _session = session;
	        _database = database;
	        _baseCapturedTrigger = baseCapturedTrigger;
	        _regionFleetDefeatedTrigger = regionFleetDefeatedTrigger;
	    }

        public void GetAdjacentRegions(int starId, int distanceMin, int distanceMax, List<Region> regions)
        {
            if (_regionList.Count < _session.Regions.ExploredRegionCount)
            {
                foreach (var id in _session.Regions.Regions)
                {
					if (_regions.ContainsKey(id)) continue;
                    _regions.Add(id, _regionList.Count);
                    _regionList.Add(null);
                }
            }

            regions.Clear();
            foreach (var item in _regions)
            {
                var homestar = RegionLayout.GetRegionHomeStar(item.Key);
                var distance = StarLayout.Distance(homestar, starId);
                if (distance < distanceMin || distance > distanceMax) continue;

				var region = _regionList[item.Value] ?? this[item.Key];
                if (region.Id == Region.UnoccupiedRegionId) continue;
				if (region.IsPirateBase) continue;

				regions.Add(region);
            }
        }

        public Region GetNearestRegion(int starId)
        {
            StarLayout.IdToPosition(starId, out var x, out var y);

            GetAdjacentRegions(x, y,
                out var x1, out var y1, out var x2, out var y2, out var x3, out var y3, out var x4, out var y4,
                out var x5, out var y5, out var x6, out var y6, out var x7, out var y7, out var x8, out var y8);

            var distance1 = Distance(x, y, x2, y2);
            var distance2 = Distance(x, y, x2, y2);
            var distance3 = Distance(x, y, x3, y3);
            var distance4 = Distance(x, y, x4, y4);
            var distance5 = Distance(x, y, x5, y5);
            var distance6 = Distance(x, y, x6, y6);
            var distance7 = Distance(x, y, x7, y7);
            var distance8 = Distance(x, y, x8, y8);

            var minDistance = distance1;
            var regionId = RegionLayout.PositionToId(x1, y1);

            if (distance2 < minDistance)
            {
                minDistance = distance2;
                regionId = RegionLayout.PositionToId(x2, y2);
            }
            if (distance3 < minDistance)
            {
                minDistance = distance3;
                regionId = RegionLayout.PositionToId(x3, y3);
			}
            if (distance4 < minDistance)
            {
                minDistance = distance4;
                regionId = RegionLayout.PositionToId(x4, y4);
            }
            if (distance5 < minDistance)
            {
                minDistance = distance5;
                regionId = RegionLayout.PositionToId(x5, y5);
            }
            if (distance6 < minDistance)
            {
                minDistance = distance6;
                regionId = RegionLayout.PositionToId(x6, y6);
            }
            if (distance7 < minDistance)
            {
                minDistance = distance7;
                regionId = RegionLayout.PositionToId(x7, y7);
			}
			if (distance8 < minDistance)
            {
                minDistance = distance8;
                regionId = RegionLayout.PositionToId(x8, y8);
            }

            return this[regionId];
        }

		public Region GetStarRegion(int starId)
		{
            StarLayout.IdToPosition(starId, out var x, out var y);

            GetAdjacentRegions(x, y, 
				out var x1, out var y1, out var x2, out var y2, out var x3, out var y3, out var x4, out var y4,
				out var x5, out var y5, out var x6, out var y6, out var x7, out var y7, out var x8, out var y8);

			var distance = int.MaxValue;
			var region = Region.Empty;

			BelongsToRegion(x, y, x1, y1, ref distance, ref region);
			BelongsToRegion(x, y, x2, y2, ref distance, ref region);
			BelongsToRegion(x, y, x3, y3, ref distance, ref region);
			BelongsToRegion(x, y, x4, y4, ref distance, ref region);
			BelongsToRegion(x, y, x5, y5, ref distance, ref region);
			BelongsToRegion(x, y, x6, y6, ref distance, ref region);
			BelongsToRegion(x, y, x7, y7, ref distance, ref region);
			BelongsToRegion(x, y, x8, y8, ref distance, ref region);

			return region;
		}

        public bool IsStarReachable(int starId, int maxDistance)
		{
            StarLayout.IdToPosition(starId, out var x, out var y);

            GetAdjacentRegions(x, y, 
	            out var x1, out var y1, out var x2, out var y2, out var x3, out var y3, out var x4, out var y4,
	        	out var x5, out var y5, out var x6, out var y6, out var x7, out var y7, out var x8, out var y8);

			if (Distance(x, y, x1, y1) <= maxDistance && this[RegionLayout.PositionToId(x1,y1)].IsCaptured)
				return true;
			if (Distance(x, y, x2, y2) <= maxDistance && this[RegionLayout.PositionToId(x2,y2)].IsCaptured)
				return true;
			if (Distance(x, y, x3, y3) <= maxDistance && this[RegionLayout.PositionToId(x3,y3)].IsCaptured)
				return true;
			if (Distance(x, y, x4, y4) <= maxDistance && this[RegionLayout.PositionToId(x4,y4)].IsCaptured)
				return true;
			if (Distance(x, y, x5, y5) <= maxDistance && this[RegionLayout.PositionToId(x5,y5)].IsCaptured)
				return true;
			if (Distance(x, y, x6, y6) <= maxDistance && this[RegionLayout.PositionToId(x6,y6)].IsCaptured)
				return true;
			if (Distance(x, y, x7, y7) <= maxDistance && this[RegionLayout.PositionToId(x7,y7)].IsCaptured)
				return true;
			if (Distance(x, y, x8, y8) <= maxDistance && this[RegionLayout.PositionToId(x8,y8)].IsCaptured)
				return true;

			return false;
		}

		public bool IsRegionReachable(int regionId)
		{
            RegionLayout.IdToPosition(regionId, out var x, out var y);

			if (this[RegionLayout.PositionToId(x,y+2)].IsCaptured)
				return true;
			if (this[RegionLayout.PositionToId(x-1,y+1)].IsCaptured)
				return true;
			if (this[RegionLayout.PositionToId(x+1,y+1)].IsCaptured)
				return true;
			if (this[RegionLayout.PositionToId(x,y-2)].IsCaptured)
				return true;
			if (this[RegionLayout.PositionToId(x-1,y-1)].IsCaptured)
				return true;
			if (this[RegionLayout.PositionToId(x+1,y-1)].IsCaptured)
				return true;

			return false;
		}

	    public static bool IsHomeStar(int x, int y)
	    {
            GetAdjacentRegions(x, y,
                out var x1, out var y1, out var x2, out var y2, out var x3, out var y3, out var x4, out var y4,
                out var x5, out var y5, out var x6, out var y6, out var x7, out var y7, out var x8, out var y8);

	        if (x == x1 * 3 * RegionLayout.RegionFourthSize && y == y1 * 2 * RegionLayout.RegionFourthSize)
	            return true;
            if (x == x2 * 3 * RegionLayout.RegionFourthSize && y == y2 * 2 * RegionLayout.RegionFourthSize)
                return true;
            if (x == x3 * 3 * RegionLayout.RegionFourthSize && y == y3 * 2 * RegionLayout.RegionFourthSize)
                return true;
            if (x == x4 * 3 * RegionLayout.RegionFourthSize && y == y4 * 2 * RegionLayout.RegionFourthSize)
                return true;
            if (x == x5 * 3 * RegionLayout.RegionFourthSize && y == y5 * 2 * RegionLayout.RegionFourthSize)
                return true;
            if (x == x6 * 3 * RegionLayout.RegionFourthSize && y == y6 * 2 * RegionLayout.RegionFourthSize)
                return true;
            if (x == x7 * 3 * RegionLayout.RegionFourthSize && y == y7 * 2 * RegionLayout.RegionFourthSize)
                return true;
            if (x == x8 * 3 * RegionLayout.RegionFourthSize && y == y8 * 2 * RegionLayout.RegionFourthSize)
                return true;

            return false;
	    }

        public Region this[int id]
		{
			get
			{
				if (id < 0) return Region.Empty;
			    var unoccupied = id != Region.PlayerHomeRegionId && _random.RandomInt(id, 5) == 0;

			    if (unoccupied && _random.RandomInt(id + 1, 100) >= 25)
			        return Region.Empty;

                if (!_regions.TryGetValue(id, out var index))
                {
                    index = _regionList.Count;
                    _regions.Add(id, index);
					_regionList.Add(null);
				}

                var region = _regionList[index];
                if (region == null)
                {
					region = Region.TryCreate(id, unoccupied, _session, _database, _baseCapturedTrigger, _regionFleetDefeatedTrigger);
                    _regionList[index] = region;
                }

                return region;
            }
		}

	    protected override void OnSessionDataLoaded()
	    {
	        _regions.Clear();
	    }

	    protected override void OnSessionCreated()
        {
        }

        private static int Distance(int starX, int starY, int regionX, int regionY)
		{
			var x1 = regionX * 3*RegionLayout.RegionFourthSize;
			var y1 = regionY * 2*RegionLayout.RegionFourthSize;
			return StarLayout.Distance(starX, starY, x1, y1);
		}

		private static void GetAdjacentRegions(
			int starX, int starY, 
	        out int x1, out int y1,
	        out int x2, out int y2,
			out int x3, out int y3,
			out int x4, out int y4,
			out int x5, out int y5,
			out int x6, out int y6,
			out int x7, out int y7,
			out int x8, out int y8)
		{
			var y0 = frame(starY, 2*RegionLayout.RegionFourthSize);
			var x0 = frame(starX, 3*RegionLayout.RegionFourthSize);
			var odd = (x0 + y0)%2 == 0;
			
			if (odd)
			{
				x1 = x0; y1 = y0;
				x2 = x0; y2 = y0+2;
				x3 = x0+1; y3 = y0+1;
				x4 = x0+1; y4 = y0-1;

				x5 = x0; y5 = y0-2;
				x6 = x0; y6 = y0+2;
				x7 = x0+2; y7 = y0;
				x8 = x0+1; y8 = y0+3;
			}
			else
			{
				x1 = x0; y1 = y0+1;
				x2 = x0; y2 = y0-1;
				x3 = x0+1; y3 = y0;
				x4 = x0+1; y4 = y0+2;

				x5 = x0; y5 = y0+3;
				x6 = x0+2; y6 = y0+1;
				x7 = x0-1; y7 = y0;
				x8 = x0+1; y8 = y0-2;
			}
		}

		private bool BelongsToRegion(int x0, int y0, int x, int y, ref int minDistance, ref Region region)
		{
			var distance = Distance(x0, y0, x, y);

			if (distance < minDistance)
			{
				var value = this[RegionLayout.PositionToId(x,y)];
				if (value == null || distance > value.Size)
					return false;
				if (distance > 0 && distance == value.Size && _random.RandomInt(StarLayout.PositionToId(x0,y0)) % 2 == 0)
					return false;

				region = value;
				minDistance = distance;
				return true;
			}

			return false;
		}

		private static int frame(int x, int size)
		{
			return x >= 0 ? x / size : (x + 1)/size - 1;
		}

		private readonly Dictionary<int, int> _regions = new Dictionary<int, int>();
        private readonly List<Region> _regionList = new List<Region>();
	    private readonly IRandom _random;
	    private readonly ISessionData _session;
	    private readonly IDatabase _database;
	    private readonly BaseCapturedSignal.Trigger _baseCapturedTrigger;
	    private readonly RegionFleetDefeatedSignal.Trigger _regionFleetDefeatedTrigger;
	}

    public class BaseCapturedSignal : SmartWeakSignal<Region>
    {
        public class Trigger : TriggerBase {}
    }

    public class RegionFleetDefeatedSignal : SmartWeakSignal<Region>
    {
        public class Trigger : TriggerBase { }
    }
}
