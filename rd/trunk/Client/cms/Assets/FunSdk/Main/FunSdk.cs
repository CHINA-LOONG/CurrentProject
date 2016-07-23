/*
 * FunSdk.cs
 * 
 * Create by Lei Gu 12/11/14
 * Version: v3.0.0_RC3 
 */

#if UNITY_IOS || UNITY_ANDROID

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HSMiniJSON;

namespace Funplus
{		
	public sealed class FunSdk : MonoBehaviour{
		private static Action<bool, FunplusError> mInstallCB = null;

		private static readonly object mLocker = new object();

		#if UNITY_IPHONE
		private static FunSdkIOS mNativeSdk = null;
		#elif UNITY_ANDROID
		private static FunSdkAndroid mNativeSdk = null;
		#endif

		private FunSdk()
		{
#if UNITY_IPHONE
			mNativeSdk = new FunSdkIOS();
#endif
		}

		private static FunSdk _instance = null;

#if UNITY_IPHONE

		public static FunSdk getInstance()
		{
			if(_instance == null)
			{
				lock(mLocker)
				{
					_instance = new FunSdk();
				}
			}

			return _instance;
		}
#endif

		/*
		 * The following code works fine under unity4.6
		 * but will raise a error on unity5.1:
		 * NullReferenceException: A null value was found where an object instance was required. 
		 * So I change the singleton implementation with "old style"
		 */
#if UNITY_ANDROID
		public static FunSdk instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = GameObject.FindObjectOfType<FunSdk>();
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

				mNativeSdk = new FunSdkAndroid();
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
#endif

		public void reset (string aGameObjName) 
		{
			MonoBehaviour.print ("------FunSdk reset------");
			mNativeSdk.reset (aGameObjName);
			mInstallCB = null;
		}

		public void install(string aId, string aKey, bool aIsProduct, Action<bool, FunplusError> aCB) 
		{
			if(String.IsNullOrEmpty(aId) || String.IsNullOrEmpty(aKey)|| (aCB == null))
			{
				return;
			}
			mInstallCB = aCB;

			mNativeSdk.install (aId, aKey, aIsProduct);
		}

#if UNITY_IOS
		public void setDebug(bool aIsDebug) {
			mNativeSdk.setDebug (aIsDebug);
		}
#endif

		//Callback functions
		void onFunSdkInstallFinished(string aMsg)
		{
			MonoBehaviour.print ("FunSdk, Unity onFunSdkInstallFinished, msg are: " + aMsg );

			if(String.IsNullOrEmpty(aMsg))
			{
				mInstallCB = null;
				return;
			}
			var dict = Json.Deserialize (aMsg) as Dictionary<string, object>;

			string isSuccess = (string)dict ["success"];
			FunplusError retError = FunplusUtils.makeFunplusError(dict);

			if(null != mInstallCB)
			{
				mInstallCB (Convert.ToBoolean(isSuccess), retError);
				mInstallCB = null;
			}
		}

	};
}

#endif
