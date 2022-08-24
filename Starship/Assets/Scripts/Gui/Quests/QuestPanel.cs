using System.Collections.Generic;
using Domain.Quests;
using Services.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Quests
{
    public class QuestPanel : MonoBehaviour
    {
        [SerializeField] private Text _name;
        [SerializeField] private Text _description;
        [SerializeField] private Button _focusButton;
        [SerializeField] private Button _cancelButton;

        public IQuest Quest => _quest;

        public void Update()
        {
            if (_quest == null) return;

            _timeFromLastUpdate += Time.deltaTime;
            if (_timeFromLastUpdate < 5) return;
            _timeFromLastUpdate = 0;

            _description.text = _quest.GetRequirementsText(_localization);
        }

        public void Initialize(IQuest quest, ILocalization localization)
        {
            _timeFromLastUpdate = 0;
            _localization = localization;
            _quest = quest;

            _name.text = localization.GetString(quest.Model.Name);
            _description.text = quest.GetRequirementsText(localization);
            _focusButton.gameObject.SetActive(quest.TryGetBeacons(new List<int>()));
            _cancelButton.gameObject.SetActive(quest.Model.QuestType.IsCancellable());
        }

        private float _timeFromLastUpdate;
        private IQuest _quest;
        private ILocalization _localization;
    }
}
