using Combat.Component.Ship;
using Combat.Unit;
using Model;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gui.Combat
{
    public class ShipNavigationPanel : MonoBehaviour, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
    {
        public void Initialize(IShip ship)
        {
            _ship = ship;
            Pressed = false;
        }

        public void OnDrag(PointerEventData data)
        {
            if (Pressed && _touchId == data.pointerId)
                _currentPosition = data.position;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (!Pressed)
            {
                Pressed = true;
                _touchId = data.pointerId;
                _startPosition = _currentPosition = data.position;
                _startDirection = _ship.Body.Rotation;
                //_doubleClicked = Time.realtimeSinceStartup - _clickTime < 0.5f;
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (_touchId == data.pointerId)
                Pressed = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //if (!Pressed)
            //	_clickTime = Time.realtimeSinceStartup;
        }

        public bool AllowThrust { get; set; }

        private void Awake()
        {
            _max = Screen.dpi == 0 ? Screen.height / 10 : Screen.dpi / 2;
            _min = _max / 5;
        }

        private void Update()
        {
            if (!_ship.IsActive())
                return;

            if (!Pressed)
                return;

            if (!AllowThrust /*|| _doubleClicked*/)
            {
                _ship.Controls.Course = RotationHelpers.Angle(_currentPosition - _startPosition);
            }
            else
            {
                if (Vector2.Distance(_currentPosition, _startPosition) > _max)
                {
                    _startPosition = _currentPosition + (_startPosition - _currentPosition).normalized * _max;
                    _startDirection = _ship.Body.Rotation;
                }

                Vector2 direction = _currentPosition - _startPosition;
                var power = direction.magnitude;
                if (power > 1)
                    _ship.Controls.Course = Mathf.LerpAngle(_startDirection, RotationHelpers.Angle(direction), Mathf.Clamp01(2 * power / _max));
                else
                    _ship.Controls.Course = null;

                _ship.Controls.Throttle = Mathf.Abs(Mathf.DeltaAngle(_ship.Controls.Course.GetValueOrDefault(_ship.Body.Rotation), _ship.Body.Rotation)) < 30 ? Mathf.Clamp01((power - _min) / (_max - _min)) : 0;
            }
        }

        private bool Pressed
        {
            get { return _pressed; }
            set
            {
                if (_pressed != value)
                {
                    _pressed = value;
                    if (!_pressed)
                    {
                        _ship.Controls.Course = null;
                        if (AllowThrust)
                            _ship.Controls.Throttle = 0.0f;
                    }
                }
            }
        }

        //private float _clickTime;
        //private bool  _doubleClicked;

        private bool _pressed;
        private int _touchId;
        private Vector2 _startPosition;
        private Vector2 _currentPosition;
        private float _startDirection;

        private float _min;
        private float _max;

        private IShip _ship;
    }
}
