using System.Collections.Generic;
using Combat.Component.Body;
using Combat.Component.Bullet;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Unit.HitPoints;
using Gui.Utils;
using UnityEngine;

namespace Combat.Component.Platform
{
    public sealed class FixedPlatform : IWeaponPlatform, IUnitAction
    {
        public FixedPlatform(IShip ship, IBody body, float cooldown, UnitBase parent, IAimingSystem aimingSystem = null)
        {
            _body = body;
            _ship = ship;
            _cooldown = cooldown;
            _aimingSystem = aimingSystem;
            parent.AddTrigger(this);
        }

        public UnitType Type { get { return _ship.Type; } }
        public IBody Body { get { return _body; } }
        public IResourcePoints EnergyPoints { get { return _ship.Stats.Energy; } }
        public bool IsTemporary { get { return false; } }

        public bool IsReady { get { return _timeFromLastShot > _cooldown; } }
        public float Cooldown { get { return Mathf.Clamp01(1f - _timeFromLastShot / _cooldown); } }

        public float FixedRotation { get { return _body.WorldRotation(); } }
        public float AutoAimingAngle { get { return 0; } }

        public void SetView(IView view, UnityEngine.Color color)
        {
            _view = view;
            _color = color;
        }

        public void Aim(float bulletVelocity, float weaponRange, bool relative)
        {
            if (_aimingSystem != null)
                _aimingSystem.Aim(bulletVelocity, weaponRange, relative);
        }

        public void OnShot()
        {
            _timeFromLastShot = 0;
        }

        public void UpdatePhysics(float elapsedTime)
        {
            _timeFromLastShot += elapsedTime;
            if (_timeFromLastCleanup >= CleanupInterval)
            {
                _attachedChildren.RetainAlive();
                _timeFromLastCleanup = 0;
            }
        }

        public void UpdateView(float elapsedTime)
        {
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

        public void Dispose() {}

        private IView _view;
        private Color _color;
        private float _timeFromLastCleanup;
        private const float CleanupInterval = 1;
        private float _timeFromLastShot;
        private readonly IShip _ship;
        private readonly float _cooldown;
        private readonly IBody _body;
        private readonly IAimingSystem _aimingSystem;
        private readonly IList<WeakReference<IBullet>> _attachedChildren = new List<WeakReference<IBullet>>();

        public ConditionType TriggerCondition => ConditionType.OnDestroy;

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
