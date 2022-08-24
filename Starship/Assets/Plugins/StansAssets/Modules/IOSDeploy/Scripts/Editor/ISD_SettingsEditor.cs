#define DISABLED


////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////
using System.Linq;
using System.IO;


#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;
using SA.Common;


namespace SA.IOSDeploy {

	[CustomEditor(typeof(ISD_Settings))]
	public class SettingsEditor : UnityEditor.Editor {

		private static string NewPlistValueName 	= string.Empty;
		private static string NewLanguage 			= string.Empty;
		private static string NewValueName 			= string.Empty;



		public static int NewBaseFrameworkIndex = 0;
		public static int NewLibraryIndex = 0;

		private static bool GUI_ENABLED  = true;

		private static GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	

		private int _Width = 500;
		public int Width {
			get {
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				Rect scale = GUILayoutUtility.GetLastRect();

				if(scale.width != 1) {
					_Width = System.Convert.ToInt32(scale.width);
				}

				return _Width;
			}
		}
			




		private Texture[] _ToolbarImages = null;

		public Texture[] ToolbarImages {
			get {
				if(_ToolbarImages == null) {
					Texture2D buildSettings 	= SA.Common.Editor.Tools.GetEditorTexture ("Plugins/StansAssets/Modules/IOSDeploy/Scripts/Editor/Icons/BuildSettings.png"); 
					Texture2D framework		 	= SA.Common.Editor.Tools.GetEditorTexture ("Plugins/StansAssets/Modules/IOSDeploy/Scripts/Editor/Icons/frameworks.png");
					Texture2D languages 		= SA.Common.Editor.Tools.GetEditorTexture ("Plugins/StansAssets/Modules/IOSDeploy/Scripts/Editor/Icons/languages.png");
					Texture2D libraries 		= SA.Common.Editor.Tools.GetEditorTexture ("Plugins/StansAssets/Modules/IOSDeploy/Scripts/Editor/Icons/Libraries.png");
					Texture2D linkerFlags 		= SA.Common.Editor.Tools.GetEditorTexture ("Plugins/StansAssets/Modules/IOSDeploy/Scripts/Editor/Icons/linkerFlags.png");		
					Texture2D plistVariables 	= SA.Common.Editor.Tools.GetEditorTexture ("Plugins/StansAssets/Modules/IOSDeploy/Scripts/Editor/Icons/plistVariables.png");

					List<Texture2D> textures =  new List<Texture2D>();
					textures.Add(buildSettings);
					textures.Add(framework);
					textures.Add(libraries);
					textures.Add(linkerFlags);
					textures.Add(plistVariables);
					textures.Add(languages);

					_ToolbarImages = textures.ToArray();

				}
				return _ToolbarImages;
			}
		}

		public override void OnInspectorGUI () {
			GUI.changed = false;
			EditorGUILayout.LabelField("IOS Deploy Settings", EditorStyles.boldLabel);
			EditorGUILayout.Space();


			GUI.SetNextControlName ("toolbar");
		
			GUILayoutOption[] toolbarSize = new GUILayoutOption[]{GUILayout.Width(Width-10), GUILayout.Height(35)};
			ISD_Settings.Instance.ToolbarIndex = GUILayout.Toolbar(ISD_Settings.Instance.ToolbarIndex, ToolbarImages, toolbarSize);





#if DISABLED
GUI_ENABLED = false;
#else
GUI_ENABLED = true;
#endif

			GUI.enabled = GUI_ENABLED;
			EditorGUILayout.Space();
			switch (ISD_Settings.Instance.ToolbarIndex) {
			case 0:
				//"Build Settings"
				BuildSettings ();
				break;
			case 1:
				Frameworks();
				break;
			case 2:
				Library ();
				break;
			case 3:
				Flags();
				break;
			case 4: 
				PlistValues ();
				break;

			case 5: 
				LanguageValues();
				break;
			case 6:

				break;
			default:

				break;

			}

			EditorGUILayout.Space();
			//GUILayout.FlexibleSpace();
			AboutGUI();


			if(GUI.changed) {
				DirtyEditor();
			}
		}

		public static void BuildSettings(){

			SA.Common.Editor.Tools.BlockHeader ("BUILD SETTINGS");

			ISD_Settings.Instance.enableBitCode 		= SA.Common.Editor.Tools.YesNoFiled ("ENABLE_BITCODE", ISD_Settings.Instance.enableBitCode);
			ISD_Settings.Instance.enableTestability 	= SA.Common.Editor.Tools.YesNoFiled ("ENABLE_TESTABILITY", ISD_Settings.Instance.enableTestability);
			ISD_Settings.Instance.generateProfilingCode = SA.Common.Editor.Tools.YesNoFiled ("GENERATE_PROFILING_CODE", ISD_Settings.Instance.generateProfilingCode);

		}

		private static bool toggle(GUIContent title, bool value){
			SA.Common.Editor.SA_YesNoBool initialValue = SA.Common.Editor.SA_YesNoBool.Yes;
			if(!value) {
				initialValue = SA.Common.Editor.SA_YesNoBool.No;
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Optional");

			initialValue = (SA.Common.Editor.SA_YesNoBool) EditorGUILayout.EnumPopup(initialValue);
			if(initialValue == SA.Common.Editor.SA_YesNoBool.Yes) {
				return  true;
			} else {
				return  false;
			}

		}

		public static void Frameworks() {

			SA.Common.Editor.Tools.BlockHeader ("FRAMEWORKS");

			EditorGUILayout.BeginHorizontal();
			EditorGUI.indentLevel++;
			ISD_Settings.Instance.IsDefFrameworksOpen = EditorGUILayout.Foldout(ISD_Settings.Instance.IsDefFrameworksOpen, "Default Unity Frameworks (17 Enabled)");
			EditorGUI.indentLevel--;
			EditorGUILayout.EndHorizontal();

			if (ISD_Settings.Instance.IsDefFrameworksOpen) {

				EditorGUILayout.BeginVertical (GUI.skin.box);
				foreach (Framework framework in ISD_FrameworkHandler.DefaultFrameworks) {
					SA.Common.Editor.Tools.SelectableLabel (framework.Type.ToString () + ".framework", "");
					//AvailableFrameworks;
				}
				EditorGUILayout.EndVertical ();


				EditorGUILayout.Space ();
			}


			EditorGUILayout.Space ();

			GUI.enabled = false ;
			EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
			GUI.enabled = GUI_ENABLED;


			EditorGUILayout.LabelField ("Custom IOS Frameworks", EditorStyles.boldLabel);
			if (ISD_Settings.Instance.Frameworks.Count == 0) {
				EditorGUILayout.HelpBox ("No Frameworks Added", MessageType.None);
			}else{



				foreach (Framework framework in ISD_Settings.Instance.Frameworks) {
					EditorGUILayout.BeginVertical (GUI.skin.box);
					EditorGUI.indentLevel++;
					EditorGUILayout.BeginHorizontal ();

					framework.IsOpen = EditorGUILayout.Foldout(framework.IsOpen, framework.TypeString);
					if (framework.IsOptional && framework.IsEmbeded) {
						EditorGUILayout.LabelField("(Optional & Embeded)");
					} else if (framework.IsOptional) {
						EditorGUILayout.LabelField ("(Optional)");
					} else if (framework.IsEmbeded) {
						EditorGUILayout.LabelField("(Embeded)");
					}



					if(framework.IsEmbeded) {
						
					}

					bool ItemWasRemoved = SA.Common.Editor.Tools.SrotingButtons ((object)framework, ISD_Settings.Instance.Frameworks);
					if (ItemWasRemoved) {
						return;
					}
					EditorGUILayout.EndHorizontal ();

					if (framework.IsOpen) {
						EditorGUI.indentLevel++;
						framework.IsOptional = SA.Common.Editor.Tools.YesNoFiled ("Optional", framework.IsOptional);
						framework.IsEmbeded = SA.Common.Editor.Tools.YesNoFiled ("Embeded", framework.IsEmbeded);
						EditorGUILayout.Space ();
						EditorGUI.indentLevel--;
					}
					EditorGUI.indentLevel--;
					EditorGUILayout.EndVertical ();
				}

			}


			EditorGUILayout.BeginHorizontal();

			EditorStyles.popup.fixedHeight = 20;

			NewBaseFrameworkIndex = EditorGUILayout.Popup(NewBaseFrameworkIndex, ISD_FrameworkHandler.BaseFrameworksArray());

			if(GUILayout.Button("Add Framework",  GUILayout.Height(20))) {
				var type = ISD_FrameworkHandler.BaseFrameworksArray () [NewBaseFrameworkIndex];
				NewBaseFrameworkIndex = 0;

				Framework f =  new Framework(type);
				ISD_Settings.Instance.Frameworks.Add(f);
			}

			//EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();


			//EditorGUILayout.Space();
			DrawSeparatorLine ();

				

			List<string> foundedFrameworks = ISD_FrameworkHandler.GetImportedFrameworks ();
			if (foundedFrameworks.Count > 0) {
				EditorGUILayout.LabelField("Frameworks Inide Current Project", EditorStyles.boldLabel);

				EditorGUILayout.BeginVertical (GUI.skin.box);
				GUI.enabled = false;
				foreach (string s in foundedFrameworks) {
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField (s);
					EditorGUILayout.EndHorizontal ();
				}
				GUI.enabled = GUI_ENABLED;
				EditorGUILayout.EndVertical ();
			} else {
				//EditorGUILayout.HelpBox ("Not found any custom \".framework\" in project", MessageType.None);
			}
				
		}


		public static void Library () {
			SA.Common.Editor.Tools.BlockHeader ("LIBRARIES");


			EditorGUILayout.BeginHorizontal();
			EditorGUI.indentLevel++;
			ISD_Settings.Instance.IsDefLibrariesOpen = EditorGUILayout.Foldout(ISD_Settings.Instance.IsDefLibrariesOpen, "Default Unity Libraries (2 Enabled)");
			EditorGUI.indentLevel--;
			EditorGUILayout.EndHorizontal();

			if (ISD_Settings.Instance.IsDefLibrariesOpen) {

				EditorGUILayout.BeginVertical (GUI.skin.box);

				SA.Common.Editor.Tools.SelectableLabel ("libiPhone-lib.a", "");
				SA.Common.Editor.Tools.SelectableLabel ("libiconv.2.dylib", "");

				EditorGUILayout.EndVertical ();


				EditorGUILayout.Space ();
			}
			EditorGUILayout.Space ();
			DrawSeparatorLine ();


			EditorGUILayout.LabelField("Custom Libraries", EditorStyles.boldLabel);	
			if (ISD_Settings.Instance.Libraries.Count == 0) {
				EditorGUILayout.HelpBox("No Libraries added", MessageType.None);
			}


			EditorGUI.indentLevel++; {
				foreach(Lib lib in ISD_Settings.Instance.Libraries) {	
					EditorGUILayout.BeginVertical (GUI.skin.box);

					EditorGUILayout.BeginHorizontal();
					lib.IsOpen = EditorGUILayout.Foldout(lib.IsOpen, lib.Name);
					if(lib.IsOptional) {
						EditorGUILayout.LabelField("(Optional)");
					}

					bool ItemWasRemoved = SA.Common.Editor.Tools.SrotingButtons((object) lib, ISD_Settings.Instance.Libraries);
					if(ItemWasRemoved) {
						return;
					}					
					EditorGUILayout.EndHorizontal();

					if(lib.IsOpen) {
						lib.IsOptional = SA.Common.Editor.Tools.YesNoFiled ("Optional", lib.IsOptional);
						EditorGUILayout.Space ();
					
					}


					EditorGUILayout.EndVertical ();					
				}
			}EditorGUI.indentLevel--;

			//EditorGUILayout.Space();


			EditorGUILayout.BeginHorizontal ();
			EditorStyles.popup.fixedHeight = 20;
			NewLibraryIndex = EditorGUILayout.Popup(NewLibraryIndex, ISD_LibHandler.BaseLibrariesArray());

			if(GUILayout.Button("Add Library",  GUILayout.Height(20))) {
				iOSLibrary type = (iOSLibrary) ISD_LibHandler.enumValueOf( ISD_LibHandler.BaseLibrariesArray()[NewLibraryIndex]);

				NewLibraryIndex = 0;
				Debug.Log ("Adding new library with type  = " + type.ToString() );
				ISD_Settings.Instance.AddLibrary(type);
			}
				
			EditorGUILayout.EndHorizontal();



			DrawSeparatorLine ();



			List<string> foundedLibraries = ISD_FrameworkHandler.GetImportedLibraries ();
			if (foundedLibraries.Count > 0) {
				EditorGUILayout.LabelField("Libraries Inide Current Project", EditorStyles.boldLabel);
				EditorGUILayout.BeginVertical (GUI.skin.box);
				foreach (string s in foundedLibraries) {
					SA.Common.Editor.Tools.SelectableLabel (s, "");
				}
				EditorGUILayout.EndVertical ();
			} 

		}


		public static void Flags() {
			SA.Common.Editor.Tools.BlockHeader ("LINKER FLAGS");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("FLAGS", EditorStyles.boldLabel);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			if (ISD_Settings.Instance.Flags.Count == 0) {				
				EditorGUILayout.HelpBox("No Linker Flags added", MessageType.None);
			}

			EditorGUI.indentLevel++; {
				foreach (Flag flag in ISD_Settings.Instance.Flags) {		



					EditorGUILayout.BeginVertical (GUI.skin.box);
					EditorGUILayout.BeginHorizontal ();
					//EditorGUILayout.LabelField(flag.Name);




					flag.IsOpen = EditorGUILayout.Foldout (flag.IsOpen, flag.Name);
					if (flag.Type.Equals (FlagType.CompilerFlag)) {
						EditorGUILayout.LabelField ("(Compiler)");
					} else {
						EditorGUILayout.LabelField ("(Linker)");
					}

					bool pressed = GUILayout.Button ("x", EditorStyles.miniButton, GUILayout.Width (20));
					if (pressed) {
						ISD_Settings.Instance.Flags.Remove (flag);
						return;
					}			
					EditorGUILayout.EndHorizontal ();

					if(flag.IsOpen) {
						EditorGUILayout.Space ();		
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Full Name:", GUILayout.Width (100));
						flag.Name = EditorGUILayout.TextField (flag.Name, GUILayout.ExpandWidth (true));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Type:", GUILayout.Width (100));
						//flag.Type	 	= EditorGUILayout.TextField(flag.Type, GUILayout.ExpandWidth(true));
						flag.Type = (FlagType)EditorGUILayout.EnumPopup (flag.Type);
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.Space ();

					}


					//EditorGUILayout.Space();




					EditorGUILayout.EndVertical ();				
				}
			}EditorGUI.indentLevel--;


			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Add New Flag");

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add New Flag",  GUILayout.Width(250))) {
				Flag newFlag = new Flag ();
				newFlag.Name = "newFlag";
				ISD_Settings.Instance.Flags.Add(newFlag);
				//NewFlag = string.Empty;
			}
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();
		}

		public static void PlistValues ()	{

			SA.Common.Editor.Tools.BlockHeader ("PLIST VALUES");



			EditorGUI.indentLevel++; {	
				foreach(Variable var in ISD_Settings.Instance.PlistVariables) {
					EditorGUILayout.BeginVertical (GUI.skin.box);
					DrawPlistVariable (var, (object) var, ISD_Settings.Instance.PlistVariables);
					EditorGUILayout.EndVertical ();

					if(!ISD_Settings.Instance.PlistVariables.Contains(var)) {
						return;
					}

				}
				EditorGUILayout.Space();
			} EditorGUI.indentLevel--;


			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(" New Variable Name");
			NewPlistValueName = EditorGUILayout.TextField(NewPlistValueName);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if (NewPlistValueName.Length > 0) {
					Variable var = new Variable ();
					var.Name = NewPlistValueName;
					ISD_Settings.Instance.AddVariable(var);					
				}
				NewPlistValueName = string.Empty;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical ();



            SA.Common.Editor.Tools.BlockHeader("FILES");
            EditorGUI.indentLevel++;
            {
                foreach (AssetFile file in ISD_Settings.Instance.Files) {
                    bool removed = DrawAssetFile(file, (object)file, ISD_Settings.Instance.Files);
                    if(removed) {
                        break;
                    }
                }
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            bool addFile = GUILayout.Button("Add", GUILayout.Width(100));
            if (addFile) {
                ISD_Settings.Instance.Files.Add(new AssetFile());
            }
            EditorGUILayout.EndHorizontal();

		}


        public static bool DrawAssetFile(AssetFile file, object valuePointer, IList valueOrigin) {
           
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();

            file.IsOpen = EditorGUILayout.Foldout(file.IsOpen, file.XCodeRelativePath);
            bool ItemWasRemoved = SA.Common.Editor.Tools.SrotingButtons(valuePointer, valueOrigin);

            EditorGUILayout.EndHorizontal();

            if (file.IsOpen) {
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Asset: ");
                    file.Asset = EditorGUILayout.ObjectField(file.Asset, typeof(UnityEngine.Object), false);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("XCode Path:");
                    file.XCodePath = EditorGUILayout.TextField(file.XCodePath);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            return ItemWasRemoved;

        }


		public static void DrawPlistVariable(Variable var, object valuePointer, IList valueOrigin) {
			EditorGUILayout.BeginHorizontal();

			if(var.Name.Length > 0) {
				var.IsOpen = EditorGUILayout.Foldout(var.IsOpen, var.Name + "   (" + var.Type.ToString() +  ")");
			} else {
				var.IsOpen = EditorGUILayout.Foldout(var.IsOpen, var.Type.ToString());
			}



			bool ItemWasRemoved = SA.Common.Editor.Tools.SrotingButtons (valuePointer, valueOrigin);
			if(ItemWasRemoved) {
				ISD_Settings.Instance.RemoveVariable (var, valueOrigin);
				return;
			}
			EditorGUILayout.EndHorizontal();

			if(var.IsOpen) {						
				EditorGUI.indentLevel++; {

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Type");
					if (var.ChildrensIds.Count > 0) {
						GUI.enabled = false;
						var.Type = (PlistValueTypes)EditorGUILayout.EnumPopup (var.Type);
						GUI.enabled = GUI_ENABLED;
					} else {
						var.Type = (PlistValueTypes)EditorGUILayout.EnumPopup (var.Type);
					}
					EditorGUILayout.EndHorizontal();


					if (var.Type == PlistValueTypes.Array) {
						DrawArrayValues (var);
					} else if (var.Type == PlistValueTypes.Dictionary) {
						DrawDictionaryValues (var);
					} else if (var.Type == PlistValueTypes.Boolean) {
						var.BooleanValue = SA.Common.Editor.Tools.YesNoFiled ("Value", var.BooleanValue);

					} else {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Value");								
						switch(var.Type) {
						case PlistValueTypes.Float:
							var.FloatValue = EditorGUILayout.FloatField(var.FloatValue);
							break;									
						case PlistValueTypes.Integer:
							var.IntegerValue = EditorGUILayout.IntField (var.IntegerValue);
							break;									
						case PlistValueTypes.String:
							var.StringValue = EditorGUILayout.TextField (var.StringValue);
							break;
						}
						EditorGUILayout.EndHorizontal();
					}

				} EditorGUI.indentLevel--;
			}

		}


		public static void DrawArrayValues (Variable var) {


			var.IsListOpen = EditorGUILayout.Foldout (var.IsListOpen, "Array Values (" + var.ChildrensIds.Count + ")");

			if (var.IsListOpen) {		

				EditorGUI.indentLevel++; {

					foreach	(string uniqueKey in var.ChildrensIds) {
						Variable v = ISD_Settings.Instance.getVariableByKey(uniqueKey);
						DrawPlistVariable (v, uniqueKey, var.ChildrensIds);

						if(!var.ChildrensIds.Contains(uniqueKey)) {
							return;
						}
					}


					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.Space ();
					if (GUILayout.Button ("Add Value", GUILayout.Width (100))) {
						Variable newVar = new Variable();

						var.AddChild (newVar);
					}
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.Space ();

				} EditorGUI.indentLevel--;
			} 
		}

		public static void DrawDictionaryValues (Variable var) {
			var.IsListOpen = EditorGUILayout.Foldout (var.IsListOpen, "Dictionary Values");

			if (var.IsListOpen) {

				EditorGUI.indentLevel++; {

					foreach	(string uniqueKey in var.ChildrensIds) {
						Variable v = ISD_Settings.Instance.getVariableByKey(uniqueKey);
						DrawPlistVariable (v, uniqueKey, var.ChildrensIds);

						if(!var.ChildrensIds.Contains(uniqueKey)) {
							return;
						}
					}


					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.PrefixLabel ("New Key");
					NewValueName = EditorGUILayout.TextField (NewValueName);

					if (GUILayout.Button ("Add", GUILayout.Width (50))) {
						if (NewValueName.Length > 0) {
							Variable v = new Variable ();
							v.Name = NewValueName;
							var.AddChild (v);									
						}
					}

					EditorGUILayout.EndHorizontal ();
				} EditorGUI.indentLevel--;
			} 

		}

		public static void DrawSeparatorLine()  {
			GUI.enabled = false ;
			EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
			GUI.enabled = GUI_ENABLED;
		}



		public static void LanguageValues ()	{
			//			ISD_Settings.Instance.IsLanguageSettingOpen = EditorGUILayout.Foldout(ISD_Settings.Instance.IsLanguageSettingOpen, "Languages");
			//
			//			if(ISD_Settings.Instance.IsLanguageSettingOpen)	 {
			if (ISD_Settings.Instance.langFolders.Count == 0)	{
				EditorGUILayout.HelpBox("No Languages added", MessageType.None);
			}

			foreach(string lang in ISD_Settings.Instance.langFolders) 	{
				EditorGUILayout.BeginVertical (GUI.skin.box);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(lang, GUILayout.Height(18));
				EditorGUILayout.Space();

				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) 	{
					ISD_Settings.Instance.langFolders.Remove(lang);
					return;
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical ();
			}
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Add New Language");
			NewLanguage = EditorGUILayout.TextField(NewLanguage, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();			
			EditorGUILayout.Space();

			if(GUILayout.Button("Add",  GUILayout.Width(100)))	{
				if(!ISD_Settings.Instance.langFolders.Contains(NewLanguage) && NewLanguage.Length > 0)	{
					ISD_Settings.Instance.langFolders.Add(NewLanguage);
					NewLanguage = string.Empty;
				}				
			}
			EditorGUILayout.EndHorizontal();
			//	}
		}





		public static void AboutGUI() {
			GUI.enabled = true;
			EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
			EditorGUILayout.Space();


			SA.Common.Editor.Tools.SelectableLabel(SdkVersion,   ISD_Settings.VERSION_NUMBER);
			SA.Common.Editor.Tools.SupportMail();

			SA.Common.Editor.Tools.DrawSALogo();
#if DISABLED
EditorGUILayout.Space();
EditorGUILayout.LabelField("Note: This version of IOS Deploy designed for Stan's Assets");
EditorGUILayout.LabelField("plugins internal use only. If you want to use IOS Deploy  ");
EditorGUILayout.LabelField("for your project needs, please, ");
EditorGUILayout.LabelField("purchase a copy of IOS Deploy plugin.");

EditorGUILayout.BeginHorizontal();
EditorGUILayout.Space();

if(GUILayout.Button("Documentation",  GUILayout.Width(150))) {
Application.OpenURL("https://goo.gl/sOJFXJ");
}

if(GUILayout.Button("Purchase",  GUILayout.Width(150))) {
Application.OpenURL("https://goo.gl/Nqbuuv");
}		
EditorGUILayout.EndHorizontal();
#endif
		}



		private static void DirtyEditor() {
#if UNITY_EDITOR
			EditorUtility.SetDirty(ISD_Settings.Instance);
#endif
		}
	}

}
#endif