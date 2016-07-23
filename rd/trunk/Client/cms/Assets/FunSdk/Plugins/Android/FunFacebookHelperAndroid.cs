#if UNITY_ANDROID

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace Funplus{

	public class FunFacebookHelperAndroid {

		private AndroidJavaClass jc = null;
		private AndroidJavaObject currentActivity, application;
		private AndroidJavaObject funPlugin;

		public FunFacebookHelperAndroid() 
		{
			if(Application.platform == RuntimePlatform.Android) {
				this.jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				this.currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
				this.application = this.currentActivity.Call<AndroidJavaObject>("getApplication");
				this.funPlugin = new AndroidJavaClass("com.funplus.sdk.unity3d.FunFbHelper4Unity3d");
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
			MonoBehaviour.print ("------FunFacebookHelperAndroid reset------");
			funApiCall ("setGameObjName", aGameObjName);
		}
		
		public void share(string aTitle, string aMessage, string aLink, string aPic)
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperAndroid, share called. -----> title: " + aTitle + "aMessage " + aMessage +
			                    "aLink " + aLink + "aPic " + aPic);

			funApiCall ("share", new object[] {aTitle, aMessage, aLink, aPic});

		}
		
		public void getUserData()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperAndroid, getUserData called. ");
			funApiCall("getUserData");
		}
		
		public void getGameFriends()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperAndroid, getGameFriends called. ");
			funApiCall("getGameFriends");
		}
		
		public bool hasFriendPermission()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperAndroid, hasFriendPermission called. ");
			//Android don't need this, just return true.
			return true;
		}
		
		public void askFriendsPermission()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperAndroid, askFriendsPermission called. ");
			//Android don't need this, just return true.
		}
		
		public void sendRequest(string aPlatformId, string aMessage)
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperAndroid sendRequest,  called. -----> aPlatformId " + aPlatformId + "aMessage " + aMessage);
			funApiCall("sendRequest", new object[]{aPlatformId, aMessage});

		}
		
		public bool hasPublishPermission()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperAndroid, hasPublishPermission called. ");
			//Android don't need this, just return true.
			return true;
		}
		
		public void askPublishPermission()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperAndroid, askPublishPermission called. ");
			//Android don't need this, just return 
		}
		
		public void pushlishOpenGraph(string aNameSpace, string aAction, string aObj, string aTitle,
		                  string aMessage, string aLink, string aPicUrl, bool aUseApiOnly)
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperAndroid, pushlishOpenGraph. called. -----> aNameSpace " + 
			                    aNameSpace + "aAction " + aAction + "aObj " + aObj + "aTitle " + aTitle +
			                    "aMessage " + aMessage + "aLink " + aLink + "aPicUrl " + aPicUrl + aUseApiOnly.ToString());

			funApiCall("pushlishOpenGraph", new object[]{aNameSpace, aAction, aObj, 
				aTitle, aMessage, aLink, aPicUrl, aUseApiOnly});
		}
	}
}

#endif

