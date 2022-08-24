////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SA.IOSDeploy {

	[System.Serializable]
	public class Variable  {
		//Editor Use Only
		public bool IsOpen = true;
		public bool IsListOpen = true;

		public string Name = string.Empty;
		public PlistValueTypes Type         = PlistValueTypes.String;

		public string StringValue = string.Empty;
		public int IntegerValue = 0;
		public float FloatValue = 0;
		public bool BooleanValue = true;

		public List<string> ChildrensIds = new List<string> ();


		public void AddChild(Variable v){
			if (Type.Equals (PlistValueTypes.Dictionary) ) {
				foreach (string ChildsId in ChildrensIds) {
					Variable var = ISD_Settings.Instance.getVariableByKey (ChildsId);
					if (var.Name.Equals (v.Name)) {
						ISD_Settings.Instance.RemoveVariable (var, ChildrensIds);
						break;
					}

				}
			} else if (Type.Equals (PlistValueTypes.Array)) {
				if (v.Type.Equals (PlistValueTypes.String)) {
					foreach (string ChildsId in ChildrensIds) {

						Variable var = ISD_Settings.Instance.getVariableByKey (ChildsId);
						if (var.Type.Equals (PlistValueTypes.String)) {
							if (v.StringValue.Equals (var.StringValue)) {
								ISD_Settings.Instance.RemoveVariable (var, ChildrensIds);
								break;
							}
						}
					}
				}
			}

			string key = SA.Common.Util.IdFactory.NextId.ToString();
			ISD_Settings.Instance.AddVariableToDictionary (key, v);
			ChildrensIds.Add(key);
		}



	}
}