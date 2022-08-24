using System;
using Constructor.Ships;
using DataModel.Technology;
using GameDatabase.DataModel;
using GameServices.Database;
using GameServices.Player;
using Maths;
using Services.Localization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;
using ModificationType = Constructor.Ships.Modification.ModificationType;
using Research = GameServices.Research.Research;

namespace Gui.Craft
{
    public class UnlockSlotPanel : MonoBehaviour
    {
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly ITechnologies _technologies;
        [Inject] private readonly PlayerResources _resources;
        [Inject] private readonly Research _research;
        [Inject] private readonly PlayerSkills _playerSkills;
        [Inject] private readonly ModificationFactory _modificationFactory;

        [SerializeField] private UnityEvent _slotUnlockedEvent = new UnityEvent();
        [SerializeField] private Text _warningText;
        [SerializeField] private Text _requirementsText;
        [SerializeField] private Button _unlockButton;

        [SerializeField] private CraftPricePanel _craftPricePanel;


        public void Initialize(IShip ship, int level, Faction faction)
        {
            _ship = ship;

            var quality = CraftItemQuality.Improved + ship.Model.Modifications.Count;

            _warningText.text = _localization.GetString("$UpgradeShipDescription", GetLevelCost(quality));

            var requiredLevel = 50;
            ITechnology tech;
            if (_technologies.TryGetShipTechnology(ship.Model.Id, out tech))
                requiredLevel = GameModel.Craft.GetWorkshopLevel(tech);
            requiredLevel = Mathf.Max(0, quality.GetWorkshopLevel(requiredLevel) + _playerSkills.CraftingLevelModifier);
            var requiredFaction = ship.Model.Faction;
            var requiredShipLevel = GetLevelRequirement(quality);

            if (requiredLevel > level || (requiredFaction != Faction.Neutral && requiredFaction != faction))
                _requirementsText.text = _localization.GetString("$RequiredShipyardLevel", requiredLevel, _localization.GetString(requiredFaction.Name));
            else if (ship.Experience.Level < requiredShipLevel)
                _requirementsText.text = _localization.GetString("$RequiredShipLevel", requiredShipLevel);
            else
                _requirementsText.text = string.Empty;

            Price = (tech != null ? tech.GetCraftPrice(quality)*1.1f - tech.GetCraftPrice(quality-1) : new CraftingPrice(ship.Price()*(ship.Model.Modifications.Count + 1)))*_playerSkills.CraftingPriceScale;
            _craftPricePanel.Initialize(Price, requiredFaction);

            CanUnlock = requiredLevel <= level && (requiredFaction == faction || requiredFaction == Faction.Neutral) && requiredShipLevel <= ship.Experience.Level && _craftPricePanel.HaveEnoughResources;
            _unlockButton.interactable = CanUnlock;
        }

        public void UnlockButtonClicked()
        {
            if (!CanUnlock)
                return;

            _resources.Money -= Price.Credits;
            _resources.Stars -= Price.Stars;
            _research.AddResearchPoints(_ship.Model.Faction, -Price.Techs);
            var quality = CraftItemQuality.Improved + _ship.Model.Modifications.Count;
            _ship.Experience = Experience.FromLevel(_ship.Experience.Level - GetLevelCost(quality));

            _ship.Model.Modifications.Add(_modificationFactory.Create(ModificationType.Empty));
            _slotUnlockedEvent.Invoke();
        }


        private bool CanUnlock { get; set; }
        private CraftingPrice Price { get; set; }

        private IShip _ship;

        private static int GetLevelCost(CraftItemQuality quality)
        {
            switch (quality)
            {
                case CraftItemQuality.Improved: return 10;
                case CraftItemQuality.Excellent: return 15;
                case CraftItemQuality.Superior: return 20;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private static int GetLevelRequirement(CraftItemQuality quality)
        {
            switch (quality)
            {
                case CraftItemQuality.Improved: return 50;
                case CraftItemQuality.Excellent: return 75;
                case CraftItemQuality.Superior: return 100;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
