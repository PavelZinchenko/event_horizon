using System.Collections;
using Economy;
using GameServices.Player;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Zenject;

namespace ViewModel
{
	public class UnlockConfirmationViewModel : MonoBehaviour
	{
	    [Inject] private readonly PlayerResources _playerResources;

		public CanvasGroup UiCanvasGroup;
		public Text PriceText;
		public GameObject OutOfMoneyLabel;
		public Button ConfirmButton;

		public UnlockConfirmationViewModel()
		{
			_instance = new WeakReference<UnlockConfirmationViewModel>(this);
		}
		
		public static void Open(Price price, System.Action action)
		{
			_instance.Target.Initialize(price, action);
		}
		
		public void ConfirmButtonClicked()
		{
			GetComponent<PanelController>().Close();
			_action.Invoke();
		}
		
		public void OnClosing()
		{
			UiCanvasGroup.blocksRaycasts = true;
			UiCanvasGroup.interactable = true;
		}
		
		private void Initialize(Price price, System.Action action)
		{
			_action = action;
			GetComponent<PanelController>().Open();
			UiCanvasGroup.blocksRaycasts = false;
			UiCanvasGroup.interactable = false;

			PriceText.text = price.ToString();
			var haveMoney = price.IsEnough(_playerResources);
			OutOfMoneyLabel.SetActive(!haveMoney);

			ConfirmButton.interactable = haveMoney;
		}
		
		private System.Action _action;
		private static WeakReference<UnlockConfirmationViewModel> _instance;
	}
}
