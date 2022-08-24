using GameServices.Settings;
using GameStateMachine.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.MainMenu
{
    public class SettingsCombat : MonoBehaviour
    {
        [SerializeField] Slider _cameraZoomSlider;
        [SerializeField] Toggle _centerOnPlayerToggle;
        [SerializeField] Toggle _showDamageToogle;

        [Inject] private readonly GameSettings _gameSettings;
        [Inject] private readonly ConfigureControlsSignal.Trigger _configureControlsTrigger;

        public void SetCameraZoom(float value)
        {
            _gameSettings.CameraZoom = value;
        }

        public void ConfigureControls()
        {
            _configureControlsTrigger.Fire();
        }

        public void SetCenterOnPlayer(bool enabled)
        {
            _gameSettings.CenterOnPlayer = enabled;
        }

        public void SetShowDamage(bool enabled)
        {
            _gameSettings.ShowDamage = enabled;
        }

        private void OnEnable()
        {
            _cameraZoomSlider.value = _gameSettings.CameraZoom;
            _centerOnPlayerToggle.isOn = _gameSettings.CenterOnPlayer;
            _showDamageToogle.isOn = _gameSettings.ShowDamage;
        }

    }
}
