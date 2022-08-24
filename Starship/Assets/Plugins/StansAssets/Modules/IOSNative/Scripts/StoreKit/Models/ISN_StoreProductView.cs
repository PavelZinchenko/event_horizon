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
using System;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif


namespace SA.IOSNative.StoreKit {

	public class StoreProductView {

		public event Action Loaded = delegate {};
		public event Action LoadFailed = delegate {};
		public event Action Appeared = delegate {};
		public event Action Dismissed = delegate {};



		#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
		[DllImport ("__Internal")]
		private static extern void _ISN_CreateProductView(int viewId, string productsId);
		
		[DllImport ("__Internal")]
		private static extern void _ISN_ShowProductView(int viewId);
		#endif

		private int _id;
		private List<string> _ids =  new List<string>();



		//--------------------------------------
		// INITIALIZE
		//--------------------------------------


		public StoreProductView() {
			foreach(string pid in IOSNativeSettings.Instance.DefaultStoreProductsView) {
				addProductId(pid);
			}

			PaymentManager.Instance.RegisterProductView(this);
		}

		public StoreProductView(params string[] ids) {
			foreach(string pid in ids) {
				addProductId(pid);
			}

			PaymentManager.Instance.RegisterProductView(this);
		}


		//--------------------------------------
		// PUBLIC METHODS
		//--------------------------------------

		public void addProductId(string productId) {
			if(_ids.Contains(productId)) {
				return;
			}
			
			_ids.Add(productId);
		}

		

		public void Load() {
			#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
				string ids = "";
				int len = _ids.Count;
				for(int i = 0; i < len; i++) {
					if(i != 0) {
						ids += ",";
					}
					
					ids += _ids[i];
				}

				_ISN_CreateProductView(Id, ids);
			#endif
		}

		public void Show() {
			#if (UNITY_IPHONE && !UNITY_EDITOR  && INAPP_API_ENABLED) || SA_DEBUG_MODE
				_ISN_ShowProductView(Id);
			#endif
		}

		
		//--------------------------------------
		// GET / SET
		//--------------------------------------

		public int Id {
			get {
				return _id;
			}
		}


		//--------------------------------------
		// EVENTS
		//--------------------------------------

		public void OnProductViewAppeard() {
			Appeared();
		}

		public void OnProductViewDismissed() {
			Dismissed();
		}

		public void OnContentLoaded() {
			Show();
			Loaded();
		}

		public void OnContentLoadFailed() {
			LoadFailed();
		}

		//--------------------------------------
		// PRIVATE METHODS
		//--------------------------------------

		public void SetId(int viewId) {
			_id = viewId;
		}
			
	}
}
