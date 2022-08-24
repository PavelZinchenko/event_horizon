//#define INAPP_API_ENABLED

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
#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
using System.Runtime.InteropServices;
#endif


namespace SA.IOSNative.StoreKit {

	public class BillingNativeBridge  {

		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
		[DllImport ("__Internal")]
		private static extern void _ISN_LoadStore(string ids);


		[DllImport ("__Internal")]
		private static extern void _ISN_EnableManulaTransactionsMode();

		[DllImport ("__Internal")]
		private static extern void _ISN_DisablePromotedPurchases();


		[DllImport ("__Internal")]
		private static extern void _ISN_BuyProduct(string id);


		[DllImport ("__Internal")]
		private static extern void _ISN_FinishTransaction(string productIdentifier);
		
		[DllImport ("__Internal")]
		private static extern void _ISN_RestorePurchases();


		[DllImport ("__Internal")]
		private static extern bool _ISN_InAppSettingState();
		
		[DllImport ("__Internal")]
		private static extern void _ISN_VerifyLastPurchase(string url);



		//SKCloudServiceController


		[DllImport ("__Internal")]
		private static extern int ISN_SKCloudService_AuthorizationStatus();

		[DllImport ("__Internal")]
		private static extern void ISN_SKCloudService_RequestAuthorization();

		[DllImport ("__Internal")]
		private static extern void ISN_SKCloudService_RequestCapabilities();


		[DllImport ("__Internal")]
		private static extern void ISN_SKCloudService_RequestStorefrontIdentifier();



		#endif


		public static void LoadStore(string ids) {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
			_ISN_LoadStore(ids);
			#endif
		}

		public static void EnableManulaTransactionsMode() {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
			_ISN_EnableManulaTransactionsMode();
			#endif
		}

		public static void DisablePromotedPurchases() {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
			_ISN_DisablePromotedPurchases();
			#endif
		}


		
		public static void BuyProduct(string productIdentifier) {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED)
			_ISN_BuyProduct(productIdentifier);
			#endif
		}

		public static void FinishTransaction(string productIdentifier) {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED)
			_ISN_FinishTransaction(productIdentifier);
			#endif
		}

		
		public static void RestorePurchases() {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED)
			_ISN_RestorePurchases();
			#endif
		}
		
		public static void VerifyLastPurchase(string url) {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
			_ISN_VerifyLastPurchase(url);
			#endif
		}


		public static bool ISN_InAppSettingState() {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
			return _ISN_InAppSettingState();
			#else
			return false;
			#endif
		}


		//--------------------------------------
		//  SKCloudServiceController
		//--------------------------------------





		public static int CloudService_AuthorizationStatus()  {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED)
			return ISN_SKCloudService_AuthorizationStatus();
			#else
			return 0;
			#endif
		}


		public static void CloudService_RequestAuthorization() {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED)
			ISN_SKCloudService_RequestAuthorization();
			#endif
		}


		public static void CloudService_RequestCapabilities() {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
			ISN_SKCloudService_RequestCapabilities();
			#endif
		}



		public static void CloudService_RequestStorefrontIdentifier() {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
			ISN_SKCloudService_RequestStorefrontIdentifier();
			#endif
		}

	}

}
