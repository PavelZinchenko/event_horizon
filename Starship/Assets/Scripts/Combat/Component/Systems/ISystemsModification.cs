using System.Collections.Generic;

namespace Combat.Component.Systems
{
    public interface ISystemsModification
    {
        bool IsAlive { get; }
        bool CanActivateSystem(ISystem system);
        void OnSystemActivated(ISystem system);
    }

    public class SystemsModifications
    {
        public void Add(ISystemsModification modification)
        {
            _items.Add(modification);
        }

        public bool CanActivateSystem(ISystem system)
        {
            var count = _items.Count;
            for (var i = 0; i < count; ++i)
            {
                if (!_items[i].CanActivateSystem(system))
                    return false;
            }

            return true;
        }

        public void OnSystemActivated(ISystem system)
        {
            var count = _items.Count;
            for (var i = 0; i < count; ++i)
                _items[i].OnSystemActivated(system);
        }

        public void Cleanup()
        {
            var needCleanup = false;
            var count = _items.Count;
            for (var i = 0; i < count; ++i)
            {
                if (_items[i].IsAlive)
                    continue;
                
                _items[i] = null;
                needCleanup = true;
            }

            if (needCleanup)
                _items.RemoveAll(item => item == null);
        }

        private readonly List<ISystemsModification> _items = new List<ISystemsModification>();
    }
}
