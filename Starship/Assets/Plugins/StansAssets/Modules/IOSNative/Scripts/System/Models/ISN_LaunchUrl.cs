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

	public class LaunchUrl {

		private Uri _URI = null;
		private string _AbsoluteUrl  		= string.Empty;
		private string _SourceApplication  	= string.Empty;


		//--------------------------------------
		//  Initialize
		//--------------------------------------

		public LaunchUrl(string data) {
			string[] dataArray 		= data.Split(SA.Common.Data.Converter.DATA_SPLITTER);

			_AbsoluteUrl = dataArray[0];
			_SourceApplication = dataArray[1];

			if(_AbsoluteUrl.Length > 0) {
				_URI = new Uri(_AbsoluteUrl);
			}
		
		}

		public LaunchUrl (string absoluteUrl, string sourceApplication) {
			_AbsoluteUrl = absoluteUrl;
			_SourceApplication = sourceApplication;

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

		public string SourceApplication {
			get {
				return _SourceApplication;
			}
		}
	}

}
