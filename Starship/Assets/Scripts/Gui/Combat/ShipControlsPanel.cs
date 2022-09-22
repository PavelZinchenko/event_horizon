using System.Collections.Generic;
using System.Linq;
using Combat.Component.Ship;
using Combat.Unit;
using GameServices.Settings;
using Gui.Windows;
using Services.Gui;
using Services.Reources;
using UnityEngine;
using Utils;
using ViewModel;
using Zenject;

namespace Gui.Combat
{
    public class ShipControlsPanel : MonoBehaviour
    {
        [SerializeField] private ShipNavigationPanel _navigationPanel;

        [Inject] private readonly GameSettings _gameSettings;
        [Inject] private readonly IResourceLocator _resourceLocator;

        public void Load(IShip ship)
        {
            if (!ship.IsActive())
            {
                Window.Close(WindowExitCode.Ok);
                return;
            }

            Window.Open();
            Initialize(ship);
        }

        public void Reload()
        {
            var ship = _ship;
            if (ship.IsActive() && GetComponent<IWindow>().IsVisible)
                Initialize(ship);
        }

        public void OnJoystickPressed(Vector2 direction)
        {
            _joystickDirection = direction;
        }

        public void OnJoystickReleased()
        {
            _joystickDirection = Vector2.zero;
            if (_thrustWithJoystick && _ship != null)
                _ship.Controls.Throttle = 0f;
        }

        public void OnThrustPressed()
        {
            if (_ship.IsActive())
                _ship.Controls.Throttle = 1;
        }

        public void OnThrustReleased()
        {
            if (_ship.IsActive())
                _ship.Controls.Throttle = 0;
        }

        public void OnLeftPressed()
        {
            _leftPressed = true;
        }

        public void OnRightPressed()
        {
            _rightPressed = true;
        }

        public void OnLeftRightReleased()
        {
            _rightPressed = _leftPressed = false;
            if (_ship.IsActive())
                _ship.Controls.Course = null;
        }

        public void ActivateSystem(int id)
        {
            ActiveButtons++;
            if (_ship.IsActive())
                _ship.Controls.SetSystemState(id, true);
        }

        public void DeactivateSystem(int id)
        {
            ActiveButtons--;
            if (_ship.IsActive())
                _ship.Controls.SetSystemState(id, false);
        }

        public void ActivateDroneBays()
        {
            ActiveButtons++;
            if (_ship.IsActive())
                foreach (var id in _ship.Systems.All.GetDroneBayIndices())
                    _ship.Controls.SetSystemState(id, true);
        }

        public void DeactivateDroneBays()
        {
            ActiveButtons--;
            if (_ship.IsActive())
                foreach (var id in _ship.Systems.All.GetDroneBayIndices())
                    _ship.Controls.SetSystemState(id, false);
        }

        public void OnKeyPressed(int key)
        {
            if (key < _keyBindings.Count)
                foreach (var id in _keyBindings[key])
                    ActivateSystem(id);
        }

        public void OnKeyReleased(int key)
        {
            if (key < _keyBindings.Count)
                foreach (var id in _keyBindings[key])
                    DeactivateSystem(id);
        }

        private void Initialize(IShip ship)
        {
            _ship = ship;
            _joystickDirection = Vector2.zero;
            _leftPressed = false;
            _rightPressed = false;
            _activeButtons = 0;

            var buttons = new Dictionary<string, RectTransform>();
            foreach (Transform item in transform)
            {
                if (item.gameObject == _navigationPanel.gameObject)
                    continue;

                item.gameObject.SetActive(false);
                buttons.Add(item.name, item.GetComponent<RectTransform>());
            }

            var layout = _gameSettings.ControlsLayout;
            if (string.IsNullOrEmpty(layout))
            {
                layout = "Action0,1430,100,180 Action1,1240,100,180 Action2,1050,100,180 Action3,860,100,180 Action4,670,100,180 Action5,480,100,180 Joystick,220,160,300";
                _slideToMove = false;
                _thrustWithJoystick = true;
                _stopWhenWeaponActive = false;
            }
            else
            {
                _slideToMove = _gameSettings.SlideToMove;
                _thrustWithJoystick = _gameSettings.ThrustWidthJoystick;
                _stopWhenWeaponActive = _gameSettings.StopWhenWeaponActive;
            }

#if UNITY_STANDALONE
            _navigationPanel.gameObject.SetActive(false);
            UpdateLayout(buttons, "Action0,1470,70,90 Action1,1370,70,90 Action2,1270,70,90 Action3,1170,70,90 Action4,1070,70,90 Action5,970,70,90");
#else
            _navigationPanel.gameObject.SetActive(_slideToMove);
            _navigationPanel.Initialize(ship);
            _navigationPanel.AllowThrust = _thrustWithJoystick;
            UpdateLayout(buttons, layout);
#endif
            UpdateActionButtons(ship, buttons);
        }

        private void UpdateLayout(IDictionary<string, RectTransform> items, string layoutData)
        {
            foreach (var layout in layoutData.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                var data = layout.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                var id = data[0];
                var x = System.Convert.ToSingle(data[1]);
                var y = System.Convert.ToSingle(data[2]);
                var size = System.Convert.ToSingle(data[3]);

                RectTransform item;
                if (!items.TryGetValue(id, out item))
                {
                    OptimizedDebug.Log("Button not found: " + id);
                    continue;
                }

                item.gameObject.SetActive(true);
                item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                item.anchoredPosition = new Vector2(x, y);
            }
        }

        private void UpdateActionButtons(IShip ship, IDictionary<string, RectTransform> buttons)
        {
            _keyBindings = ship.Systems.All.GetKeyBindings();
            _cooldowns = new List<ButtonCooldown>();

            RectTransform item;
            for (int i = 0; buttons.TryGetValue("Action" + i, out item); i++)
            {
                if (i < _keyBindings.Count)
                {
                    item.gameObject.SetActive(true);
                    var button = item.GetComponent<PushButton>();
                    button.image.sprite = _resourceLocator.GetSprite(_ship.Systems.All[_keyBindings[i].First()].ControlButtonIcon);
                    _cooldowns.Add(button.GetComponent<ButtonCooldown>());
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        public void Update()
        {
            if (!_ship.IsActive())
            {
                Load(null);
                return;
            }

            if (_joystickDirection != Vector2.zero)
            {
                var rotation = RotationHelpers.Angle(_joystickDirection);
                _ship.Controls.Course = rotation;
                if (_thrustWithJoystick && !(_stopWhenWeaponActive && _activeButtons > 0))
                    _ship.Controls.Throttle = Mathf.Abs(Mathf.DeltaAngle(_ship.Body.Rotation, rotation)) < 30 ? Mathf.Clamp01(_joystickDirection.magnitude * 2) : 0f;
            }

            if (_leftPressed)
            {
                _ship.Controls.Course = _ship.Body.Rotation + 175;
            }

            if (_rightPressed)
            {
                _ship.Controls.Course = _ship.Body.Rotation - 175;
            }

            for (int i = 0; i < _keyBindings.Count; ++i)
            {
                var item = _cooldowns[i];
                if (item == null)
                    continue;

                var systems = _keyBindings[i];
                var minCooldown = 1.0f;
                var maxCooldown = 0.0f;
                for (int j = 0; j < systems.Count; ++j)
                {
                    var systemId = systems[j];
                    var system = _ship.Systems.All[systemId];
                    var cooldown = system.Cooldown;
                    if (cooldown < minCooldown)
                        minCooldown = cooldown;
                    if (cooldown > maxCooldown)
                        maxCooldown = cooldown;
                }

                item.SetCooldown(minCooldown, maxCooldown);
            }
        }

        private int ActiveButtons
        {
            get
            {
                return _activeButtons;
            }
            set
            {
                _activeButtons = value;
                if (_activeButtons > 0 && _stopWhenWeaponActive && _ship.IsActive())
                {
                    _ship.Controls.Throttle = 0;
                }

                _navigationPanel.AllowThrust = _thrustWithJoystick && !(_stopWhenWeaponActive && _activeButtons > 0);
            }
        }

        private AnimatedWindow Window { get { return _window ?? (_window = GetComponent<AnimatedWindow>()); } }

        private List<List<int>> _keyBindings;
        private List<ButtonCooldown> _cooldowns;

        private int _activeButtons;
        private bool _leftPressed;
        private bool _rightPressed;
        private Vector2 _joystickDirection;

        private bool _slideToMove;
        private bool _thrustWithJoystick;
        private bool _stopWhenWeaponActive;

        private IShip _ship;

        private AnimatedWindow _window;
    }
}
