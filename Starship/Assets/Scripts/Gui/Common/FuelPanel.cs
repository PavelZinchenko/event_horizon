using GameServices.Player;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Common
{
    public class FuelPanel : MonoBehaviour
    {
        [SerializeField] private Text _fuelText;

        [Inject]
        private void Initialize(IMessenger messenger, PlayerResources playerResources)
        {
            messenger.AddListener<int>(EventType.FuelValueChanged, SetValue);
            SetValue(playerResources.Fuel);
        }

        private void SetValue(int value)
        {
            if (_fuel == value) return;
            _fuel = value;
            _fuelText.text = _fuel.ToString();
        }

        private int _fuel = -1;
    }
}
