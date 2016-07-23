/*
 * FunSdkAndroid.cs
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
	public class FunSdkAndroid {

		private AndroidJavaClass jc;
		private AndroidJavaObject currentActivity, application;
		private AndroidJavaObject funPlugin;
		
		public FunSdkAndroid() {
			if(Application.platform == RuntimePlatform.Android)
			{
				this.jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				this.currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
				this.application = this.currentActivity.Call<AndroidJavaObject>("getApplication");

				this.funPlugin = new AndroidJavaClass("com.funplus.sdk.unity3d.FunSdk4Unity3d");
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
		
		public void reset (string aGameObjName) 
		{
			MonoBehaviour.print ("------FunSdkAndroid reset------");
			funApiCall ("setGameObjName", aGameObjName);
		}

		public void install(string aId, string aKey, bool aIsProduct) 
		{
			MonoBehaviour.print ("------FunSdkAndroid install------");

			if(String.IsNullOrEmpty(aId) || String.IsNullOrEmpty(aKey) )
			{
				return;
			}

			funApiCall ("install", new object[] {this.currentActivity, aId, aKey, aIsProduct});
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


