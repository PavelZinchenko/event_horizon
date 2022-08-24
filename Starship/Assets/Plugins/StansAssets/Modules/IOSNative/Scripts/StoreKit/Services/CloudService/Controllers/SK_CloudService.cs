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

using SA.IOSNative.StoreKit;

public class SK_CloudService : SA.Common.Pattern.Singleton<SK_CloudService> {


	public static event Action<SK_AuthorizationResult> OnAuthorizationFinished = delegate {};
	public static event Action<SK_RequestCapabilitieResult> OnCapabilitiesRequestFinished = delegate {};
	public static event Action<SK_RequestStorefrontIdentifierResult> OnStorefrontIdentifierRequestFinished = delegate {};

	//--------------------------------------
	//  Initialization
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	//--------------------------------------
	//  Public Methods
	//--------------------------------------

	public void RequestAuthorization() {
		BillingNativeBridge.CloudService_RequestAuthorization();
	}

	public void RequestCapabilities() {
		BillingNativeBridge.CloudService_RequestCapabilities();
	}

	public void RequestStorefrontIdentifier() {
		BillingNativeBridge.CloudService_RequestStorefrontIdentifier();
	}



	//--------------------------------------
	//  Get / Set
	//--------------------------------------

	public static int AuthorizationStatus {
		get {
			return BillingNativeBridge.CloudService_AuthorizationStatus();
		}
	}



	//--------------------------------------
	//  Native Event Handlers
	//--------------------------------------

	private void Event_AuthorizationFinished(string data) {
		int val = Convert.ToInt32(data);
		SK_AuthorizationResult result =  new SK_AuthorizationResult((SK_CloudServiceAuthorizationStatus) val);

		OnAuthorizationFinished(result);

	}


	private void Event_RequestCapabilitieSsuccess(string data) {
		int val = Convert.ToInt32(data);
		SK_RequestCapabilitieResult result =  new SK_RequestCapabilitieResult((SK_CloudServiceCapability) val);
		OnCapabilitiesRequestFinished(result);
	}
		

	private void Event_RequestCapabilitiesFailed(string errorData) {
		SK_RequestCapabilitieResult result =  new SK_RequestCapabilitieResult(errorData);
		OnCapabilitiesRequestFinished(result);
	}


	private void Event_RequestStorefrontIdentifierSsuccess(string storefrontIdentifier) {
		SK_RequestStorefrontIdentifierResult result =  new SK_RequestStorefrontIdentifierResult();
		result.StorefrontIdentifier = storefrontIdentifier;
		OnStorefrontIdentifierRequestFinished(result);
	}


	private void Event_RequestStorefrontIdentifierFailed(string errorData) {
		SK_RequestStorefrontIdentifierResult result =  new SK_RequestStorefrontIdentifierResult(errorData);
		OnStorefrontIdentifierRequestFinished(result);
	}


}
