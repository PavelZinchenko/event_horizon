namespace Combat.Component.Controls
{
    public class CommonControls : IControls
    {
        public bool DataChanged { get; set; }

        public float Throttle
        {
            get { return _throttle; }
            set
            {
                _throttle = value;
                DataChanged = true;
            }
        }

        public float? Course
        {
            get
            {
                return _course;
            }
            set
            {
                _course = value;
                DataChanged = true;
            }
        }

        public void SetSystemState(int id, bool active)
        {
            if (id < 0 || id >= 64) return;

            var key = 1UL << id;
            _systems |= key;
            if (!active)
                _systems ^= key;

            DataChanged = true;
        }

        public bool GetSystemState(int id)
        {
            if (id < 0 || id >= 64) return false;

            return (_systems & (1UL << id)) != 0;
        }

        public ulong SystemsState
        {
            get
            {
                return _systems;
            }
            set
            {
                _systems = value;
                DataChanged = true;
            }
        }

        private ulong _systems;
        private float _throttle;
        private bool _hasCourse;
        private float? _course;
    }
}
