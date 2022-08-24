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

public class SK_AuthorizationResult : SA.Common.Models.Result {

	private SK_CloudServiceAuthorizationStatus _AuthorizationStatus = SK_CloudServiceAuthorizationStatus.NotDetermine;

	public SK_AuthorizationResult(SK_CloudServiceAuthorizationStatus status):base() {
		_AuthorizationStatus = status;
	}

	public SK_CloudServiceAuthorizationStatus AuthorizationStatus {
		get {
			return _AuthorizationStatus;
		}
	}
}
