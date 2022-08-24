using Combat.Component.Ship;
using Combat.Component.Unit.Classification;
using Combat.Scene;
using Combat.Unit;

namespace Combat.Ai
{
	public class Computer : IController
	{
		public Computer(IScene scene, IShip ship, int level, bool autopilotMode)
		{
			_ship = ship;
			_level = level;
		    _scene = scene;
		    _autopilotMode = autopilotMode;
            _attackRange = Helpers.ShipMaxRange(_ship);
            _targets = new TargetList(_scene, ship.Type.Side == UnitSide.Player);
            _threats = new ThreatList(_scene);

		    if (autopilotMode)
		        _autoPilotCooldown = AutoPilotDelay;
		}

		public bool IsAlive { get { return _ship.IsActive(); } }

		public void Update(float deltaTime)
		{
		    if (_autopilotMode)
		    {
		        if (_ship.Controls.DataChanged)
		        {
		            _ship.Controls.DataChanged = false;
		            _autoPilotCooldown = AutoPilotDelay;
		        }

		        if (_autoPilotCooldown > 0)
		        {
		            _autoPilotCooldown -= deltaTime;
		            return;
		        }
		    }

		    var enemy = GetEnemy();
			var strategy = GetStrategy();
			if (strategy == null)
			{
				Stop();
				return;
			}

			_threats.Update(deltaTime, _ship, strategy);
		    _targets.Update(deltaTime, _ship, enemy);
			var context = new Context(_ship, enemy, _targets, _threats, _currentTime);

			strategy.Apply(context);
		    _ship.Controls.DataChanged = false;

			_currentTime += deltaTime;
			_enemyUpdateCooldown -= deltaTime;
			_strategyUpdateCooldown -= deltaTime;
		}

		private void Stop()
		{
			_ship.Controls.Throttle = 0;
			_ship.Controls.Course = null;
			_ship.Controls.SystemsState = 0;
		}

		private IStrategy GetStrategy()
		{
		    if (!_enemy.IsActive())
		        return null;//_strategy = _ship.Type.Side == UnitSide.Player ? new CollectLoot() : null;
		    if (_level < 0)
		        return null;

			if (_strategy != null && _strategyUpdateCooldown > 0)
				return _strategy;

			_strategy = StrategySelector./*BestAvailable*/Random(_ship, _enemy, _level, new System.Random(), _scene);
			_strategyUpdateCooldown = StrategyUpdateInterval;

			//UnityEngine.Debug.Log("Strategy: " + _strategy.GetType().Name);
			return _strategy;
		}

		private IShip GetEnemy()
		{
			if (_enemy.IsActive() && _enemyUpdateCooldown > 0)
				return _enemy;

			_enemyUpdateCooldown = EnemyUpdateInterval;

			var newEnemy = _scene.Ships.GetEnemy(_ship, 0, _attackRange, 360, true, true);
			if (newEnemy != _enemy)
				_strategy = null;

			return _enemy = newEnemy;
		}

		private IShip _enemy;
		private float _enemyUpdateCooldown;
		private float _strategyUpdateCooldown;
		private float _currentTime;
	    private float _autoPilotCooldown;
		private IStrategy _strategy;
	    private readonly bool _autopilotMode;
	    private readonly ThreatList _threats;
	    private readonly TargetList _targets;
        private readonly float _attackRange;
		private readonly int _level;
 		private readonly IShip _ship;
	    private readonly IScene _scene;
		private const float EnemyUpdateInterval = 5.0f;
		private const float StrategyUpdateInterval = 10.0f;
	    private const float AutoPilotDelay = 2.0f;

        public class Factory : IControllerFactory
        {
            public Factory(IScene scene, int level, bool autopilotMode = false)
            {
                _scene = scene;
                _level = level;
                _autopilotMode = autopilotMode;
            }

            public IController Create(IShip ship)
            {
                return new Computer(_scene, ship, _level, _autopilotMode);
            }

            private readonly bool _autopilotMode;
            private readonly int _level;
            private readonly IScene _scene;
        }
	}
}
