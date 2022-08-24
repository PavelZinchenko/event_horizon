using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ViewModel
{
	public class RadioGroupViewModel : MonoBehaviour
	{
		public ToggleGroup Group;

		public ValueChangedEvent OnValueChanged = new ValueChangedEvent();

	    [SerializeField] private bool _allowSwitchOff;

		[Serializable]
		public class ValueChangedEvent : UnityEvent<int> {};

	    private void Awake()
	    {
	        if (Group)
	        {
                throw new InvalidOperationException("RadioGroup: toggle group not null - " + gameObject.name);
	        }

	        foreach (Transform child in transform)
	        {

	            var item = child.GetComponent<Toggle>();
	            if (item == null)
	                continue;

                _toggles.Add(item);
	        }

	        _initialized = true;
	    }

	    private void OnEnable()
	    {
	        for (var i = 0; i < _toggles.Count; ++i)
	        {
	            var toggle = _toggles[i];
	            toggle.isOn = _value == i;

	            Subscribe(_toggles[i], i);
	        }

	        if (_value >= _toggles.Count)
                _value = -1;
	    }

	    private void OnDisable()
	    {
	        foreach (var toggle in _toggles)
	            toggle.onValueChanged.RemoveAllListeners();
	    }

        public int Value
		{
			get => _value;
		    set => SetValue(value);
        }

	    private void SetValue(int value)
	    {
	        if (value == _value) return;

	        if (!_initialized)
	        {
	            _value = value;
	            OnValueChanged.Invoke(_value);
	            return;
	        }

	        if (value >= _toggles.Count)
	        {
	            Debug.LogError("RadioGroup: invalid value " + value);
	            return;
	        }

	        if (!gameObject.activeSelf)
	        {
	            OnValueChanged.Invoke(_value);
                return;
	        }

	        var oldValue = _value;
	        _value = value;

            if (_value >= 0 && !_toggles[_value].isOn)
	            _toggles[_value].isOn = true;

	        if (oldValue >= 0 && oldValue < _toggles.Count)
	            _toggles[oldValue].isOn = false;

	        OnValueChanged.Invoke(_value);
	    }

        private void OnItemValueChanged(int id, bool selected)
		{
			if (selected)
			{
			    SetValue(id);
			}
            else if (Value == id)
		    {
                if (_allowSwitchOff)
                    SetValue(-1);
		        else
		            _toggles[Value].isOn = true;
		    }
		}

		private void Subscribe(Toggle toggle, int id)
		{
			toggle.onValueChanged.AddListener(value => OnItemValueChanged(id, value));
		}

	    private bool _initialized;
		private int _value = -1;
	    private readonly List<Toggle> _toggles = new List<Toggle>();
	}
}
