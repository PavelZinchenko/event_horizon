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

namespace SA.IOSNative.StoreKit {

	public class RestoreResult : SA.Common.Models.Result {


		//--------------------------------------
		// Initialize
		//--------------------------------------


		public  RestoreResult(SA.Common.Models.Error e) : base(e) {
		
		}

		public RestoreResult() : base()  {
		
		}

		public TransactionErrorCode TransactionErrorCode {
			get {
				if(_Error != null) {
					return (TransactionErrorCode) _Error.Code;
				} else {
					return TransactionErrorCode.SKErrorNone;
				}

			}
		}

	}

}