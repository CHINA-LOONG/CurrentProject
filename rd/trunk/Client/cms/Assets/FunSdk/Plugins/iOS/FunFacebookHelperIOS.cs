#if UNITY_IPHONE

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace Funplus{

	public class FunFacebookHelperIOS {

		[DllImport ("__Internal")]
		private static extern void obj_fp_sdk_fb_setObjName (string aGameObjName);
		[DllImport ("__Internal")]
		private static extern void obj_fp_sdk_fb_share (string aTitle, string aMessage, string aLink, string aPic);
		[DllImport ("__Internal")]
		private static extern void obj_fp_sdk_fb_share_image (string aPicPath, string aMessage, string aLink);
		[DllImport ("__Internal")]
		private static extern string obj_fp_sdk_fb_get_access_token ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_sdk_fb_get_user_data ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_sdk_fb_get_game_friends ();
		[DllImport ("__Internal")]
		private static extern bool obj_fp_sdk_fb_has_friends_permission ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_sdk_fb_ask_friends_permission ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_sdk_fb_send_request (string aPlatformId, string aMessage);
		[DllImport ("__Internal")]
		private static extern bool obj_fp_sdk_fb_has_publish_permission ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_sdk_fb_ask_publish_permission ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_sdk_fb_publish_open_graph (string aNameSpace, string aAction, string aObj, string aTitle,
		                                                             string aMessage, string aLink, string aPicUrl, bool aUseApiOnly);

		public FunFacebookHelperIOS() {

		}

		public void reset (string aGameObjName) 
		{
			MonoBehaviour.print ("------FunFacebookHelperIOS reset------");
			obj_fp_sdk_fb_setObjName (aGameObjName);
		}
		
		public void share(string aTitle, string aMessage, string aLink, string aPicUrl)
		{
			MonoBehaviour.print("FunFacebookHelperIOS, share called. -----> title: " + aTitle + "aMessage " + aMessage +
			                    "aLink " + aLink + "aPicUrl " + aPicUrl);
			
			obj_fp_sdk_fb_share (aTitle, aMessage, aLink, aPicUrl);
		}

//		Please user share function.
//		public void shareImage(string aPicPath, string aMessage, string aLink)
//		{
//			MonoBehaviour.print("FunFacebookHelperIOS, shareImage called. -----> aPicPath is: " + aPicPath + "aMessage " + aMessage +
//			                    "aLink " + aLink);
//			obj_fp_sdk_fb_share_image (aPicPath, aMessage, aLink);
//		}
		
		public string getAccessToken()
		{
			MonoBehaviour.print("FunFacebookHelperIOS, getAccessToken called. ");
			return obj_fp_sdk_fb_get_access_token ();
		}
		
		public void getUserData()
		{
			MonoBehaviour.print("FunFacebookHelperIOS, getUserData called. ");
			obj_fp_sdk_fb_get_user_data ();
		}
		
		public void getGameFriends()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperIOS, getGameFriends called. ");
			obj_fp_sdk_fb_get_game_friends ();
		}
		
		public bool hasFriendPermission()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperIOS, hasFriendPermission called. ");
			return obj_fp_sdk_fb_has_friends_permission ();
		}
		
		public void askFriendsPermission()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperIOS, askFriendsPermission called. ");
			obj_fp_sdk_fb_ask_friends_permission ();
		}
		
		public void sendRequest(string aPlatformId, string aMessage)
		{
			MonoBehaviour.print("FunSDK Unity, sendRequest, askFriendsPermission called. -----> aPlatformId " + aPlatformId + "aMessage " + aMessage);
			
			obj_fp_sdk_fb_send_request (aPlatformId, aMessage);
		}
		
		public bool hasPublishPermission()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperIOS, hasPublishPermission called. ");
			return obj_fp_sdk_fb_has_publish_permission ();
		}
		
		public void askPublishPermission()
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperIOS, askPublishPermission called. ");
			obj_fp_sdk_fb_ask_publish_permission ();
		}
		
		public void pushlishOpenGraph(string aNameSpace, string aAction, string aObj, string aTitle,
		                  string aMessage, string aLink, string aPicUrl, bool aUseApiOnly)
		{
			MonoBehaviour.print("FunSDK Unity, FunFacebookHelperIOS, pushlishOpenGraph. called. -----> aNameSpace " + 
			                    aNameSpace + "aAction " + aAction + "aObj " + aObj + "aTitle " + aTitle +
			                    "aMessage " + aMessage + "aLink " + aLink + "aPicUrl " + aPicUrl + aUseApiOnly.ToString());
			
			obj_fp_sdk_fb_publish_open_graph (aNameSpace, aAction, aObj, aTitle, aMessage, aLink, aPicUrl, aUseApiOnly);
		}
	}
}

#endif

