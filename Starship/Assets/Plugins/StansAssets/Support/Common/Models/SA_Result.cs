////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;


namespace SA.Common.Models {
	
	public class Result  {

		protected Error _Error = null;


		//--------------------------------------
		// Intialization
		//--------------------------------------

		public Result() {

		}

		public Result(Error error) {
			_Error = error;
		}



		//--------------------------------------
		// Get / Set
		//--------------------------------------


		/// <summary>
		/// Result Error Object. Can be null of result is succeeded
		/// </summary>
		public Error Error {
			get {
				return _Error;
			}
		}

		/// <summary>
		/// True if result has an error
		/// </summary>
		public bool HasError {
			get {
				if (_Error == null) {
					return false;
				} else {
					return true;
				}
			}
		}

		/// <summary>
		/// True if is succeeded
		/// </summary>
		public bool IsSucceeded {
			get {
				return !HasError;
			}
		}


		/// <summary>
		/// True if result is failed
		/// </summary>
		public bool IsFailed {
			get {
				return HasError;
			}
		}

	}
}
