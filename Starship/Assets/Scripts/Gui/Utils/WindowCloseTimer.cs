using Gui.Windows;
using Services.Gui;
using UnityEngine;

namespace Gui.Utils
{
    [RequireComponent(typeof(IWindow))]
    public class WindowCloseTimer : MonoBehaviour
    {
        [SerializeField] private float _time = 5.0f;
        [SerializeField] private bool _unscaledTime = true;

        public void ResetTimer()
        {
            _lifetime = _time;
        }

        private void Awake()
        {
            var window = GetComponent<AnimatedWindow>();
            if (window != null)
                window.OnInitializedEvent.AddListener(args => ResetTimer());
        }

        private void OnEnable()
        {
            ResetTimer();
        }

        private void Update()
        {
            if (_lifetime < 0)
                return;

            _lifetime -= _unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            if (_lifetime < 0)
                GetComponent<IWindow>().Close();
        }

        private float _lifetime;
    }
}
