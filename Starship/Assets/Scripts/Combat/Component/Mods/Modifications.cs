using System.Collections.Generic;

namespace Combat.Component.Mods
{
    public class Modifications<T> where T : struct 
    {
        public void Add(IModification<T> modification)
        {
            _items.Add(modification);
        }

        public void Apply(ref T data)
        {
            var needCleanup = false;
            var count = _items.Count;
            for (var i = 0; i < count; ++i)
            {
                if (_items[i].TryApplyModification(ref data))
                    continue;

                _items[i] = null;
                needCleanup = true;
            }

            if (needCleanup)
                _items.RemoveAll(item => item == null);
        }

        private readonly List<IModification<T>> _items = new List<IModification<T>>();
    }
}
