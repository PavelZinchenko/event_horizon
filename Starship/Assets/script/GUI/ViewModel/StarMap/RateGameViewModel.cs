using GameServices.Settings;
using UnityEngine;
using Utils;
using Zenject;

namespace ViewModel
{
	public class RateGameViewModel : MonoBehaviour
	{
        [Inject] private readonly GameServices.Player.MotherShip _motherShip;
	    [Inject] private readonly GameSettings _gameSettings;

        public RectTransform Content;

		public void OnEnable()
		{
#if UNITY_ANDROID
            var available = _motherShip.CurrentStar.Level >= 10 && !_gameSettings.RateButtonClicked && Application.internetReachability != NetworkReachability.NotReachable;
			Content.gameObject.SetActive(available);
#else
			Content.gameObject.SetActive(false);
#endif
        }

        public void RateButtonClicked()
		{
			OptimizedDebug.Log("RateButtonClicked");
			Application.OpenURL("market://details?id=" + AppConfig.bundleIdentifier);
			Content.gameObject.SetActive(false);
			_gameSettings.RateButtonClicked = true;
		}
	}
}
