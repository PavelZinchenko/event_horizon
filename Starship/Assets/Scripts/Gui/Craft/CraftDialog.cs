using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DataModel.Technology;
using Economy.ItemType;
using Economy.Products;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices.Database;
using GameServices.Gui;
using GameServices.Player;
using GameServices.Random;
using GameServices.Research;
using Services.Audio;
using Services.Localization;
using Services.Messenger;
using Services.ObjectPool;
using Zenject;

namespace Gui.Craft
{
    public class CraftDialog : MonoBehaviour
    {
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly IRandom _random;
        [Inject] private readonly ItemTypeFactory _factory;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly Research _research;
        [Inject] private readonly ITechnologies _technologies;
        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly GameObjectFactory _gameObjectFactory;
        [Inject] private readonly GuiHelper _helper;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<int>(EventType.MoneyValueChanged, value => UpdateResources());
            messenger.AddListener<int>(EventType.StarsValueChanged, value => UpdateResources());
            messenger.AddListener(EventType.TechPointsChanged, UpdateResources);
        }

        [SerializeField] private LayoutGroup _technologyList;
        [SerializeField] private Image _factionIcon;
        [SerializeField] private Text _factionText;
        [SerializeField] private Text _levelText;
        [SerializeField] private ToggleGroup _techGroup;
        [SerializeField] private AudioClip _buySound;
        [SerializeField] private CraftPanel _commonCraftPanel;
        [SerializeField] private CraftPanel _improvedCraftPanel;
        [SerializeField] private CraftPanel _superiorCraftPanel;
        [SerializeField] private Text _creditsText;
        [SerializeField] private Text _starsText;
        [SerializeField] private Text _techsText;
        [SerializeField] private Image _techsIcon;

        public void OnItemCreated(IProduct item)
        {
            UpdateTechPanel();
            _soundPlayer.Play(_buySound);
            _helper.ShowItemInfoWindow(item);
        }

        public void OnTechItemSelected(ITechnology tech)
        {
            _selectedTech = tech;
            UpdateTechPanel();
        }

        public void OnTechItemDeselected(ITechnology tech)
        {
            _selectedTech = null;
            UpdateTechPanel();
        }

        public void InitializeWindow()
        {
            var star = _motherShip.CurrentStar;
            _faction = star.Region.Faction;
            var color =  _faction.Color;
            _factionText.text = _localization.GetString(_faction.Name);
            _factionIcon.color = _faction.Color;

            _techsIcon.color = color;
            _level = Mathf.Max(5, star.Level);
            _levelText.text = _level.ToString();

#if UNITY_EDITOR
            if (true)
            {
                _technologyList.InitializeElements<ViewModel.CraftListItem, ITechnology>(_technologies.All, UpdateTechnology, _gameObjectFactory);
            }
            else
#endif
            _technologyList.InitializeElements<ViewModel.CraftListItem, ITechnology>(_technologies.All.ForWorkshop(_faction).
                Where(_research.IsTechResearched), UpdateTechnology, _gameObjectFactory);

            _techGroup.SetAllTogglesOff();

            UpdateResources();
            OnTechItemSelected(null);
        }

        //private static int GetImprovedTechPrice(ITechnology technology)
        //{
        //    return Mathf.Max(1, technology.CraftPrice.Amount / 500);
        //}

        private void UpdateTechnology(ViewModel.CraftListItem item, ITechnology tech)
        {
            item.InitializeForCraft(tech, _level);
        }

        private void UpdateTechPanel()
        {
            if (_selectedTech == null)
            {
                _commonCraftPanel.Cleanup();
                _improvedCraftPanel.Cleanup();
                _superiorCraftPanel.Cleanup();
                return;
            }

            _commonCraftPanel.Initialize(_selectedTech);
            _improvedCraftPanel.Initialize(_selectedTech);
            _superiorCraftPanel.Initialize(_selectedTech);
        }

        private void UpdateResources()
        {
            if (!gameObject.activeSelf)
                return;

            _creditsText.text = _playerResources.Money.ToString();
            _starsText.text = _playerResources.Stars.ToString();
            _techsText.text = _research.GetAvailablePoints(_faction).ToString();
        }

        private Faction _faction;
        private ObscuredInt _level;
        private ITechnology _selectedTech;
    }
}
