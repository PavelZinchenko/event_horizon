using System.Linq;
using System.Collections.Generic;
using GameModel;
using UnityEngine;
using UnityEngine.UI;
using GameModel.Quests;
using GameModel.Skills;
using GameServices.Player;
using ViewModel.Quests;
using Zenject;
using Services.Localization;

namespace ViewModel
{
    namespace Common
    {
        public class PlayerExperienceItem : MonoBehaviour, IItemDescription
        {
            [Inject] private readonly ILocalization _localization;
            [Inject] private readonly PlayerSkills _playerSkills;

            [SerializeField] Text ExperienceText;
            [SerializeField] GameObject RankPanel;
            [SerializeField] Text RankText;

            public string Name { get; private set; }
            public Color Color { get { return ColorTable.DefaultTextColor; } }

            public void Initialize(ExperienceData data)
            {
                var before = (long)data.ExperienceBefore;
                var after = Mathf.Min(data.ExperienceAfter, Experience.MaxExperience);

                if (after <= before)
                {
                    gameObject.SetActive(false);
                    return;
                }

                gameObject.SetActive(true);
                
                Name = _localization.GetString("$PlayerExperience", after - before);
                ExperienceText.text = "+" + (after - before);

                var rank = ((Experience)after).Level - ((Experience)before).Level;
                RankPanel.gameObject.SetActive(rank > 0);
                if (rank > 0)
                    RankText.text = "+" + rank;
            }
        }
    }
}
