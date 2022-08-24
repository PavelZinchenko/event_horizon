////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SA.IOSDeploy {

	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	public class ISD_Settings : ScriptableObject{

		public const string VERSION_NUMBER = "3.5/" + SA.Common.Config.LIB_VERSION;


		//Editor Window
		public bool IsfwSettingOpen;
		public bool IsLibSettingOpen;
		public bool IslinkerSettingOpne;
		public bool IscompilerSettingsOpen;
		public bool IsPlistSettingsOpen;
		public bool IsLanguageSettingOpen = true;
		public bool IsDefFrameworksOpen = false;
		public bool IsDefLibrariesOpen = false;
		public bool IsBuildSettingsOpen;
		public int ToolbarIndex = 0;


		//BuildOptions
		public bool enableBitCode = false;
		public bool enableTestability = false;
		public bool generateProfilingCode = false;

		//Post Process Libs
		public List<Framework> Frameworks =  new List<Framework>();
		public List<Lib> Libraries =  new List<Lib>(); 
		public List<Flag> Flags = new List<Flag> ();
		public List<Variable>  PlistVariables =  new List<Variable>();
		public List<VariableId> VariableDictionary = new List<VariableId>();
		public List<string> langFolders = new List<string>();

        public List<AssetFile> Files = new List<AssetFile>();

		
		private const string ISDAssetName = "ISD_Settings";
		private const string ISDAssetExtension = ".asset";

		private static ISD_Settings instance;
		public static ISD_Settings Instance {
			get {
				if(instance == null) {
					instance = Resources.Load(ISDAssetName) as ISD_Settings;
					if(instance == null) {
						instance = CreateInstance<ISD_Settings>();
						#if UNITY_EDITOR

						SA.Common.Util.Files.CreateFolder(SA.Common.Config.SETTINGS_PATH);
						string fullPath = Path.Combine(Path.Combine("Assets", SA.Common.Config.SETTINGS_PATH), ISDAssetName + ISDAssetExtension );
						
						AssetDatabase.CreateAsset(instance, fullPath);
						#endif

					}
				}
			
				return instance;
			}
		}


		//--------------------------------------
		// Varaibles
		//--------------------------------------

		public void AddVariable(Variable var){
			foreach (Variable v in PlistVariables.ToList()) {
				if (v.Name.Equals (var.Name)) {
					PlistVariables.Remove (v);
				}
			}
			PlistVariables.Add(var);
		}


		public void AddVariableToDictionary(string uniqueIdKey,Variable var){
			VariableId newVar = new VariableId ();
			newVar.uniqueIdKey = uniqueIdKey;
			newVar.VariableValue = var;
			VariableDictionary.Add(newVar);
		}

		public void RemoveVariable(Variable v, IList ListWithThisVariable){
			if (ISD_Settings.Instance.PlistVariables.Contains (v)) {
				ISD_Settings.Instance.PlistVariables.Remove (v);
			} else {
				foreach(VariableId vid in VariableDictionary){
					if (vid.VariableValue.Equals (v)) {
						VariableDictionary.Remove (vid);
						string id = vid.uniqueIdKey;
						if(ListWithThisVariable.Contains(id))
							ListWithThisVariable.Remove (vid.uniqueIdKey);
						break;
					}
				}
			}
		}

		public Variable getVariableByKey(string uniqueIdKey){
			foreach (VariableId vid in VariableDictionary) {
				if (vid.uniqueIdKey.Equals (uniqueIdKey)) {
					return vid.VariableValue;
				}
			}
			return null;
		}

		public Variable GetVariableByName(string name) {
			foreach(Variable var in ISD_Settings.Instance.PlistVariables) {
				if(var.Name.Equals(name)) {
					return var;
				}
			}

			return null;
		}


		public string getKeyFromVariable(Variable var){
			foreach (VariableId vid in VariableDictionary) {
				if (vid.VariableValue.Equals (var)) {
					return vid.uniqueIdKey;
				}
			}
			return null;
		}

		public bool ContainsPlistVarWithName(string name) {
			foreach(Variable var in ISD_Settings.Instance.PlistVariables) {
				if(var.Name.Equals(name)) {
					return true;
				}
			}

			return false;
		}
			

		//--------------------------------------
		// Frameworks
		//--------------------------------------


		public bool ContainsFramework(iOSFramework framework) {
			foreach(Framework f in ISD_Settings.Instance.Frameworks) {
				if(f.Type.Equals(framework)) {
					return true;
				}
			}
			return false;
		}

		public Framework GetFramework(iOSFramework framework) {
			foreach(Framework f in ISD_Settings.Instance.Frameworks) {
				if(f.Type.Equals(framework)) {
					return f;
				}
			}
			return null;
		}

		public Framework AddFramework(iOSFramework framework, bool embaded = false) {

			var f = GetFramework (framework);
			if(f ==  null) {
				f = new Framework (framework, embaded);
				ISD_Settings.Instance.Frameworks.Add (f);
			}

			return f;
		}


		//--------------------------------------
		// Libraries
		//--------------------------------------


		public bool ContainsLibWithName(string name) {
			foreach(Lib l in ISD_Settings.Instance.Libraries) {
				if(l.Name.Equals(name)) {
					return true;
				}
			}
			return false;
		}

		public Lib GetLibrary(iOSLibrary library){
			foreach (Lib l in ISD_Settings.instance.Libraries) {
				if (l.Type.Equals(library)) {
					return l;
				}
			}
			return null;
		}

		public Lib AddLibrary(iOSLibrary library){
			var l = GetLibrary (library);
			if (l == null) {
				l = new Lib (library);
				ISD_Settings.Instance.Libraries.Add (l);
			}
			return l;
		}
			
		//--------------------------------------
		// Flags
		//--------------------------------------

		public void AddLinkerFlag(string s){
			Flag newFlag = new Flag ();
			newFlag.Name = s;
			newFlag.Type = FlagType.LinkerFlag;
			foreach (Flag f in Flags) {
				if (f.Type.Equals (FlagType.LinkerFlag) && f.Name.Equals (s)) {
					break;
				}
			}
			Flags.Add (newFlag);
		}

	








			




							
	}
}
