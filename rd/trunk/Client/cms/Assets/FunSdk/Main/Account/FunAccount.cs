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
	public sealed class FunAccount : MonoBehaviour{
		private static Action<FunplusAccountStatus, FunplusError, string, string> mLoginCB = null;
		private static Action<FunplusAccountStatus, FunplusError, string, string> mShowUserCB = null;
		private static Action<IList, FunplusError> mLoginTypesCB = null;
		private static Action<FunplusAccountStatus> mLoginStateCB = null;
		private static Action<FunplusAccountStatus, FunplusError, string, string> mBindAccountCB = null;
		private static Action<FunplusAccountStatus, FunplusError, string, string> mLogoutCB = null;
		
		#if UNITY_IPHONE
		private static FunAccountIOS mNativeSdk = null;
		#elif UNITY_ANDROID
		private static FunAccountAndroid mNativeSdk = null;
		#endif
		
		private FunAccount()
		{
			
		}
		
		private static FunAccount _instance;
		
		public static FunAccount instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = GameObject.FindObjectOfType<FunAccount>();
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
				mNativeSdk = new FunAccountIOS();
				#elif UNITY_ANDROID
				mNativeSdk = new FunAccountAndroid();
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
			MonoBehaviour.print ("------FunSdk reset------");
			mNativeSdk.reset (aGameObjName);
			mLoginCB = null;
			mShowUserCB = null;
			mLoginTypesCB = null;
			mLoginStateCB = null;
			mBindAccountCB = null;
		}
		
		public void login(Action<FunplusAccountStatus, FunplusError, string, string> aCB)
		{
			mLoginCB = aCB;
			mNativeSdk.login ();
		}
		
		public void login(FunplusAccountType aType, Action<FunplusAccountStatus, FunplusError, string, string> aCB) 
		{
			mLoginCB = aCB;
			mNativeSdk.login (aType);
		}
		
		public void showUserCenter(Action<FunplusAccountStatus, FunplusError, string, string> aCB) 
		{
			mShowUserCB = aCB;
			mNativeSdk.showUserCenter ();
		}
		
		public void logout(Action<FunplusAccountStatus, FunplusError, string, string> aCB) 
		{
			mLogoutCB = aCB;
			mNativeSdk.logout ();
		}
		
		public void getAvailableAccountTypes(Action<IList, FunplusError> aCB) 
		{
			mLoginTypesCB = aCB;
			mNativeSdk.getAvailableAccountTypes ();
		}
		
		public void getLoginStatus(Action<FunplusAccountStatus> aCB) 
		{
			mLoginStateCB = aCB;
			mNativeSdk.getLoginStatus ();
		}
		
		public void bindAccount(FunplusAccountType aType, Action<FunplusAccountStatus, FunplusError, string, string> aCB) 
		{
			mBindAccountCB = aCB;
			mNativeSdk.bindAccount (aType);
		}
		
		#if UNITY_ANDROID
		public void onPause() 
		{
			mNativeSdk.onPause();
		}
		
		public void onResume() 
		{
			mNativeSdk.onResume();
		}
		
		public void onDestroy() 
		{
			mNativeSdk.onDestroy();
		}
		
		public void onActivityResult() 
		{
			mNativeSdk.onActivityResult();
		}
		
		public void onNewIntent() 
		{
			//	mNativeSdk.onNewIntent();
		}
		
		public void onBackPressed() 
		{
			mNativeSdk.onBackPressed();
		}
		#endif
		
		//Callback functions
		void onFunSdkLoginFinished(string aMsg)
		{
			MonoBehaviour.print ("FunSdk, Unity onFunSdkLoginFinished, msg are: " + aMsg );
			
			if(String.IsNullOrEmpty(aMsg))
			{
				mLoginCB = null;
				return;
			}
			
			var dict = Json.Deserialize(aMsg) as Dictionary<string,object>;
			string stateInt = (string)dict["state"];
			FunplusAccountStatus state = (FunplusAccountStatus)Convert.ToInt32(stateInt);
			string fpid = (string) dict["fpid"];
			string sessionKey = (string)dict["sessionkey"];
			
			FunplusError retError = FunplusUtils.makeFunplusError(dict);
			
			if(null != mLoginCB)
			{
				mLoginCB(state, retError, fpid, sessionKey);
				mLoginCB = null;
			}
		}
		
		void onFunSdkShowUserCenterFinished(string aMsg)
		{
			MonoBehaviour.print ("FunSdk, Unity onFunSdkShowUserCenterFinished, msg are: " + aMsg );
			
			if(String.IsNullOrEmpty(aMsg))
			{
				return;
			}
			
			var dict = Json.Deserialize(aMsg) as Dictionary<string,object>;
			string stateInt = (string)dict["state"];
			FunplusAccountStatus state = (FunplusAccountStatus)Convert.ToInt32(stateInt);
			string fpid = (string) dict["fpid"];
			string sessionKey = (string)dict["sessionkey"];
			FunplusError retError = FunplusUtils.makeFunplusError(dict);
			
			if(null != mShowUserCB)
			{
				mShowUserCB(state, retError, fpid, sessionKey);
				mShowUserCB = null;
			}
			
		}
		
		void onFunSdkGetAvailableAccountTypesFinished(string aMsg)
		{
			MonoBehaviour.print ("FunSdk, Unity onFunSdkGetAvailableAccountTypesFinished, msg are: " + aMsg );
			
			if(String.IsNullOrEmpty(aMsg))
			{
				return;
			}
			
			var dict = Json.Deserialize(aMsg) as Dictionary<string,object>;
			
			if(dict.ContainsKey("types"))
			{
				var retList = dict["types"];
				IList newList = (IList)retList;
				FunplusError retError = FunplusUtils.makeFunplusError(dict);
				
				if(null != mLoginTypesCB)
				{
					mLoginTypesCB(newList, retError);
					mLoginTypesCB = null;
				}
			}
			
		}
		
		void onFunSdkGetLoginStatusFinished(string aMsg)
		{
			MonoBehaviour.print ("FunSdk, Unity onFunSdkGetLoginStatusFinished, msg are: " + aMsg );
			
			if(String.IsNullOrEmpty(aMsg))
			{
				return;
			}
			
			if(null != mLoginStateCB)
			{
				mLoginStateCB((FunplusAccountStatus)Convert.ToInt32(aMsg));
				mLoginStateCB = null;
			}
			
		}
		
		void onFunSdkBindAccountFinished(string aMsg)
		{
			MonoBehaviour.print ("FunSdk, Unity onFunSdkBindAccountWithouUIFinished, msg are: " + aMsg );
			
			if(String.IsNullOrEmpty(aMsg))
			{
				return;
			}
			
			var dict = Json.Deserialize(aMsg) as Dictionary<string,object>;
			string stateInt = (string)dict["state"];
			FunplusAccountStatus state = (FunplusAccountStatus)Convert.ToInt32(stateInt);
			string fpid = (string) dict["fpid"];
			string sessionKey = (string)dict["sessionkey"];
			FunplusError retError = FunplusUtils.makeFunplusError(dict);
			
			if(null != mBindAccountCB)
			{
				mBindAccountCB(state, retError, fpid, sessionKey);
				mBindAccountCB = null;
			}
			
		}
		
		void onFunSdkLogoutFinished(string aMsg)
		{
			MonoBehaviour.print ("FunSdk, Unity onFunSdkLogoutFinished, msg are: " + aMsg );
			
			if(String.IsNullOrEmpty(aMsg))
			{
				return;
			}
			
			var dict = Json.Deserialize(aMsg) as Dictionary<string,object>;
			string stateInt = (string)dict["state"];
			FunplusAccountStatus state = (FunplusAccountStatus)Convert.ToInt32(stateInt);
			string fpid = (string) dict["fpid"];
			string sessionKey = (string)dict["sessionkey"];
			FunplusError retError = FunplusUtils.makeFunplusError(dict);
			
			if(null != mLogoutCB)
			{
				mLogoutCB(state, retError, fpid, sessionKey);
				mLogoutCB = null;
			}
		}
	};
}

#endif
