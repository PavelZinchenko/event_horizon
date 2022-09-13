using System.Collections.Generic;
using Domain.Quests;
using Galaxy;
using GameDatabase.DataModel;
using GameModel;
using GameServices;
using GameServices.Player;
using Services.Messenger;
using Session;
using Session.Content;
using Zenject;

namespace Domain.Player
{
    public class StarMapManager : GameServiceBase
    {
        [Inject] private readonly RegionMap _regionMap;
        [Inject] private readonly StarData _starData;
        [Inject] private readonly PlayerSkills _playerSkills;
        [Inject] private readonly IMessenger _messenger;
        [Inject] private readonly QuestEventSignal.Trigger _questEventTrigger;

        public StarMapManager(
            ISessionData session,
            SessionDataLoadedSignal sessionDataLoadedSignal,
            SessionCreatedSignal sessionCreatedSignal,
            RegionFleetDefeatedSignal regionFleetDefeatedSignal,
            PlayerPositionChangedSignal playerPositionChangedSignal,
            NewStarExploredSignal newStarExploredSignal)
            : base(sessionDataLoadedSignal, sessionCreatedSignal)
        {
            _session = session;
            _regionFleetDefeatedSignal = regionFleetDefeatedSignal;
            _regionFleetDefeatedSignal.Event += OnRegionFleetDefeated;
            _playerPositionChangedSignal = playerPositionChangedSignal;
            _playerPositionChangedSignal.Event += OnPlayerPositionChanged;
            _newStarExploredSignal = newStarExploredSignal;
            _newStarExploredSignal.Event += OnNewStarExplored;
        }

        public bool IsFactionUnlocked(Faction faction)
        {
            return faction == Faction.Neutral || _factions.Contains(faction);
        }

        protected override void OnSessionDataLoaded()
        {
        }

        protected override void OnSessionCreated()
        {
            _factions.Clear();
            foreach (var id in _session.Regions.DiscoveredRegions)
                _factions.Add(_regionMap[id].Faction);
        }

        private void OnRegionFleetDefeated(Region region)
        {
            _factions.Add(region.Faction);
        }

        private void OnPlayerPositionChanged(int starId)
        {
            if (_starData.GetOccupant(starId).IsAggressive)
            {
                // A lot of things can happen between arriving at node with encounter and when "OnNewStarExplored"
                // triggers, but we assume that player was NOT moved to a different start in a meanwhile, so we store
                // star ID in a variable, and if by the time NewStarExplore is triggered this variable is the same,
                // we trigger the logic that should've originally happened here.
                // All of this work just to avoid double quest trigger
                _shouldTriggerMoveOnExplorationAt = starId;
                return;
            }

            UpdatePosition(starId);
        }

        private void OnNewStarExplored(int starId)
        {
            if (_shouldTriggerMoveOnExplorationAt == starId)
            {
                UpdatePosition(starId);   
            }
            _questEventTrigger.Fire(new StarEventData(QuestEventType.NewStarSystemExplored, starId));
        }

        private void UpdatePosition(int starId)
        {
            _shouldTriggerMoveOnExplorationAt = -1;
            ExploreAdjacentStars(starId);
            _questEventTrigger.Fire(new StarEventData(QuestEventType.ArrivedAtStarSystem, starId));
        }

        private void ExploreAdjacentStars(int starId)
        {
            var distance = _playerSkills.SpaceScanner;
            if (distance < 1)
                return;

            for (var i = 1; i <= distance; ++i)
                foreach (var id in StarLayout.GetAdjacentStars(starId, i))
                    _starData.SetVisited(id);

            _messenger.Broadcast(EventType.StarMapChanged);
        }

        private int _shouldTriggerMoveOnExplorationAt = -1;
        private readonly ISessionData _session;
        private readonly RegionFleetDefeatedSignal _regionFleetDefeatedSignal;
        private readonly PlayerPositionChangedSignal _playerPositionChangedSignal;
        private readonly NewStarExploredSignal _newStarExploredSignal;
        private readonly HashSet<Faction> _factions = new HashSet<Faction>();
    }
}
