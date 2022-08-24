using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameModel.Quests;
using GameServices.Player;
using ViewModel.Quests;
using Zenject;
using Services.Localization;
using Services.Reources;

namespace ViewModel
{
	namespace Common
	{
		public class ShipExperienceItem : MonoBehaviour, IItemDescription
        {
			[Inject] private readonly ILocalization _localization;
            [Inject] private readonly IResourceLocator _resourceLocator;
            [Inject] private readonly PlayerSkills _playerSkills;

            [SerializeField] Image Icon;
			[SerializeField] Text ExperienceText;
			[SerializeField] GameObject RankPanel;
			[SerializeField] Text RankText;

            public string Name { get; private set; }
            public Color Color { get { return ColorTable.DefaultTextColor; } }

            public void Initialize(GameModel.ExperienceData data)
            {
                var experienceAfter = (Maths.Experience)Mathf.Min(_playerSkills.MaxShipExperience, data.ExperienceAfter);
                var experienceBefore = (Maths.Experience)(long)data.ExperienceBefore;

                var experience = experienceAfter - data.ExperienceBefore;
				Name = _localization.GetString("$ShipExperience", _localization.GetString(data.ShipName), experience.ToString());
				ExperienceText.text = "+" + experience;
				Icon.sprite = _resourceLocator.GetSprite(data.Ship.Model.ModelImage);
				var rank = ((Maths.Experience)experienceAfter).Level - experienceBefore.Level;
				RankPanel.gameObject.SetActive(rank > 0);
				if (rank > 0)
					RankText.text = "+" + rank;
			}
		}
	}
}
