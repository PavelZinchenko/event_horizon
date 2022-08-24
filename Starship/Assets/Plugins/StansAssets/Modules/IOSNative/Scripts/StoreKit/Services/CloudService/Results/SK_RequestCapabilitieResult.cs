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

public class SK_RequestCapabilitieResult : SA.Common.Models.Result {

	private SK_CloudServiceCapability _Capability = SK_CloudServiceCapability.None;

	public SK_RequestCapabilitieResult(SK_CloudServiceCapability capability):base() {
		_Capability = capability;
	}

	public SK_RequestCapabilitieResult(string errorData):base(new SA.Common.Models.Error(errorData)) {

	}


	public SK_CloudServiceCapability Capability {
		get {
			return _Capability;
		}
	}
}
