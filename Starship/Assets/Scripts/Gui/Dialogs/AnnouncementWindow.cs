using GameServices.Settings;
using Gui.Windows;
using Services.Gui;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Dialogs
{
    public class AnnouncementWindow : MonoBehaviour
    {
        [SerializeField] private Toggle _dontAskToggle;

        [Inject] private GameSettings _gameSettings;

        public void InitializeWindow(WindowArgs args)
        {
            var isOn = _gameSettings.DontAskAgainId >= AnnouncementId;

            if (_dontAskToggle != isOn)
                _dontAskToggle.isOn = isOn;
        }

        public void OnWindowClosed()
        {
            if (_dontAskToggle.isOn)
            {
                _gameSettings.DontAskAgainId = AnnouncementId;
                PlayerPrefs.Save();
            }
        }

        public void OpenStoreButtonClicked()
        {
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=com.ZipasGames.Frontier");
#elif UNITY_IPHONE
            Application.OpenURL("https://itunes.apple.com/app/id1336417415");
#endif

            GetComponent<AnimatedWindow>().Close(WindowExitCode.Ok);
        }

        public const int AnnouncementId = 1;
    }
}
