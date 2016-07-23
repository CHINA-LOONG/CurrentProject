/*
 * FunAccountAndroid.cs
 * 
 * Create by Lei Gu 12/11/14
 */

#if UNITY_ANDROID

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HSMiniJSON;

namespace Funplus
{	
	public class FunAccountAndroid {
		
		private AndroidJavaClass jc;
		private AndroidJavaObject currentActivity, application;
		private AndroidJavaObject funPlugin;
		
		public FunAccountAndroid() {
			if(Application.platform == RuntimePlatform.Android)
			{
				this.jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				this.currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
				this.application = this.currentActivity.Call<AndroidJavaObject>("getApplication");
				
				this.funPlugin = new AndroidJavaClass("com.funplus.sdk.unity3d.FunAccount4Unity3d");
			}
		}
		
		private void funApiCall(string aApi, params object[] aArgs) 
		{
			this.funPlugin.CallStatic (aApi, aArgs);
		}
		
		private void funApiCall(string aApi) 
		{
			this.funPlugin.CallStatic (aApi);
		}
		
		private void funApiCallRunOnUiThread(string aApi, params object[] aArgs)
		{
			this.currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			                                                                   {
				this.funPlugin.CallStatic (aApi, aArgs);
			}));
		}
		
		public void reset (string aGameObjName) 
		{
			MonoBehaviour.print ("------FunSdkAndroid reset------");
			funApiCall ("setGameObjName", aGameObjName);
		}
		
		public void login() 
		{
			MonoBehaviour.print ("------FunSdkAndroid login------");
			funApiCall("login");
		}
		
		public void login(FunplusAccountType aType) 
		{
			MonoBehaviour.print ("------FunSdkAndroid login------");
			MonoBehaviour.print ("------FunSdkAndroid login type is: ------" + Convert.ToString((int)aType));
			funApiCall("login", (int)aType);
		}
		
		public void showUserCenter() 
		{
			MonoBehaviour.print ("------FunSdkAndroid showUserCenter------");
			funApiCall("showUserCenter");
		}
		
		public void logout() 
		{
			MonoBehaviour.print ("------FunSdkAndroid logout------");
			funApiCall("logout");
		}
		
		public void getAvailableAccountTypes() 
		{
			MonoBehaviour.print ("------FunSdkAndroid getAvailableAccountTypes------");
			funApiCall("getAvailableAccountTypes");
		}
		
		public void getLoginStatus() 
		{
			MonoBehaviour.print ("------FunSdkAndroid getLoginStatus------");
			funApiCall("getLoginStatus");
		}
		
		public void bindAccount(FunplusAccountType aType) 
		{
			MonoBehaviour.print ("------FunSdkAndroid bindAccount------");
			funApiCall("bindAccount", (int)aType);
		}
		
		public void onPause()
		{
			MonoBehaviour.print ("------FunSdkAndroid onPause------");
			funApiCall("onPause");
		}
		
		public void onResume()
		{
			MonoBehaviour.print ("------FunSdkAndroid onResume------");
			funApiCall("onResume");
		}
		
		public void onDestroy()
		{
			MonoBehaviour.print ("------FunSdkAndroid onDestroy------");
			funApiCall("onDestroy");
		}
		
		public void onActivityResult()
		{
			MonoBehaviour.print ("------FunSdkAndroid onActivityResult------");
			funApiCall("onActivityResult");
		}
		
		//		public void onNewIntent()
		//		{
		//			MonoBehaviour.print ("------FunSdkAndroid onNewIntent------");
		//			funApiCall("onNewIntent");
		//		}
		
		public void onBackPressed()
		{
			MonoBehaviour.print ("------FunSdkAndroid onBackPressed------");
			funApiCall("onBackPressed");
		}
	};
}

#endif


