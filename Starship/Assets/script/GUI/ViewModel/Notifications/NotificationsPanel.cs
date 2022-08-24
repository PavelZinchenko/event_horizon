using Economy.Products;
using Services.Messenger;
using UnityEngine;
using Zenject;

namespace ViewModel
{
	//public class NotificationsPanel : MonoBehaviour 
	//{
	//	[SerializeField] LootPanel LootPanel;
	//	[SerializeField] PanelController IapErrorPanel;
 //       [SerializeField] ItemDescriptionWindow ItemDescriptionWindow;

	//    [Inject] private readonly IMessenger _messenger;

 //       private void Start()
	//	{
 //           _messenger.AddListener<IProduct>(EventType.LootItemReceived, ShowLootPanel);
 //           _messenger.AddListener<IProduct>(EventType.ItemInfoRequested, ShowItemDescriptionPanel);
 //           _messenger.AddListener(EventType.IapPurchaseFailed, ShowIapErrorPanel);
 //       }

	//	private void ShowLootPanel(IProduct item)
	//	{
	//		LootPanel.Open(item);
	//	}

 //       private void ShowItemDescriptionPanel(IProduct item)
 //       {
 //           ItemDescriptionWindow.Open(item);
 //       }

 //       private void ShowIapErrorPanel()
	//	{
	//		IapErrorPanel.Open();
	//	}
	//}
}