using System.Collections.Generic;
using Combat.Component.Body;
using Combat.Component.Bullet;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Scene;
using Combat.Unit.HitPoints;
using Gui.Utils;
using UnityEngine;

namespace Combat.Component.Platform
{
    public sealed class AutoAimingPlatform : IWeaponPlatform, IUnitAction
    {
        public AutoAimingPlatform(IShip ship, UnitBase parent, IScene scene, Vector2 position, float rotation, float offset, float maxAngle, float cooldown, float rotationSpeed)
        {
            _body = WeaponPlatformBody.Create(scene, parent, position, rotation, offset, maxAngle, rotationSpeed);
            _cooldown = cooldown;
            _ship = ship;
            parent.AddTrigger(this);
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
            _timeFromLastCleanup += elapsedTime;
            if (_timeFromLastCleanup >= CleanupInterval)
            {
                _attachedChildren.RetainAlive();
                _timeFromLastCleanup = 0;
            }
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
        public void AddAttachedChild(IBullet bullet)
        {
            _attachedChildren.Add(new WeakReference<IBullet>(bullet));
        }

        public void Dispose() { }

        private IView _view;
        private Color _color;
        private float _timeFromLastCleanup;
        private const float CleanupInterval = 1;
        private float _timeFromLastShot;
        private readonly float _cooldown;
        private readonly IWeaponPlatformBody _body;
        private readonly IShip _ship;
        public ConditionType TriggerCondition => ConditionType.OnDestroy;
        private readonly IList<WeakReference<IBullet>> _attachedChildren = new List<WeakReference<IBullet>>();

        public bool TryUpdateAction(float elapsedTime)
        {
            return false;
        }

        public bool TryInvokeAction(ConditionType condition)
        {
            foreach (var child in _attachedChildren)
            {
                if(child.IsAlive)
                {
                    child.Target.Detonate();  
                }
            }

            return true;
        }
    }
}
