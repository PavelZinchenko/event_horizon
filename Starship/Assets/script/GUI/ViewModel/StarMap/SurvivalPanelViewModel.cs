using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameServices.Player;
using Constructor.Ships;
using Services.Reources;
using Zenject;

namespace ViewModel
{
	public class SurvivalPanelViewModel : MonoBehaviour
	{
	    [Inject] private readonly MotherShip _motherShip;
	    [Inject] private readonly IResourceLocator _resourceLocator;

		public Button StartButton;
		public LayoutGroup ShipsPanel;
		public TimerViewModel Timer;

		public void StartButtonClicked()
		{
			UnityEngine.Debug.Log("SurvivalPanelViewModel.StartButtonClicked");
            _motherShip.CurrentStar.Survival.Attack();
		}
		
		private void OnEnable()
		{
			var survival = _motherShip.CurrentStar.Survival;
			var ships = survival.CreateFleet().Ships;
			_useTime = survival.LastAttackTime;
		    _cooldown = survival.CooldownTime;
			UpdateItems(ships);
		}		
		
		private void UpdateItems(IEnumerable<IShip> ships)
		{
			var enumerator = ships.GetEnumerator();
			Image item = null;
			foreach (Transform transform in ShipsPanel.transform)
			{
				item = transform.GetComponent<Image>();
				if (item == null) 
					continue;
				if (enumerator.MoveNext())
					UpdateItem(item, enumerator.Current);
				else
					item.gameObject.SetActive(false);
			}
			
			while (enumerator.MoveNext())
			{
				var newItem = (Image)Instantiate(item);
				newItem.rectTransform.SetParent(item.transform.parent);
				newItem.rectTransform.localScale = Vector3.one;
				UpdateItem(newItem, enumerator.Current);
			}
		}
		
		private void UpdateItem(Image item, IShip data)
		{
			item.gameObject.SetActive(true);
			item.sprite = _resourceLocator.GetSprite(data.Model.IconImage) ?? _resourceLocator.GetSprite(data.Model.ModelImage);
		}

		private void Update()
		{
			var timeLeft = _cooldown + _useTime - System.DateTime.UtcNow.Ticks;
			Timer.SetTime(timeLeft);
			StartButton.gameObject.SetActive(timeLeft <= 0);
		}

		private long _useTime;
	    private long _cooldown;
	}
}
