using Services.Gui;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Gui.Windows
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AnimatedWindow : AnimatedWindowBase, IWindow
    {
		[SerializeField] private WindowClass _class = WindowClass.Singleton;
		[SerializeField] private bool _waitOpenAnimationDone = false;
        [SerializeField] private bool _waitCloseAnimationDone = false;
        [SerializeField] private EscapeKeyAction _escapeKeyAction = EscapeKeyAction.None;

        public WindowInitializedEvent OnInitializedEvent = new WindowInitializedEvent();
        public UnityEvent OnWindowClosedEvent = new UnityEvent();
        public UnityEvent OnWindowClosingEvent = new UnityEvent();
        public UnityEvent OnWindowOpenedEvent = new UnityEvent();
        public UnityEvent OnWindowOpeningEvent = new UnityEvent();
        //public UnityEventBool OnWindowActivated = new UnityEventBool();

        [Inject]
        private void Initialize(
            WindowOpenedSignal.Trigger windowOpenedTrigger,
            WindowClosedSignal.Trigger windowClosedTrigger)
        {
            _windowOpenedTrigger = windowOpenedTrigger;
            _windowClosedTrigger = windowClosedTrigger;

            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnWindowClosed()
        {
            _canvasGroup.blocksRaycasts = true;
            //Debug.LogError("OnWindowClosed - " + gameObject.name);

            if (_waitCloseAnimationDone)
				_windowClosedTrigger.Fire(Id, _exitCode);
			
            OnWindowClosedEvent.Invoke();
            //OnWindowActivated.Invoke(false);
        }

        protected override void OnWindowClosing()
        {
            _canvasGroup.blocksRaycasts = false;
            //Debug.LogError("OnWindowClosing - " + gameObject.name);

            if (!_waitCloseAnimationDone)
            	_windowClosedTrigger?.Fire(Id, _exitCode);
			
            OnWindowClosingEvent.Invoke();
        }

        protected override void OnWindowOpened()
        {
            _canvasGroup.blocksRaycasts = true;
            //Debug.LogError("OnWindowOpened - " + gameObject.name);

            if (_waitOpenAnimationDone)
				_windowOpenedTrigger?.Fire(Id);

            OnWindowOpenedEvent.Invoke();
            //OnWindowActivated.Invoke(_canvasGroup.interactable);
        }

        protected override void OnWindowOpening()
        {
            _canvasGroup.blocksRaycasts = false;
            //Debug.LogError("OnWindowOpening - " + gameObject.name);

            if (!_waitOpenAnimationDone)
            	_windowOpenedTrigger?.Fire(Id);
			
            OnWindowOpeningEvent.Invoke();
        }

        public string Id => name;
        public WindowClass Class => _class;

        public void Open()
		{
			Open(null);
		}

		public void Open(WindowArgs args)
        {
            _exitCode = WindowExitCode.Cancel;
            OpenWindow();

            OnInitializedEvent.Invoke(args ?? new WindowArgs());
        }

        public void Close()
        {
            Close(WindowExitCode.Cancel);
        }

        public void Close(WindowExitCode exitCode)
        {
            _exitCode = exitCode;
            CloseWindow();
        }

        public bool IsVisible => IsWindowVisible;

        public bool Enabled
        {
            get => _canvasGroup.interactable;
            set
            {
                if (_canvasGroup.interactable == value) return;
                _canvasGroup.interactable = value;
                //OnWindowActivated.Invoke(value);
            }
        }

        public EscapeKeyAction EscapeAction => _escapeKeyAction;

        private WindowExitCode _exitCode = WindowExitCode.Cancel;
        private CanvasGroup _canvasGroup;
        private WindowOpenedSignal.Trigger _windowOpenedTrigger;
        private WindowClosedSignal.Trigger _windowClosedTrigger;
    }
}
