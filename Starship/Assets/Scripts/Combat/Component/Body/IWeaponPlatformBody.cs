namespace Combat.Component.Body
{
    public interface IWeaponPlatformBody : IBody
    {
        float FixedRotation { get; }
        float AutoAimingAngle { get; }
        void Aim(float bulletVelocity, float weaponRange, bool relative);
    }
}
