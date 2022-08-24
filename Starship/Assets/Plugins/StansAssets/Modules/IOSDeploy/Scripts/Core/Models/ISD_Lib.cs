////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Reflection;
using System.ComponentModel;


namespace SA.IOSDeploy {
		
	[System.Serializable]
	public class Lib  {

		//Editor Use Only
		public bool IsOpen = false;
		
		public iOSLibrary Type;
		public bool IsOptional;

		public Lib(iOSLibrary lib){
			Type = lib;
		}

		public string Name {
			get{
				return ISD_LibHandler.stringValueOf(Type);
			}
		}




	}

}