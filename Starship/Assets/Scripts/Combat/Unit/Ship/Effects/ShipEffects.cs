using System.Collections.Generic;

namespace Combat.Component.Ship.Effects
{
    public class ShipEffects : IShipEffects
    {
        public ShipEffects(IShip ship)
        {
            _ship = ship;
        }

        public void UpdatePhysics(float elapsedTime)
        {
            var needCleanup = false;
            var count = _effects.Count;
            for (var index = 0; index < count; ++index)
            {
                var effect = _effects[index];
                effect.UpdatePhysics(_ship, elapsedTime);
                if (!effect.IsAlive)
                {
                    _effects[index] = null;
                    needCleanup = true;
                }
            }

            if (needCleanup)
                _effects.RemoveAll(item => item == null);
        }

        public void UpdateView(float elapsedTime)
        {
            var needCleanup = false;
            var count = _effects.Count;
            for (var index = 0; index < count; ++index)
            {
                var effect = _effects[index];
                effect.UpdateView(_ship, elapsedTime);
                if (!effect.IsAlive)
                {
                    _effects[index] = null;
                    needCleanup = true;
                }
            }

            if (needCleanup)
                _effects.RemoveAll(item => item == null);
        }

        public IList<IShipEffect> All { get { return _effects.AsReadOnly(); } }

        public bool TryAdd(IShipEffect effect)
        {
            if (_effects.Contains(effect))
                return false;

            _effects.Add(effect);
            return true;
        }

        public void Dispose()
        {
            foreach (var effect in _effects)
                effect.Dispose();
        }

        private readonly List<IShipEffect> _effects = new List<IShipEffect>();
        private readonly IShip _ship;
    }
}
