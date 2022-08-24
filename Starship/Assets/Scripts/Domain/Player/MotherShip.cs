using Galaxy;
using Session;
using UnityEngine;
using Zenject;
using Services.Messenger;

namespace GameServices.Player
{
    public sealed class MotherShip : GameServiceBase
    {
		[Inject] private readonly StarData _starData;
        [Inject] private readonly StarMap _starMap;
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly PlayerSkills _playerSkills;
        [Inject] private readonly IMessenger _messenger;

        [Inject]
        public MotherShip(SessionDataLoadedSignal dataLoadedSignal, SessionCreatedSignal sessionCreatedSignal)
            : base(dataLoadedSignal, sessionCreatedSignal)
        {
        }

        public int Position
        {
            get { return _session.StarMap.PlayerPosition; }
            set { _session.StarMap.PlayerPosition = value; }
        }

		public Galaxy.Star CurrentStar { get { return new Galaxy.Star(Position, _starData); } }

		public ViewMode ViewMode 
		{
			get
			{
				return _viewMode;
			}
			set
			{
				if (_viewMode == value)
					return;

				_viewMode = value;
				_messenger.Broadcast<ViewMode>(EventType.ViewModeChanged, value);
			}		
		}

        public int CalculateRequiredFuel(int star1, int star2)
        {
            var distance = _starMap.Distance(star1, star2);
            return Mathf.Max(1, Mathf.FloorToInt(distance));
        }

        public float CalculateFlightTime(int star1, int star2)
        {
            return _starMap.Distance(star1, star2) / _playerSkills.MainEnginePower;
        }

        public bool IsStarReachable(int starId)
        {
            if (_starData.IsVisited(starId))
                return true;
            var nearestStar = _starMap.GetNearestVisited(starId, true);
            if (nearestStar < 0)
                return false;

            return _starMap.Distance(starId, nearestStar) <= _playerSkills.MainFilghtRange;
        }

        protected override void OnSessionDataLoaded() {}
        protected override void OnSessionCreated() {}

        private ViewMode _viewMode = ViewMode.StarMap;

        public const int FuelMinimum = 100;
    }
}
