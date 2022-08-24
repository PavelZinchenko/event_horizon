using GameServices.Player;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Common
{
    public class MoneyPanel : MonoBehaviour
    {
        [SerializeField] private Text _creditsText;

        [Inject]
        private void Initialize(IMessenger messenger, PlayerResources playerResources)
        {
            messenger.AddListener<int>(EventType.MoneyValueChanged, SetValue);
            SetValue(playerResources.Money);
        }

        private void SetValue(int value)
        {
            if (_credits == value) return;
            _credits = value;
            _creditsText.text = _credits.ToString("N0");
        }

        private int _credits = -1;
    }
}
