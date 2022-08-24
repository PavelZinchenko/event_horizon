using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Combat
{
    public class ShipCounter : MonoBehaviour
    {
        public enum Type
        {
            Player,
            Enemy,
        }

        [SerializeField] private Text _countText;
        [SerializeField] private Type _type;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<int>(_type == Type.Player ? EventType.PlayerShipCountChanged : EventType.EnemyShipCountChanged, OnShipCountChanged);
        }

        private void OnShipCountChanged(int count)
        {
            _countText.text = count.ToString();
        }
    }
}
