////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;


namespace SA.IOSDeploy {

	public class Menu : EditorWindow {
				
	#if UNITY_EDITOR
		[MenuItem("Window/Stan's Assets/IOS Deploy/Edit Settings" , false, 400)]
		public static void Edit() {
			Selection.activeObject = ISD_Settings.Instance;
		}

		
		[MenuItem("Window/Stan's Assets/IOS Deploy/Documentation/Introduction" , false, 400)]
		public static void ISDSetupPluginSetUp() {
			string url = "https://unionassets.com/ios-deploy/introduction-92";
			Application.OpenURL(url);
		}
	#endif

	}

}
#endif