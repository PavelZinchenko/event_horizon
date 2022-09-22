using UnityEngine;
using GameServices.Player;
using Utils;
using Zenject;

namespace ViewModel
{
	public class RuinsPanelViewModel : MonoBehaviour
	{
	    [Inject] private readonly MotherShip _motherShip;

		public RectTransform[] CompletedOnlyControls;
		public RectTransform[] NotCompletedOnlyControls;
		
		private void OnEnable()
		{
            var completed = _motherShip.CurrentStar.Ruins.IsDefeated;
            foreach (var control in CompletedOnlyControls)
                control.gameObject.SetActive(completed);
            foreach (var control in NotCompletedOnlyControls)
                control.gameObject.SetActive(!completed);
        }		
		
		public void StartButtonClicked()
		{
			OptimizedDebug.Log("RuinsPanelViewModel.StartButtonClicked");
            _motherShip.CurrentStar.Ruins.Attack();
		}
	}
}
