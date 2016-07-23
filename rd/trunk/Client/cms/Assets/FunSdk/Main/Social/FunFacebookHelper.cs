#if UNITY_IOS || UNITY_ANDROID

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HSMiniJSON;

namespace Funplus {
	public class FunFacebookHelper : MonoBehaviour{

		private static Action<FunplusUser, FunplusError> mGetUserDataCB = null;
		private static Action<IList, FunplusError> mGetGameFriendsCB = null;
		private static Action<bool, FunplusError> mShareCB = null;
		private static Action<bool, FunplusError> mShareImageCB = null;
		private static Action<bool, FunplusError> mAskFriendPermissionCB = null;
		private static Action<bool, FunplusError> mAskPublishPermissionCB = null;
		private static Action<bool, FunplusError> mSendRequestCB = null;
		private static Action<bool, FunplusError> mPublishOpenGraphCB = null;

		#if UNITY_IOS
		private static FunFacebookHelperIOS mNativeSdk = null;
		#elif UNITY_ANDROID
		private static FunFacebookHelperAndroid mNativeSdk = null;
		#endif

		private FunFacebookHelper() 
		{
		}

		private static FunFacebookHelper _instance;
		
		public static FunFacebookHelper instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = GameObject.FindObjectOfType<FunFacebookHelper>();
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
				mNativeSdk = new FunFacebookHelperIOS();
				#elif UNITY_ANDROID
				mNativeSdk = new FunFacebookHelperAndroid();
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
			MonoBehaviour.print ("------FunFacebookHelper reset------");
			mNativeSdk.reset (aGameObjName);
			mGetUserDataCB = null;
			mGetGameFriendsCB = null;
			mShareCB = null;
			mShareImageCB = null;
			mAskFriendPermissionCB = null;
			mAskPublishPermissionCB = null;
			mSendRequestCB = null;
			mPublishOpenGraphCB = null;
		}

		public void share(string aTitle, string aMessage, string aLink, string aPicUrl, Action<bool, FunplusError> aCb)
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelper, share called. -----> title: " + aTitle + "aMessage " + aMessage +
			                    "aLink " + aLink + "aPicUrl " + aPicUrl);
			mShareCB = aCb;
			mNativeSdk.share (aTitle, aMessage, aLink, aPicUrl);
		}

#if UNITY_IPHONE
		public string getAccessToken()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelper, getAccessToken called. ");
			return mNativeSdk.getAccessToken ();
		}
#endif		
		public void getUserData(Action<FunplusUser, FunplusError> aCb)
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelper, getUserData called. ");
			mGetUserDataCB = aCb;
			mNativeSdk.getUserData ();
		}
		
		public void getGameFriends(Action<IList, FunplusError> aCb)
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelper, getGameFriends called. ");
			mGetGameFriendsCB = aCb;
			mNativeSdk.getGameFriends ();
		}
		
		public bool hasFriendPermission()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelper, hasFriendPermission called. ");
			return mNativeSdk.hasFriendPermission ();
		}

		public bool hasPublishPermission()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelper, hasPublishPermission called. ");
			return mNativeSdk.hasPublishPermission ();
		}
		
		public void askFriendsPermission(Action<bool, FunplusError> aCb)
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelper, askFriendsPermission called. ");
			mAskFriendPermissionCB = aCb;
			mNativeSdk.askFriendsPermission ();
		}

		public void askPublishPermission(Action<bool, FunplusError> aCb)
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelper, askPublishPermission called. ");
			mAskPublishPermissionCB = aCb;
			mNativeSdk.askPublishPermission ();
		}
		
		public void sendRequest(string aPlatformId, string aMessage, Action<bool, FunplusError> aCb)
		{
			MonoBehaviour.print("FunSDK Unity, sendRequest, askFriendsPermission called. -----> aPlatformId " + aPlatformId + "aMessage " + aMessage);
			mSendRequestCB = aCb;
			mNativeSdk.sendRequest (aPlatformId, aMessage);
		}

		public void pushlishOpenGraph(string aNameSpace, string aAction, string aObj, string aTitle,
		                              string aMessage, string aLink, string aPicUrl, bool aUseApiOnly, Action<bool, FunplusError> aCb)
		{
			MonoBehaviour.print("FunSDK Unity, sendRequest, askFriendsPermission called. -----> aNameSpace " + 
			                    aNameSpace + "aAction " + aAction + "aObj " + aObj + "aTitle " + aTitle +
			                    "aMessage " + aMessage + "aLink " + aLink + "aPicUrl " + aPicUrl + aUseApiOnly.ToString());
			mPublishOpenGraphCB = aCb;
			mNativeSdk.pushlishOpenGraph (aNameSpace, aAction, aObj, aTitle, aMessage, aLink, aPicUrl, aUseApiOnly);
		}

		//callback
		void onFbGetUserDataFinished(string aMsg)
		{
			Debug.Log ("FunFacebookHelper, onFbGetUserDataFinished" + aMsg);

			if(String.IsNullOrEmpty(aMsg))
			{
				mGetUserDataCB = null;
				return;
			}

			var dict = Json.Deserialize(aMsg) as Dictionary<string,object>;
			FunplusError retError = FunplusUtils.makeFunplusError(dict);
			FunplusUser retUser = FunplusUtils.makeFunplusUser(dict);

			if(null != mGetUserDataCB)
			{
				mGetUserDataCB(retUser, retError);
				mGetUserDataCB = null;
			}
		}

		void onFbGetGameFriendsFinished(string aMsg)
		{
			Debug.Log ("FunFacebookHelper, onFbGetGameFriendsFinished" + aMsg);

			if(String.IsNullOrEmpty(aMsg))
			{
				mGetGameFriendsCB = null;
				return;
			}

			var dict = Json.Deserialize(aMsg) as Dictionary<string, object>;
			FunplusError retError = FunplusUtils.makeFunplusError(dict);

			var retList = dict["friends"];
			IList newList = (IList)retList;

			if(null != mGetGameFriendsCB)
			{
				mGetGameFriendsCB(newList, retError);
				mGetGameFriendsCB = null;
			}
		}
		
		void onFbShareFinished(string aMsg)
		{
			Debug.Log ("FunFacebookHelper, onFbShareFinished" + aMsg);

			if(String.IsNullOrEmpty(aMsg))
			{
				mShareCB = null;
				return;
			}
			
			var dict = Json.Deserialize(aMsg) as Dictionary<string, object>;
			bool retBool = (string) dict["success"] == "true" ? true : false;
			FunplusError retError = FunplusUtils.makeFunplusError(dict);

			if(null != mShareCB)
			{
				mShareCB(retBool, retError);
				mShareCB = null;
			}
		}

//		void onFbShareImageFinished(string aMsg)
//		{
//			bool ret = (aMsg == "true" ? true : false);
//			if(null != mShareCB)
//			{
//				mShareImageCB(ret);
//				mShareImageCB = null;
//			}
//		}

		void onFbAskFriendsPermissionFinished(string aMsg)
		{
			Debug.Log ("FunFacebookHelper, onFbAskFriendsPermissionFinished" + aMsg);

			var dict = Json.Deserialize(aMsg) as Dictionary<string, object>;
			bool retBool = (string) dict["success"] == "true" ? true : false;
			FunplusError retError = FunplusUtils.makeFunplusError(dict);

			if(null != mAskFriendPermissionCB)
			{
				mAskFriendPermissionCB(retBool, retError);
				mAskFriendPermissionCB = null;
			}
		}

		void onFbAskPublishPermissionFinished(string aMsg)
		{
			Debug.Log ("FunFacebookHelper, onFbAskPublishPermissionFinished" + aMsg);

			var dict = Json.Deserialize(aMsg) as Dictionary<string, object>;
			bool retBool = (string) dict["success"] == "true" ? true : false;
			FunplusError retError = FunplusUtils.makeFunplusError(dict);
			if(null != mAskPublishPermissionCB)
			{
				mAskPublishPermissionCB(retBool, retError);
				mAskPublishPermissionCB = null;
			}
		}
		
		void onFbSendRequestFinished(string aMsg)
		{
			if(String.IsNullOrEmpty(aMsg))
			{
				mSendRequestCB = null;
				return;
			}
			
			var dict = Json.Deserialize(aMsg) as Dictionary<string, object>;
			bool retBool = (string) dict["success"] == "true" ? true : false;
			FunplusError retError = FunplusUtils.makeFunplusError(dict);
			
			if(null != mSendRequestCB)
			{
				mSendRequestCB(retBool, retError);
				mSendRequestCB = null;
			}
		}

		void onPublishOpenGraphFinished(string aMsg)
		{
			if(String.IsNullOrEmpty(aMsg))
			{
				mPublishOpenGraphCB = null;
				return;
			}
			
			var dict = Json.Deserialize(aMsg) as Dictionary<string, object>;
			bool retBool = (string) dict["success"] == "true" ? true : false;
			FunplusError retError = FunplusUtils.makeFunplusError(dict);
			
			if(null != mPublishOpenGraphCB)
			{
				mPublishOpenGraphCB(retBool, retError);
				mPublishOpenGraphCB = null;
			}

		}

	}
}

#endif