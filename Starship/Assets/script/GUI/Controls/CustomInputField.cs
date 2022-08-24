using UnityEngine;
using UnityEngine.UI;

public class CustomInputField : InputField
{
	private TouchScreenKeyboard _openedKeyboard;

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	public void OnEndEdit()
	{
		if (TouchScreenKeyboard.isSupported == true)
		{
			text = _openedKeyboard.text;
		}
	}

	public void OnValueChange()
	{
		if (TouchScreenKeyboard.isSupported == true)
		{
			_openedKeyboard = m_Keyboard;
		}
	}
}
