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

namespace SA.IOSNative.StoreKit {

	public class VerificationResponse  {
		
		private int 	_Status;
		private string _Receipt;
		private string _ProductIdentifier;
		private string _OriginalJSON;


		public VerificationResponse(string productIdentifier, string dataArray) {
			string[] data = dataArray.Split(SA.Common.Data.Converter.DATA_SPLITTER);

			_Status = Convert.ToInt32(data[0]);
			_OriginalJSON = data [1];
			_Receipt = data [2];
			_ProductIdentifier = productIdentifier;
		}



		public int Status {
			get {
				return _Status;
			}
		}

		public string Receipt {
			get {
				return _Receipt;
			}
		}

		public string ProductIdentifier {
			get {
				return _ProductIdentifier;
			}
		}

		public string OriginalJSON {
			get {
				return _OriginalJSON;
			}
		}
	}
}