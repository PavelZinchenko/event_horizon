using Combat.Component.View;
using Combat.Helpers;
using UnityEngine;

namespace Combat.Effects
{
    public class ViewEffect : MonoBehaviour, IEffectComponent
    {
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
        public float Size { get { return _view.Size; } set { _view.Size = value; } }
        public Color Color { get { return _view.Color; } set { _view.Color = value; } }
        public float Life { get { return _view.Life; } set { _view.Life = value; } }

        public void Initialize(GameObjectHolder objectHolder)
        {
            _view = GetComponent<IView>();
            _gameObjectHolder = objectHolder;
            _isAutomatic = false;
            _lifetime = 1.0f;
            _velocity = Vector2.zero;
            _angularVelocity = 0;
            Position = Vector2.zero;
            Rotation = 0;
            IsAlive = true;
            objectHolder.Transform.localScale = Vector3.one;
            objectHolder.IsActive = true;
        }

        public bool IsAlive { get; private set; }

        public void Run(float lifetime, Vector2 velocity, float angularVelocity)
        {
            _isAutomatic = true;
            _lifetime = lifetime;
            _velocity = velocity;
            _angularVelocity = angularVelocity;

            Life = 1.0f;
        }

        public void Dispose()
        {
            _gameObjectHolder.Dispose();
            IsAlive = false;
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

            _view.UpdateView(Time.deltaTime);
        }

        private void UpdatePosition()
        {
            gameObject.transform.localPosition = Position;
            gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
        }

        private bool _isAutomatic;
        private Vector2 _velocity;
        private float _angularVelocity;
        private float _lifetime;

        private Vector2 _position;
        private float _rotation;
        private bool _positionChanged;

        private IView _view;
        private GameObjectHolder _gameObjectHolder;
    }
}
