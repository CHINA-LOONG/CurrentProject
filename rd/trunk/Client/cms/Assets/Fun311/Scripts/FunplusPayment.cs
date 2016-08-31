using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HSMiniJSON;

namespace Funplus {

	/// <summary>
	/// The payment module unifies different payment methods into one.
	/// To purcdhase a product, basically you should take the following steps:
	/// <list type="number">
	/// <item>After player logs in, set a delegate class to the payment module,
	/// so that your game can receive notifications sent by Funplus SDK.</item>
	/// <item>Start the payment helper.</item>
	/// <item>Purchase a product.</item>
	/// </list>
	/// </summary>
	public class FunplusPayment : MonoBehaviour {

		/// <summary>
		/// This interface defines methods which will be called
		/// on specific moment in the life circle of the payment helper.
		/// </summary>
		public interface IDelegate {

			/// <summary>
			/// This method will be called after the initialization succeeds.
			/// The products list is passed back.
			/// </summary>
			/// <param name="products">Products.</param>
			void OnInitializeSuccess (List<FunplusProduct> products);

			/// <summary>
			/// This method will be called fater the initialization fails.
			/// </summary>
			/// <param name="error">Error.</param>
			void OnInitializeError (FunplusError error);

			/// <summary>
			/// This method will be called after a purchase process succeeds.
			/// </summary>
			/// <param name="productId">The product ID.</param>
			/// <param name="throughCargo">Through cargo.</param>
			void OnPurchaseSuccess (string productId, string throughCargo);

			/// <summary>
			/// This method will be called after a purchase process fails.
			/// </summary>
			/// <param name="error">Error.</param>
			void OnPurchaseError (FunplusError error);
		}
		
		private static FunplusPayment instance;
		private static IDelegate _delegate;

		private BasePaymentWrapper GetWrapper () {
			if (Application.platform == RuntimePlatform.Android) {
				return FunplusGoogleIabAndroid.GetInstance ();
			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				return FunplusAppleIapIOS.GetInstance ();
			} else {
				return FunplusPaymentStub.GetInstance ();
			}
		}
		
		void Awake () {
			instance = this;
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		public static FunplusPayment GetInstance () {
			return instance;
		}

		/// <summary>
		/// Sets the game object and delegate to the payment module.
		/// </summary>
		/// <param name="gameObjectName">Name of the game object.</param>
		/// <param name="sdkDelegate">The delegate.</param>
		public void SetGameObjectAndDelegate (string gameObjectName, IDelegate paymentDelegate) {
			if (_delegate == null) {
				_delegate = paymentDelegate;
				GetWrapper ().SetGameObject (gameObjectName);
			} else {
				Logger.LogWarning("{FunplusPayment.SetGameObjectAndDelegate ()} --> Delegate has already been set}");
			}
		}
		
		/// <summary>
		/// Starts the payment helper.
		/// You have to call <code>SetGameObjectAndDelegate</code> before calling this method.
		/// </summary>
		public void StartHelper () {
			if (_delegate == null) {
				// set the game object and delegate first!
				Logger.LogError ("{FunplusPayment.StartHelper ()} --> Please call SetGameObjectAndDelegate() first");
			} else {
				GetWrapper ().StartHelper ();
			}
		}

		/// <summary>
		/// Test if purchase action can be performed or not.
		/// </summary>
		/// <returns><c>true</c>, if purchase action can be performed, <c>false</c> otherwise.</returns>
		public bool CanMakePurchases () {
			return (_delegate != null) && GetWrapper ().CanMakePurchases ();
		}

		/// <summary>
		/// Buy a product identified by the given product ID.
		/// </summary>
		/// <param name="productId">Product ID.</param>
		/// <param name="throughCargo">Through cargo.</param>
		public void Buy (string productId, string throughCargo) {
			GetWrapper ().Buy (productId, throughCargo);
		}

		//\cond
		#region callbacks
		public void OnInitializeSuccess (string message) {
			if (Application.platform == RuntimePlatform.Android) {
				_delegate.OnInitializeSuccess (FunplusProduct.FromGoogleIabMessage (message));
			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				_delegate.OnInitializeSuccess (FunplusProduct.FromAppleIapMessage (message));
			} else {
				_delegate.OnInitializeSuccess (FunplusProduct.FromAppleIapMessage (message));
			}
		}
		
		public void OnInitializeError (string message) {
			_delegate.OnInitializeError (FunplusError.FromMessage (message));
		}
		
		public void OnPurchaseSuccess (string message) {
			var dict = Json.Deserialize (message) as Dictionary<string,object>;
			string productId = (string)dict ["product_id"];
			string throughCargo = (string)dict ["through_cargo"];
			_delegate.OnPurchaseSuccess (productId, throughCargo);
		}
		
		public void OnPurchaseError (string message) {
			_delegate.OnPurchaseError (FunplusError.FromMessage (message));
		}
		#endregion //callbacks
		//\endcond
	}

}