namespace Combat.Component.Bullet.Lifetime
{
    public class Lifetime : ILifetime
    {
        public Lifetime(float lifetime)
        {
            _lifetime = lifetime;
        }

        public void Restore()
        {
            _elapsed = 0;
        }

        public float Value { get { return _lifetime <= 0 ? 0 : _elapsed < _lifetime ? 1.0f - _elapsed/_lifetime : 0.0f; } }
        public bool IsExpired { get { return _elapsed > _lifetime; } }

        public void Update(float elapsedTime)
        {
            _elapsed += elapsedTime;
        }

        private float _elapsed;
        private readonly float _lifetime;
    }
}
