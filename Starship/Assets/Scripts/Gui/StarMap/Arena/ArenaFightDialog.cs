using GameServices.Multiplayer;
using Services.Gui;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Multiplayer
{
    [RequireComponent(typeof(IWindow))]
    public class ArenaFightDialog : MonoBehaviour
    {
        [Inject] private OfflineMultiplayer _offlineMultiplayer;

        [SerializeField] private Text _name;
        [SerializeField] private Text _rating;

        public void Initialize(IPlayerInfo player)
        {
            GetComponent<IWindow>().Open();

            _player = player;
            _name.text = player.Name;
            _rating.text = player.Rating.ToString();
        }

        public void OkButtonClicked()
        {
            GetComponent<IWindow>().Close();
            _offlineMultiplayer.Fight(_player);
        }

        private IPlayerInfo _player;
    }
}
