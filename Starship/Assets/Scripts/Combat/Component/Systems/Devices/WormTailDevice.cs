using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Combat.Collision;
using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Component.Stats;
using Combat.Component.Unit;
using GameDatabase.DataModel;
using GameDatabase.Model;
using UnityEditor.MemoryProfiler;

namespace Combat.Component.Systems.Devices
{
    public class WormTailDevice : SystemBase, IDevice, IStatsModification
    {
        public WormTailDevice(IShip ship, DeviceStats deviceSpec, IEnumerable<WormSegment> wormTail)
            : base(-1, SpriteId.Empty)
        {
            _units = new List<WormSegment>(wormTail);
            _size = _units.Count;
            AddDependants(ship.Body, _units);
        }

        public override float ActivationCost { get { return 0f; } }
        public override bool CanBeActivated { get { return false; } }

        public override IStatsModification StatsModification { get { return this; } }
        public bool TryApplyModification(ref Resistance data)
        {
            var power = _units.Count / (float)_size;

            data.Heat = power + (1f - power) * data.Heat;
            data.Energy = power + (1f - power) * data.Energy;
            data.Kinetic = power + (1f - power) * data.Kinetic;

            return true;
        }

        public void Deactivate() {}

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            _updateCooldown -= elapsedTime;
            if (_updateCooldown > 0)
                return;

            _updateCooldown = 0.5f;
            _units.RemoveAll(item => !item.Enabled);
        }

        protected override void OnUpdateView(float elapsedTime)
        {
        }

        protected override void OnDispose() { }


        private static void AddDependants(IBody parent, IEnumerable<WormSegment> segments)
        {
            if (!Dependencies.TryGetValue(parent, out var dependants))
            {
                dependants = new List<WeakReference<IBody>>();
                Dependencies.Add(parent, dependants);
            }
            foreach (var wormSegment in segments)
            {
                dependants.Add(new WeakReference<IBody>(wormSegment.Body));
            }
        }
        
        // TODO: This is a dirty hack to avoid rework of the whole body system
        // This should be converted in an actual parameter of the body system later on
        public static readonly ConditionalWeakTable<IBody, IList<WeakReference<IBody>>> Dependencies =
            new ConditionalWeakTable<IBody, IList<WeakReference<IBody>>>();
        private float _updateCooldown;
        private readonly int _size;
        private readonly List<WormSegment> _units;
    }
}
