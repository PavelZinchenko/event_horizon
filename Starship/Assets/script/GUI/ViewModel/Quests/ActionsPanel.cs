using System.Linq;
using System.Collections.Generic;
using Domain.Quests;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ViewModel
{
	namespace Quests
	{
		public class ActionsPanel : MonoBehaviour
		{
			[Inject] private readonly Services.ObjectPool.GameObjectFactory _factory;

			public LayoutGroup LayoutGroup;

			public void Initialize(IEnumerable<UserAction> actions)
			{
				gameObject.SetActive(true);
				LayoutGroup.InitializeElements<QuestAction, UserAction>(actions, UpdateActionItem, _factory);
			}
			
			private void UpdateActionItem(QuestAction item, UserAction action)
			{
				item.Initialize(action);
			}
		}
	}
}
