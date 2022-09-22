using UnityEngine;
using GameServices.Player;
using Utils;
using Zenject;

namespace ViewModel
{
	public class WormholePanelViewModel : MonoBehaviour
	{
	    [Inject] private readonly MotherShip _motherShip;

		public void EnterButtonClicked()
		{
			OptimizedDebug.Log("WormholePanelViewModel.EnterButtonClicked");
            _motherShip.CurrentStar.Wormhole.Enter();
		}
	}
}
