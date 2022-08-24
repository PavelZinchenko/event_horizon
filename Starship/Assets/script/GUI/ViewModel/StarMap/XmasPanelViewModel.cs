using GameModel.Quests;
using UnityEngine;
using GameServices.Player;
using Gui.Windows;
using Services.Gui;
using Zenject;

namespace ViewModel
{
    public class XmasPanelViewModel : MonoBehaviour
    {
        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly InventoryFactory _factory;

        [SerializeField] private AnimatedWindow _storeWindow;

        public GameObject AttackButton;

        private void OnEnable()
        {
            var completed = _motherShip.CurrentStar.Xmas.IsDefeated;
            AttackButton.gameObject.SetActive(!completed);
        }

        public void OpenStore()
        {
            _storeWindow.Open(new WindowArgs(_factory.CreateSantaInventory(_motherShip.Position), null));
        }

        public void Attack()
        {
            _motherShip.CurrentStar.Xmas.Attack();
        }
    }
}
