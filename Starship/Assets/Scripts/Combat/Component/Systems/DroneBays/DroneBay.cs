using System.Collections.Generic;
using Combat.Component.Body;
using Combat.Component.Platform;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Factory;
using Combat.Unit;
using Constructor;
using GameDatabase.DataModel;
using GameDatabase.Model;

namespace Combat.Component.Systems.DroneBays
{
    public class DroneBay : SystemBase, IDroneBay
    {
        public DroneBay(IWeaponPlatform platform, IShip mothership, IDroneBayData data, ShipFactory shipFactory, ShipSettings shipSettings, IDroneReplicator replicator)
            : base(data.KeyBinding, data.DroneBay.ControlButtonIcon)
        {
            _shipFactory = shipFactory;
            _platform = platform;
            _mothership = mothership;
            _replicator = replicator;

            var stats = data.DroneBay;

            _dronesLeft = _capacity = stats.Capacity;
            _energyCost = stats.EnergyConsumption;
            _range = stats.Range;
            _behaviour = data.Behaviour;
            _improvedAi = data.DroneBay.ImprovedAi;

            var random = new System.Random();
            var builder = new ShipBuilder(data.Drone);
            builder.Bonuses.ArmorPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier);
            builder.Bonuses.ShieldPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier);
            builder.Bonuses.DamageMultiplier = StatMultiplier.FromValue(stats.DamageMultiplier);
            builder.Bonuses.VelocityMultiplier = StatMultiplier.FromValue(stats.SpeedMultiplier + (random.NextFloat() - 0.5f) * 0.4f);
            _shipSpec = builder.Build(shipSettings);
        }

        public override bool CanBeActivated { get { return _dronesLeft > 0 && _platform.IsReady && _platform.EnergyPoints.Value >= _energyCost; } }
        
        public override float Cooldown
        {
            get
            {
                if (_dronesLeft > 0)
                    return _platform.Cooldown;
                else if (_drones.Count >= _capacity)
                    return 1.0f;
                else if (_replicator != null)
                    return _replicator.Cooldown;
                else
                    return 1.0f;
            }
        }

        public float Range { get { return _range; } }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && _platform.EnergyPoints.TryGet(_energyCost))
            {
                _platform.OnShot();
                CreateDrone();
                InvokeTriggers(ConditionType.OnActivate);
            }

            _refreshCooldown -= elapsedTime;
            if (_refreshCooldown < 0)
            {
                _refreshCooldown = _refreshInterval;
                _drones.RemoveAll(item => !item.IsActive());
                var count = _drones.Count;
                if (count + _dronesLeft < _capacity && _replicator != null)
                    _replicator.Start();
            }
        }

        protected override void OnUpdateView(float elapsedTime) {}

        public bool TryRestoreDrone()
        {
            if (_drones.Count + _dronesLeft >= _capacity)
                return false;

            _dronesLeft++;
            return true;
        }

        protected override void OnDispose() {}

        private void CreateDrone()
        {
            if (_dronesLeft > 0)
                _dronesLeft--;

            var model = _shipFactory.CreateDrone(_shipSpec, _mothership, _range, _platform.Body.WorldPosition(), _platform.Body.WorldRotation(), _behaviour, _improvedAi);
            _drones.Add(model);
        }

        private int _dronesLeft;
        private float _refreshCooldown;
        private readonly bool _improvedAi;
        private readonly float _energyCost;
        private readonly int _capacity;
        private readonly List<IShip> _drones = new List<IShip>();
        private readonly IWeaponPlatform _platform;
        private readonly IShipSpecification _shipSpec;
        private readonly ShipFactory _shipFactory;
        private readonly IShip _mothership;
        private readonly float _range;
        private readonly DroneBehaviour _behaviour;
        private readonly IDroneReplicator _replicator;
        private const float _refreshInterval = 0.5f;
    }
}
