using System;
using GameDatabase;
using GameServices.GameManager;
using GameServices.Gui;
using GameServices.LevelManager;
using GameServices.Settings;
using Services.Audio;
using Services.Localization;
using Services.Messenger;
using Session;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.MainMenu
{
    public class SettingsGeneral : MonoBehaviour
    {
        [SerializeField] Slider _soundVolumeSlider;
        [SerializeField] Slider _musicVolumeSlider;
        [SerializeField] Toggle _powerSavingModeToggle;
        [SerializeField] Toggle _runInBackgroundToggle;
        [SerializeField] GameObject _deleteProgressPanel;
        [SerializeField] private Dropdown _languagesDropdown;
        [SerializeField] private Dropdown _resolutionsDropdown;

        [Inject] private readonly ILevelLoader _levelLoader;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly IMusicPlayer _musicPlayer;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly GameSettings _gameSettings;
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly GuiHelper _guiHelper;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly IGameDataManager _gameDataManager;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener(EventType.SessionCreated, OnSessionCreated);

            _localizations = Resources.Load<TextAsset>("Localization/languages").text.Split(' ');
        }

        public void OnResolutionChanged(int value)
        {
            if (value < 0 || value >= Screen.resolutions.Length*2) return;
            var resolution = Screen.resolutions[value/2];
            var fullScreen = value % 2 == 0;

            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            if (resolution.width == screenWidth && resolution.height == screenHeight && Screen.fullScreen == fullScreen) return;
            Screen.SetResolution(resolution.width, resolution.height, fullScreen);
        }

        public void OnLanguageChanged(int value)
        {
            var language = _localizations[value];
            if (language.Equals(_gameSettings.Language, StringComparison.OrdinalIgnoreCase))
                return;

            _gameSettings.Language = language;
            _localization.Reload(_database);
            _levelLoader.ReloadLevel();
        }

        public void SetSoundVolume(float value)
        {
            _soundPlayer.Volume = value;
        }

        public void SetMusicVolume(float value)
        {
            _musicPlayer.Volume = value;
        }

        public void SetPowerSavingMode(bool enabled)
        {
            _gameSettings.LowQualityMode = enabled;
            QualitySettings.SetQualityLevel(enabled ? 0 : 1);
        }

        public void RunInBackground(bool enabled)
        {
            _gameSettings.RunInBackground = enabled;
            Application.runInBackground = enabled;
        }

        public void SetFullScreenMode(bool enabled)
        {
            Screen.fullScreen = enabled;
        }

        public void CreateNewGameButtonClicked()
        {
            _guiHelper.ShowConfirmation(_localization.GetString("$DeleteConfirmationText"), CreateNewGame);
        }

        private void CreateNewGame()
        {
            _gameDataManager.CreateNewGame();
        }

        private void OnEnable()
        {
            OnSessionCreated();
            _musicVolumeSlider.value = _musicPlayer.Volume;
            _soundVolumeSlider.value = _soundPlayer.Volume;
            _powerSavingModeToggle.isOn = _gameSettings.LowQualityMode;
            _runInBackgroundToggle.isOn = _gameSettings.RunInBackground;

            var id = Array.IndexOf(_localizations, _gameSettings.Language);
            _languagesDropdown.value = id > 0 ? id : 0;

#if UNITY_STANDALONE
            _resolutionsDropdown.options.Clear();
            var selected = 0;
            var windowedText = _localization.GetString("$WindowedMode");
            for (var i = 0; i < Screen.resolutions.Length; ++i)
            {
                var resolution = Screen.resolutions[i];
                var screenWidth = Screen.width;
                var screenHeight = Screen.height;

                _resolutionsDropdown.options.Add(new Dropdown.OptionData(resolution.width + "x" + resolution.height));

                if (i < Screen.resolutions.Length - 1)
                    _resolutionsDropdown.options.Add(new Dropdown.OptionData(resolution.width + "x" + resolution.height + " " + windowedText));

                if (resolution.width == screenWidth && resolution.height == screenHeight)
                    selected = Screen.fullScreen ? 2*i : 2*i + 1;
            }
            _resolutionsDropdown.value = selected;
#endif
        }

        private void OnSessionCreated()
        {
            if (gameObject.activeSelf)
                _deleteProgressPanel.gameObject.SetActive(_session.IsGameStarted);
        }

        private string[] _localizations;
    }
}
