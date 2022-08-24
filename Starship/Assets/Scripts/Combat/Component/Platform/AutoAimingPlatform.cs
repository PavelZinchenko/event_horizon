using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Scene;
using Combat.Unit.HitPoints;
using UnityEngine;

namespace Combat.Component.Platform
{
    public sealed class AutoAimingPlatform : IWeaponPlatform
    {
        public AutoAimingPlatform(IShip ship, IUnit parent, IScene scene, Vector2 position, float rotation, float offset, float maxAngle, float cooldown, float rotationSpeed)
        {
            _body = WeaponPlatformBody.Create(scene, parent, position, rotation, offset, maxAngle, rotationSpeed);
            _cooldown = cooldown;
            _ship = ship;
        }

        public UnitType Type { get { return _ship.Type; } }
        public IBody Body { get { return _body; } }
        public IResourcePoints EnergyPoints { get { return _ship.Stats.Energy; } }
        public bool IsTemporary { get { return false; } }

        public bool IsReady { get { return _timeFromLastShot > _cooldown; } }
        public float Cooldown { get { return Mathf.Clamp01(1f - _timeFromLastShot / _cooldown); } }

        public float FixedRotation { get { return _body.FixedRotation; } }
        public float AutoAimingAngle { get { return _body.AutoAimingAngle; } }

        public void SetView(IView view, UnityEngine.Color color)
        {
            _view = view;
            _color = color;
        }
        
        public void Aim(float bulletVelocity, float weaponRange, bool relative)
        {
            _body.Aim(bulletVelocity, weaponRange, relative);
        }

        public void OnShot()
        {
            _timeFromLastShot = 0;
        }

        public void UpdatePhysics(float elapsedTime)
        {
            _body.UpdatePhysics(elapsedTime);
            _timeFromLastShot += elapsedTime;
        }

        public void UpdateView(float elapsedTime)
        {
            _body.UpdateView(elapsedTime);

            if (_view != null)
            {
                _view.Color = _color * _ship.Features.Color;
                _view.UpdateView(elapsedTime);
            }
        }

        public void Dispose() { }

        private IView _view;
        private Color _color;
        private float _timeFromLastShot;
        private readonly float _cooldown;
        private readonly IWeaponPlatformBody _body;
        private readonly IShip _ship;
    }
}
