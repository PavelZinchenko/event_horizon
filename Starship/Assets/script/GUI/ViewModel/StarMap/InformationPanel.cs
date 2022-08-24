using System.Linq;
using Galaxy;
using Game.Exploration;
using GameDatabase.DataModel;
using UnityEngine;
using UnityEngine.UI;
using GameServices.Player;
using Gui.Exploration;
using Gui.StarMap;
using Services.Gui;
using Services.Localization;
using Services.Messenger;
using Services.ObjectPool;
using Services.Reources;
using Zenject;

namespace ViewModel
{
	public class InformationPanel : MonoBehaviour
	{
	    [Inject] private readonly MotherShip _motherShip;
	    [Inject] private readonly Planet.Factory _planetFactory;
	    [Inject] private readonly GameObjectFactory _factory;
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly IMessenger _messenger;
	    [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private Text NameText;
        [SerializeField] private Text FactionNameText;
        [SerializeField] private LayoutGroup Planets;
        [SerializeField] private InputField Bookmark;
        [SerializeField] private LayoutGroup ObjectsGroup;

        public void OnBookmarkChanged(string value)
		{
			if (_suppressBookmarkChangeEvent) return;

			var star = _motherShip.CurrentStar;
			star.Bookmark = value;
		}

		public void Initialize(WindowArgs args)
		{
			var star = _motherShip.CurrentStar;

			NameText.text = star.Name;

			_suppressBookmarkChangeEvent = true;
			Bookmark.text = star.Bookmark;
			_suppressBookmarkChangeEvent = false;

			var faction = star.Region.Faction;
			if (faction != Faction.Neutral)
			{
				FactionNameText.gameObject.SetActive(true);
				FactionNameText.color = faction.Color;
				FactionNameText.text = _localization.GetString(faction.Name);
			}
			else
			{
				FactionNameText.gameObject.SetActive(false);
			}

			Planets.InitializeElements<PlanetInfo, Planet>(_planetFactory.CreatePlanets(star.Id), UpdatePlanetInfo, _factory);
		    ObjectsGroup.InitializeElements<StarSystemObjectItem, StarObjectType>(star.Objects.ToEnumerable().Where(item => item.IsActive(star)), UpdateStarObject);
		}

		private void UpdatePlanetInfo(PlanetInfo planet, Planet model)
		{
			planet.UpdatePlanet(model);
		}

        private void UpdateStarObject(StarSystemObjectItem item, StarObjectType type)
        {
            item.Initialize(_motherShip.CurrentStar, type, _messenger, _localization, _resourceLocator);
        }

        private bool _suppressBookmarkChangeEvent = false;
	}
}
