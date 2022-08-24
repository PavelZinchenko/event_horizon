using System.Collections.Generic;
using Combat.Component.Body;
using Combat.Factory;
using Combat.Scene;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using UnityEngine;

namespace Combat.Effects
{
    public class CompositeEffect : MonoBehaviour, IEffect
    {
        public static CompositeEffect Create(VisualEffect effectData, EffectFactory factory, IBody parent)
        {
            var gameObject = new GameObject(effectData.Id.ToString());

            if (parent == null)
            {
                gameObject.transform.localPosition = new Vector3(0, 0, -3);
            }
            else
            {
                parent.AddChild(gameObject.transform);
                gameObject.transform.localPosition = new Vector3(parent.Offset, 0, 0);
            }

            var composite = gameObject.AddComponent<CompositeEffect>();
            composite.Initialize(parent, effectData, factory);

            return composite;
        }

        public void Detach()
        {
            gameObject.transform.parent = null;
            gameObject.transform.localPosition = new Vector3(0, 0, -3);
            Position = _parent.WorldPosition();
            Rotation = _parent.WorldRotation();
            _parent = null;
        }

        public bool Visible
        {
            get { return gameObject.activeSelf; }
            set
            {
                if (gameObject.activeSelf)
                {
                    if (!value) gameObject.SetActive(false);
                }
                else
                {
                    if (value) gameObject.SetActive(true);
                }
            }
        }

        public Vector2 Position { get { return _position; } set { _position = value; _positionChanged = true; } }
        public float Rotation { get { return _rotation; } set { _rotation = value; _positionChanged = true; } }
        public float Size { get { return _size; } set { _size = value; _sizeChanged = true; } }
        public Color Color { get { return _color; } set { _color = value; _colorChanged = true; } }
        public float Life { get { return _life; } set { _life = value; _lifeChanged = true; } }

        public bool IsAlive { get; private set; }

        public void Run(float lifetime, Vector2 velocity, float angularVelocity)
        {
            Life = 1.0f;
            _lifetime = lifetime > 0.01f ? lifetime : _lifetimeMax;
            _velocity = velocity;
            _angularVelocity = angularVelocity;
            _isAutomatic = true;
        }

        public void Initialize(IBody parent, VisualEffect effectData, EffectFactory factory)
        {
            _data = effectData;
            _parent = parent;

            _lifetimeMax = 0f;

            foreach (var element in effectData.Elements)
            {
                var effect = factory.CreateEffect(element, gameObject.transform);
                effect.Visible = false;
                effect.Color = element.Color;
                effect.Size = element.Size;
                _effects.Add(effect);

                var lifetime = element.Lifetime + element.StartTime;
                if (_lifetimeMax < lifetime)
                    _lifetimeMax = lifetime;
            }

            Position = Vector2.zero;
            Rotation = 0;
            Life = 1.0f;
            Size = 1.0f;
            IsAlive = true;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!IsAlive)
                return;

            if (_isAutomatic)
            {
                Position += _velocity * Time.deltaTime;
                Rotation += _angularVelocity * Time.deltaTime;
                Life -= Time.deltaTime / _lifetime;

                if (Life <= 0)
                {
                    Dispose();
                    return;
                }
            }

            if (_positionChanged)
            {
                UpdatePosition();
                _positionChanged = false;
            }

            if (_sizeChanged)
            {
                UpdateSize();
                _sizeChanged = false;
            }

            if (_colorChanged)
            {
                UpdateColor();
                _colorChanged = false;
            }

            if (_lifeChanged)
            {
                UpdateLife();
                _lifeChanged = false;
            }
        }

        private void UpdateLife()
        {
            for (var i = 0; i < _effects.Count; ++i)
            {
                var data = _data.Elements[i];
                var effect = _effects[i];

                var time = (1f - Life) * _lifetimeMax;
                if (time <= data.StartTime || time >= data.StartTime + data.Lifetime)
                {
                    effect.Visible = false;
                    continue;
                }

                effect.Visible = true;
                effect.Life = 1f - (time - data.StartTime) / data.Lifetime;
            }
        }

        private void UpdatePosition()
        {
            if (_parent != null)
                gameObject.Move(new Vector2(Position.x + _parent.Offset, Position.y));
            else
                gameObject.Move(Position);

            gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
        }

        private void UpdateSize()
        {
            gameObject.transform.localScale = Size * Vector3.one;
        }

        private void UpdateColor()
        {
            for (var i = 0; i < _effects.Count; ++i)
            {
                var data = _data.Elements[i];
                var effect = _effects[i];
                effect.Color = data.ColorMode.Apply(data.Color, Color);
            }
        }

        public void Dispose()
        {
            gameObject.transform.localScale = Vector3.one;

            foreach (var effect in _effects)
                effect.Dispose();

            Destroy(gameObject);
            IsAlive = false;
        }

        private IBody _parent;

        private Vector2 _position;
        private float _rotation;
        private bool _positionChanged;

        private float _size;
        private float _scale;
        private bool _sizeChanged;

        private float _opacity;
        private Color _color;
        private bool _colorChanged;

        private float _life;
        private bool _lifeChanged;

        private Vector2 _velocity;
        private float _angularVelocity;
        private float _lifetime;
        private bool _isAutomatic;

        private float _lifetimeMax;
        private VisualEffect _data;
        private readonly List<IEffect> _effects = new List<IEffect>();
    }
}
