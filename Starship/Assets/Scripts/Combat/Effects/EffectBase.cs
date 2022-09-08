using Combat.Helpers;
using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(Renderer))]
    public abstract class EffectBase : MonoBehaviour, IEffectComponent
    {
        [SerializeField] private float _alphaScale = 1.0f;
        [SerializeField] private Color _defaultColor = Color.white;

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
                    if (value)
                    {
                        gameObject.SetActive(true);
                        Update();
                    }
                }
            }
        }

        public Vector2 Position { get { return _position; } set { _position = value; _positionChanged = true; } }
        public float Rotation { get { return _rotation; } set { _rotation = value; _positionChanged = true; } }
        public float Size { get { return _size; } set { _size = value; _sizeChanged = true; } }
        public Color Color { get { return _color; } set { _color = value; _colorChanged = true; } }
        public float Life { get { return _life; } set { _life = value; _lifeChanged = true; } }

        public void Initialize(GameObjectHolder objectHolder)
        {
            _gameObjectHolder = objectHolder;
            IsAutomatic = false;
            Position = Vector2.zero;
            Rotation = 0;
            Velocity = Vector2.zero;
            AngularVelocity = 0;
            Lifetime = 1.0f;
            Life = 1.0f;
            Size = 1.0f;
            Color = _defaultColor;
            Scale = 1.0f;
            Opacity = 1.0f;
            IsAlive = true;
            gameObject.SetActive(true);

            OnInitialize();
        }

        public bool IsAlive { get; private set; }

        public void Run(float lifetime, Vector2 velocity, float angularVelocity)
        {
            IsAutomatic = true;
            Lifetime = lifetime;
            Velocity = velocity;
            AngularVelocity = angularVelocity;

            Life = 1.0f;
        }

        public void Dispose()
        {
            OnDispose();

            _gameObjectHolder.Dispose();
            IsAlive = false;
        }

        protected float Opacity { get { return _opacity; } set { _opacity = value; _colorChanged = true; } }
        protected float Scale { get { return _scale; } set { _scale = value; _sizeChanged = true; } }

        protected virtual void SetColor(Color color)
        {
            //var spriteRenderer = GetComponent<SpriteRenderer>();
            //if (spriteRenderer != null)
            //    spriteRenderer.color = color;
            //else
                Renderer.material.color = color;
        }

        protected abstract void OnInitialize();
        protected abstract void OnDispose();
        protected abstract void OnGameObjectDestroyed();

        private void OnDestroy()
        {
            if (Renderer)
                foreach (var item in Renderer.materials)
                    GameObject.DestroyImmediate(item);

            OnGameObjectDestroyed();
        }

        protected abstract void UpdateLife();

        protected virtual void UpdatePosition()
        {
            var o = gameObject;
            o.Move(Position);
            o.transform.localEulerAngles = new Vector3(0, 0, Rotation);
        }

        protected virtual void UpdateSize()
        {
            gameObject.transform.localScale = Size * Scale * Vector3.one;
        }

        protected virtual void UpdateColor()
        {
            SetColor(new Color(Color.r, Color.g, Color.b, Color.a * Opacity * _alphaScale));
        }

        protected virtual void OnBeforeUpdate() { }
        protected virtual void OnAfterUpdate() { }

        protected void Update()
        {
            if (!IsAlive)
                return;

            OnBeforeUpdate();

            if (IsAutomatic)
            {
                Position += Velocity * Time.deltaTime;
                Rotation += AngularVelocity * Time.deltaTime;
                Life -= Time.deltaTime / Lifetime;

                if (Life <= 0)
                {
                    Dispose();
                    return;
                }
            }

            if (_lifeChanged)
            {
                UpdateLife();
                _lifeChanged = false;
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

            OnAfterUpdate();
        }

        protected virtual Vector2 Velocity { get; private set; }
        protected virtual float AngularVelocity { get; private set; }

        protected float Lifetime { get; private set; }
        protected bool IsAutomatic { get; private set; }

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

        private GameObjectHolder _gameObjectHolder;
        
        private Renderer _renderer;
        // ReSharper disable once Unity.NoNullCoalescing
        private Renderer Renderer => _renderer ?? (_renderer = GetComponent<Renderer>());
    }
}
