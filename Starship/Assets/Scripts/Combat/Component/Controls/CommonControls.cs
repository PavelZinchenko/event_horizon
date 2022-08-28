using System.Collections;
using Combat.Component.Ship;

namespace Combat.Component.Controls
{
    public class CommonControls : IControls
    {
        private float? _course;
        private bool _hasCourse;

        private BitArray _systems = new BitArray(0);
        private float _throttle;

        public CommonControls(IShip ship)
        {
            _systems = new BitArray(ship.Systems.All.Count);
        }

        public bool DataChanged { get; set; }

        public float Throttle
        {
            get => _throttle;
            set
            {
                _throttle = value;
                DataChanged = true;
            }
        }

        public float? Course
        {
            get => _course;
            set
            {
                _course = value;
                DataChanged = true;
            }
        }

        public void SetSystemState(int id, bool active)
        {
            if (id < 0) return;
            _systems[id] = active;

            DataChanged = true;
        }

        public bool GetSystemState(int id)
        {
            if (id < 0) return false;
            return _systems[id];
        }

        public BitArray SystemsState
        {
            get => _systems;
            set
            {
                _systems = value;
                DataChanged = true;
            }
        }
    }
}
