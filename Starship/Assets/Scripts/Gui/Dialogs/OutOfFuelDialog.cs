using GameServices.Player;
using Services.Gui;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using ViewModel;
using ViewModel.Common;
using Zenject;

namespace Gui.Dialogs
{
    public class OutOfFuelDialog : MonoBehaviour
    {
        [Inject] private readonly SupplyShip _supplyShip;
        [Inject] private readonly PlayerResources _resources;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<bool>(EventType.SupplyShipActivated, OnSupplyShipActivated);
        }

        [SerializeField] private TimerViewModel _timer;
        [SerializeField] private PricePanel _pricePanel;
        [SerializeField] private Button _buyButton;

        public void SpeedUpButtonClicked()
        {
            _supplyShip.SpeedUp();
        }

        private void OnSupplyShipActivated(bool active)
        {
            if (!active)
                GetComponent<IWindow>().Close();

            UpdateTimer();
        }

        private void Update()
        {
            _timeout -= Time.deltaTime;
            if (_timeout <= 0)
            {
                _timeout = 1.0f;
                UpdateTimer();
            }
        }

        private void UpdateTimer()
        {
            _timer.SetTime(_supplyShip.WaitingTime);
            var price = _supplyShip.SpeedUpPrice;
            _pricePanel.Initialize(null, price);
            _buyButton.interactable = price.IsEnough(_resources);
        }

        private float _timeout = 0;
    }
}
