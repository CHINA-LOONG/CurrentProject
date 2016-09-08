using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Funplus {

	public class FunplusAppleIapIOS : BasePaymentWrapper {

		private static FunplusAppleIapIOS instance;
		private static readonly object locker = new object ();
		
		public static FunplusAppleIapIOS GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusAppleIapIOS ();
				}
			}
			return instance;
		}
		
		public override void SetGameObject (string gameObjectName) {
			com_funplus_sdk_payment_appleiap_setGameObject (gameObjectName);
		}
		
		public override bool CanMakePurchases () {
			return com_funplus_sdk_payment_appleiap_canMakePurchases ();
		}
		
		public override void StartHelper () {
			com_funplus_sdk_payment_appleiap_startHelper ();
		}
		
		public override void Buy (string productId, string throughCargo) {
			com_funplus_sdk_payment_appleiap_buy (productId, throughCargo);
		}

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_payment_appleiap_setGameObject (string gameObjectName);

		[DllImport ("__Internal")]
		private static extern bool com_funplus_sdk_payment_appleiap_canMakePurchases ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_payment_appleiap_startHelper ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_payment_appleiap_buy (string productId, string throughCargo);
	}

}