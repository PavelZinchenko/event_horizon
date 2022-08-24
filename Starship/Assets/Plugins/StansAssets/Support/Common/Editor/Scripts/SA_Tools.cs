////////////////////////////////////////////////////////////////////////////////
//  
// @module Stan's Assets Commons Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SA.Common.Editor {

	public class Tools {


		public static void ContactSupportWithSubject(string subject) {
			string url = "mailto:support@stansassets.com?subject=" + EscapeURL(subject);
			Application.OpenURL(url);
		}

		static	string  EscapeURL (string url){
			return WWW.EscapeURL(url).Replace("+","%20");
		}

		private static Texture2D _SALogo = null;
		
		public static Texture2D SALogo {
			get {
				if(_SALogo == null) {

					string path = "Assets/" + SA.Common.Config.SUPPORT_MODULS_PATH + "Common/Editor/Content/";

                    if (EditorGUIUtility.isProSkin) {
                        path = path + "sa_logo_small.png";
                    } else {
                        path = path + "sa_logo_small_light.png";
                    }

                    _SALogo = EditorIcon.GetIconAtPath(path);
				} 
				
				return _SALogo;
			}
		}



		//--------------------------------------
		// App info
		//--------------------------------------


		public static string ApplicationIdentifier {

			get {
				#if UNITY_5_6_OR_NEWER
				return PlayerSettings.applicationIdentifier;
				#else
				return PlayerSettings.bundleIdentifier;
				#endif
			}

			set {
				#if UNITY_5_6_OR_NEWER
				PlayerSettings.applicationIdentifier = value;
				#else
				PlayerSettings.bundleIdentifier = value;
				#endif
			}
		}



		public static Texture2D GetEditorTexture(string path) {
			path = "Assets/" + path;
			TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
			importer.textureType = TextureImporterType.GUI;
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

			return  AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
		}
		
		public static void DrawSALogo() {
			
			GUIStyle s =  new GUIStyle();
			GUIContent content =  new GUIContent(SALogo, "Visit site");
			
			bool click = GUILayout.Button(content, s);
			if(click) {
				Application.OpenURL(SA.Common.Config.WEBSITE_ROOT_URL);
			}
		}


		public static void DrawSeparatorLine()  {
			GUI.enabled = false ;
			EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
			GUI.enabled = true;
		}


		public static bool ToggleFiled(string title, bool value, string tooltip = "") {
			return ToggleFiled (new GUIContent (title, tooltip), value);
		}


		public static bool ToggleFiled(GUIContent title, bool value) {
			
			Bool initialValue = Bool.Enabled;
			if(!value) {
				initialValue = Bool.Disabled;
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(title);
			
			initialValue = (Bool) EditorGUILayout.EnumPopup(initialValue);
			if(initialValue == Bool.Enabled) {
				value = true;
			} else {
				value = false;
			}
			EditorGUILayout.EndHorizontal();
			
			return value;
		}

		public static bool YesNoFiled(string title, bool value, string tooltip = "") {
			return YesNoFiled (new GUIContent (title, tooltip), value);
		}

		public static bool YesNoFiled(GUIContent title, bool value) {

			SA_YesNoBool initialValue = SA_YesNoBool.Yes;
			if(!value) {
				initialValue = SA_YesNoBool.No;
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(title);

			initialValue = (SA_YesNoBool) EditorGUILayout.EnumPopup(initialValue);
			if(initialValue == SA_YesNoBool.Yes) {
				value = true;
			} else {
				value = false;
			}
			EditorGUILayout.EndHorizontal();

			return value;
		}


		public static  System.Enum EnumPopup(string title, System.Enum selected, string tooltip = "") {
			GUIContent c =  new GUIContent(title, tooltip);
			return EnumPopup(c, selected);
		}

		public static  System.Enum EnumPopup(GUIContent title, System.Enum selected) {

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(title);
			System.Enum value = EditorGUILayout.EnumPopup (selected);
			EditorGUILayout.EndHorizontal();

			return value;

		}
			


		public static string TextField(string title, string value, string tooltip = "") {
			GUIContent c =  new GUIContent(title, tooltip);
			return TextField(c, value);
		}

		public static string TextField(GUIContent title, string value) {


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(title);
			value	 	= EditorGUILayout.TextField(value);
			if(value.Length > 0) {
				value 		= value.Trim();
			}
			EditorGUILayout.EndHorizontal();

			return value;

		}

		public static void LabelField(string title, string message) {
			GUIContent c =  new GUIContent(title, "");
			LabelField (c, message);
		}

		public static void LabelField(GUIContent label, string message) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(label);
			EditorGUILayout.LabelField(message);
			EditorGUILayout.EndHorizontal();
		}


		public static void SelectableLabel(string title, string message) {
			GUIContent c =  new GUIContent(title, "");
			SelectableLabel (c, message);
		}

		public static void SelectableLabel(GUIContent label, string message) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
			EditorGUILayout.SelectableLabel(message, GUILayout.Height(16));
			EditorGUILayout.EndHorizontal();
		}

		public static void SelectableLabelField(GUIContent label, string message) {
			SelectableLabel (label, message);
		}



		public static bool SrotingButtons(object currentObject, IList ObjectsList) {
			
			int ObjectIndex = ObjectsList.IndexOf(currentObject);
			if(ObjectIndex == 0) {
				GUI.enabled = false;
			} 
			
			bool up 		= GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
			if(up) {
				object c = currentObject;
				ObjectsList[ObjectIndex]  		= ObjectsList[ObjectIndex - 1];
				ObjectsList[ObjectIndex - 1] 	=  c;
			}
			
			
			if(ObjectIndex >= ObjectsList.Count -1) {
				GUI.enabled = false;
			} else {
				GUI.enabled = true;
			}
			
			bool down 		= GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20));
			if(down) {
				object c = currentObject;
				ObjectsList[ObjectIndex] =  ObjectsList[ObjectIndex + 1];
				ObjectsList[ObjectIndex + 1] = c;
			}
			
			
			GUI.enabled = true;
			bool r 			= GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20));
			if(r) {
				ObjectsList.Remove(currentObject);
			}
			
			return r;
		}



		public static void ChnageDefineState(string file, string tag, bool IsEnabled) {
			if(SA.Common.Util.Files.IsFileExists(file)) {
				string content = SA.Common.Util.Files.Read(file);
				//	Debug.Log(file);
				//Debug.Log(content);
				
				int endlineIndex;
				endlineIndex = content.IndexOf(System.Environment.NewLine);
				if(endlineIndex == -1) {
					endlineIndex = content.IndexOf("\n");
				}
				string TagLine = content.Substring(0, endlineIndex);
				
				if(IsEnabled) {
					content 	= content.Replace(TagLine, "#define " + tag);
				} else {
					content 	= content.Replace(TagLine, "//#define " + tag);
				}
				//		Debug.Log(content);
				
				SA.Common.Util.Files.Write(file, content);
			}		
		}


		public static void BlockHeader(string header) {
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox(header, MessageType.None);
			EditorGUILayout.Space();
		}


		private static GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical questions, feel free to drop us an e-mail");
		public static void SupportMail() {
			SelectableLabelField(SupportEmail, SA.Common.Config.SUPPORT_EMAIL);
		}

		private static GUIContent FBdkVersion   	= new GUIContent("Facebook SDK Version [?]", "Version of Unity Facebook SDK Plugin");
		public static void FBSdkVersionLabel () {

			string SdkVersionCode = SA.Common.Config.FB_SDK_VersionCode;




			SelectableLabelField(FBdkVersion,  SdkVersionCode);
		}


	}

}

#endif
