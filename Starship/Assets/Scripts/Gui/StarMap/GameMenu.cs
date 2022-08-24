using Galaxy;
using Game;
using UnityEngine;
using UnityEngine.UI;
using GameServices.Player;
using GameStateMachine.States;
using Services.Messenger;
using Gui.Windows;
using Services.Gui;
using Zenject;

namespace Gui.StarMap
{
    public class GameMenu : MonoBehaviour
    {
        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly ExitSignal.Trigger _exitTrigger;
        [Inject] private readonly SupplyShip _supplyShip;
        [Inject] private readonly Galaxy.StarMap _starMap;
        [Inject] private readonly HolidayManager _holidayManager;
        [Inject] private readonly IMessenger _messenger;

        public AnimatedWindow InformationPanel;
        public AnimatedWindow CargoHoldPanel;
        public AnimatedWindow FleetPanel;
        public AnimatedWindow ResearchPanel;

        public AnimatedWindow SurvivalPanel;
        public AnimatedWindow ArenaPanel;
        public AnimatedWindow RuinsPanel;
        public AnimatedWindow XmasPanel;
        public AnimatedWindow MilitaryPanel;
        public AnimatedWindow PlanetPanel;
        public AnimatedWindow BossPanel;
        public AnimatedWindow FactionPanel;
        public AnimatedWindow WormholePanel;
        public AnimatedWindow BlackMarketPanel;
        public AnimatedWindow ChallengePanel;
        public AnimatedWindow OutOfFuelDialog;
        public AnimatedWindow IapStoreWindow;
        public AnimatedWindow QuestLogWindow;

        [SerializeField] private Button StarViewButton;
        [SerializeField] private Button GalaxyViewButton;
        [SerializeField] private Button OutOfFuelButton;
        [SerializeField] private GameObject GalaxyButtonsGroup;
        [SerializeField] private GameObject FiltersGroup;
        [SerializeField] private Toggle BookmarkFilterToggle;
        [SerializeField] private Toggle BossFilterToggle;
        [SerializeField] private Toggle ShopFilterToggle;
        [SerializeField] private Toggle ArenaFilterToggle;
        [SerializeField] private Toggle XmasFilterToggle;

        public void ShowInformation() { InformationPanel.Open(); }
        public void ShowCargoHold() { CargoHoldPanel.Open(); }
        public void ShowFleet() { FleetPanel.Open(); }
        public void ShowResearch() { ResearchPanel.Open(); }
        public void ShowSurvival() { SurvivalPanel.Open(); }
        public void ShowArena() { ArenaPanel.Open(); }
        public void ShowRuins() { RuinsPanel.Open(); }
        public void ShowXmas() { XmasPanel.Open(); }
        public void ShowMilitaryBase() { MilitaryPanel.Open(); }
        public void ShowPandemic() { PlanetPanel.Open(new WindowArgs(Game.Exploration.Planet.InfectedPlanetId)); }
        public void ShowPlanet(int id) { PlanetPanel.Open(new WindowArgs(id)); }
        public void ShowBoss() { BossPanel.Open(); }
        public void ShowFaction() { FactionPanel.Open(); }
        public void ShowWormhole() { WormholePanel.Open(); }
        public void ShowBlackMarket() { BlackMarketPanel.Open(); }
        public void ShowChallenge() { ChallengePanel.Open(); }
        public void ShowOutOfFuel() { OutOfFuelDialog.Open(); }
        public void ShowIapStore() { IapStoreWindow.Open(); }
        public void ShowQuestLog() { QuestLogWindow.Open(); }

        public void ExitToMainMenu()
        {
            _exitTrigger.Fire();
        }

        public void ShowStarSystem(bool visible)
        {
            _motherShip.ViewMode = visible ? ViewMode.StarSystem : ViewMode.StarMap;
        }

        public void OnFiltersChanged()
        {
            _starMap.ShowBosses = BossFilterToggle.isOn;
            _starMap.ShowStores = ShopFilterToggle.isOn;
            _starMap.ShowBookmarks = BookmarkFilterToggle.isOn;
            _starMap.ShowArenas = ArenaFilterToggle.isOn;
            _starMap.ShowXmas = XmasFilterToggle.isOn && _holidayManager.IsChristmas;
            _messenger.Broadcast(EventType.StarMapChanged);
        }

        private void Start()
        {
            _messenger.AddListener<int>(EventType.PlayerPositionChanged, OnPlayerPositionChanged);
            _messenger.AddListener<ViewMode>(EventType.ViewModeChanged, OnMapStateChanged);
            _messenger.AddListener<Galaxy.StarObjectType>(EventType.ArrivedToObject, OnArrivedToObject);
            _messenger.AddListener<int>(EventType.ArrivedToPlanet, OnArrivedToPlanet);
            _messenger.AddListener<bool>(EventType.SupplyShipActivated, OnSupplyShipActivated);

            XmasFilterToggle.gameObject.SetActive(_holidayManager.IsChristmas);

            InitButtons();
            OnFiltersChanged();
        }

        private void OnPlayerPositionChanged(int starId)
        {
            InitButtons();
        }

        private void OnMapStateChanged(ViewMode view)
        {
            InitButtons();
        }

        private void InitButtons()
        {
            var view = _motherShip.ViewMode;

            StarViewButton.gameObject.SetActive(view == ViewMode.StarMap);
            GalaxyViewButton.gameObject.SetActive(view == ViewMode.StarSystem || view == ViewMode.GalaxyMap);
            GalaxyButtonsGroup.SetActive(view == ViewMode.StarMap);
            FiltersGroup.gameObject.SetActive(view == ViewMode.GalaxyMap);
            OutOfFuelButton.gameObject.SetActive(_supplyShip.IsActive);
            ShowInformation();
        }

        private void OnSupplyShipActivated(bool value)
        {
            OutOfFuelButton.gameObject.SetActive(value);
        }

        private void OnArrivedToObject(Galaxy.StarObjectType objectType)
        {
            switch (objectType)
            {
                case Galaxy.StarObjectType.Undefined:
                    ShowInformation();
                    break;
                case Galaxy.StarObjectType.Boss:
                    ShowBoss();
                    break;
                case Galaxy.StarObjectType.StarBase:
                    ShowFaction();
                    break;
                case Galaxy.StarObjectType.Wormhole:
                    ShowWormhole();
                    break;
                case Galaxy.StarObjectType.Military:
                    ShowMilitaryBase();
                    break;
                case Galaxy.StarObjectType.Challenge:
                    ShowChallenge();
                    break;
                case Galaxy.StarObjectType.Arena:
                    ShowArena();
                    break;
                case Galaxy.StarObjectType.Ruins:
                    ShowRuins();
                    break;
                case Galaxy.StarObjectType.Xmas:
                    ShowXmas();
                    break;
                case Galaxy.StarObjectType.Survival:
                    ShowSurvival();
                    break;
                case Galaxy.StarObjectType.BlackMarket:
                    ShowBlackMarket();
                    break;
                case Galaxy.StarObjectType.Hive:
                    ShowPandemic();
                    break;
                case Galaxy.StarObjectType.Event:
                    _motherShip.CurrentStar.LocalEvent.Start();
                    break;
            }
        }

        private void OnArrivedToPlanet(int planetId)
        {
            ShowPlanet(planetId);
        }
    }
}
