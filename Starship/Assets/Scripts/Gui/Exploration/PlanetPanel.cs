using Constructor.Ships;
using Economy;
using Game.Exploration;
using GameServices.Player;
using GameStateMachine.States;
using Services.Gui;
using Services.Localization;
using Session;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Common;
using Zenject;

namespace Gui.Exploration
{
	public class PlanetPanel : MonoBehaviour
	{
	    [Inject] private readonly Planet.Factory _factory;
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly PlayerResources _playerResources;
	    [Inject] private readonly ISessionData _session;
	    [Inject] private readonly StartExplorationSignal.Trigger _startExplorationTrigger;
	    [Inject] private readonly PlayerFleet _playerFleet;
        [Inject] private readonly MotherShip _motherShip;

	    [SerializeField] private Text _factionText;
	    [SerializeField] private Image _factionIcon;
	    [SerializeField] private Text _levelText;
	    [SerializeField] private FleetPanel _fleetPanel;
        [SerializeField] private PlanetInfo _planetInfo;
        [SerializeField] private Text _exploredText;
	    [SerializeField] private Text _notExploredText;
	    [SerializeField] private PricePanel _price;
	    [SerializeField] private Text _fuelText;
	    [SerializeField] private GameObject _notEnoughFuel;
	    [SerializeField] private Button _exploreButton;

        public void StartExploration()
		{
            if (_playerFleet.ExplorationShip == null) return;
		    if (!_playerResources.TryConsumeFuel(Planet.RequiredFuel)) return;
            if (!GetPrice().TryWithdraw(_playerResources)) return;
            
		    _startExplorationTrigger.Fire(_planet);
        }

	    public void Initialize(WindowArgs args)
	    {
	        if (!args.TryGet<int>(0, out var index))
	            return;

	        _planet = _factory.Create(_motherShip.Position, index);

            _factionText.text = _localization.GetString(_planet.Faction.Name);
	        _factionIcon.color = _planet.Faction.Color;
	        _levelText.text = _planet.Level.ToString();

            _fleetPanel.Initialize();
            _planetInfo.UpdatePlanet(_planet);

	        var count = _planet.ObjectivesExplored;
	        _notExploredText.gameObject.SetActive(count == 0);
	        _exploredText.gameObject.SetActive(count > 0);
	        _exploredText.text = _localization.GetString("$ExplorationProgress", 100 * count / _planet.TotalObjectives);

            UpdateButton();
	    }

	    public void OnShipSelected(IShip ship)
	    {
	        _playerFleet.ExplorationShip = ship;
            UpdateButton();
	    }

        private void UpdateButton()
        {
            var haveEnoughFuel = _playerResources.Fuel >= Planet.RequiredFuel;
            _fuelText.text = Planet.RequiredFuel.ToString();
            _notEnoughFuel.SetActive(!haveEnoughFuel);

            var price = GetPrice();
            var haveEnoughMoney = price.IsEnough(_playerResources);

            if (price.Amount == 0)
                _price.gameObject.SetActive(false);
            else
                _price.Initialize(price, haveEnoughMoney);

            _exploreButton.gameObject.SetActive(_planet.TotalObjectives > _planet.ObjectivesExplored);
            _exploreButton.interactable = haveEnoughMoney && haveEnoughFuel && _playerFleet.ExplorationShip != null;
        }

        private Price GetPrice()
	    {
            if (!_planet.WasExplored) return Price.Premium(0);

	        var price = Mathf.Min(10, 1 + _planet.Level/5);
            return Price.Premium(price);
	    }

        private Planet _planet;
	}
}
