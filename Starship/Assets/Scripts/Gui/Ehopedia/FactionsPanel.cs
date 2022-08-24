using Domain.Player;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Extensions;
using Services.Localization;
using Services.ObjectPool;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Ehopedia
{
    public class FactionsPanel : MonoBehaviour
    {
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly GameObjectFactory _factory;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly StarMapManager _starMapManager;

        [SerializeField] private LayoutGroup FactionsLayout;

        [SerializeField] private Image _factionIcon;
        [SerializeField] private Text _factionName;
        [SerializeField] private Text _description;
        [SerializeField] private Text _starbaseDistance;
        [SerializeField] private Text _shipsDistance;

        public void Initialize()
        {
            FactionsLayout.InitializeElements<FactionListItem, Faction>(_database.FactionList.Visible(), UpdateFaction, _factory);
        }

        private void UpdateFaction(FactionListItem item, Faction faction)
        {
            item.Initialize(faction, _starMapManager.IsFactionUnlocked(faction) || true, _localization);
        }

        public void OnFactionSelected(FactionListItem item)
        {
            _factionName.text = _localization.GetString(item.Faction.Name);
        }
    }
}
