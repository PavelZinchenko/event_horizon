using System;
using Constructor.Ships;
using Constructor.Ships.Modification;
using GameDatabase.DataModel;
using GameServices.Gui;
using GameServices.Player;
using Services.Audio;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Craft
{
    public class ModificationsPanel : MonoBehaviour
    {
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly GuiHelper _guiHelper;
        [Inject] private readonly ModificationFactory _modificationFactory;

        [SerializeField] private Image _shipImage;
        [SerializeField] private ShipModSlot[] _modSlots;
        [SerializeField] private LayoutGroup _modsLayoutGroup;
        [SerializeField] private UnlockSlotPanel _unlockSlotPanel;
        [SerializeField] private GameObject _installPanel;
        [SerializeField] private AudioClip _installSound;

        public void Initialize(IShip ship, int level, Faction faction)
        {
            _level = level;
            _faction = faction;
            _selectedShip = ship;
            _shipImage.gameObject.SetActive(ship != null);
            if (ship != null)
            {
                _shipImage.sprite = _resourceLocator.GetSprite(ship.Model.ModelImage);
                _shipImage.color = ship.ColorScheme.HsvColor;
            }

            InitializeSlots();
            UpdateActions();

            _modSlots[0].Selected = true;
        }

        public void OnSlotSelected(ShipModSlot slot)
        {
            _selectedSlot = slot;
            UpdateActions();
        }

        public void OnSlotDeselected(ShipModSlot slot)
        {
            if (slot == _selectedSlot)
            {
                _selectedSlot = null;
                UpdateActions();
            }
        }

        public void OnSlotUnlocked()
        {
            InitializeSlots();
            UpdateActions();

            _soundPlayer.Play(_installSound);
        }

        public void InstalModification(ShipModListItem item)
        {
            var mod = item.Modification;
            var index = Array.IndexOf(_modSlots, _selectedSlot);
            var price = mod.Type.GetInstallPrice();

            if (_selectedShip == null || index < 0 || mod.Type == _selectedShip.Model.Modifications[index].Type || !mod.Type.IsSuitable(_selectedShip.Model) || !mod.Type.GetInstallPrice().IsEnough(_playerResources))
                return;

            Action action = () => 
            {
                if (!price.TryWithdraw(_playerResources))
                    return;

                _selectedShip.Model.Modifications[index] = _modificationFactory.Create(mod.Type);
                InitializeSlots();
                UpdateActions();

                if (mod.Type != ModificationType.Empty)
                    _soundPlayer.Play(_installSound);
            };

            if (price.Amount > 0)
                _guiHelper.ShowConfirmation(_localization.GetString("$InstallModConfirmation"), price, action);
            else
                _guiHelper.ShowConfirmation(_localization.GetString("$DeleteModConfirmation"), action);
        }

        private void InitializeSlots()
        {
            foreach (var slot in _modSlots)
                slot.gameObject.SetActive(_selectedShip != null);

            if (_selectedShip == null)
                return;

            var enumerator = _selectedShip.Model.Modifications.GetEnumerator();

            foreach (var slot in _modSlots)
            {
                var mod = enumerator.MoveNext() ? enumerator.Current : null;
                slot.Initialize(mod, _resourceLocator);
            }
        }

        private void InitializeMods()
        {
            _modsLayoutGroup.InitializeElements<ShipModListItem, IShipModification>(_modificationFactory.AvailableMods, UpdateModListItem);
        }

        private void UpdateModListItem(ShipModListItem item, IShipModification data)
        {
            var price = data.Type.GetInstallPrice();

            item.Initialize(data, _localization, _resourceLocator, data.Type.IsSuitable(_selectedShip.Model), price.IsEnough(_playerResources));
        }

        private void UpdateActions()
        {
            if (_selectedShip == null || _selectedSlot == null)
            {
                _installPanel.gameObject.SetActive(false);
                _unlockSlotPanel.gameObject.SetActive(false);
            }
            else if (_selectedSlot.Modification != null)
            {
                _unlockSlotPanel.gameObject.SetActive(false);
                _installPanel.gameObject.SetActive(true);
                InitializeMods();
            }
            else
            {
                _unlockSlotPanel.gameObject.SetActive(true);
                _unlockSlotPanel.Initialize(_selectedShip, _level, _faction);
                _installPanel.gameObject.SetActive(false);
            }
        }

        private int _level;
        private Faction _faction;
        private ShipModSlot _selectedSlot;
        private IShip _selectedShip;
    }
}
