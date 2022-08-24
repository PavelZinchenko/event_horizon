using GameServices.Player;
using GameStateMachine.States;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ViewModel.Skills
{
    public class PlayerPanel : MonoBehaviour
    {
        [SerializeField] Text _skillPointsText;
        [SerializeField] GameObject _skillPointsPanel;
        [SerializeField] Slider _expSlider;

        [Inject] private readonly IMessenger _messenger;
        [Inject] private readonly PlayerSkills _playerSkills;
        [Inject] private readonly OpenSkillTreeSignal.Trigger _openSkillTreeTrigger;

        public void SkillsButtonClicked()
        {
            _openSkillTreeTrigger.Fire();
        }

        private void Start()
        {
            _messenger.AddListener(EventType.PlayerGainedExperience, UpdateInfo);
        }

        private void OnEnable()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (!this) return;
            var available = _playerSkills.AvailablePoints;
            _skillPointsPanel.gameObject.SetActive(available > 0);
            _skillPointsText.text = "+" + available;
            _expSlider.gameObject.SetActive(_playerSkills.Experience < GameModel.Skills.Experience.MaxExperience);
            _expSlider.value = (float)_playerSkills.Experience.ExpFromLastLevel / (float)_playerSkills.Experience.NextLevelCost;
        }
    }
}
