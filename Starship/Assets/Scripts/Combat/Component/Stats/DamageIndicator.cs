using Combat.Collision;
using Combat.Component.Unit;
using Combat.Factory;
using UnityEngine;

namespace Combat.Component.Stats
{
    public class DamageIndicator : IDamageIndicator
    {
        public DamageIndicator(IUnit unit, EffectFactory effectFactory, float opacity = 1.0f)
        {
            _effectFactory = effectFactory;
            _unit = unit;
            _kineticDamageColor = new Color(0.5f, 1.0f, 0.5f, opacity);
            _heatDamageColor = new Color(1, 1, 0, opacity);
            _energyDamageColor = new Color(0, 1, 1, opacity);
            _directDamageColor = new Color(1, 0.5f, 1, opacity);
            _shieldDamageColor = new Color(0.5f, 0.5f, 0.5f, opacity);
        }

        public void ApplyDamage(Impact damage)
        {
            if (damage.KineticDamage > 0)
            {
                _kineticChanged = true;
                _kinetic += damage.KineticDamage;
            }

            if (damage.HeatDamage > 0)
            {
                _heatChanged = true;
                _heat += damage.HeatDamage;
            }

            if (damage.EnergyDamage > 0)
            {
                _energyChanged = true;
                _energy += damage.EnergyDamage;
            }

            if (damage.DirectDamage > 0)
            {
                _directChanged = true;
                _direct += damage.DirectDamage;
            }

            if (damage.ShieldDamage > 0)
            {
                _shieldChanged = true;
                _shield += damage.ShieldDamage;
            }
        }

        public void Dispose()
        {
            if (_kinetic > 1f)
                CreateDamageEffect(_kinetic, _kineticDamageColor);
            if (_heat > 1f)
                CreateDamageEffect(_heat, _heatDamageColor);
            if (_energy > 1f)
                CreateDamageEffect(_energy, _energyDamageColor);
            if (_direct > 1f)
                CreateDamageEffect(_direct, _directDamageColor);
            if (_shield > 1f)
                CreateDamageEffect(_shield, _shieldDamageColor);
        }

        public void Update(float elapsedTime)
        {
            _currentTime += elapsedTime;
            if (_currentTime - _lastShowTime > _cooldown)
            {
                _lastShowTime = _currentTime;

                if (_kinetic > 1f && !_kineticChanged)
                {
                    CreateDamageEffect(_kinetic, _kineticDamageColor);
                    _kinetic = 0;
                }
                if (_heat > 1f && !_heatChanged)
                {
                    CreateDamageEffect(_heat, _heatDamageColor);
                    _heat = 0;
                }
                if (_energy > 1f && !_energyChanged)
                {
                    CreateDamageEffect(_energy, _energyDamageColor);
                    _energy = 0;
                }
                if (_direct > 1f && !_directChanged)
                {
                    CreateDamageEffect(_direct, _directDamageColor);
                    _direct = 0;
                }
                if (_shield > 1f && !_shieldChanged)
                {
                    CreateDamageEffect(_shield, _shieldDamageColor);
                    _shield = 0;
                }
            }

            _kineticChanged = false;
            _heatChanged = false;
            _energyChanged = false;
            _directChanged = false;
            _shieldChanged = false;
        }

        private void CreateDamageEffect(float damage, Color color)
        {
            _effectFactory.CreateDamageTextEffect(damage, color, _unit.Body.Position, _unit.Body.Velocity);
        }

        private float _kinetic;
        private float _heat;
        private float _energy;
        private float _direct;
        private float _shield;

        private bool _kineticChanged;
        private bool _heatChanged;
        private bool _energyChanged;
        private bool _directChanged;
        private bool _shieldChanged;

        private float _lastShowTime;
        private float _currentTime;

        private const float _cooldown = 0.2f;

        private readonly Color _kineticDamageColor;
        private readonly Color _heatDamageColor;
        private readonly Color _energyDamageColor;
        private readonly Color _directDamageColor;
        private readonly Color _shieldDamageColor;

        private readonly EffectFactory _effectFactory;
        private readonly IUnit _unit;
    }
}
