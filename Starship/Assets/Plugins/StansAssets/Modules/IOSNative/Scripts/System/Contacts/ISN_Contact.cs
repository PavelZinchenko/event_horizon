////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SA.IOSNative.Contacts {

	public class Contact  {


		public string GivenName =  string.Empty;
		public string FamilyName =  string.Empty;

		public List<string> Emails =  new List<string>();
		public List<PhoneNumber> PhoneNumbers =  new List<PhoneNumber>();


	}

}