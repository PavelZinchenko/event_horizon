using Gui.Windows;
using Services.Gui;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Exploration
{
    [RequireComponent(typeof(AnimatedWindow))]
    public class ScanningPanel : MonoBehaviour
    {
        [SerializeField] private float _cooldown = 10;
        [SerializeField] private Image _progressImage;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<int>(EventType.PlayerShipUndocked, OnPlayerUndocked);
        }

        public void InitializeWindow(WindowArgs args)
        {
            _window = GetComponent<AnimatedWindow>();
            _elapsedTime = 0;
        }

        private void OnPlayerUndocked(int stationId)
        {
            _window?.Close(WindowExitCode.Cancel);
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            _progressImage.fillAmount = Mathf.Clamp01(_elapsedTime / _cooldown);

            if (_elapsedTime < _cooldown)
                return;

            _window.Close(WindowExitCode.Ok);
        }

        private float _elapsedTime;
        private AnimatedWindow _window;

    }
}
