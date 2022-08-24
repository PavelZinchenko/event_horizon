////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SA.IOSNative.StoreKit {

	[Serializable]
	public class Product  {

		//Editor Only
		public bool IsOpen = true;


		[SerializeField]
		private bool _IsAvailable = false;

		[SerializeField]
		private string _Id = string.Empty;

		[SerializeField]
		private string _DisplayName =  "New Product";

		[SerializeField]
		private string _Description;

		[SerializeField]
		private float _Price = 0.99f;

		[SerializeField]
		private string _LocalizedPrice = string.Empty;

		[SerializeField]
		private string _CurrencySymbol = "$";

		[SerializeField]
		private string _CurrencyCode = "USD";

		[SerializeField]
		private Texture2D _Texture;

		[SerializeField]
		private ProductType _ProductType = ProductType.Consumable;

		[SerializeField]
		private PriceTier _PriceTier = PriceTier.Tier1;



		public void UpdatePriceByTier() {
			

			_Price = SK_Util.GetPriceByTier(_PriceTier);


		}
		

		public string Id {
			get {
				return _Id;
			}

			set {
				_Id = value;
			}
		}

		public string DisplayName {
			get {
				return _DisplayName;
			}
			
			set {
				_DisplayName = value;
			}
		}




		public string Description {
			get {
				return _Description;
			}

			set {
				_Description = value;
			}
		}

		public ProductType Type {
			get {
				return _ProductType;
			}

			set {
				_ProductType =  value;
			}
		}

		public float Price {
			get {
				return _Price;
			} 

			set {
				_Price = value;
			}
		}

		public long PriceInMicros {
			get {
				return Convert.ToInt64(_Price * 1000000f);
			} 

		}

		public string LocalizedPrice {
			get {
				if(_LocalizedPrice.Equals(string.Empty)) {
					return Price + " " + _CurrencySymbol;
				} else {
					return _LocalizedPrice;
				}

			}

			set {
				_LocalizedPrice = value;
			}
		}

		public string CurrencySymbol {
			get {
				return _CurrencySymbol;
			} 

			set {
				_CurrencySymbol = value;
			}
		}

		public string CurrencyCode {
			get {
				return _CurrencyCode;
			}

			set {
				_CurrencyCode = value;
			}
		}
		
		public Texture2D Texture {
			get {
				return _Texture;
			}

			set {
				_Texture = value;
			}
		}

		public PriceTier PriceTier {
			get {
				return _PriceTier;
			}

			set {
				_PriceTier = value;
			}
		}

		public bool IsAvailable {
			get {
				return _IsAvailable;
			}

			set {
				_IsAvailable = value;
			}
		}
	}
}
