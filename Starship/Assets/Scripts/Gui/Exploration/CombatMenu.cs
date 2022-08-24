using Services.Messenger;
using UnityEngine;
using Zenject;

namespace Gui.Exploration
{
    public class CombatMenu : MonoBehaviour
    {
        [Inject]
        private void Initialize(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public void ExitButtonClicked()
        {
            _messenger.Broadcast(EventType.Surrender);
        }

        public void KillThemAll()
        {
            _messenger.Broadcast(EventType.KillAllEnemies);
        }

        private IMessenger _messenger;
    }
}
