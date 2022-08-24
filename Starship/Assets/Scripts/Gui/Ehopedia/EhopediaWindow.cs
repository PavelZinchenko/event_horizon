using GameStateMachine.States;
using Services.Gui;
using Services.Messenger;
using UnityEngine;
using Zenject;

namespace Gui.Ehopedia
{
    public class EhopediaWindow : MonoBehaviour
    {
        [Inject] private readonly ExitSignal.Trigger _exitTrigger;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener(EventType.EscapeKeyPressed, OnCancel);
        }

        private void OnCancel()
        {
            _exitTrigger.Fire();
        }
    }
}
