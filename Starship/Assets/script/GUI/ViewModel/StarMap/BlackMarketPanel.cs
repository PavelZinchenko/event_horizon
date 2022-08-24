using UnityEngine;
using GameModel.Quests;
using GameServices.Player;
using GameStateMachine.States;
using Zenject;

namespace ViewModel
{
	public class BlackMarketPanel : MonoBehaviour
	{
	    [Inject] private readonly OpenShopSignal.Trigger _openShopTrigger;
	    [Inject] private readonly MotherShip _motherShip;
	    [Inject] private readonly InventoryFactory _factory;

		public void OpenStore()
		{
		    var starId = _motherShip.Position;
            _openShopTrigger.Fire(_factory.CreateBlackMarketInventory(starId), _factory.CreateBlackMarketPlayerInventory(starId));
        }
	}
}
