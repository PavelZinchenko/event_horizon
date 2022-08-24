using Combat.Manager;
using GameServices;
using Services.Gui;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Combat
{
    public class CombatMenu : MonoBehaviour
    {
        [SerializeField] private Button _nextEnemyButton;
        [SerializeField] private Button _changeShipButton;

        [Inject]
        private void Initialize(IMessenger messenger, CombatManager manager)
        {
            _manager = manager;
            messenger.AddListener<int>(EventType.EnemyShipCountChanged, OnEnemyShipCountChanged);
        }

        public void Open()
        {
            GetComponent<IWindow>().Open();
        }

        public void InitializeWindow()
        {
            _nextEnemyButton.gameObject.SetActive(_manager.CanCallNextEnemy());
            _nextEnemyButton.interactable = true;
            _changeShipButton.gameObject.SetActive(_manager.CanChangeShip());
        }

        public void ExitButtonClicked()
        {
            _manager.Surrender();
        }

        public void NextEnemyButtonClicled()
        {
            _manager.CallNextEnemy();
        }

        public void ChangeShipButtonClicked()
        {
            _manager.ChangeShip();
        }

        public void KillThemAll()
        {
            _manager.KillAllEnemies();
        }

        private void OnEnemyShipCountChanged(int count)
        {
            if (!gameObject.activeSelf)
                return;
            
            _nextEnemyButton.interactable = _manager.CanCallNextEnemy();
        }

        private CombatManager _manager;
    }
}
