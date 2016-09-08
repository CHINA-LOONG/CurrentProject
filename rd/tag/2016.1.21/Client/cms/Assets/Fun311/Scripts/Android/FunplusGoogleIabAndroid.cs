using UnityEngine;
using System.Collections;

namespace Funplus {

	public class FunplusGoogleIabAndroid : BasePaymentWrapper {

		private AndroidJavaClass funplusGoogleiabWrapper;
		
		private static FunplusGoogleIabAndroid instance;
		private static readonly object locker = new object ();

		private FunplusGoogleIabAndroid () {
			funplusGoogleiabWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusGoogleiabWrapper");
		}

		public static FunplusGoogleIabAndroid GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusGoogleIabAndroid ();
				}
			}
			return instance;
		}

		public override void SetGameObject (string gameObjectName) {
			funplusGoogleiabWrapper.CallStatic ("setGameObject", gameObjectName);
		}

		public override bool CanMakePurchases () {
			return funplusGoogleiabWrapper.CallStatic<bool> ("canMakePurchases");
		}

		public override void StartHelper () {
			funplusGoogleiabWrapper.CallStatic ("startHelper");
		}

		public override void Buy (string productId, string throughCargo) {
			funplusGoogleiabWrapper.CallStatic ("buy", productId, throughCargo);
		}
	}
	
}