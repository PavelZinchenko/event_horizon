using System.Linq;
using Combat.Component.Ship;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Systems.DroneBays
{
    public interface IDroneReplicator : ISystem
    {
        void Start();
    }

    public class DroneReplicator : SystemBase, IDroneReplicator
    {
        public DroneReplicator(IShip ship, float replicationSpeed)
            : base(-1, SpriteId.Empty)
        {
            _ship = ship;
            _speed = replicationSpeed;
            _cooldown = 1.0f / _speed;
        }

        public void Start()
        {
            if (_cooldown < 0)
                _cooldown = 1.0f / _speed;
        }

        public override bool CanBeActivated { get { return false; } }
        public override float Cooldown { get { return Mathf.Clamp01(_cooldown * _speed); } }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (_cooldown < 0)
                return;

            _cooldown -= elapsedTime;
            if (_cooldown > 0)
                return;

            _ship.Systems.All.OfType<IDroneBay>().Any(droneBay => droneBay.TryRestoreDrone());
        }

        protected override void OnUpdateView(float elapsedTime) { }
        protected override void OnDispose() {}

        private float _cooldown;
        private readonly IShip _ship;
        private readonly float _speed;
    }
}
