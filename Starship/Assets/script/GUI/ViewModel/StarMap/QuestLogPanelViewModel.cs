using System.Collections.Generic;
using System.Linq;
using Domain.Quests;
using GameServices.Gui;
using GameServices.Quests;
using Gui.Quests;
using Services.Localization;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ViewModel
{
	public class QuestLogPanelViewModel : MonoBehaviour
	{
	    [Inject] private readonly IQuestManager _questManager;
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly IMessenger _messenger;
	    [Inject] private readonly GuiHelper _guiHelper;

		[SerializeField] private LayoutGroup _itemsLayoutGroup;
	    [SerializeField] private GameObject _emptyText;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
			messenger.AddListener(EventType.QuestListChanged, UpdateItems);
        }

	    public void OnFocusButtonClicked(QuestPanel quest)
	    {
	        var beacons = new List<int>();
	        if (!quest.Quest.TryGetBeacons(beacons)) return;

	        _messenger.Broadcast<int>(EventType.FocusedPositionChanged, beacons.First());
        }

	    public void OnCancelButtonClicked(QuestPanel quest)
	    {
            _guiHelper.ShowConfirmation(_localization.GetString("$AbandonQuestConfirmation"), () =>
            {
                _questManager.AbandonQuest(quest.Quest);
                UpdateItems();
            });
	    }

	    public void OnWindowOpened()
	    {
	        UpdateItems();
	    }

        public void OnWindowClosed()
	    {
	        _messenger.Broadcast<int>(EventType.FocusedPositionChanged, -1);
	    }

        private void UpdateItems()
        {
            if (!gameObject.activeSelf) return;

			var quests = _questManager.Quests.Where(quest => !string.IsNullOrEmpty(quest.Model.Name));
		    _itemsLayoutGroup.InitializeElements<QuestPanel, IQuest>(quests, UpdateQuestItem);
            _emptyText.gameObject.SetActive(!quests.Any());
		}
		
		private void UpdateQuestItem(QuestPanel item, IQuest quest)
		{
			item.gameObject.SetActive(true);
            item.Initialize(quest, _localization);
		}
	}
}
