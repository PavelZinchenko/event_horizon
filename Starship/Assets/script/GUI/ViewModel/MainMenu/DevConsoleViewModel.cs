using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameServices.Settings;
using Services.Audio;
using Zenject;

namespace ViewModel
{
	public class DevConsoleViewModel : MonoBehaviour
	{
	    [Inject] private readonly ISoundPlayer _soundPlayer;
	    [Inject] private readonly Cheats _cheats;

        public RectTransform[] Buttons;
		public AudioClip ClickSound;
		public AudioClip ConfirmSound;
		public Text DeviceId;

	    [Inject]
	    private void Initialize(GameSettings gameSettings)
	    {
	        _hash = DebugCommands.GetHashCode(gameSettings.UniqueId);
	    }

		public void OnButtonClicked(string key)
		{
			_idletime = 0;

			if (string.IsNullOrEmpty(key))
			{
			    if (TryExecuteCommand(_value))
			        DeviceId.text = "OK";
			    else
                    DeviceId.text = "ERROR";

			    _value = string.Empty;
                return;
			}

            if (key == "*")
			{
				_value = string.Empty;
			}
			else
			{
				_value += key;
			}

		    DeviceId.text = _value.Length <= 8 ? _value : _value.Substring(_value.Length - 8);
			_soundPlayer.Play(ClickSound);
		}

		public void OnPointerClick(BaseEventData data)
		{
			if (Time.unscaledTime - _lastClickTime > 0.4f)
			{
				_clickCount = 1;
			}
			else if (++_clickCount == 3)
			{
				_idletime = 0;
				foreach (var item in Buttons)
					item.gameObject.SetActive(true);
				DeviceId.gameObject.SetActive(true);
                DeviceId.text = _hash.ToString();
            }

			_lastClickTime = Time.unscaledTime;
		}

		private void OnEnable()
		{
			_value = string.Empty;
			foreach (var item in Buttons)
				item.gameObject.SetActive(false);
			DeviceId.gameObject.SetActive(false);
		}

		private bool TryExecuteCommand(string command)
		{
		    if (_cheats.TryExecuteCommand(command, _hash))
		    {
		        _soundPlayer.Play(ConfirmSound);
		        return true;
		    }

		    return false;
		}

		private void Update()
		{
			_idletime += Time.deltaTime;
			if (_idletime > 5f)
			{
				foreach (var item in Buttons)
					item.gameObject.SetActive(false);
				DeviceId.gameObject.SetActive(false);
			}
		}

		private float _idletime;
		private float _lastClickTime;
		private int _clickCount;
		private int _hash;
		string _value = string.Empty;
	}
}
