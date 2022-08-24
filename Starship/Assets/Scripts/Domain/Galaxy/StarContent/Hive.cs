using Combat.Component.Unit.Classification;
using Combat.Domain;
using Game.Exploration;
using GameDatabase;
using GameServices.Economy;
using GameServices.Player;
using GameServices.Random;
using GameStateMachine.States;
using Model.Factories;
using Session;
using Zenject;

namespace Galaxy.StarContent
{
    public class Hive
    {
        [Inject] private readonly ISessionData _session;
        [Inject] Game.Exploration.Planet.Factory _factory;

        public bool IsDefeated(int starId)
        {
            if (_planet == null || _planet.StarId != starId)
                _planet = _factory.Create(starId, Planet.InfectedPlanetId);

            return _planet.ObjectivesExplored >= _planet.TotalObjectives;
        }

        private Planet _planet;

        public struct Facade
        {
            public Facade(Hive hive, int starId)
            {
                _hive = hive;
                _starId = starId;
            }

            public bool IsDefeated { get { return _hive.IsDefeated(_starId); } }

            private readonly Hive _hive;
            private readonly int _starId;
        }
    }
}
