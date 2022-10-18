using System;
using System.Linq;
using System.Collections.Generic;
using DataModel.Technology;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices.Database;
using Session;
using Services.Messenger;
using Utils;
using Zenject;

namespace GameServices.Research
{
    public sealed class Research : GameServiceBase
    {
        [Inject]
        public Research(ISessionData session, ITechnologies technologies, IMessenger messenger,
            SessionDataLoadedSignal dataLoadedSignal, SessionCreatedSignal sessionCreatedSignal, IDatabase database)
            : base(dataLoadedSignal, sessionCreatedSignal)
        {
            _session = session;
            _technologies = technologies;
            _messenger = messenger;
            _database = database;
        }

        public ITechnologies Technologies
        {
            get { return _technologies; }
        }

        public bool IsTechResearched(ITechnology technology)
        {
            return _researchedTech.Contains(technology);
        }

        public bool IsTechAvailable(ITechnology technology)
        {
            return IsAvailabe(technology, false);
        }

        public void ResearchTechForFree(ITechnology technology)
        {
            if (!_researchedTech.Add(technology))
                return;

            _availableTech.Add(technology);
            var faction = technology.Faction;
            _session.Research.SetResearchPoints(faction,
                _session.Research.GetResearchPoints(faction) + technology.Price);
            _session.Research.AddTechnology(technology.Id);

            CheckConsistency();
            _messenger.Broadcast(EventType.TechResearched);
            ResearchFreeDependants(technology);
        }

        public bool ResearchTech(ITechnology technology)
        {
            if (!IsAvailabe(technology, false))
                return false;

            var faction = technology.Faction;
            var points = _researchPoints[faction.Id.Value] - technology.Price;

            if (points < 0)
                return false;
            if (!_researchedTech.Add(technology))
                return false;

            _researchPoints[faction.Id.Value] = points;
            _availableTech.Add(technology);
            _session.Research.AddTechnology(technology.Id);

            CheckConsistency();
            _messenger.Broadcast(EventType.TechResearched);
            ResearchFreeDependants(technology);
            return true;
        }

        public int GetAvailablePoints(Faction faction)
        {
            return _researchPoints[faction.Id.Value];
        }

        public void AddResearchPoints(Faction faction, int amount)
        {
            _researchPoints[faction.Id.Value] += amount;
            _session.Research.SetResearchPoints(faction, _session.Research.GetResearchPoints(faction) + amount);
            _messenger.Broadcast(EventType.TechPointsChanged);
        }

        public IEnumerable<ITechnology> GetAvailableTechs(Faction faction)
        {
            return _technologies.All.OfFaction(faction)
                .Where(item => !_researchedTech.Contains(item) && IsAvailabe(item, false));
        }

        private void ResearchFreeDependants(ITechnology root)
        {
            foreach (var technology in _technologies.Dependants(root))
            {
                if (technology.Price == 0) ResearchTech(technology);
            }
        }

        protected override void OnSessionDataLoaded()
        {
            _researchedTech.Clear();
            _researchPoints.Clear();
            _availableTech.Clear();

            var freeWithDependencies = new HashSet<ITechnology>();
            foreach (var tech in _technologies.All.Free())
            {
                if (tech.Requirements.Any()) freeWithDependencies.Add(tech);
                else _researchedTech.Add(tech);
            }

            foreach (var item in _session.Research.Technologies)
            {
                var tech = _technologies.Get(new ItemId<Technology>(item));
                if (tech == null)
                {
                    OptimizedDebug.Log("unknown tech: " + item);
                    continue;
                    //ExceptionHandler.HandleException(new System.ArgumentOutOfRangeException("unknown tech: " + item));
                }

                _researchedTech.Add(tech);
            }

            _researchPoints = _database.FactionList.ToDictionary(item => item.Id.Value,
                item => (ObscuredInt)_session.Research.GetResearchPoints(item));

            foreach (var tech in _researchedTech)
                _researchPoints[tech.Faction.Id.Value] -= tech.Price;
            
            foreach (var tech in freeWithDependencies)
                if (IsAvailabe(tech, true))
                    _researchedTech.Add(tech);

            foreach (var tech in _researchedTech)
                if (IsAvailabe(tech, true))
                    _availableTech.Add(tech);
        }

        protected override void OnSessionCreated() { }

        private bool IsAvailabe(ITechnology technology, bool recursive)
        {
            if (technology.Special && !_researchedTech.Contains(technology))
                return false;

            foreach (var item in technology.Requirements)
            {
                if (!_availableTech.Contains(item) && !recursive)
                    return false;
                if (!_researchedTech.Contains(item))
                    return false;
                if (!IsAvailabe(item, recursive))
                    return false;
            }

            return true;
        }

        private void CheckConsistency()
        {
            foreach (var item in _researchedTech)
            {
                if (_availableTech.Contains(item))
                    continue;
                if (IsAvailabe(item, true))
                    _availableTech.Add(item);
            }
        }

        private HashSet<ITechnology> _researchedTech = new HashSet<ITechnology>();
        private HashSet<ITechnology> _availableTech = new HashSet<ITechnology>();
        private Dictionary<int, ObscuredInt> _researchPoints = new Dictionary<int, ObscuredInt>();
        private readonly IDatabase _database;
        private readonly ISessionData _session;
        private readonly IMessenger _messenger;
        private readonly ITechnologies _technologies;
    }
}
