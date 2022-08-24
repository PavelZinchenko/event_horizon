using GameModel.Achievements;
using UnityEngine;
using UnityEngine.UI;
using ViewModel;

namespace Gui.Achievements
{
    [RequireComponent(typeof(PanelController))]
    public class Notification : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
        [SerializeField] private Text _description;

        private void Awake()
        {
            //TODO
            //ServiceLocator.Messenger.AddListener<IAchievement>(EventType.AchievementUnlocked, OnAchievementUnlocked);
            //GetComponent<PanelController>().OnWindowClosedEvent.AddListener(OnWindowClosed);
            //gameObject.SetActive(false);
            //ShowRecentAchievement();
        }

        private void Update()
        {
            if (_cooldown < 0)
                return;

            _cooldown -= Time.unscaledDeltaTime;

            if (_cooldown < 0)
                GetComponent<PanelController>().Close();
        }

        private void OnAchievementUnlocked(IAchievement achievement)
        {
            if (!GetComponent<PanelController>().IsVisible)
                ShowRecentAchievement();
        }

        private void ShowRecentAchievement()
        {
            //var achievement = Game.Session.Achievements.GetRecentAchievement();
            //if (achievement == null)
            //    return;

            //_icon.sprite = achievement.Type.GetIcon();
            //_name.text = achievement.Type.GetName();
            //_description.text = achievement.Type.GetDescription();

            GetComponent<PanelController>().Open();
            _cooldown = 5f;
        }

        private void OnWindowClosed()
        {
            ShowRecentAchievement();
        }

        private float _cooldown;
    }
}
