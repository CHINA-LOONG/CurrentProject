using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Funplus {
	
	public class FunplusPaymentStub : BasePaymentWrapper {
		
		private static FunplusPaymentStub instance;
		private static readonly object locker = new object ();

		private bool canBuy;
		
		public static FunplusPaymentStub GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusPaymentStub ();
				}
			}
			return instance;
		}
		
		public override void SetGameObject (string gameObjectName) {
			Debug.Log ("Calling FunplusPaymentStub.SetGameObject ().");
		}
		
		public override bool CanMakePurchases () {
			return canBuy;
		}
		
		public override void StartHelper () {
			canBuy = true;
//			FunplusPayment.getInstance ().onInitializeSuccess (
//				"[" +
//				"{\"productId\":\"com.funplus.p1\",\"type\":\"inapp\",\"price\":\"$0.99\",\"price_amount_micros\":990000,\"price_currency_code\":\"USD\",\"product_title\":\"test product 1 (Funplus Test App)\",\"description\":\"test product 1\"}," +
//				"{\"productId\":\"com.funplus.p2\",\"type\":\"inapp\",\"price\":\"$0.99\",\"price_amount_micros\":990000,\"price_currency_code\":\"USD\",\"title\":\"test product 2 (Funplus Test App)\",\"description\":\"test product 3\"}," +
//				"{\"productId\":\"com.funplus.p3\",\"type\":\"inapp\",\"price\":\"$0.99\",\"price_amount_micros\":990000,\"price_currency_code\":\"USD\",\"title\":\"test product 3 (Funplus Test App)\",\"description\":\"test product 3\"}" +
//				"]");
			FunplusPayment.GetInstance ().OnInitializeSuccess (
				"[" +
				"{\"product_id\":\"com.funplus.p1\",\"type\":\"inapp\",\"price\":990000,\"formatted_price\":\"$0.99\",\"locale_currency_symbol\":\"$\",\"locale_currency_code\":\"USD\",\"product_title\":\"test product 1 (Funplus Test App)\",\"product_description\":\"test product 1\"}," +
				"{\"product_id\":\"com.funplus.p2\",\"type\":\"inapp\",\"price\":990000,\"formatted_price\":\"$0.99\",\"locale_currency_symbol\":\"$\",\"locale_currency_code\":\"USD\",\"product_title\":\"test product 2 (Funplus Test App)\",\"product_description\":\"test product 3\"}," +
				"{\"product_id\":\"com.funplus.p3\",\"type\":\"inapp\",\"price\":990000,\"formatted_price\":\"$0.99\",\"locale_currency_symbol\":\"$\",\"locale_currency_code\":\"USD\",\"product_title\":\"test product 3 (Funplus Test App)\",\"product_description\":\"test product 3\"}" +
				"]");
		}

		public override void Buy (string productId, string throughCargo) {
			FunplusPayment.GetInstance ().OnPurchaseSuccess ("{\"product_id\": \"" + productId + "\", \"through_cargo\": \"" + throughCargo + "\"}");
		}

	}
	
}