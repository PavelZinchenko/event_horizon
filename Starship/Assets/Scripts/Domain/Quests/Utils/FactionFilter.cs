using System;
using System.Collections.Generic;
using GameDatabase.DataModel;
using GameDatabase.Enums;

namespace Domain.Quests
{
    public class FactionFilter
    {
        public FactionFilter(RequiredFactions requiredFactions, int level, Faction starFaction = null)
        {
            if (requiredFactions == null) return;

            _starFaction = starFaction;
            _starLevel = level;

            _type = requiredFactions.Type;
            var count = requiredFactions.List.Count;

            if (count == 0)
                return;
            if (count == 1)
                _faction = requiredFactions.List[0];
            else if (count > 1)
                _factions = new HashSet<Faction>(requiredFactions.List);
        }

        public bool IsSuitable(Faction faction)
        {
            return IsSuitable(faction, _starLevel, _starFaction);
        }

        public bool IsSuitable(Faction faction, int level, Faction starFaction)
        {
            if (_type == FactionFilterType.StarOwnersAndList && faction == starFaction)
                return true;
            if (_type == FactionFilterType.AllAvailable)
                return faction.HomeStarDistance <= level && !faction.Hidden;

            var foundInList = faction == _faction || (_factions != null && _factions.Contains(faction));
            if (_type == FactionFilterType.AllButList)
                return !foundInList;

            if (_type == FactionFilterType.ListOnly || _type == FactionFilterType.StarOwnersAndList)
                return foundInList;

            throw new ArgumentException();
        }

        private readonly FactionFilterType _type;
        private readonly HashSet<Faction> _factions;
        private readonly Faction _faction;
        private readonly Faction _starFaction;
        private readonly int _starLevel;
    }
}
