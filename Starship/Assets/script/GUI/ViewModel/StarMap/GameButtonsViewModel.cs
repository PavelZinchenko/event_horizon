using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using GameModel;
using GameServices.Player;
using Services.Messenger;
using Zenject;

namespace ViewModel
{
	//public class GameButtonsViewModel : MonoBehaviour // TODO: delete
	//{
	//    [Inject] private readonly IMessenger _messenger;
	//    [Inject] private readonly GameServices.Player.PlayerShip _playerShip;
	//    //[Inject] private readonly GameLogic _gameLogic;
 //       //[Inject] private readonly StarMap _starMap;

 //       public PanelController InformationPanel;
	//	public PanelController ControlPanel;
	//	public PanelController FleetPanel;
	//	public PanelController ResearchPanel;
	//	//public PanelController CraftInfoPanel;

	//	public PanelController SurvivalPanel;
	//	public PanelController MultiplayerPanel;
	//	public PanelController ArenaPanel;
	//	//public PanelController LabPanel;
	//	public PanelController RuinsPanel;
	//	public PanelController MilitaryPanel;
	//	public PanelController PlanetPanel;
	//	public PanelController BossPanel;
	//	public PanelController FactionPanel;
	//	public PanelController WormholePanel;
	//	public PanelController BlackMarketPanel;
	//	public PanelController ChallengePanel;

	//	public Button StarViewButton;
	//	public Button GalaxyViewButton;
	//	public ToggleGroup GalaxyButtonsGroup;

	//	public void ShowGameButtons()
	//	{
	//		gameObject.SetActive(true);

	//		if (_lastActivePanel != null)
	//			SetActivePanel(_lastActivePanel);
	//		else if (GalaxyButtonsGroup.gameObject.activeSelf)
	//			ActivateFirstButton();
	//		else
	//			ShowInformationPanel();
	//	}

	//	public void HideGameButtons()
	//	{
	//		gameObject.SetActive(false);
	//		SetActivePanel(null);
	//	}

	//	public void ExitToMainMenu()
	//	{
 //           // TODO: Game.Level.LoadMainMenu();
	//	}

	//	public void ShowInformationPanel()
	//	{
	//		SetActivePanel(InformationPanel);
	//	}

	//	public void ShowControlPanel()
	//	{
	//		SetActivePanel(ControlPanel);
	//	}

	//	public void ShowFleetPanel()
	//	{
	//		SetActivePanel(FleetPanel);
	//	}

	//	public void ShowResearchPanel()
	//	{
	//		SetActivePanel(ResearchPanel);
	//	}

	//	public void PanelClosed()
	//	{
	//		if (_activePanel != null)
	//			_activePanel.Open();
	//	}

	//	public void ShowStarSystem(bool visible)
	//	{
	//		//_gameLogic.TrySetMapState(visible ? GameModel.MapState.StarSystem : GameModel.MapState.StarMap);
	//	}

	//	private void ActivateFirstButton()
	//	{
	//		GalaxyButtonsGroup.SetAllTogglesOff();
	//		foreach (Transform child in GalaxyButtonsGroup.transform)
	//		{
	//			var toggle = child.GetComponent<Toggle>();
	//			if (toggle == null)
	//				continue;

	//			toggle.isOn = true;
	//			break;
	//		}
	//	}

	//	private void Start()
	//	{
	//		//_messenger.AddListener<ViewMode>(EventType.ViewModeChanged, OnMapStateChanged);
 //  //         _messenger.AddListener<GameModel.PointOfInterest>(EventType.ArrivedToObject, OnArrivedToObject);
 //  //         _messenger.AddListener<int>(EventType.ArrivedToPlanet, OnArrivedToPlanet);
	//		//OnMapStateChanged(_gameLogic.CurrentMapState);
	//	}

	//	private void OnMapStateChanged(ViewMode view)
	//	{
	//		StarViewButton.gameObject.SetActive(view == ViewMode.StarMap);
	//		GalaxyViewButton.gameObject.SetActive(view == ViewMode.StarSystem || view == ViewMode.GalaxyMap);
	//		GalaxyButtonsGroup.gameObject.SetActive(view == ViewMode.StarMap);

	//		if (view == ViewMode.StarMap)
	//			ActivateFirstButton();
	//		else if (view == ViewMode.GalaxyMap)
	//			ShowInformationPanel();
	//	}

	//	private void OnArrivedToObject(Galaxy.StarObjectType pointOfInterest)
	//	{
	//		//switch (pointOfInterest)
	//		//{
	//		//case Galaxy.StarObjectType.Undefined:
	//		//	ShowInformationPanel();
	//		//	break;
	//		//case Galaxy.StarObjectType.Boss:
	//		//	SetActivePanel(BossPanel);
	//		//	break;
	//		//case GameModel.PointOfInterest.StarBase:
	//		//	SetActivePanel(FactionPanel);
	//		//	break;
	//		//case GameModel.PointOfInterest.Wormhole:
	//		//	SetActivePanel(WormholePanel);
	//		//	break;
	//		//case GameModel.PointOfInterest.Military:
	//		//	SetActivePanel(MilitaryPanel);
	//		//	break;
	//		//case GameModel.PointOfInterest.Challenge:
	//		//	SetActivePanel(ChallengePanel);
	//		//	break;
	//		//case GameModel.PointOfInterest.Arena:
	//		//	SetActivePanel(ArenaPanel);
	//		//	break;
	//		////case Game.PointOfInterest.Laboratory:
	//		////	SetActivePanel(LabPanel);
	//		////	break;
	//		//case GameModel.PointOfInterest.Ruins:
	//		//	SetActivePanel(RuinsPanel);
	//		//	break;
	//		//case GameModel.PointOfInterest.Multiplayer:
	//		//	SetActivePanel(MultiplayerPanel);
	//		//	break;
	//		//case GameModel.PointOfInterest.Survival:
	//		//	SetActivePanel(SurvivalPanel);
	//		//	break;
	//		//case GameModel.PointOfInterest.BlackMarket:
	//		//	SetActivePanel(BlackMarketPanel);
	//		//	break;
	//		//case GameModel.PointOfInterest.Event:
	//		//	//_playerShip.CurrentStarDeprecated.LocalEvent.Start();
	//		//	break;
	//		//}
	//	}

	//	private void OnArrivedToPlanet(int planetId)
	//	{
	//		PlanetPanel.GetComponent<ViewModel.PlanetPanel>().OnArrivedToPlanet(planetId);
	//		SetActivePanel(PlanetPanel);
	//	}

	//	private void SetActivePanel(PanelController panel)
	//	{
	//		if (!gameObject.activeSelf)
	//			panel = null;

	//		if (panel == _activePanel)
	//			return;

	//		var current = _activePanel;
	//		_activePanel = panel;

	//		if (_activePanel != null)
	//			_lastActivePanel = _activePanel;

	//		if (current != null)
	//		{
	//			current.Close();
	//		}
	//		else if (_activePanel != null)
	//		{
	//			_activePanel.Open();
	//		}
	//	}

	//	private PanelController _activePanel;
	//	private PanelController _lastActivePanel;
	//}
}
