using GameServices.Player;
using Session;
using Zenject;

namespace Galaxy.StarContent
{
    public class Wormhole
    {
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly StarData _starData;

        public int GetTarget(int starId)
        {
            return _session.Wormholes.GetTarget(starId);
        }

        public void Enter(int starId)
        {
            var target = GetTarget(starId);
            if (target < 0)
            {
                target = FindTarget(starId);
                _session.Wormholes.SetTarget(starId, target);
            }

            _motherShip.ViewMode = ViewMode.StarMap;
            _motherShip.Position = target;
        }

        private int FindTarget(int starId)
        {
            for (var index = starId + 1; index < starId + 1000; ++index)
                if (_starData.GetObjects(index).Contain(StarObjectType.Wormhole) && GetTarget(index) < 0)
                    return index;

            throw new System.InvalidOperationException();
        }

        public struct Facade
        {
            public Facade(Wormhole wormhole, int starId)
            {
                _wormhole = wormhole;
                _starId = starId;
            }

            public int Target { get { return _wormhole.GetTarget(_starId); } }
            public void Enter() { _wormhole.Enter(_starId); }

            private readonly Wormhole _wormhole;
            private readonly int _starId;
        }
    }
}
