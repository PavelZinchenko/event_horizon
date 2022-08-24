using System.Linq;
using GameDatabase;
using GameServices.GameManager;
using GameServices.Gui;
using Services.Localization;
using Services.Messenger;
using Services.Storage;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.MainMenu
{
    public class SettingsLoadSave : MonoBehaviour
    {
        [Inject] private readonly ICloudStorage _cloudStorage;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly IGameDataManager _manager;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly GuiHelper _guiHelper;

        [SerializeField] private LayoutGroup _gamesGroup;
        [SerializeField] private ToggleGroup _gamesToggleGroup;
        [SerializeField] private Button _saveGameButton;
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _deleteGameButton;
        [SerializeField] private GameObject _waiter;
        [SerializeField] private NewSavedGameItem _newGameItem;
        [SerializeField] private Text _errorText;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<CloudStorageStatus>(EventType.CloudStorageStatusChanged, OnCloudStorageStatusChanged);
            messenger.AddListener(EventType.CloudGamesReceived, OnCloudGamesReceived);
            messenger.AddListener(EventType.DatabaseLoaded, OnDatabaseLoaded);
        }

        public void LoadGame()
        {
            var game = SelectedGame;
            if (game == null)
                return;

            _guiHelper.ShowConfirmation(_localization.GetString("$LoadGameConfirmation"),
                () => _manager.LoadGameFromCloud(game));
        }

        public void DeleteGame()
        {
            var game = SelectedGame;
            if (game == null)
                return;

            _guiHelper.ShowConfirmation(_localization.GetString("$DeleteGameConfirmation"),
                () => game.Delete());
        }

        public void SaveGame()
        {
            if (_newGameItem.IsSelected && !string.IsNullOrEmpty(_newGameItem.Name))
            {
                _manager.SaveGameToCloud(_newGameItem.Name);
                return;
            }

            var game = SelectedGame;
            if (game == null)
                return;

            _guiHelper.ShowConfirmation(_localization.GetString("$SaveGameConfirmation"),
                () => _manager.SaveGameToCloud(game));
        }

        public void RefreshGames()
        {
            _cloudStorage.Synchronize();
        }

        public void OnToggleSelectionChanged()
        {
            UpdateButtons();
        }

        private ISavedGame SelectedGame
        {
            get
            {
                var active = _gamesToggleGroup.ActiveToggles().FirstOrDefault();
                if (active == null)
                    return null;
                var savedgame = active.GetComponent<SavedGameItem>();
                return savedgame == null ? null : savedgame.Game;
            }
        }

        private void UpdateButtons()
        {
            var game = SelectedGame;

            _loadGameButton.interactable = game != null;
            _saveGameButton.interactable = game != null || _newGameItem.IsSelected;
            _deleteGameButton.interactable = game != null;
        }

        private void UpdateItem(SavedGameItem item, ISavedGame game)
        {
            item.Initialize(game, _localization);
        }

        private void OnCloudGamesReceived()
        {
            if (gameObject.activeSelf)
                _gamesGroup.InitializeElements<SavedGameItem, ISavedGame>(_cloudStorage.Games, UpdateItem);

            var index = 0;
            foreach (var item in _gamesGroup.transform.Children<Toggle>())
                item.isOn = index++ == 0;
        }

        private void OnCloudStorageStatusChanged(CloudStorageStatus status)
        {
            if (!gameObject.activeSelf)
                return;

            _waiter.gameObject.SetActive(status == CloudStorageStatus.Synchronizing);
            _gamesGroup.gameObject.SetActive(status == CloudStorageStatus.Idle);
            _errorText.text = _cloudStorage.LastErrorMessage;
        }

        private void OnDatabaseLoaded()
        {
            UpdateButtons();
        }

        private void OnEnable()
        {
            OnCloudGamesReceived();
            OnCloudStorageStatusChanged(_cloudStorage.Status);

            if (_cloudStorage.Status == CloudStorageStatus.NotReady)
                _cloudStorage.Synchronize();
        }
    }
}
