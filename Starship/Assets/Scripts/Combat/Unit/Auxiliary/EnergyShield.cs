using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.DamageHandler;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Unit;
using Combat.Unit.Auxiliary;
using UnityEngine;

namespace Combat.Component.Unit
{
    public class EnergyShield : UnitBase, IAuxiliaryUnit
    {
        public EnergyShield(IShip parent, IBody body, IView view, ICollider collider, float defaultOpacity = 0.5f)
            : base(new UnitType(UnitClass.Shield, parent.Type.Side, parent), body, view, collider, null)
        {
            _defaultColor = _activeColor = view.Color;
            _defaultColor.a *= defaultOpacity;
            Collider.Enabled = _isEnabled = false;
        }

        public override ICollisionBehaviour CollisionBehaviour { get { return null; } }

        public override UnitState State { get { return _state; } }

        public IDamageHandler DamageHandler { get; set; }

        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData)
        {
            _timeFromLastHit = 0;
            DamageHandler.ApplyDamage(impact);
        }

        public bool Active { get; set; }

        public bool Enabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value)
                    return;

                Collider.Enabled = value;

                _isEnabled = value;
                InvokeTriggers(_isEnabled ? ConditionType.OnActivate : ConditionType.OnDeactivate);
            }
        }

        public override void Vanish()
        {
            _state = UnitState.Inactive;
        }

        public void Destroy()
        {
            _state = UnitState.Destroyed;
        }

        protected override void OnUpdateView(float elapsedTime)
        {
            var color = Color.Lerp(_disabledColor, _timeFromLastHit < 1.0f ? Color.Lerp(_activeColor, _defaultColor, _timeFromLastHit) : _defaultColor, _power);
            View.Color = color;
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            _timeFromLastHit += elapsedTime;
            _power = Mathf.Lerp(_power, _isEnabled ? 1.0f : 0.0f, 4 * elapsedTime);
        }

        protected override void OnDispose()
        {
            if (DamageHandler != null)
                DamageHandler.Dispose();
        }

        private float _timeFromLastHit = 100f;
        private float _power;
        private bool _isEnabled;
        private UnitState _state = UnitState.Active;

        private readonly Color _defaultColor;
        private readonly Color _activeColor;
        private static readonly Color _disabledColor = new Color(0, 0, 0, 0);
    }
}
