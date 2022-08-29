using System;
using Combat.Component.Body;
using Combat.Component.Bullet;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Unit.HitPoints;
using Constructor.Model;

namespace Combat.Component.Platform
{
    public interface IWeaponPlatform : IDisposable
    {
        [Obsolete] UnitType Type { get; }
        IBody Body { get; }
        IResourcePoints EnergyPoints { get; }
        bool IsTemporary { get; }

        float FixedRotation { get; } // TODO: remove

        bool IsReady { get; }
        float Cooldown { get; }
        float AutoAimingAngle { get; }

        void Aim(float bulletVelocity, float weaponRange, bool relative);
        void OnShot();
        void SetView(IView view, UnityEngine.Color color);

        void UpdatePhysics(float elapsedTime);
        void UpdateView(float elapsedTime);

        void AddAttachedChild(IBullet bullet);
    }
}
