using Services.Messenger;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Gui.Utils
{
    public class BackButtonEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _backButtonPressed = new UnityEvent();

        [SerializeField] private bool _ignoreIfStandaloneBuild = false;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
#if UNITY_STANDALONE
            if (_ignoreIfStandaloneBuild) return;
#endif

            messenger.AddListener(EventType.EscapeKeyPressed, OnCancel);
        }

        private void OnCancel()
        {
            if (this) _backButtonPressed.Invoke();
        }
    }
}
