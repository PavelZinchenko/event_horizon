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

namespace SA.IOSNative.StoreKit {
	
	public class PaymentManager : SA.Common.Pattern.Singleton<PaymentManager> {		


		public const string APPLE_VERIFICATION_SERVER   = "https://buy.itunes.apple.com/verifyReceipt";
		public const string SANDBOX_VERIFICATION_SERVER = "https://sandbox.itunes.apple.com/verifyReceipt";


		//Actions
		public static event Action<SA.Common.Models.Result> OnStoreKitInitComplete = delegate{};

		public static event Action OnRestoreStarted 				= delegate{};
		public static event Action<RestoreResult> OnRestoreComplete = delegate{};


		public static event Action<string> OnTransactionStarted 	= delegate{};
		public static event Action<PurchaseResult> OnTransactionComplete = delegate{};
		public static event Action<string> OnProductPurchasedExternally = delegate{};


		public static event Action<VerificationResponse> OnVerificationComplete = delegate{};



		private bool _IsStoreLoaded = false;
		private bool _IsWaitingLoadResult = false;
		private static int _nextId = 1;


		private Dictionary<int, StoreProductView> _productsView =  new Dictionary<int, StoreProductView>(); 

		private static string lastPurchasedProduct;

		//--------------------------------------
		// INITIALIZE
		//--------------------------------------


		void Awake() {
			DontDestroyOnLoad(gameObject);
		}

		//--------------------------------------
		//  PUBLIC METHODS
		//--------------------------------------


		/// <summary>
		/// Initializes the Store Kit with the set of perviostly defined product
		/// </summary>
		public void LoadStore(bool forceLoad = false) {

			if(_IsStoreLoaded) {

				if(!forceLoad) {
					Invoke("FireSuccessInitEvent", 1f);
					return;
				} 
			}

			if(_IsWaitingLoadResult) {
				return;
			}

			_IsWaitingLoadResult = true;

			string ids = "";
			int len = Products.Count;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					ids += ",";
				}

				ids += Products[i].Id;
			}

			//ISN_SoomlaGrow.Init();

			if(!Application.isEditor) {
				BillingNativeBridge.LoadStore(ids);

				if(IOSNativeSettings.Instance.TransactionsHandlingMode == TransactionsHandlingMode.Manual) {
					BillingNativeBridge.EnableManulaTransactionsMode ();
				}

				if(!IOSNativeSettings.Instance.PromotedPurchaseSupport) {
					BillingNativeBridge.DisablePromotedPurchases ();
				}

			} else {
				if(IOSNativeSettings.Instance.InAppsEditorTesting) {
					Invoke("EditorFakeInitEvent", 1f);
				}
			}
		}
			
		public void BuyProduct(string productId) {

			if(!Application.isEditor) {
				OnTransactionStarted(productId);
				if(!_IsStoreLoaded) {

					ISN_Logger.Log("buyProduct shouldn't be called before StoreKit is initialized"); 
					var error = new SA.Common.Models.Error((int) TransactionErrorCode.SKErrorPaymentNotAllowed, "StoreKit not yet initialized");
					SendTransactionFailEvent(productId, error);

					return;
				} 

				BillingNativeBridge.BuyProduct(productId);


			} else {
				if(IOSNativeSettings.Instance.InAppsEditorTesting) {
					FireProductBoughtEvent(productId, "", "", "", false);
				}
			}
		}


		public void FinishTransaction(string productId) {
			BillingNativeBridge.FinishTransaction (productId);
		}
			
		public void AddProductId(string productId) {

			Product tpl  = new Product();
			tpl.Id = productId;
			AddProduct(tpl);
		}


		public void AddProduct(Product product) {

			bool IsPordcutAlreadyInList = false;
			int replaceIndex = 0;
			foreach(Product p in Products) {
				if(p.Id.Equals(product.Id)) {
					IsPordcutAlreadyInList = true;
					replaceIndex = Products.IndexOf(p);
					break;
				}
			}

			if(IsPordcutAlreadyInList) {
				Products[replaceIndex] = product;
			} else {
				Products.Add(product);
			}
		}

		public Product GetProductById(string prodcutId) {
			foreach(Product p in Products) {
				if(p.Id.Equals(prodcutId)) {
					return p;
				}
			} 

			Product tpl =  new Product();
			tpl.Id = prodcutId;
			Products.Add(tpl);

			return tpl;
		}
			

		public void RestorePurchases() {

			if(!_IsStoreLoaded) {

				SA.Common.Models.Error e = new SA.Common.Models.Error((int) TransactionErrorCode.SKErrorPaymentServiceNotInitialized, "Store Kit Initilizations required"); 

				RestoreResult r =  new RestoreResult(e);
				OnRestoreComplete(r);
				return;
			}

			OnRestoreStarted();

			if(!Application.isEditor) {
				BillingNativeBridge.RestorePurchases();
			} else {
				if(IOSNativeSettings.Instance.InAppsEditorTesting) {
					foreach(Product product in Products) {
						if(product.Type == ProductType.NonConsumable) {
							ISN_Logger.Log("Restored: " + product.Id);
							FireProductBoughtEvent(product.Id, "", "", "", true);
						}
					}

					FireRestoreCompleteEvent();
				}
			}
		}


		public void VerifyLastPurchase(string url) {
			BillingNativeBridge.VerifyLastPurchase (url);
		}

		public void RegisterProductView(StoreProductView view) {
			view.SetId(NextId);
			_productsView.Add(view.Id, view);
		}


		//--------------------------------------
		//  GET/SET
		//--------------------------------------


		public List<Product> Products {
			get {
				return IOSNativeSettings.Instance.InAppProducts;
			}
		}

		public bool IsStoreLoaded {
			get {
				return _IsStoreLoaded;
			}
		}

		public bool IsInAppPurchasesEnabled {
			get {
				return BillingNativeBridge.ISN_InAppSettingState();
			}
		}

		public bool IsWaitingLoadResult {
			get {
				return _IsWaitingLoadResult;
			}
		}

		private static int NextId {
			get {
				_nextId++;
				return _nextId;
			}
		}


		//--------------------------------------
		//  EVENTS
		//--------------------------------------


		private void OnStoreKitInitFailed(string data) {

			SA.Common.Models.Error e =  new SA.Common.Models.Error(data);

			_IsStoreLoaded = false;
			_IsWaitingLoadResult = false;


			SA.Common.Models.Result res = new SA.Common.Models.Result (e);
			OnStoreKitInitComplete (res);


			if(!IOSNativeSettings.Instance.DisablePluginLogs) 
				ISN_Logger.Log("STORE_KIT_INIT_FAILED Error: " + e.Message);
		}

		private void onStoreDataReceived(string data) {
			if(data.Equals(string.Empty)) {
				ISN_Logger.Log("InAppPurchaseManager, no products avaiable");
				SA.Common.Models.Result res = new SA.Common.Models.Result();
				OnStoreKitInitComplete(res);
				return;
			}


			string[] storeData = data.Split(SA.Common.Data.Converter.DATA_SPLITTER);

			for(int i = 0; i < storeData.Length; i+=7) {
				string prodcutId = storeData[i];
				Product tpl =  GetProductById(prodcutId);


				tpl.DisplayName 	= storeData[i + 1];
				tpl.Description 	= storeData[i + 2];
				tpl.LocalizedPrice 	= storeData[i + 3];
				tpl.Price 			= Convert.ToSingle(storeData[i + 4]);
				tpl.CurrencyCode 	= storeData[i + 5];
				tpl.CurrencySymbol 	= storeData[i + 6];
				tpl.IsAvailable = true;

			}

			ISN_Logger.Log("InAppPurchaseManager, total products in settings: " + Products.Count.ToString());


			int avaliableProductsCount = 0;
			foreach(Product tpl in Products) {
				if(tpl.IsAvailable) {
					avaliableProductsCount++;
				}
			}

			ISN_Logger.Log("InAppPurchaseManager, total avaliable products" + avaliableProductsCount);
			FireSuccessInitEvent();
		}

		private void onProductBought(string array) {

			string[] data;
			data = array.Split("|" [0]);

			bool IsRestored = false;
			if(data [1].Equals("0")) {
				IsRestored = true;
			}

			string productId = data [0];



			FireProductBoughtEvent(productId, data [2], data [3], data [4], IsRestored);

		}

		private void onProductPurchasedExternally(string productIdentifier) {
			OnProductPurchasedExternally (productIdentifier);
		}


		private void onProductStateDeferred(string productIdentifier) {
			PurchaseResult response = new PurchaseResult (productIdentifier, PurchaseState.Deferred);


			OnTransactionComplete (response);
		}


		private void onTransactionFailed(string data) {
			string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2.ToString() }, StringSplitOptions.None);

			string prodcutId = DataArray [0];
			var error = new SA.Common.Models.Error (DataArray [1]);

			SendTransactionFailEvent(prodcutId, error);
		}


		private void onVerificationResult(string data) {
			VerificationResponse response = new VerificationResponse (lastPurchasedProduct, data);
			OnVerificationComplete (response);
		}

		public void onRestoreTransactionFailed(string array) {

			SA.Common.Models.Error e = new SA.Common.Models.Error(array);

			RestoreResult r =  new RestoreResult(e);

			OnRestoreComplete (r);
		}

		public void onRestoreTransactionComplete(string array) {
			FireRestoreCompleteEvent();
		}



		private void OnProductViewLoaded(string viewId) {
			int id = Convert.ToInt32(viewId);
			if(_productsView.ContainsKey(id)) {
				_productsView[id].OnContentLoaded();
			}
		}

		private void OnProductViewLoadedFailed(string viewId) {
			int id = Convert.ToInt32(viewId);
			if(_productsView.ContainsKey(id)) {
				_productsView[id].OnContentLoadFailed();
			}
		}

		private void OnProductViewDismissed(string viewId) {
			int id = Convert.ToInt32(viewId);
			if(_productsView.ContainsKey(id)) {
				_productsView[id].OnProductViewDismissed();
			}
		}

		//--------------------------------------
		//  PRIVATE METHODS
		//--------------------------------------

		private void FireSuccessInitEvent() {
			_IsStoreLoaded = true;
			_IsWaitingLoadResult = false;
			SA.Common.Models.Result r = new SA.Common.Models.Result();
			OnStoreKitInitComplete(r);
		}


		private void FireRestoreCompleteEvent() {

			RestoreResult r =  new RestoreResult();
			OnRestoreComplete (r);
		}

		private void FireProductBoughtEvent(string productIdentifier, string applicationUsername, string receipt, string transactionIdentifier, bool IsRestored) {

			PurchaseState state;
			if(IsRestored) {
				state = PurchaseState.Restored;
			} else {
				state = PurchaseState.Purchased;
			}

			PurchaseResult response = new PurchaseResult (productIdentifier, state, applicationUsername, receipt, transactionIdentifier);



			lastPurchasedProduct = response.ProductIdentifier;
			OnTransactionComplete (response);
		}


		private void SendTransactionFailEvent(string productIdentifier, SA.Common.Models.Error error) {
			PurchaseResult response = new PurchaseResult (productIdentifier, error);

			OnTransactionComplete (response);
		}

		//--------------------------------------
		//  UNITY EDITOR FAKE SECTION
		//--------------------------------------

		private void EditorFakeInitEvent() {
			FireSuccessInitEvent();
		}



		//--------------------------------------
		//  DESTROY
		//--------------------------------------

	}
}



