using Services.Gui;
using UnityEngine;
using Zenject;

namespace Gui.Windows
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Window : MonoBehaviour, IWindow
    {
        [SerializeField] private WindowClass _class = WindowClass.HudElement;
        [SerializeField] private EscapeKeyAction _escapeKeyAction = EscapeKeyAction.None;

        [SerializeField] WindowInitializedEvent OnInitializedEvent = new WindowInitializedEvent();
        [SerializeField] UnityEventBool OnActivatedEvent = new UnityEventBool();

        [Inject]
        private void Initialize(
            WindowOpenedSignal.Trigger windowOpenedTrigger,
            WindowClosedSignal.Trigger windowClosedTrigger)
        {
            _windowOpenedTrigger = windowOpenedTrigger;
            _windowClosedTrigger = windowClosedTrigger;

            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public string Id => name;
        public WindowClass Class => _class;

        public void Open()
        {
            Open(null);
        }

        public void Open(WindowArgs args)
        {
            if (!IsVisible)
            {
                _exitCode = WindowExitCode.Cancel;
                IsVisible = true;
            }

            OnInitializedEvent.Invoke(args ?? new WindowArgs());
            _windowOpenedTrigger.Fire(name);
        }

        public void Close()
        {
            IsVisible = false;

            _windowClosedTrigger.Fire(name, _exitCode);
        }

        public bool IsVisible
        {
            get => gameObject.activeSelf;
            set
            {
                gameObject.SetActive(value);
                OnActivatedEvent.Invoke(gameObject.activeSelf && _canvasGroup.interactable);
            }
        }

        public bool Enabled
        {
            get => _canvasGroup.interactable;
            set
            {
                _canvasGroup.interactable = value;
                OnActivatedEvent.Invoke(gameObject.activeSelf && _canvasGroup.interactable);
            }
        }

        public EscapeKeyAction EscapeAction => _escapeKeyAction;

        private WindowExitCode _exitCode = WindowExitCode.Cancel;
        private CanvasGroup _canvasGroup;
        private WindowOpenedSignal.Trigger _windowOpenedTrigger;
        private WindowClosedSignal.Trigger _windowClosedTrigger;
    }
}
