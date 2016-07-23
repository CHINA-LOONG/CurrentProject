#if UNITY_IOS || UNITY_ANDROID

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HSMiniJSON;

namespace Funplus{

	public class FunPayment : MonoBehaviour {

		private static Action<string, FunplusError> mOrderCompleteCB = null;
		private static Action<IList, FunplusError> mGetProductsIdsCB = null;

#if UNITY_IPHONE
		private static FunPaymentIOS mNativeSdk = null;
#elif UNITY_ANDROID
		private static FunPaymentAndroid mNativeSdk = null;
#endif

		private static FunPayment _instance;

		public static FunPayment instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = GameObject.FindObjectOfType<FunPayment>();
					DontDestroyOnLoad(_instance.gameObject);
				}

				return _instance;
			}
		}

		void Awake()
		{
			if(_instance == null)
			{
				_instance = this;
				#if UNITY_IPHONE
				mNativeSdk = new FunPaymentIOS();
				#elif UNITY_ANDROID
				mNativeSdk = new FunPaymentAndroid();
				#endif
				DontDestroyOnLoad(this);
			}
			else
			{
				if(this != _instance)
				{
					Destroy(this.gameObject);
				}
			}
		}
		
		public void reset (string aGameObjName) 
		{
			print ("------FunPayment reset------");
			mNativeSdk.reset (aGameObjName);
			mOrderCompleteCB = null;
		}

		public void initPaymentEnv(Action<IList, FunplusError> aProdInfoCb, Action<string, FunplusError> aBuyCb)
		{
			print ("FunPayment initPaymentEnv");
			mGetProductsIdsCB = aProdInfoCb;
			mOrderCompleteCB = aBuyCb;
#if UNITY_ANDROID
			mNativeSdk.setResultHandler();
#elif UNITY_IOS
			mNativeSdk.setOrderCompleteCallback();
			mNativeSdk.getProductsInfo ();
#endif
		}

//		public void getProductsInfo(Action<IList> aCb)
//		{
//			mGetProductsIdsCB = aCb;
//			mNativeSdk.getProductsInfo ();
//		}
//
//		public void setOrderCompleteCallback(Action<Dictionary<string, object>> aCb)
//		{
//			mOrderCompleteCB = aCb;
//			mNativeSdk.setOrderCompleteCallback();
//		}

		public void buy(string aProId, string aThroughCargo)
		{
			print ("FunPayment buy called" );
			mNativeSdk.buy (aProId, aThroughCargo);
		}

		//callbacks from plugins
		public void onGetProductsIdsFinished(string aMsg)
		{
			print ("FunPayment onGetProductsIdsFinished, msg are: " + aMsg );

			if("" == aMsg)
			{
				mGetProductsIdsCB = null;
				return;
			}

			var dict = Json.Deserialize(aMsg) as Dictionary<string,object>;
			FunplusError retError = FunplusUtils.makeFunplusError(dict);
			IList newList = (IList)dict["prods"];

			if(null != mGetProductsIdsCB)
			{
				mGetProductsIdsCB(newList, retError);
				mGetProductsIdsCB = null;
			}
		}

		public void onOrderFinished(string aMsg)
		{
			print ("FunPayment onOrderFinished, msg are: " + aMsg );

			if("" == aMsg)
			{
				mGetProductsIdsCB = null;
				return;
			}
			
			var dict = Json.Deserialize(aMsg) as Dictionary<string,object>;
			string throughCargo = (string) dict["throughcargo"];
			FunplusError retError = FunplusUtils.makeFunplusError(dict);

			if(null != mOrderCompleteCB)
			{
				mOrderCompleteCB(throughCargo, retError);
				mOrderCompleteCB = null;
			}
		}

	}
}

#endif

