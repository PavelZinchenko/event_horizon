using System.Collections.Generic;
using GameServices.Settings;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Utils;
using Zenject;

namespace ViewModel
{
	public class ControlSetupViewModel : UIBehaviour
	{
		public RectTransform ControlsArea;
        public Slider SizeSlider;
		public ToggleGroup ToggleGroup;
		public Image ControlIcon;
		public GameObject EnabledPanel;
		public Toggle EnabledToggle;
		public Toggle SlideToMove;
		public Toggle ThrustWithJoystick;
		public Toggle StopWhenWeaponActive;

		public UnityEvent OnExitEvent = new UnityEvent();

        [Inject] private readonly GameSettings _gameSettings;

        public void OnSelected(ButtonLayoutViewModel control)
		{
			if (_selected != null)
				_selected.Focused = false;
            
            _selected = control;
			control.transform.SetAsLastSibling();
			control.Focused = true;
			UpdateControlInfo();
        }
        
        public void OnSizeSliderValueChanged(float value)
		{
			if (_selected != null)
				_selected.Size = _sizeMin + (_sizeMax - _sizeMin)*value;
		}

		public void OnEnableToggleValueChanged(bool value)
		{
			if (_selected != null)
				_selected.gameObject.SetActive(value);
		}

		public void OnLoadPreset(int index)
		{
			switch (index)
			{
			case 0:
				SlideToMove.isOn = false;
				ThrustWithJoystick.isOn = true;
				StopWhenWeaponActive.isOn = false;
				LoadControlsLayout("Action0,1430,100,180 Action1,1240,100,180 Action2,1050,100,180 Action3,860,100,180 Action4,670,100,180 Action5,480,100,180 Joystick,220,160,300");
				break;
			case 1:
				SlideToMove.isOn = true;
				ThrustWithJoystick.isOn = true;
				StopWhenWeaponActive.isOn = false;
				LoadControlsLayout("Action0,1430,100,180 Action1,1240,100,180 Action2,1050,100,180 Action3,860,100,180 Action4,670,100,180 Action5,480,100,180");
				break;
			case 2:
				SlideToMove.isOn = false;
				ThrustWithJoystick.isOn = false;
				StopWhenWeaponActive.isOn = false;
				LoadControlsLayout("Left,180,120,220 Right,400,120,220 Thrust,1350,170,320 Action0,1150,70,120 Action1,1130,190,120 Action2,1174,305,120 Action3,1274,378,120 Action4,1397,386,120 Action5,1504,330,120");
				break;
			case 3:
				SlideToMove.isOn = false;
				ThrustWithJoystick.isOn = false;
				StopWhenWeaponActive.isOn = false;
				LoadControlsLayout("Joystick,220,160,300 Thrust,1350,170,320 Action0,1150,70,120 Action1,1130,190,120 Action2,1174,305,120 Action3,1274,378,120 Action4,1397,386,120 Action5,1504,330,120");
				break;
			}

			UpdateControlInfo();
		}

        public void Exit()
		{
			SaveControlsLayout();
			OnExitEvent.Invoke();
		}

		public void SaveControlsLayout()
		{
			string layout = string.Empty;
			foreach (Transform child in ControlsArea.transform)
			{
				if (!child.gameObject.activeSelf)
					continue;
				var item = child.GetComponent<ButtonLayoutViewModel>();
				if (!string.IsNullOrEmpty(layout))
					layout += ' ';
				layout += item.name + ',' + Mathf.RoundToInt(item.Position.x) + ',' + Mathf.RoundToInt(item.Position.y) + ',' + Mathf.RoundToInt(item.Size);
			}
			_gameSettings.ControlsLayout = layout;

			_gameSettings.SlideToMove = SlideToMove.isOn;
			_gameSettings.ThrustWidthJoystick = ThrustWithJoystick.isOn;
			_gameSettings.StopWhenWeaponActive = StopWhenWeaponActive.isOn;
        }		

		protected override void OnRectTransformDimensionsChange()
		{
			_sizeMax = 3*GetComponent<RectTransform>().rect.height/5;
			_sizeMin = 0.15f*_sizeMax;
			UpdateControlInfo();
        }

		protected void Update()
		{
			if (!_initialized)
			{
				_initialized = true;
				ToggleGroup.GetComponentInChildren<Toggle>().isOn = true;
				LoadControlsLayout();
				UpdateControlInfo();
			}

			if (Input.GetKeyDown(KeyCode.Escape))
				Exit();
		}

		private void LoadControlsLayout()
		{
			var layoutData = _gameSettings.ControlsLayout;
			if (string.IsNullOrEmpty(layoutData))
			{
				OnLoadPreset(0);
				return;
			}

			LoadControlsLayout(layoutData);
			SlideToMove.isOn = _gameSettings.SlideToMove;
			ThrustWithJoystick.isOn = _gameSettings.ThrustWidthJoystick;
			StopWhenWeaponActive.isOn = _gameSettings.StopWhenWeaponActive;
		}

		private void LoadControlsLayout(string layoutData)
		{
			var buttons = new Dictionary<string, ButtonLayoutViewModel>();
			foreach (Transform item in ControlsArea.transform)
			{
				item.gameObject.SetActive(false);
				buttons.Add(item.name, item.GetComponent<ButtonLayoutViewModel>());
			}
            
			foreach (var layout in layoutData.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries))
			{
				var data = layout.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
				var id = data[0];
				var x = System.Convert.ToSingle(data[1]);
				var y = System.Convert.ToSingle(data[2]);
				var size = System.Convert.ToSingle(data[3]);

				ButtonLayoutViewModel item;
				if (!buttons.TryGetValue(id, out item))
				{
                    OptimizedDebug.Log("Button not found: " + id);
                    continue;
                }

				item.gameObject.SetActive(true);
                item.Size = size;
				item.Position = new Vector2(x,y);
			}
		}

		private void UpdateControlInfo()
		{
			if (_selected == null)
				return;

			ControlIcon.sprite = _selected.Icon.sprite;
			ControlIcon.rectTransform.localRotation = _selected.Icon.rectTransform.localRotation;

            var temp = _selected;
			_selected = null;
            SizeSlider.value = (temp.Size - _sizeMin)/(_sizeMax - _sizeMin);
			EnabledPanel.SetActive(temp.CanBeDisabled);
			EnabledToggle.isOn = temp.gameObject.activeSelf;
			_selected = temp;
        }
        
        private float _sizeMin;
		private float _sizeMax;
		private bool _initialized = false;
        private ButtonLayoutViewModel _selected;
	}
}
