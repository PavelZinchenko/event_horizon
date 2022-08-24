using System.Collections.Generic;
using Combat.Component.Ship;
using Combat.Unit;

namespace Combat.Ai
{
    public class KeyboardController : IController
    {
        public KeyboardController(IShip ship, IKeyboard keyboard)
        {
            _ship = ship;
            _keyBindings = ship.Systems.All.GetKeyBindings();
            _keyboard = keyboard;
        }

        public bool IsAlive { get { return _ship.IsActive(); } }

        public void Update(float deltaTime)
        {
            var controls = _ship.Controls;
            var currentRotation = _ship.Body.Rotation;

            if (_keyboard.Throttle)
            {
                _throttle = true;
                controls.Throttle = 1.0f;
            }
            else if (_throttle)
            {
                _throttle = false;
                controls.Throttle = 0.0f;
            }

            var dir = new UnityEngine.Vector2(_keyboard.JoystickX, _keyboard.JoystickY);
            if (dir.sqrMagnitude > 0)
            {
                _joystick = true;
                controls.Throttle = dir.sqrMagnitude > 0.5f ? 1 : 0;
                controls.Course = RotationHelpers.Angle(dir);
            }
            else if (_joystick)
            {
                _joystick = false;
                controls.Throttle = 0;
                controls.Course = null;
            }

            if (_keyboard.Left)
            {
                _left = true;
                controls.Course = currentRotation + 175;
            }
            else if (_left)
            {
                _left = false;
                controls.Course = null;
            }

            if (_keyboard.Right)
            {
                _right = true;
                controls.Course = currentRotation - 175;
            }
            else if (_right)
            {
                _right = false;
                controls.Course = null;
            }

            var actions = _keyboard.Actions;

            var fire = false;
            for (var i = 0; i < _keyBindings.Count; ++i)
            {
                if (i >= actions.Count)
                    break;

                if (actions[i])
                {
                    fire = true;
                    foreach (var id in _keyBindings[i])
                        controls.SetSystemState(id, true);
                }
                else if (_fire)
                {
                    foreach (var id in _keyBindings[i])
                        controls.SetSystemState(id, false);
                }
            }

            _fire = fire;
        }

        private bool _joystick;
        private bool _throttle;
        private bool _left;
        private bool _right;
        private bool _fire;

        private readonly List<List<int>> _keyBindings;

        private readonly IShip _ship;
        private readonly IKeyboard _keyboard;

        public class Factory : IControllerFactory
        {
            public Factory(IKeyboard keyboard)
            {
                _keyboard = keyboard;
            }

            public IController Create(IShip ship)
            {
                return new KeyboardController(ship, _keyboard);
            }

            private readonly IKeyboard _keyboard;
        }
    }
}
