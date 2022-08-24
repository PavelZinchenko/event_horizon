using UnityEngine;
using GameServices.Player;
using Zenject;

namespace ViewModel
{
	public class WormholePanelViewModel : MonoBehaviour
	{
	    [Inject] private readonly MotherShip _motherShip;

		public void EnterButtonClicked()
		{
			UnityEngine.Debug.Log("WormholePanelViewModel.EnterButtonClicked");
            _motherShip.CurrentStar.Wormhole.Enter();
		}
	}
}
