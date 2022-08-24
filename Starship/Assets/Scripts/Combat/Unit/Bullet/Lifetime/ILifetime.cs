namespace Combat.Component.Bullet.Lifetime
{
    public interface ILifetime
    {
        void Restore();
        float Value { get; }
        bool IsExpired { get; }

        void Update(float elapsedTime);
    }
}
