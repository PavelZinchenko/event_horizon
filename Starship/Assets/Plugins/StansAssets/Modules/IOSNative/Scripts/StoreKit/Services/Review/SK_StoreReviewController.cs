//#define INAPP_API_ENABLED

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
using System.Runtime.InteropServices;
#endif


namespace SA.IOSNative.StoreKit {

	public static class SK_StoreReviewController  {

		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 

		[DllImport ("__Internal")]
		private static extern bool _ISN_StoreReviewControllerAvaliable();

		[DllImport ("__Internal")]
		private static extern void _ISN_StoreRrequestReview();

		#endif

		
		/// <summary>
		/// Use the RrequestReview() method to indicate when it makes sense 
		/// within the logic of your app to ask the user for ratings and reviews within your app.
		/// </summary>
		public static void RrequestReview() {
			#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
			_ISN_StoreRrequestReview();
			#endif
		}


		/// <summary>
		/// The API is avaliable starting from iOS 10.3+ 
		/// so you probably want to check if you can use this feature with current enviroment
		/// </summary>
		public static bool IsAvaliable {
			get {
				#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) 
				return _ISN_StoreReviewControllerAvaliable();
				#else
				return false;
				#endif
			}
		}

	}

}
