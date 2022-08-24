////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SA.IOSNative.Models {


	[Serializable]
	public class UrlType {

		public string Identifier = string.Empty;
		public List<string> Schemes =  new List<string>();

		public bool IsOpen = true;

		//--------------------------------------
		//  Initialize
		//--------------------------------------

		public UrlType (string identifier) {
			Identifier = identifier;
		}


		//--------------------------------------
		//  Public methods
		//--------------------------------------

		public void AddSchemes(string schemes) {
			Schemes.Add (schemes);
		}


	}
}


