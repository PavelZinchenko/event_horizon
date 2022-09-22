using UnityEngine;
using UnityEngine.UI;
using GameServices.Player;
using Utils;
using Zenject;

namespace ViewModel
{
	public class MilitaryPanelViewModel : MonoBehaviour
	{
        [Inject] private readonly PlayerResources _playerResources;
	    [Inject] private readonly MotherShip _motherShip;

        public Button StartButton;
		public Text MoneyText;
		public GameObject NotEnoughMoneyLabel;

		public void OnEnable()
		{
            // TODO:
			//var money = _playerShip.CurrentStar.Military.Price;
			//if (_money != money)
			//{
			//	_money = money;
			//	MoneyText.text = _money.ToString();
			//	var haveMoney = _money <= _playerResources.Money;
			//	NotEnoughMoneyLabel.gameObject.SetActive(!haveMoney);
			//	StartButton.interactable = haveMoney;
			//}			
		}
		
		public void StartButtonClicked()
		{
			OptimizedDebug.Log("MilitaryPanelViewModel.StartButtonClicked");
            // TODO:
			//if (Game.Session.GameLogic.StartCombat(Game.Session.CurrentStar.Military.CombatData, GameUiManager.Instance.ShowRewardDialog))
			//{
			//	Game.Session.Player.Money -= Game.Session.CurrentStar.Military.Price;
			//}
		}

		private int _money = -1;
	}
}
