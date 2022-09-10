using System.Linq;
using Constructor;
using GameServices.Player;
using GameStateMachine.States;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using Gui.ComponentList;
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
	    [Inject] private readonly IDatabase _database;

        public FleetContentFiller FleetFiller;
        public ListScrollRect List;

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
				FleetFiller.SetShips(_database.ShipBuildList.OrderBy(build => build.Ship.Id.Value)
					.Select<ShipBuild, IShip>(build => new EditorModeShip(build, _database)));
			else if (_playerFleet != null)
				FleetFiller.SetShips(_playerFleet.ActiveShipGroup.Ships);
			
			List.MarkForRefresh = true;
		}
    }
}
