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

public class SK_RequestStorefrontIdentifierResult : SA.Common.Models.Result {

	private string _StorefrontIdentifier = string.Empty;


	public SK_RequestStorefrontIdentifierResult():base() {
		
	}

	public SK_RequestStorefrontIdentifierResult(string errorData):base(new SA.Common.Models.Error(errorData)) {

	}


	public string StorefrontIdentifier {
		get {
			return _StorefrontIdentifier;
		}

		set {
			_StorefrontIdentifier = value;
		}
	}
}
