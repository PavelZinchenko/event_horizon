////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SA.IOSNative.Models {

	public class UniversalLink {

		private Uri _URI = null;
		private string _AbsoluteUrl  		= string.Empty;


		//--------------------------------------
		//  Initialize
		//--------------------------------------

		public UniversalLink(string absoluteUrl) {
			
			_AbsoluteUrl = absoluteUrl;

			if(_AbsoluteUrl.Length > 0) {
				_URI = new Uri(_AbsoluteUrl);
			}
		
		}
			

		//--------------------------------------
		//  Get / Set
		//--------------------------------------


		public bool IsEmpty {
			get {
				return _AbsoluteUrl.Equals (string.Empty);
			}
		}

		public Uri URI {
			get {
				return _URI;
			}
		}

		public string Host {
			get {
				return _URI.Host;
			}
		}

		public string AbsoluteUrl {
			get {
				return _AbsoluteUrl;
			}
		}
			
	}

}
