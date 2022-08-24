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

	public class BillingInitChecker  {
		public delegate void BillingInitListener();

		BillingInitListener _listener;


		public BillingInitChecker(BillingInitListener listener) {
			_listener = listener;

			if(PaymentManager.Instance.IsStoreLoaded) {
				_listener();
			} else {

				PaymentManager.OnStoreKitInitComplete += HandleOnStoreKitInitComplete;
				if(!PaymentManager.Instance.IsWaitingLoadResult) {
					PaymentManager.Instance.LoadStore();
				}
			}
		}

		void HandleOnStoreKitInitComplete (SA.Common.Models.Result obj) {
			PaymentManager.OnStoreKitInitComplete -= HandleOnStoreKitInitComplete;
			_listener();
		}

	}

}

