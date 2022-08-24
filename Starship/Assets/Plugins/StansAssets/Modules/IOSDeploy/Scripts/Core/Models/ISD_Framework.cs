
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace SA.IOSDeploy {

	[System.Serializable]
	public class Framework  {


		//Editor Use Only
		public bool IsOpen = false;

		public iOSFramework Type;
		public bool IsOptional;
		public bool IsEmbeded;
	
		public Framework(iOSFramework type, bool Embaded = false){
			Type = type;
			IsEmbeded = Embaded;
		}

		public Framework(string frameworkName){
			frameworkName = frameworkName.Replace (".framework", string.Empty);
			Type = SA.Common.Util.General.ParseEnum<iOSFramework> (frameworkName);
		}
			

		public string Name {
			get{
				return Type.ToString () + ".framework";
			}
		}


		public int[] BaseIndexes(){
			string[] mainArray = ISD_FrameworkHandler.BaseFrameworksArray();
			List<int> indexes = new List<int>(mainArray.Length);
			for (int i = 0; i < mainArray.Length; i++) {
				indexes.Add (i);
			}
			return indexes.ToArray ();
		}


		public string TypeString {
			get{
				return Type.ToString ();
			}
		}

	}


}