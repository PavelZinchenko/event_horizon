using System.Linq;
using Constructor;
using GameServices.Player;
using GameStateMachine.States;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ViewModel
{
	public class FleetPanelViewModel : MonoBehaviour
	{
	    [InjectOptional] private readonly PlayerFleet _playerFleet;
	    [Inject] private readonly ShipSelectedSignal.Trigger _shipSelectedTrigger;
        [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly IDatabase _database;
	    [Inject] private readonly IResourceLocator _resourceLocator;

        public LayoutGroup ActiveShipsGroup;

		public void Open(bool isEditorMode)
		{
            gameObject.SetActive(true);
			UpdateActiveShips(isEditorMode);
		}

	    public void Close()
	    {
	        gameObject.SetActive(false);
	    }

		private void UpdateActiveShips(bool isEditorMode)
		{
			if (!gameObject.activeSelf)
				return;

            if (isEditorMode)
                ActiveShipsGroup.InitializeElements<ShipInfoViewModel, IShip>(_database.ShipBuildList.OrderBy(build => build.Ship.Id.Value).Select<ShipBuild,IShip>(build => new EditorModeShip(build, _database)), UpdateShip);
            else if (_playerFleet != null)
			    ActiveShipsGroup.InitializeElements<ShipInfoViewModel, IShip>(_playerFleet.ActiveShipGroup.Ships, UpdateShip);
		}

		private void UpdateShip(ShipInfoViewModel item, IShip ship)
		{
			item.Icon.sprite = _resourceLocator.GetSprite(ship.Model.IconImage) ?? _resourceLocator.GetSprite(ship.Model.ModelImage);
		    item.Icon.color = ship.ColorScheme.HsvColor;
            item.Icon.rectTransform.localScale = 1.4f*ship.Model.IconScale*Vector3.one;
			item.NameText.text = _localization.GetString(ship.Name);
			item.SetLevel(ship.Experience.Level);
		    item.ClassText.text = ship.Model.SizeClass.ToString(_localization);
			item.Button.onClick.RemoveAllListeners();
			item.Button.onClick.AddListener(() => ShipButtonClicked(ship));
		}

		private void ShipButtonClicked(IShip ship)
		{
            _shipSelectedTrigger.Fire(ship);
        }
    }
}
