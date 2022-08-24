using UnityEngine;

namespace FSG.iOSKeychain
{
	public class KeychainDebug : MonoBehaviour
	{
		string key = "yourKeyHere";
		string value = string.Empty;

		void OnGUI()
		{
			GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
			{
				GUILayout.Label("Key:");
				key = GUILayout.TextField(key);
				GUILayout.Label("Value:");
				value = GUILayout.TextField(value);

				if (GUILayout.Button(string.Format("Set Value For {0}", key), GUILayout.MinHeight(150)))
					Keychain.SetValue(key, value);
				if (GUILayout.Button(string.Format("Get Value For {0}", key), GUILayout.MinHeight(150)))
				{
					value = Keychain.GetValue(key);
					Debug.Log(string.Format("Retrieved Value: {0}", value));
				}
				if (GUILayout.Button(string.Format("Delete Value For {0}", key), GUILayout.MinHeight(150)))
					Keychain.DeleteValue(key);
			}
			GUILayout.EndArea();
		}
	}
}