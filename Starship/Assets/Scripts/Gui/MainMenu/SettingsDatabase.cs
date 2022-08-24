using System;
using System.Linq;
using GameDatabase;
using GameServices.GameManager;
using GameServices.Gui;
using Services.Localization;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Database = GameDatabase.Database;

namespace Gui.MainMenu
{
    public class SettingsDatabase : MonoBehaviour
    {
        [Inject] private readonly IGameDataManager _gameDataManager;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly GuiHelper _guiHelper;

        [SerializeField] private LayoutGroup _modsGroup;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener(EventType.DatabaseLoaded, OnDatabaseLoaded);
            OnDatabaseLoaded();
        }

        public void OnModSelected(ModInfoItem item)
        {
            if (_database.Id.Equals(item.Id, StringComparison.OrdinalIgnoreCase))
                return;

            _guiHelper.ShowConfirmation(_localization.GetString("$LoadModConfirmation"), () => _gameDataManager.LoadMod(item.Id));
        }

        private void OnDatabaseLoaded()
        {
            UpdateMods();
            _localization.Reload(_database);
        }

        private void OnEnable()
        {
            UpdateMods();
        }

        private void UpdateMods()
        {
            _modsGroup.InitializeElements<ModInfoItem, ModInfo>(ModInfo.Default.ToEnumerable().Concat(_database.AvailableMods), UpdateItem);
        }

        private void UpdateItem(ModInfoItem item, ModInfo mod)
        {
            var isDefault = string.IsNullOrEmpty(mod.Id);
            var name = isDefault ? _localization.GetString("$NoMods") : mod.Name;
            var active = _database.Id.Equals(mod.Id, StringComparison.OrdinalIgnoreCase);
            item.Initialize(name, mod.Id, active);
        }

        private bool _restoreOnNextLogin = false;
    }
}
