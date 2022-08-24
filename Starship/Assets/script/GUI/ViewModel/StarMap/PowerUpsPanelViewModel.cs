using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ViewModel
{
	public class PowerUpsPanelViewModel : MonoBehaviour
	{
		public LayoutGroup ItemsLayoutGroup;

		/*private void Awake()
		{
			Messenger.AddListener(EventType.PowerUpsChanged, UpdateItems);
		}

		private void OnEnable()
		{
			UpdateItems();
		}
		
		private void UpdateItems()
		{
			if (!gameObject.activeSelf)
				return;

			var enumerator = Game.Data.Player.Fleet.PowerUps.GetEnumerator();

			RectTransform item = null;

			foreach (Transform transform in ItemsLayoutGroup.transform)
			{
				item = transform.GetComponent<RectTransform>();
				if (enumerator.MoveNext())
					UpdateItem(item, enumerator.Current);
				else
					item.gameObject.SetActive(false);
			}
			
			while (enumerator.MoveNext())
			{
				var newItem = (RectTransform)Instantiate(item);
				newItem.SetParent(item.parent);
				newItem.localScale = Vector3.one;
				UpdateItem(newItem, enumerator.Current);
			}
		}
		
		private void UpdateItem(RectTransform item, Game.PowerUps.IPowerUp powerup)
		{
			item.gameObject.SetActive(true);
			item.GetComponent<Image>().sprite = powerup.Icon;
		}*/
	}
}
