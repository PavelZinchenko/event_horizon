namespace Combat.Component.Platform
{
    public interface IAimingSystem
    {
        void Aim(float bulletVelocity, float weaponRange, bool relative);
    }
}
