using System;
using UnityEngine;
using Zenject;

namespace GameServices.Settings
{
    public class GameSettings : IInitializable, IDisposable
    {
        [Inject]
        public GameSettings(GamePausedSignal gamePausedSignal)
        {
            _musicVolume = new FloatDataItem("music_volume", 1.0f);
            _soundVolume = new FloatDataItem("sound_volume", 0.5f);
            _cameraZoom = new FloatDataItem("camera_zoom", 0.0f);
            _language = new StringDataItem("language");
            _rateButtonClicked = new BoolDataItem("settings");
            _signedIn = new BoolDataItem("signedin", true);
            _controlsLayout = new StringDataItem("controlsLayout");
            _slideToMove = new BoolDataItem("slideToMove");
            _joystickThrust = new BoolDataItem("joystickThrust");
            _stopOnFire = new BoolDataItem("stopOnFire");
            _lowQualityMode = new BoolDataItem("powerSavingMode");
            _startCounter = new IntDataItem("startCounter");
            _centerOnPlayer = new BoolDataItem("centerOnPlayer");
            _showDamage = new BoolDataItem("showDamage", true);
            _autosave = new BoolDataItem("autosave");
            _guid = new StringDataItem("guid");
            _lastFacebookPostDate = new IntDataItem("fpost");
            _lastDailyRewardDate = new IntDataItem("rdata");
            _editorText = new StringDataItem();
            _dontAskAgainId = new IntDataItem("dontask");
            _activeMod = new StringDataItem("mod");
            _runInBackground = new BoolDataItem("runInBackground");
            _keyBindings = new StringDataItem("keyBindings");

            _gamePausedSignal = gamePausedSignal;
            _gamePausedSignal.Event += OnGamePaused;
        }

        public void Initialize()
        {
            AppStartCounter++;
            UnityEngine.Debug.Log("AppStartCounter: " + AppStartCounter);
        }

        public float MusicVolume
        {
            get => _musicVolume.Read();
            set => _musicVolume.Write(value);
        }

        public float SoundVolume
        {
            get => _soundVolume.Read();
            set => _soundVolume.Write(value);
        }

        public float CameraZoom
        {
            get => _cameraZoom.Read();
            set => _cameraZoom.Write(value);
        }

        public string Language
        {
            get => _language.Read();
            set => _language.Write(value);
        }

        public bool RateButtonClicked
        {
            get => _rateButtonClicked.Read();
            set => _rateButtonClicked.Write(value);
        }

        public bool SignedIn
        {
            get => _signedIn.Read();
            set => _signedIn.Write(value);
        }

        public string ControlsLayout
        {
            get => _controlsLayout.Read();
            set => _controlsLayout.Write(value);
        }

        public bool SlideToMove
        {
            get => _slideToMove.Read();
            set => _slideToMove.Write(value);
        }

        public bool ThrustWidthJoystick
        {
            get => _joystickThrust.Read();
            set => _joystickThrust.Write(value);
        }

        public bool StopWhenWeaponActive
        {
            get => _stopOnFire.Read();
            set => _stopOnFire.Write(value);
        }

        public bool LowQualityMode
        {
            get => _lowQualityMode.Read();
            set => _lowQualityMode.Write(value);
        }

        public bool RunInBackground
        {
            get => _runInBackground.Read();
            set => _runInBackground.Write(value);
        }

        public int AppStartCounter
        {
            get => _startCounter.Read();
            set => _startCounter.Write(value);
        }

        public bool CenterOnPlayer
        {
            get => _centerOnPlayer.Read();
            set => _centerOnPlayer.Write(value);
        }

        public bool ShowDamage
        {
            get => _showDamage.Read();
            set => _showDamage.Write(value);
        }

        public bool AutoSave
        {
            get => _autosave.Read();
            set => _autosave.Write(value);
        }

        public string EditorText
        {
            get => _editorText.Read();
            set => _editorText.Write(value);
        }

        public string ActiveMod
        {
            get => _activeMod.Read();
            set => _activeMod.Write(value);
        }

        public int LastFacebookPostDate
        {
            get => _lastFacebookPostDate.Read();
            set => _lastFacebookPostDate.Write(value);
        }

        public int LastDailyRewardDate
        {
            get => _lastDailyRewardDate.Read();
            set => _lastDailyRewardDate.Write(value);
        }

        public int DontAskAgainId
        {
            get => _dontAskAgainId.Read();
            set => _dontAskAgainId.Write(value);
        }

        public string KeyBindings
        {
            get => _keyBindings.Read();
            set => _keyBindings.Write(value);
        }

        public string UniqueId
        {
            get
            {
                if (string.IsNullOrEmpty(_uniqueId))
                {
                    _uniqueId = _guid.Read();
                    if (string.IsNullOrEmpty(_uniqueId))
                    {
                        _uniqueId = System.Guid.NewGuid().ToString();
                        _guid.Write(_uniqueId);
                    }
                }

                return _uniqueId;
            }
        }

        public void Dispose()
        {
            PlayerPrefs.Save();
        }

        private readonly FloatDataItem _musicVolume;
        private readonly FloatDataItem _soundVolume;
        private readonly FloatDataItem _cameraZoom;
        private readonly StringDataItem _language;
        private readonly BoolDataItem _rateButtonClicked;
        private readonly BoolDataItem _signedIn;
        private readonly StringDataItem _controlsLayout;
        private readonly BoolDataItem _slideToMove;
        private readonly BoolDataItem _joystickThrust;
        private readonly BoolDataItem _stopOnFire;
        private readonly BoolDataItem _lowQualityMode;
        private readonly BoolDataItem _runInBackground;
        private readonly IntDataItem _startCounter;
        private readonly BoolDataItem _centerOnPlayer;
        private readonly BoolDataItem _showDamage;
        private readonly BoolDataItem _autosave;
        private readonly StringDataItem _guid;
        private readonly StringDataItem _editorText;
        private string _uniqueId;
        private readonly GamePausedSignal _gamePausedSignal;
        private readonly IntDataItem _lastFacebookPostDate;
        private readonly IntDataItem _lastDailyRewardDate;
        private readonly IntDataItem _dontAskAgainId;
        private readonly StringDataItem _activeMod;
        private readonly StringDataItem _keyBindings;

        public struct StringDataItem
        {
            public StringDataItem(string key, string defaultValue)
            {
                _key = key;
                _default = defaultValue;
            }

            public StringDataItem(string key)
                : this(key, string.Empty)
            {
            }

            public string Read()
            {
                return PlayerPrefs.GetString(_key, _default);
            }

            public void Write(string value)
            {
                if (value == _default)
                    PlayerPrefs.DeleteKey(_key);
                else
                    PlayerPrefs.SetString(_key, value ?? string.Empty);
            }

            private readonly string _default;
            private readonly string _key;
        }

        public struct IntDataItem
        {
            public IntDataItem(string key, int defaultValue)
            {
                _key = key;
                _default = defaultValue;
            }

            public IntDataItem(string key)
                : this(key, default(int))
            {
            }

            public int Read()
            {
                return PlayerPrefs.GetInt(_key, _default);
            }

            public void Write(int value)
            {
                if (value == _default)
                    PlayerPrefs.DeleteKey(_key);
                else
                    PlayerPrefs.SetInt(_key, value);
            }

            private readonly int _default;
            private readonly string _key;
        }

        public struct FloatDataItem
        {
            public FloatDataItem(string key, float defaultValue)
            {
                _key = key;
                _default = defaultValue;
            }

            public FloatDataItem(string key)
            : this(key, default(float))
            {
            }

            public float Read()
            {
                return PlayerPrefs.GetFloat(_key, _default);
            }

            public void Write(float value)
            {
                if (Mathf.Approximately(value, _default))
                    PlayerPrefs.DeleteKey(_key);
                else
                    PlayerPrefs.SetFloat(_key, value);
            }

            private readonly float _default;
            private readonly string _key;
        }

        public struct BoolDataItem
        {
            public BoolDataItem(string key, bool defaultValue = false)
            {
                _key = key;
                _defaultValue = defaultValue ? _true : _false;
            }

            public bool Read()
            {
                return PlayerPrefs.GetInt(_key, _defaultValue) == _true;
            }

            public void Write(bool value)
            {
                PlayerPrefs.SetInt(_key, value ? _true : _false);
            }

            private readonly string _key;
            private readonly int _defaultValue;

            private const int _true = 1;
            private const int _false = 0;
        }

        private void OnGamePaused(bool paused)
        {
            if (paused)
                PlayerPrefs.Save();
        }
    }
}
