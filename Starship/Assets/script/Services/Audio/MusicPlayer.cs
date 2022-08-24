using UnityEngine;
using GameServices.LevelManager;
using GameServices.Settings;
using Zenject;

namespace Services.Audio
{	
	public enum GameLevel
	{
		MainMenu,
		GalaxyMap,
		Combat,
		Exploration,
	}

	public class MusicPlayer : MonoBehaviour, IMusicPlayer
	{
		[SerializeField] AudioClip[] MenuMusic;
		[SerializeField] AudioClip[] CombatMusic;
		[SerializeField] AudioClip[] GalaxyMapMusic;
	    [SerializeField] AudioClip[] ExplorationMusic;

        [Inject]
	    private void Initialize(GameSettings gameSettings, SceneLoadedSignal sceneLoadedSignal, ILevelLoader levelLoader)
	    {
	        _gameSettings = gameSettings;
	        _levelLoader = levelLoader;
	        _sceneLoadedSignal = sceneLoadedSignal;
	        _sceneLoadedSignal.Event += OnSceneLoaded;
	        Volume = _gameSettings.MusicVolume;
        }

        public float Volume
		{
			get
			{
				return _volume;
			}
			set
			{
				_volume = value;
				_audioSource.volume = _volume;
				_gameSettings.MusicVolume = _volume;
			}
		}

	    private void OnSceneLoaded()
	    {
	        switch (_levelLoader.Current)
	        {
	            case LevelName.MainMenu:
	                Play(MenuMusic);
                    break;
                case LevelName.Combat:
	                Play(CombatMusic);
                    break;
	            case LevelName.StarMap:
	                Play(GalaxyMapMusic);
                    break;
	            case LevelName.Exploration:
				case LevelName.Ehopedia:
                    Play(ExplorationMusic);
                    break;
            }
	    }

		public void Mute(bool mute) { _audioSource.volume = mute ? _volume/3 : _volume; }

		public void Pause()
		{
			_audioSource.Pause();
		}

		public void Resume()
		{
			if (!_audioSource.isPlaying)
				_audioSource.UnPause();
		}
		
		private void Play(AudioClip[] clips)
		{
			Mute(false);
			var random = new System.Random();
			var clip = clips.Length > 0 ? clips[random.Next() % clips.Length] : null;
			if (_audioSource.clip == clip)
				return;

			_isPlaying = false;
			_audioSource.Stop();
			_clipToPlay = clip;
		}

		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		private void Update()
		{
			if (!_isPlaying && _clipToPlay != null && _volume > 0f)
			{
				_isPlaying = true;
				_audioSource.clip = _clipToPlay;
				_audioSource.Play();
			}
			else if (_isPlaying && _volume <= 0f)
			{
				_isPlaying = false;
				_audioSource.Stop();
			}
		}

		private bool _isPlaying;
		private float _volume;
		private AudioClip _clipToPlay;
		private AudioSource _audioSource;

        private GameSettings _gameSettings;
        private SceneLoadedSignal _sceneLoadedSignal;
	    private ILevelLoader _levelLoader;
	}
}
