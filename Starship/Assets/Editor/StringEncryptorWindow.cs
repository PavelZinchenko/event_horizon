using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class StringEncryptorWindow : EditorWindow
{
	[MenuItem("Window/String Encryptor")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(StringEncryptorWindow));
	}
	
	private void OnGUI()
	{
		_normalString = EditorGUILayout.TextField("Encrypt", _normalString);
		// EditorGUILayout.TextField("Result", _normalString.Encrypt());
		_encryptedString = EditorGUILayout.TextField("Decrypt", _encryptedString);
		// EditorGUILayout.TextField("Result", _encryptedString.Decrypt());
	}
	
	private string _normalString = string.Empty;
	private string _encryptedString = string.Empty;
}
