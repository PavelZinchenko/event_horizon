using System.Linq;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Extensions;
using Session;
using UnityEngine;

namespace GameModel
{
	public class Region
	{
		private Region(int id, bool isPirateBase, ISessionData session, IDatabase database, BaseCapturedSignal.Trigger baseCapturedTrigger, RegionFleetDefeatedSignal.Trigger regionFleetDefeatedTrigger)
        {
            _session = session;
            _database = database;
            _baseCapturedTrigger = baseCapturedTrigger;
            _regionFleetDefeatedTrigger = regionFleetDefeatedTrigger;

			Id = Mathf.Max(id, 0);
			OwnerId = Id;

			if (Id == 0 || Id == PlayerHomeRegionId || isPirateBase)
			{
				_faction = Faction.Neutral;
			    Size = 0;
			}
			else
			{
				Size = RegionLayout.RegionFourthSize*2 - 1;// Game.Data.RandomInt(id, 1, RegionLayout.RegionMaxSize);
			}
			
			// If session have data about faction ID stored, we clear any faction that might've been set during initialization
			if (_session.Regions.GetRegionFactionId(Id) != Faction.Undefined.Id)
			{
				_faction = Faction.Undefined;
			}

			_defeatedFleetCount = _session.Regions.GetDefeatedFleetCount(Id);
			_isCaptured = Id == PlayerHomeRegionId || _session.Regions.IsRegionCaptured(Id);
		}

		public static Region TryCreate(int id, bool isPirateBase, ISessionData session, IDatabase database,
			BaseCapturedSignal.Trigger baseCapturedTrigger,
			RegionFleetDefeatedSignal.Trigger regionFleetDefeatedTrigger)
		{
			var region = new Region(id, isPirateBase, session, database, baseCapturedTrigger,
				regionFleetDefeatedTrigger);
			return region.Faction == Faction.Undefined ? Empty : region;
		}

		public void OnFleetDefeated()
		{
			UnityEngine.Debug.Log("RegionFleetDefeated: " + Id);

			if (Id == UnoccupiedRegionId)
				return;

			_defeatedFleetCount++;
			_session.Regions.SetDefeatedFleetCount(Id, _defeatedFleetCount);
            _regionFleetDefeatedTrigger.Fire(this);
		}

		public int Id { get; private set; }
		public int OwnerId { get; private set; }
		public int Size { get; private set; }

	    public bool IsPirateBase => _faction == Faction.Neutral;

        public int Relations
        {
            get
            {
                if (Id == PlayerHomeRegionId || IsCaptured) return 100;
                return _session.Quests.GetFactionRelations(HomeStar);
            }
        }

        public bool IsCaptured 
		{
			get
			{
				return _isCaptured;
			}
			set
			{
				if (Id == UnoccupiedRegionId || _isCaptured == value)
					return;

				_isCaptured = value;
                _session.Regions.SetRegionCaptured(Id, _isCaptured);
                _baseCapturedTrigger.Fire(this);
			}
		}

		public float BaseDefensePower 
		{
			get
			{
				return Mathf.Max(0.5f, Mathf.Min(3.0f + 0.05f * MilitaryPower, 10.0f) - _defeatedFleetCount*0.1f);
			}
		}

		public int MilitaryPower 
		{
			get
			{
				var level = Mathf.RoundToInt(StarLayout.GetStarPosition(HomeStar, _session.Game.Seed).magnitude);
				return level;
			}
		}

		public Faction Faction
		{
			get
			{
			    if (_faction != Faction.Undefined)
			        return _faction;

    	        var factionId = _session.Regions.GetRegionFactionId(Id);
			    if (factionId != Faction.Undefined.Id)
			    {
			        var faction = _database.GetFaction(factionId);
			        if (faction != Faction.Undefined)
			            return _faction = faction;
			    }

			    _faction = _database.FactionList.Visible().AtDistance(MilitaryPower).Where(item => item != Faction.Neutral).RandomElement(new System.Random(HomeStar + _session.Game.Seed)) ?? Faction.Undefined;
				_session.Regions.SetRegionFactionId(Id, _faction.Id);

                return _faction;
			}
            set
            {
				if (_faction != Faction.Undefined && value == _faction) return;
                _faction = value;
				_session.Regions.SetRegionFactionId(Id, value.Id);
				_baseCapturedTrigger.Fire(this);
            }
		}

		public int HomeStar
		{
			get
			{
				if (_homeStar < 0)
					_homeStar = RegionLayout.GetRegionHomeStar(Id);

				return _homeStar;
			}
		}

	    private Region()
	    {
	        Id = 0;
	        OwnerId = 0;
	        _faction = Faction.Neutral;
	        Size = 0;
	        _isCaptured = true;
	    }

        private bool _isCaptured;
		private int _defeatedFleetCount = 0;
		private int _homeStar = -1;
		private Faction _faction = Faction.Undefined;

	    private readonly IDatabase _database;
	    private readonly ISessionData _session;
	    private readonly BaseCapturedSignal.Trigger _baseCapturedTrigger;
	    private readonly RegionFleetDefeatedSignal.Trigger _regionFleetDefeatedTrigger;

        public const int UnoccupiedRegionId = 0;
		public const int PlayerHomeRegionId = 1;

	    public static readonly Region Empty = new Region();
	}
}
