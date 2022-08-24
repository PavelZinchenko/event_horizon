using UnityEngine;
using UnityEngine.UI;
using GameServices.Player;
using Services.Reources;
using Zenject;

namespace ViewModel
{
	public class ChallengePanel : MonoBehaviour
	{
	    [Inject] private readonly MotherShip _motherShip;
	    [Inject] private readonly IResourceLocator _resourceLocator;

		public Button StartButton;
		public Image EnemyShipIcon;
		public Image PlayerShipIcon;
		public Gui.ImagesColorSelector LevelPanel;

		public void StartButtonClicked()
		{
			UnityEngine.Debug.Log("ChallengePanel.StartButtonClicked");
            _motherShip.CurrentStar.Challenge.Attack();
		}

		private void OnEnable()
		{
			Initialize();
		}		

		private void Initialize()
		{
            var challenge = _motherShip.CurrentStar.Challenge;

			PlayerShipIcon.sprite = _resourceLocator.GetSprite(challenge.GetPlayerShip().Ship.ModelImage);
			EnemyShipIcon.sprite = _resourceLocator.GetSprite(challenge.GetEnemyShip().Ship.ModelImage);

			LevelPanel.TotalCount = challenge.MaxLevel;
			LevelPanel.EnabledCount = challenge.CurrentLevel;
		}
    }
}
