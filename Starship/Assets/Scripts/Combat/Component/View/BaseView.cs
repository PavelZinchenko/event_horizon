using System;
using UnityEngine;

namespace Combat.Component.View
{
    public abstract class BaseView : MonoBehaviour, IView
    {
        public virtual void ApplyHsv(float hue, float saturation) { throw new NotImplementedException(); }

        public Vector2 Position { get { return _position; } set { _position = value; _positionChanged = true; } }
        public float Rotation { get { return _rotation; } set { _rotation = value; _rotationChanged = true; } }
        public float Size { get { return _size; } set { _size = value; _sizeChanged = true; } }
        public Color Color { get { return _color; } set { _color = value; _colorChanged = true; } }
        public float Life { get { return _life; } set { _life = value; _lifeChanged = true; } }

        public virtual void UpdateView(float elapsedTime)
        {
            if (_lifeChanged)
            {
                UpdateLife(Life);
                _lifeChanged = false;
            }

            if (_positionChanged)
            {
                UpdatePosition(Position);
                _positionChanged = false;
            }

            if (_rotationChanged)
            {
                UpdateRotation(Rotation);
                _rotationChanged = false;
            }

            if (_sizeChanged)
            {
                UpdateSize(Size * Scale);
                _sizeChanged = false;
            }

            if (_colorChanged)
            {
                UpdateColor(new Color(Color.r, Color.g, Color.b, Color.a * Opacity));
                _colorChanged = false;
            }
        }

        public abstract void Dispose();
        protected abstract void OnGameObjectCreated();
        protected abstract void OnGameObjectDestroyed();

        protected float Opacity { get { return _opacity; } set { _opacity = value; _colorChanged = true; } }
        protected float Scale { get { return _scale; } set { _scale = value; _sizeChanged = true; } }

        protected abstract void UpdateLife(float life);
        protected abstract void UpdatePosition(Vector2 position);
        protected abstract void UpdateRotation(float rotation);
        protected abstract void UpdateSize(float size);
        protected abstract void UpdateColor(Color color);

        private void Awake()
        {
            Position = transform.localPosition;
            Rotation = transform.localEulerAngles.z;
            Size = 1.0f;
            Color = Color.white;
            Life = 1.0f;
            Opacity = 1.0f;
            Scale = 1.0f;

            OnGameObjectCreated();
        }

        private void OnDestroy()
        {
            OnGameObjectDestroyed();
        }

        private Vector2 _position;
        private bool _positionChanged;

        private float _rotation;
        private bool _rotationChanged;

        private float _size;
        private float _scale;
        private bool _sizeChanged;

        private float _opacity;
        private Color _color;
        private bool _colorChanged;

        private float _life;
        private bool _lifeChanged;
    }
}
