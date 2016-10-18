/**
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015-Present Funplus
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HSMiniJSON;
using Funplus.Abstract;
using Funplus.Android;
using Funplus.IOS;
using Funplus.Stub;

namespace Funplus
{

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
	public class FunplusPayment : MonoBehaviour
	{

		/// <summary>
		/// This interface defines methods which will be called
		/// on specific moment in the life circle of the payment helper.
		/// </summary>
		public interface IDelegate
		{

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
		
		public static FunplusPayment Instance { get; set; }
		private static IDelegate _delegate;

		private BasePaymentWrapper Wrapper
		{
			get 
			{
				#if UNITY_ANDROID
				return FunplusGoogleIabAndroid.Instance;
				#elif UNITY_IOS
				return FunplusAppleIapIOS.Instance;
				#else
				return FunplusPaymentStub.Instance;
				#endif
			}
		}
		
		void Awake ()
		{
			Instance = this;
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		[Obsolete("This method is deprecated, please use FunplusPayment.Instance instead.")]
		public static FunplusPayment GetInstance ()
		{
			return Instance;
		}

		public FunplusPayment SetDelegate (IDelegate sdkDelegate)
		{
			if (sdkDelegate == null)
			{
				Debug.LogError ("[funsdk] `sdkDelegate` must not be null.");
				return Instance;
			}
			SetGameObjectAndDelegate (this.gameObject.name, sdkDelegate);

			return Instance;
		}

		/// <summary>
		/// Sets the game object and delegate to the payment module.
		/// </summary>
		/// <param name="gameObjectName">Name of the game object.</param>
		/// <param name="sdkDelegate">The delegate.</param>
		public void SetGameObjectAndDelegate (string gameObjectName, IDelegate paymentDelegate)
		{
			if (!FunplusSdk.Instance.IsSdkInstalled ()) 
			{
				Debug.LogError ("[funsdk] Please call FunplusSdk.Instance.Install() first.");
				return;
			}

			if (_delegate == null) 
			{
				_delegate = paymentDelegate;
				Wrapper.SetGameObject (gameObjectName);
			}
			else 
			{
				Debug.LogWarning ("[funsdk] Delegate has already been set.");
			}
		}
		
		/// <summary>
		/// Starts the payment helper.
		/// You have to call <code>SetGameObjectAndDelegate</code> before calling this method.
		/// </summary>
		public void StartHelper ()
		{
			if (_delegate == null) 
			{
				// set the game object and delegate first!
				Debug.LogError ("[funsdk] Please call FunplusPayment.SetDelegate() first.");
			}
			else 
			{
				Debug.Log ("[funsdk] Trying to start payment helper.");
				Wrapper.StartHelper ();
			}
		}

		/// <summary>
		/// Test if purchase action can be performed or not.
		/// </summary>
		/// <returns><c>true</c>, if purchase action can be performed, <c>false</c> otherwise.</returns>
		public bool CanMakePurchases ()
		{
			return (_delegate != null) && Wrapper.CanMakePurchases ();
		}

		/// <summary>
		/// Buy a product identified by the given product ID.
		/// </summary>
		/// <param name="productId">Product ID.</param>
		/// <param name="throughCargo">Through cargo.</param>
		public void Buy (string productId, string throughCargo)
		{
			if (string.IsNullOrEmpty (productId))
			{
				Debug.LogError ("[funsdk] `productId` must not be empty.");
				return;
			}
			if (string.IsNullOrEmpty (throughCargo))
			{
				Debug.LogWarning ("[funsdk] `throughCargo` is empty, is this really what you want?");
			}
			Wrapper.Buy (productId, throughCargo);
		}

		//\cond
		#region callbacks
		public void OnPaymentInitializeSuccess (string message)
		{
			List<FunplusProduct> products = null;

			#if UNITY_ANDROID
			products = FunplusProduct.FromGoogleIabMessage (message);
			#elif UNITY_IOS
			products = FunplusProduct.FromAppleIapMessage (message);
            #else
            products = FunplusProduct.FromAppleIapMessage(message);
			#endif

			_delegate.OnInitializeSuccess (products);
		}
		
		public void OnPaymentInitializeError (string message)
		{
			_delegate.OnInitializeError (FunplusError.FromMessage (message));
		}
		
		public void OnPaymentPurchaseSuccess (string message)
		{
			var dict = Json.Deserialize (message) as Dictionary<string,object>;
			try
			{
				string productId = (string)dict ["product_id"];
				string throughCargo = (string)dict ["through_cargo"];
				_delegate.OnPurchaseSuccess (productId, throughCargo);
			}
			catch (System.Exception e) 
			{
				Debug.LogErrorFormat ("[funsdk] Purchase failed: {0}.", e.Message);
			}
		}
		
		public void OnPaymentPurchaseError (string message)
		{
			_delegate.OnPurchaseError (FunplusError.FromMessage (message));
		}
		#endregion //callbacks
		//\endcond
	}

}