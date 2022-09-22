using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Constructor;
using Constructor.Satellites;
using GameModel;
using GameServices.Player;
using GameStateMachine.States;
using Gui.Constructor;
using Services.Messenger;
using Constructor.Ships;
using Diagnostics;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameServices.Gui;
using Services.Audio;
using Services.Localization;
using Services.Reources;
using UnityEngine.Events;
using Utils;
using Zenject;
using Component = GameDatabase.DataModel.Component;
using ICommand = Gui.Constructor.ICommand;

namespace ViewModel
{
	public class ConstructorViewModel : MonoBehaviour
	{
	    [InjectOptional] private readonly PlayerFleet _playerFleet;
        [InjectOptional] private readonly PlayerInventory _playerInventory;
        [Inject] private readonly Config _config;
	    [Inject] private readonly ExitSignal.Trigger _exitTrigger;
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly GuiHelper _guiHelper;
	    [Inject] private readonly IDatabase _database;
	    [Inject] private readonly IResourceLocator _resourceLocator;
	    [Inject] private readonly ISoundPlayer _soundPlayer;
	    [Inject] private readonly IDebugManager _debugManager;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<IShip>(EventType.ConstructorShipChanged, OnShipSelected);
			messenger.AddListener(EventType.EscapeKeyPressed, OnCancel);
        }

		[SerializeField] public ToggleGroup HeaderButtons;
        [SerializeField] private ComponentList _componentList;
        [SerializeField] public ComponentInfoViewModel ComponentInfo;
        [SerializeField] public FleetPanelViewModel FleetPanel;
        [SerializeField] public StatsPanelViewModel Stats;
        [SerializeField] public ShipLayoutViewModel ShipLayoutViewModel;
        [SerializeField] public ShipLayoutViewModel LeftPlatformLayoutViewModel;
        [SerializeField] public ShipLayoutViewModel RightPlatformLayoutViewModel;
        [SerializeField] public SatellitesPanel SatellitesPanel;
        [SerializeField] public CustomInputField NameText;
        [SerializeField] public CanvasGroup ShipPanel;
        [SerializeField] public GameObject[] DisabledInViewMode;
		[SerializeField] public Toggle ViewModeToggle;
	    [SerializeField] private Button _rollbackButton;
        [SerializeField] private AudioClip _installSound;

        [Serializable]
        public class CommandEvent : UnityEvent<ICommand> { }

        public int ShipSize { get; private set; }

		public IShip Ship { get; private set; }

        public bool IsEditorMode { get; private set; }

	    public int GetDefaultKey(ItemId<Component> componentId)
		{
			var keys = new HashSet<int>();
			var items = ShipLayoutViewModel.Components.
				Concat(LeftPlatformLayoutViewModel.Components).
				Concat(RightPlatformLayoutViewModel.Components);

			foreach (var item in items)
			{
				if (item.Info.CreateComponent(ShipSize).ActivationType == ActivationType.None)
					continue;
				var key = item.KeyBinding;
				if (item.Info.Data.Id == componentId)
					return key;
				keys.Add(key);
			}
			
			for (int i = 0; i < 6; ++i)
				if (!keys.Contains(i))
					return i;
			
			return 0;
		}

		public bool IsUniqueItemInstalled(Component component)
		{
			var key = component.GetUniqueKey();
			if (string.IsNullOrEmpty(key))
				return false;

			var items = ShipLayoutViewModel.Components.
				Concat(LeftPlatformLayoutViewModel.Components).
					Concat(RightPlatformLayoutViewModel.Components);

		    return items.Any(item => item.Info.Data.GetUniqueKey() == key);
		}

		public void OnComponentInstalled(ComponentInfo component)
		{
			_components.Remove(component);
		    var shouldShowComponent = ComponentInfo.gameObject.activeSelf && _components.Items.GetQuantity(component) > 0;

            UpdateStats();
			UpdateComponentList();

			if (shouldShowComponent)
				ShowComponent(component);

            if (_installSound)
                _soundPlayer.Play(_installSound);
		}

		public void OnComponentRemoved(ComponentInfo component)
		{
			_components.Add(component);

			UpdateStats();
			UpdateComponentList();
		}

		public void OnComponentUnlocked(IntegratedComponent component)
		{
		}

		public void OnViewModeSelected(bool selected)
		{
			ShowComponentList();
            ShipPanel.blocksRaycasts = !selected;
			foreach (var item in DisabledInViewMode)
				item.SetActive(!selected);

			var transform = ShipPanel.GetComponent<RectTransform>();
			if (selected)
			{
				var size = transform.rect.size;
				var container = transform.parent.GetComponent<RectTransform>().rect.size;
				transform.localScale = Vector3.one * Mathf.Clamp01(container.y / size.y);
			}
			else
			{
				transform.localScale = Vector3.one;
            }
        }

		public void ShowWeapons()
		{
			OptimizedDebug.Log("ShowWeapons");
            ShowComponentList();
		    _componentList.ShowWeapon();
		}

		public void ShowEnergy()
		{
			OptimizedDebug.Log("ShowEnergy");
		    ShowComponentList();
		    _componentList.ShowEnergy();
		}

		public void ShowDefense()
		{
			OptimizedDebug.Log("ShowDefense");
		    ShowComponentList();
		    _componentList.ShowArmor();
		}

		public void ShowEngine()
		{
			OptimizedDebug.Log("ShowEngine");
		    ShowComponentList();
		    _componentList.ShowEngine();
		}
        
        public void ShowDrones()
		{
			OptimizedDebug.Log("ShowDrones");
		    ShowComponentList();
		    _componentList.ShowDrone();
		}

		public void ShowSpecial()
		{
			OptimizedDebug.Log("ShowSpecial");
		    ShowComponentList();
		    _componentList.ShowSpecial();
		}

        public void ShowAll()
	    {
	        ShowComponentList();
	        _componentList.ShowAll();
        }

        public void OnItemDeselected()
	    {
	        if (!HeaderButtons.AnyTogglesOn())
                ShowAll();
	    }

		public void RemoveAll()
		{
			if (HasUnlockedComponents)
                _guiHelper.ShowConfirmation(_localization.GetString("$RemoveAllConfirmation"), RemoveAllComponents);
		}

	    public void OnCommandExecuted(ICommand command)
	    {
	        _commands.Push(command);
	        _rollbackButton.interactable = true;
	    }

	    public void Undo()
	    {
	        if (_commands.Count > 0)
	        {
	            var command = _commands.Pop();

	            if (!command.TryRollback())
	            {
	                OptimizedDebug.Log("Undo - failed");
                    _commands.Clear();
	            }
	        }

            _rollbackButton.interactable = _commands.Count > 0;
        }
        
		public void Exit()
		{
			if (!this) return;
            SaveCurrentShip();
		    _exitTrigger.Fire();
		}

		public void SaveCurrentShip()
		{
			Ship.Components.Assign(_layout.Components);
			if (!String.IsNullOrEmpty(NameText.text))
				Ship.Name = NameText.text;

            Ship.FirstSatellite?.Components.Assign(_leftPlatformLayout.Components);
            Ship.SecondSatellite?.Components.Assign(_rightPlatformLayout.Components);

            if (!IsEditorMode)
				_components.SaveToInventory(_playerInventory);
        }

        public void ShowSatellites()
        {
            _componentList.gameObject.SetActive(false);
            ComponentInfo.gameObject.SetActive(false);
            FleetPanel.Close();
            SatellitesPanel.Open(Ship, IsEditorMode);
        }

        public void ShowComponent(ComponentInfo component)
		{
			FleetPanel.Close();
            _componentList.gameObject.SetActive(false);
			ComponentInfo.gameObject.SetActive(true);
            ComponentInfo.SetComponent(component);
            SatellitesPanel.gameObject.SetActive(false);
        }

		public void ShowComponent(ShipLayoutViewModel activeLayout, int id)
		{
			FleetPanel.Close();
            _componentList.gameObject.SetActive(false);
			ComponentInfo.gameObject.SetActive(true);
			ComponentInfo.SetComponent(activeLayout, id);
            SatellitesPanel.gameObject.SetActive(false);
        }

        public void ShowComponentList()
		{
			FleetPanel.Close();
            ComponentInfo.gameObject.SetActive(false);
            _componentList.gameObject.SetActive(true);
            SatellitesPanel.gameObject.SetActive(false);
        }

        public void ShowShipList()
		{
	        _componentList.gameObject.SetActive(false);
			ComponentInfo.gameObject.SetActive(false);
			FleetPanel.Open(IsEditorMode);
            SatellitesPanel.gameObject.SetActive(false);
        }

	    public void InstallSatellite(ItemId<SatelliteBuild> buildId, ItemId<Satellite> id, CompanionLocation location)
	    {
            SaveCurrentShip();

            ISatellite satellite = null;
            if (!id.IsNull)
            {
                if (IsEditorMode)
                {
                    var item = _database.GetSatelliteBuild(buildId);
                    if (item == null || !Ship.IsSuitableSatelliteSize(item.Satellite))
                        throw new ArgumentException("cannot install " + buildId + " in " + Ship.Id);

                    satellite = new EditorModeSatellite(item, _database);
                }
                else
                {
                    var item = _database.GetSatellite(id);
                    if (item == null || !Ship.IsSuitableSatelliteSize(item))
                        throw new ArgumentException("cannot install " + id + " in " + Ship.Id);

                    if (_playerInventory.Satellites.Remove(item) < 1)
                        throw new ArgumentException("satellite not found in inventory");

                    satellite = new CommonSatellite(item, Enumerable.Empty<IntegratedComponent>());
                }
            }

	        ISatellite oldValue;
	        if (location == CompanionLocation.Left)
	        {
	            oldValue = Ship.FirstSatellite;
	            Ship.FirstSatellite = satellite;
	        }
	        else
	        {
	            oldValue = Ship.SecondSatellite;
	            Ship.SecondSatellite = satellite;
	        }

            if (oldValue != null && !IsEditorMode)
            {
            	_playerInventory.Satellites.Add(oldValue.Information);
                foreach (var item in oldValue.Components)
                    _playerInventory.Components.Add(item.Info);
            }

            Initialize(Ship);
	    }

        private void OnShipSelected(IShip ship)
        {
            if (Ship != null)
                SaveCurrentShip();

            Ship = ship;
            Initialize(ship);
        }

		private void Initialize(IShip ship)
		{
		    Ship = ship;

		    IsEditorMode = _playerFleet == null || _playerInventory == null || !_playerFleet.Ships.Contains(ship);

		    var debug = _debugManager.CreateLog(ship.Name);
			_layout = new ShipLayout(Ship.Model.Layout, ship.Model.Barrels, ship.Components, debug);
			
			NameText.text = _localization.GetString(ship.Name);

            _leftPlatformLayout = ship.FirstSatellite != null ? new ShipLayout(ship.FirstSatellite.Information.Layout, ship.FirstSatellite.Information.Barrels, ship.FirstSatellite.Components, debug) : null;
            _rightPlatformLayout = ship.SecondSatellite != null ? new ShipLayout(ship.SecondSatellite.Information.Layout, ship.SecondSatellite.Information.Barrels, ship.SecondSatellite.Components, debug) : null;

			ShipSize = Ship.Model.Layout.CellCount;

		    if (IsEditorMode)
				_components.LoadFromDatabase(_database);
		    else
		        _components.LoadFromInventory(_playerInventory);

            _componentList.Initialize(_components.Items);

		    ResetLayout();
			UpdateStats();
		}
		
		private void ResetLayout()
		{
            _commands.Clear();
		    
			ShipLayoutViewModel.Initialize(_layout, _components);
			ShipLayoutViewModel.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Model.ModelImage);
			
			if (_leftPlatformLayout != null)
			{
				LeftPlatformLayoutViewModel.Initialize(_leftPlatformLayout, _components);
				LeftPlatformLayoutViewModel.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.FirstSatellite.Information.ModelImage);
			}
			else
			{
				LeftPlatformLayoutViewModel.Reset();
			}

            if (_rightPlatformLayout != null)
            {
                RightPlatformLayoutViewModel.Initialize(_rightPlatformLayout, _components);
                RightPlatformLayoutViewModel.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.SecondSatellite.Information.ModelImage);
            }
            else
            {
                RightPlatformLayoutViewModel.Reset();
            }
        }

        private bool HasUnlockedComponents
		{
			get
			{
				if (_layout.Components.Any(item => !item.Locked)) return true;
				if (_leftPlatformLayout != null && _leftPlatformLayout.Components.Any(item => !item.Locked)) return true;
				if (_rightPlatformLayout != null && _rightPlatformLayout.Components.Any(item => !item.Locked)) return true;
				return false;
			}
		}

		private void RemoveAllComponents()
		{
            if (IsEditorMode)
			{
                _layout.Clear();
			    _leftPlatformLayout?.Clear();
			    _rightPlatformLayout?.Clear();
			}
			else
			{
				var components = _layout.Components.Where(item => !item.Locked).ToArray();
				foreach (var item in components)
				{
					_components.Add(item.Info);
					_layout.RemoveComponent(item);
				}

				if (_leftPlatformLayout != null)
				{
					components = _leftPlatformLayout.Components.Where(item => !item.Locked).ToArray();
					foreach (var item in components)
					{
						_components.Add(item.Info);
						_leftPlatformLayout.RemoveComponent(item);
					}
				}

				if (_rightPlatformLayout != null)
				{
					components = _rightPlatformLayout.Components.Where(item => !item.Locked).ToArray();
					foreach (var item in components)
					{
						_components.Add(item.Info);
						_rightPlatformLayout.RemoveComponent(item);
					}
				}
			}

			ResetLayout();
			UpdateStats();
			UpdateComponentList();
		}
		
		private void UpdateStats()
		{
			var builder = new ShipBuilder(Ship.Model, _layout.Components);
			if (_leftPlatformLayout != null)
				builder.AddSatellite(new CommonSatellite(Ship.FirstSatellite.Information, _leftPlatformLayout.Components), CompanionLocation.Left);
			if (_rightPlatformLayout != null)
				builder.AddSatellite(new CommonSatellite(Ship.SecondSatellite.Information, _rightPlatformLayout.Components), CompanionLocation.Right);

			Stats.UpdateStats(builder.Build(_database.ShipSettings));
		    _rollbackButton.interactable = _commands.Count > 0;
		}

		private void UpdateComponentList()
		{
            ShowComponentList();
            _componentList.RefreshList();
		}

		private void OnCancel()
        {
            if (!this) return;

			if (ViewModeToggle.isOn)
				ViewModeToggle.isOn = false;
			else
				Exit();
		}

		private readonly InventoryComponents _components = new InventoryComponents();
	    private ShipLayout _layout;
		private ShipLayout _leftPlatformLayout;
		private ShipLayout _rightPlatformLayout;
        private Stack<ICommand> _commands = new Stack<ICommand>();
	}
}
