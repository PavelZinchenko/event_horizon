using Economy;
using GameServices.Player;
using Gui.Windows;
using Services.Gui;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Common;
using Zenject;

namespace Gui.Dialogs
{
    public class BuyConfirmationDialog : MonoBehaviour
    {
        [InjectOptional] private readonly PlayerResources _playerResources;

        [SerializeField] private Text _text;
        [SerializeField] private PricePanel _pricePanel;
        [SerializeField] private Button _confirmButton;

        public void InitializeWindow(WindowArgs args)
        {
            _text.text = args.Get<string>();
            var price = args.Get<Price>(1);
            var isEnough = _playerResources == null || price.IsEnough(_playerResources);

            _confirmButton.interactable = isEnough;
            _pricePanel.Initialize(null, price, !isEnough);
        }

        public void ConfirmButtonClicked()
        {
            GetComponent<AnimatedWindow>().Close(WindowExitCode.Ok);
        }
    }
}
