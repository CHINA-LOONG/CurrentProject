#if UNITY_IPHONE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HSMiniJSON;

namespace Funplus {

	/// <summary>
	/// The Apple IAP module.
	/// </summary>
	public class FunplusAppleIap : MonoBehaviour {

		/// <summary>
		/// This interface defines methods which will be called
		/// on specific moment in the life circle of the Apple IAP helper.
		/// </summary>
		public interface IDelegate {
			void OnInitializeSuccess (string productsInfo);
			void OnInitializeError (FunplusError error);
			void OnPurchaseSuccess (string productId, string throughCargo);
			void OnPurchaseError (FunplusError error);
		}

		private static FunplusAppleIap instance;
		private static IDelegate _delegate;

		private FunplusAppleIapIOS GetWrapper () {
			return FunplusAppleIapIOS.GetInstance ();
		}

		void Awake () {
			instance = this;
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		public static FunplusAppleIap GetInstance () {
			return instance;
		}

		/// <summary>
		/// Sets the game object and delegate to the Apple IAP module.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		/// <param name="sdkDelegate">The delegate.</param>
		public void SetGameObjectAndDelegate (string gameObjectName, IDelegate appleiapDelegate) {
			if (_delegate == null) {
				_delegate = appleiapDelegate;
				GetWrapper ().SetGameObject (gameObjectName);
			} else {
				Logger.LogWarning("{FunplusAppleIap.SetGameObjectAndDelegate ()} --> Delegate has already been set}");
			}
		}

		/// <summary>
		/// Starts the payment helper.
		/// You have to call <code>SetGameObjectAndDelegate</code> before calling this method.
		/// </summary>
		public void StartHelper () {
			GetWrapper ().StartHelper ();
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
		public void Buy(string productId, string throughCargo) {
			GetWrapper ().Buy (productId, throughCargo);
		}

		//\cond
		#region callbacks
		public void OnInitializeSuccess(string message) {
			_delegate.OnInitializeSuccess (message);
		}

		public void OnInitializeError(string message) {
			_delegate.OnInitializeError (FunplusError.FromMessage (message));
		}
		
		public void OnPurchaseSuccess(string message) {
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

#endif