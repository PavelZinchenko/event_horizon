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

	public enum TransactionErrorCode  {

		SKErrorUnknown = 0,
		SKErrorClientInvalid = 1,               // client is not allowed to issue the request, etc.
		SKErrorPaymentCanceled = 2,            // user canceled the request, etc.
		SKErrorPaymentInvalid = 3,              // purchase identifier was invalid, etc.
		SKErrorPaymentNotAllowed = 4,           // this device is not allowed to make the payment
		SKErrorStoreProductNotAvailable = 5,    // Product is not available in the current storefront
		SKErrorPaymentNoPurchasesToRestore = 6,  // No purchases to restore"
		SKErrorPaymentServiceNotInitialized = 7,  //StoreKit initialization required
		SKErrorNone = 8 //No error occurred
	}
}