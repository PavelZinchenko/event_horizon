using System.Linq;
using Constructor.Component;
using GameDatabase.Enums;
using Services.Localization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Zenject;

namespace ViewModel
{
	public class ControlsPanel : MonoBehaviour
	{
	    [Inject] private readonly ILocalization _localization;

		public GameObject AutomaticPanel;
		public GameObject KeyBindingLabel;
		public Toggle AutomaticButton;
		public RadioGroupViewModel KeyGroup;
        public RadioGroupViewModel DroneBehaviourGroup;
        public Text EnergyLevelText;
		public Slider EnergyLevelSlider;

		public UnityEvent OnControlsChanged = new UnityEvent();

		public int KeyBinding { get { return _keyBinding; } }
        public int ComponentMode { get { return _componentMode; } }

		public void Clear()
		{
			KeyGroup.Value = _keyBinding = 0;
		}

		public void Initialize(IComponent component, int key, int defaultKey, int componentMode)
		{
		    _suppressEvents = true;

            var activationType = component.ActivationType;
			AutomaticPanel.gameObject.SetActive(false);
			KeyBindingLabel.gameObject.SetActive(false);
			KeyGroup.gameObject.SetActive(false);
			EnergyLevelText.gameObject.SetActive(false);
			EnergyLevelSlider.gameObject.SetActive(false);
            _keyBinding = -1;
		    _componentMode = componentMode;

            DroneBehaviourGroup.gameObject.SetActive(component.DroneBays.Any());
            DroneBehaviourGroup.Value = _componentMode;

            if (activationType == ActivationType.Mixed && key < 0)
			{
				AutomaticPanel.gameObject.SetActive(true);
				AutomaticButton.isOn = true;
				EnergyLevelText.gameObject.SetActive(true);
				var level = -key;
				EnergyLevelSlider.gameObject.SetActive(true);
				EnergyLevelSlider.value = level;
				UpdateLevelText(level);
			}
			else if (activationType == ActivationType.Mixed && key >= 0)
			{
				AutomaticPanel.gameObject.SetActive(true);
				AutomaticButton.isOn = false;
				KeyGroup.gameObject.SetActive(true);
				KeyGroup.Value = _keyBinding = key;
			}
			else if (activationType == ActivationType.Manual)
			{
				AutomaticPanel.gameObject.SetActive(false);
				KeyBindingLabel.gameObject.SetActive(true);
				KeyGroup.gameObject.SetActive(true);
				KeyGroup.Value = _keyBinding = key >= 0 ? key : defaultKey;
            }

		    _suppressEvents = false;
		}
        
        public void OnKeyBindingChanged(int value)
        {
            _keyBinding = value;

            if (!_suppressEvents)
                OnControlsChanged.Invoke();
		}

	    public void OnComponentModeChanged(int value)
	    {
            _componentMode = value;

            if (!_suppressEvents)
                OnControlsChanged.Invoke();
	    }
		
		public void OnActivationModeChanged(bool value)
		{
			KeyGroup.gameObject.SetActive(!value);
			EnergyLevelText.gameObject.SetActive(value);
			EnergyLevelSlider.gameObject.SetActive(value);
			OnKeyBindingChanged(value ? -Mathf.RoundToInt(EnergyLevelSlider.value) : KeyGroup.Value);
		}

		public void OnSliderValueChanged(float value)
		{
			var level = Mathf.RoundToInt(value);
			UpdateLevelText(level);
			OnKeyBindingChanged(-level);
		}

		private void UpdateLevelText(int value)
		{
			EnergyLevelText.text = _localization.GetString("$MinEnergy", Mathf.RoundToInt(value-1)*10);
		}

		private int _keyBinding;
	    private int _componentMode;
	    private bool _suppressEvents;
	}
}
