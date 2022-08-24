using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Constructor;
using GameDatabase.Enums;

namespace ViewModel
{
	public class WeaponFilterGroup : MonoBehaviour
	{
		public ToggleGroup Group;
		
		public ValueChangedEvent OnValueChanged = new ValueChangedEvent();
		
		[Serializable]
		public class ValueChangedEvent : UnityEvent<WeaponSlotType> {};
		
		public WeaponSlotType Value { get { return _value; } }
		
		private void OnItemValueChanged(bool selected)
		{
			var activeItem = Group.ActiveToggles().FirstOrDefault();
			_value = activeItem == null ? WeaponSlotType.Default : (WeaponSlotType)activeItem.name.First();
			OnValueChanged.Invoke(_value);
		}
		
		private void Awake()
		{
			int id = 0;
			foreach (Transform child in Group.transform)
			{
				
				var item = child.GetComponent<Toggle>();
				if (item == null)
					continue;
				
				Subscribe(item, id++);
			}
		}
		
		private void Subscribe(Toggle toggle, int id)
		{
			toggle.onValueChanged.RemoveAllListeners();
			toggle.onValueChanged.AddListener(OnItemValueChanged);
		}
		
		private WeaponSlotType _value = WeaponSlotType.Default;
	}
}
