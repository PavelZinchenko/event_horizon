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

	public class PurchaseResult : SA.Common.Models.Result  {

		private string _ProductIdentifier  = string.Empty;
		private PurchaseState _State = PurchaseState.Failed;
		private string _Receipt  = string.Empty;
		private string _TransactionIdentifier = string.Empty;
		private string _ApplicationUsername = string.Empty;


		//--------------------------------------
		// Initialize
		//--------------------------------------

		public PurchaseResult(string productIdentifier, SA.Common.Models.Error e): base(e) {
			_ProductIdentifier = productIdentifier;
			_State = PurchaseState.Failed;
		}

		public PurchaseResult(string productIdentifier, PurchaseState state, string applicationUsername = "", string receipt = "", string transactionIdentifier = ""):base() {
			_ProductIdentifier = productIdentifier;
			_State = state;
			_Receipt = receipt;
			_TransactionIdentifier = transactionIdentifier;
			_ApplicationUsername = applicationUsername;
		}


		//--------------------------------------
		// Get / Set
		//--------------------------------------


		public TransactionErrorCode TransactionErrorCode {
			get {
				if(_Error != null) {
					return (TransactionErrorCode) _Error.Code;
				} else {
					return TransactionErrorCode.SKErrorNone;
				}
				
			}
		}

		public PurchaseState State {
			get {
				return _State;
			}
		}

		public string ProductIdentifier {
			get {
				return _ProductIdentifier;
			}
		}

		//An opaque identifier for the user’s account on your system. 
		//This is used to help the store detect irregular activity. 
		//For example, in a game, it would be unusual for dozens of different iTunes Store accounts making purchases on behalf of the same in-game character.
		//The recommended implementation is to use a one-way hash of the user’s account name to calculate the value for this property.
		public string ApplicationUsername {
			get {
				return _ApplicationUsername;
			}
		}

		public string Receipt {
			get {
				return _Receipt;
			}
		}

		public string TransactionIdentifier {
			get {
				return _TransactionIdentifier;
			}
		}
	}
}
