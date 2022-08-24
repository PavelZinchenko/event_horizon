using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.IOSNative.StoreKit {

	public static class SK_Util {



		public static float GetPriceByTier(PriceTier priceTier) {


			int tierint = (int) priceTier;
			tierint++;

			float price = 0f;

			float tier = (float)tierint;
			if(tier < 51) {
				price = tier - 0.01f;
			} else if(tier < 61) {
				float dif = tier - 50f;
				price = 50f + (dif * 5f) - 0.01f;
			} else {

				switch(tierint) {
				case 61:
					price = 109.99f;
					break;

				case 62:
					price = 119.99f;
					break;

				case 63:
					price = 124.99f;
					break;

				case 64:
					price = 129.99f;
					break;


				case 65:
					price = 139.99f;
					break;


				case 66:
					price = 149.99f;
					break;


				case 67:
					price = 159.99f;
					break;


				case 68:
					price = 169.99f;
					break;


				case 69:
					price = 174.99f;
					break;


				case 70:
					price = 179.99f;
					break;



				case 72:
					price = 199.99f;
					break;


				case 73:
					price = 209.99f;
					break;


				case 74:
					price = 219.99f;
					break;


				case 75:
					price = 229.99f;
					break;



				case 76:
					price = 239.99f;
					break;


				case 77:
					price = 249.99f;
					break;

				case 78:
					price = 299.99f;
					break;


				case 79:
					price = 349.99f;
					break;


				case 80:
					price = 399.99f;
					break;


				case 81:
					price = 449.99f;
					break;


				case 82:
					price = 499.99f;
					break;


				case 83:
					price = 599.99f;
					break;

				case 84:
					price = 699.99f;
					break;

				case 85:
					price = 799.99f;
					break;

				case 86:
					price = 899.99f;
					break;

				case 87:
					price = 999.99f;
					break;
				}


			}




			return price;
		}
	}


}