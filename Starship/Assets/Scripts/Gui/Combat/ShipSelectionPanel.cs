using Combat.Component.Ship;
using Combat.Domain;
using Combat.Manager;
using Combat.Scene;
using Combat.Unit;
using Services.Gui;
using UnityEngine;
using Zenject;

namespace Gui.Combat
{
    public class ShipSelectionPanel : MonoBehaviour
    {
        [Inject] private readonly CombatManager _manager;
        [Inject] private readonly IScene _scene;

        [SerializeField] private ShipList _enemyShips;
        [SerializeField] private ShipList _playerShips;

        public void Open(ICombatModel combatModel)
        {
            if (Window.IsVisible)
                return;

            _enemyShips.Initialize(combatModel.EnemyFleet, combatModel.EnemyFleet.Ships.FindIndex(item => item.Status == ShipStatus.Active));
            _playerShips.Initialize(combatModel.PlayerFleet, combatModel.PlayerFleet.Ships.FindIndex(item => item.Status == ShipStatus.Active));
            GetComponent<IWindow>().Open();
        }

        public void StartButtonClicked()
        {
            var ship = _playerShips.SelectedShip;
            if (ship.Status != ShipStatus.Ready)
                return;

            _manager.CreateShip(ship);
            GetComponent<IWindow>().Close();
        }

        private void Update()
        {
            if (_scene.PlayerShip.IsActive() && Window.IsVisible)
                Window.Close();
        }

        private IWindow Window { get { return GetComponent<IWindow>(); } }
    }
}
