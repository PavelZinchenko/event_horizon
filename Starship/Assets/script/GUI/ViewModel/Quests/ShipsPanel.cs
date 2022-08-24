using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Constructor.Ships;
using Services.Reources;
using Zenject;

namespace ViewModel
{
	namespace Quests
	{
		public class ShipsPanel : MonoBehaviour
		{
		    [Inject] private readonly IResourceLocator _resourceLocator;

			public LayoutGroup ShipsLayoutGroup;
	        public ScrollRect ScrollRect;

			public void Initialize(IEnumerable<IShip> ships)
			{
				if (ships != null && ships.Any())
				{
					gameObject.SetActive(true);
					UpdateShips(ships);
				}
				else
				{
					gameObject.SetActive(false);
				}
			}

			private void UpdateShips(IEnumerable<IShip> ships)
			{
				var enumerator = ships.GetEnumerator();
				RectTransform item = null;
				foreach (Transform transform in ShipsLayoutGroup.transform)
				{
					item = transform.GetComponent<RectTransform>();
					if (enumerator.MoveNext())
						UpdateShipItem(item.GetComponent<ShipInfoViewModel>(), enumerator.Current);
					else
						item.gameObject.SetActive(false);
				}

				while (enumerator.MoveNext())
				{
					var newItem = (RectTransform)Instantiate(item);
					newItem.SetParent(item.parent);
					newItem.localScale = Vector3.one;
					UpdateShipItem(newItem.GetComponent<ShipInfoViewModel>(), enumerator.Current);
				}
			}		

			private void UpdateShipItem(ShipInfoViewModel item, IShip ship)
			{
				item.gameObject.SetActive(true);
				item.Icon.sprite = _resourceLocator.GetSprite(ship.Model.IconImage) ?? _resourceLocator.GetSprite(ship.Model.ModelImage);
				item.Icon.rectTransform.localScale = Vector3.one*ship.Model.IconScale;
				item.SetLevel(ship.Experience.Level);
				item.SetClass(ship.ExtraThreatLevel);
			}

			private void Start()
			{
				ScrollRect.horizontalNormalizedPosition = 0;
			}

			private void Update()
			{
				ScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(ScrollRect.horizontalNormalizedPosition + Time.deltaTime*0.01f);
			}
		}
	}
}
