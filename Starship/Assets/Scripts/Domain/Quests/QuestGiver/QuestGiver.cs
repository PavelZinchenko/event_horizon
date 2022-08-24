using System.Collections.Generic;
using System.Diagnostics;
using GameDatabase.Enums;
using GameModel;
using Session;

namespace Domain.Quests
{
    public class QuestGiver
    {
        public QuestGiver(GameDatabase.DataModel.QuestOrigin data, RegionMap regionMap, ISessionData session)
        {
            _data = data;
            _regionMap = regionMap;
            _session = session;
            _factionFilter = new FactionFilter(data.Factions, 0);
        }

        public int GetStartSystem(int currentStarId, int seed)
        {
            switch (_data.Type)
            {
                case QuestOriginType.CurrentStar:
                    return currentStarId;
                case QuestOriginType.HomeStar:
                    return 0;
                case QuestOriginType.CurrentFactionBase:
                    return _regionMap.GetStarRegion(currentStarId).HomeStar;
                case QuestOriginType.RandomStar:
                    return GetRandomStar(currentStarId, seed);
                case QuestOriginType.RandomFactionBase:
                    return GetRandomFactionBase(currentStarId, seed);
                default:
                    return -1;
            }
        }

        private int GetRandomStar(int center, int seed)
        {
            var random = new System.Random(seed);
            var minDistance = _data.MinDistance;
            var maxDistance = _data.MaxDistance > minDistance ? _data.MaxDistance : minDistance;
            var distance = maxDistance > minDistance ? minDistance + random.Next(maxDistance - minDistance + 1) : minDistance;
            var starId = GameModel.StarLayout.GetAdjacentStars(center, distance).RandomElement(random);
            return starId;
        }

        private int GetRandomFactionBase(int center, int seed)
        {
            var random = new System.Random(seed);

            var minDistance = _data.MinDistance;
            var maxDistance = _data.MaxDistance > minDistance ? _data.MaxDistance : minDistance;

            var minRelations = _data.MinRelations;
            var maxRelations = _data.MaxRelations > minRelations ? _data.MaxRelations : minRelations;

            _regionMap.GetAdjacentRegions(center, minDistance, maxDistance, _adjacentRegions);
            var index = random.Next(_adjacentRegions.Count);

            var count = _adjacentRegions.Count;
            for (var i = 0; i < count; ++i)
            {
                var region = _adjacentRegions[index + i < count ? index + i : index + i - count];
                if (region.Id == Region.PlayerHomeRegionId) continue;
                if (!_factionFilter.IsSuitable(region.Faction)) continue;
                if (!_session.StarMap.IsVisited(region.HomeStar)) continue;

                if (minRelations != 0 || maxRelations != 0)
                {
                    var relations = region.Relations;
                    if (relations < minRelations || relations > maxRelations) continue;
                }

                var starId = region.HomeStar;
                return starId;
            }

            return -1;
        }

        private readonly List<Region> _adjacentRegions = new List<Region>();
        private readonly FactionFilter _factionFilter;
        private readonly GameDatabase.DataModel.QuestOrigin _data;
        private readonly RegionMap _regionMap;
        private readonly ISessionData _session;
    }
}