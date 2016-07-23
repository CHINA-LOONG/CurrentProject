#if UNITY_IOS || UNITY_ANDROID

using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using HSMiniJSON;
using Funplus;

namespace Funplus {
	public class FunTestCallbacks {

		public FunTestCallbacks()
		{
		
		}

		public static void funsdkInstallCB(bool aIsSuccess, FunplusError aError)
		{
			MonoBehaviour.print ("install success " + aIsSuccess.ToString());
			MonoBehaviour.print("install error code is " + aError.getErrorCode().ToString());
			MonoBehaviour.print ("install error msg is" + aError.getErrorMsg());
			MonoBehaviour.print ("install error msg is" + aError.getLocalizedMsg());

		}
		
		public static void funsdkLoginCBEX(FunplusAccountStatus aState, 
		                                   FunplusError aError, string aFpid, string aSeesionKey)
		{
			MonoBehaviour.print ("funsdkLoginCBEX called");
			MonoBehaviour.print("funsdkLoginCBEX fpid is " + aFpid);
			MonoBehaviour.print("funsdkLoginCBEX aSeesionKey is " + aSeesionKey);
			MonoBehaviour.print("funsdkLoginCBEX errorMsg is " + aError.getErrorMsg());
			MonoBehaviour.print("funsdkLoginCBEX errorLocalMsg is " + aError.getLocalizedMsg());

			if(aError.getErrorCode() == 0) {
				FunTest.mShowFlag = FunTest.ESHOWGROUPID.eshowAccountMore;
			}

		}
		
		public static void funsdkLoginCBFB(FunplusAccountStatus aState, 
		                                   FunplusError aError, string aFpid, string aSeesionKey)
		{
			MonoBehaviour.print ("funsdkLoginCBFB called");
			
			MonoBehaviour.print ("funsdkLoginCBFB called");
			MonoBehaviour.print("funsdkLoginCBFB fpid is " + aFpid);
			MonoBehaviour.print("funsdkLoginCBFB aSeesionKey is " + aSeesionKey);
			MonoBehaviour.print("funsdkLoginCBFB errorMsg is " + aError.getErrorMsg());
			MonoBehaviour.print("funsdkLoginCBFB errorLocalMsg is " + aError.getLocalizedMsg());

			if(aError.getErrorCode() == 0) {
				FunTest.mShowFlag = FunTest.ESHOWGROUPID.eShowFBService;
			}

		}
		
		public static void funsdkLoginCBVK(FunplusAccountStatus aState, 
		                                   FunplusError aError, string aFpid, string aSeesionKey)
		{
			MonoBehaviour.print ("funsdkLoginCBVK called");
			
			MonoBehaviour.print ("funsdkLoginCBVK called");
			MonoBehaviour.print("funsdkLoginCBVK fpid is " + aFpid);
			MonoBehaviour.print("funsdkLoginCBVK aSeesionKey is " + aSeesionKey);
			MonoBehaviour.print("funsdkLoginCBVK errorMsg is " + aError.getErrorMsg());
			MonoBehaviour.print("funsdkLoginCBVK errorLocalMsg is " + aError.getLocalizedMsg());

			if(aError.getErrorCode() == 0) {
				FunTest.mShowFlag = FunTest.ESHOWGROUPID.eShowVKService;
			}
		}
		
		public static void funsdkLoginCBWX(FunplusAccountStatus aState, 
		                                   FunplusError aError, string aFpid, string aSeesionKey)
		{
			MonoBehaviour.print ("funsdkLoginCBWX called");
			
			MonoBehaviour.print("funsdkLoginCBWX called");
			MonoBehaviour.print("funsdkLoginCBWX fpid is " + aFpid);
			MonoBehaviour.print("funsdkLoginCBWX aSeesionKey is " + aSeesionKey);
			MonoBehaviour.print("funsdkLoginCBWX errorMsg is " + aError.getErrorMsg());
			MonoBehaviour.print("funsdkLoginCBWX errorLocalMsg is " + aError.getLocalizedMsg());
			
			if(aError.getErrorCode() == 0) {
				FunTest.mShowFlag = FunTest.ESHOWGROUPID.eShowWXService;
			}
		}
		
		public static void funsdkLoginCBUI(FunplusAccountStatus aState, 
		                                   FunplusError aError, string aFpid, string aSeesionKey)
		{
			MonoBehaviour.print ("funsdkLoginCBUI called");
			
			MonoBehaviour.print("funsdkLoginCBUI called");
			MonoBehaviour.print("funsdkLoginCBUI fpid is " + aFpid);
			MonoBehaviour.print("funsdkLoginCBUI aSeesionKey is " + aSeesionKey);
			MonoBehaviour.print("funsdkLoginCBUI errorMsg is " + aError.getErrorMsg());
			MonoBehaviour.print("funsdkLoginCBUI errorLocalMsg is " + aError.getLocalizedMsg());
			
			if(aError.getErrorCode() == 0) {
				FunTest.mShowFlag = FunTest.ESHOWGROUPID.eshowAccountMore;
			}
		}

		public static void funsdkLogoutCB(FunplusAccountStatus aState, 
		                                  FunplusError aError, string aFpid, string aSeesionKey)
		{
			MonoBehaviour.print ("funsdkLogoutCB called");
			
			MonoBehaviour.print("funsdkLogoutCB called");
			MonoBehaviour.print("funsdkLogoutCB fpid is " + aFpid);
			MonoBehaviour.print("funsdkLogoutCB aSeesionKey is " + aSeesionKey);
			MonoBehaviour.print("funsdkLogoutCB errorMsg is " + aError.getErrorMsg());
			MonoBehaviour.print("funsdkLogoutCB errorLocalMsg is " + aError.getLocalizedMsg());
			
			if(aError.getErrorCode() == 0) {
				FunTest.mShowFlag = FunTest.ESHOWGROUPID.eShowLogin;
			}
		}
		
		public static void funsdkShowUserCenterCB(FunplusAccountStatus aState, 
		                                          FunplusError aError, string aFpid, string aSeesionKey)
		{
			MonoBehaviour.print ("funsdkShowUserCenterCB called");
			
			MonoBehaviour.print("funsdkShowUserCenterCB called");
			MonoBehaviour.print("funsdkShowUserCenterCB fpid is " + aFpid);
			MonoBehaviour.print("funsdkShowUserCenterCB errorMsg is " + aError.getErrorMsg());
			MonoBehaviour.print("funsdkShowUserCenterCB errorLocalMsg is " + aError.getLocalizedMsg());		
		}
		
		public static void funsdkGetAvailableAccountTypesCB(IList aList, FunplusError aError)
		{
			MonoBehaviour.print ("funsdkGetAvailableAccountTypesCB called");
			MonoBehaviour.print ("first types " + aList[0].ToString());
			MonoBehaviour.print("funsdkGetAvailableAccountTypesCB errorMsg is " + aError.getErrorMsg());
			MonoBehaviour.print("funsdkGetAvailableAccountTypesCB errorLocalMsg is " + aError.getLocalizedMsg());
		}
		
		public static void funsdkGetLoginStatusCB(FunplusAccountStatus aStatus)
		{
			MonoBehaviour.print ("funsdkGetLoginStatusCB called, status is: " + Convert.ToString(aStatus));
		}

		 //Facebook callbacks
		public static void funFbGetGameFriendsCB(IList aList, FunplusError aError)
		{
			MonoBehaviour.print ("funFbGetGameFriendsCB called");

			if(aError.getErrorCode() == FunplusError.EFunplusErrorCode.ErrorNone) 
			{
				for(int i=0; i<aList.Count; ++i) 
				{
					MonoBehaviour.print("user index is : " + i);
					IDictionary tempDic = ((IDictionary)aList[i]);
					MonoBehaviour.print ("user uid is: "  + tempDic["uid"]);
					MonoBehaviour.print ("user name is: "  + tempDic["name"]);
					MonoBehaviour.print ("user email is: "  + tempDic["email"]);
					MonoBehaviour.print ("user pic is: "  + tempDic["pic"]);
					MonoBehaviour.print ("user gender is: "  + tempDic["gender"]);
				}
			}
			else
			{
				MonoBehaviour.print("funFbGetGameFriendsCB wrong, error are: " + aError.getErrorMsg() );
			}
		}

		public static void funFbGetUserDataCB(FunplusUser aUser, FunplusError aError)
		{
			MonoBehaviour.print ("funFbGetUserDataCB called");

			if(aError.getErrorCode() == 0) 
			{
				MonoBehaviour.print("uid:" + aUser.getUid());
				MonoBehaviour.print("name: " + aUser.getName());
				MonoBehaviour.print("email: " + aUser.getEmail());
				MonoBehaviour.print("gender: " + aUser.getGender());
				MonoBehaviour.print("access token: " + aUser.getAccessToken());
			}
			else 
			{
				MonoBehaviour.print("funFbGetUserDataCB wrong, error are: " + aError.getErrorMsg());
			}

		}

		public static void funFbShareCB(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("funFbShare called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));
		}

		public static void funFbShareImageCB(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("funFbShareImageCB called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));
		}

		public static void funFbSendRequestCB(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("funFbSendRequestCB called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));
		}

		public static void funFbPublishOpenGraphCB(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("funFbPublishOpenGraphCB called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));
		}

		public static void funFbAskFriendsPermissionCB(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("funFbAskFriendsPermissionCB called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));
		}

		public static void funFbAskPublishPermissionCB(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("funFbAskPublishPermissionCB called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));
		}

		//Vk callbacks
		public static void funVkGetGameFriendsCB(IList aList, FunplusError aError)
		{
			MonoBehaviour.print ("funVkGetGameFriendsCB called");
			
			if(aError.getErrorCode() == 0) 
			{
				for(int i=0; i<aList.Count; ++i) 
				{
					MonoBehaviour.print("user index is : " + i);
					IDictionary tempDic = ((IDictionary)aList[i]);
					MonoBehaviour.print ("user uid is: "  + tempDic["uid"]);
					MonoBehaviour.print ("user name is: "  + tempDic["name"]);
					MonoBehaviour.print ("user email is: "  + tempDic["email"]);
					MonoBehaviour.print ("user pic is: "  + tempDic["pic"]);
					MonoBehaviour.print ("user gender is: "  + tempDic["gender"]);
				}
			}
			else
			{
				MonoBehaviour.print("funFbGetGameFriendsCB wrong, error are: " + aError.getErrorMsg() );
			}
		}
		
		public static void funVkGetUserDataCB(FunplusUser aUser, FunplusError aError)
		{
			MonoBehaviour.print ("funVkGetUserDataCB called");
			
			if(aError.getErrorCode() == 0) 
			{
				MonoBehaviour.print("uid:" + aUser.getUid());
				MonoBehaviour.print("name: " + aUser.getName());
				MonoBehaviour.print("email: " + aUser.getEmail());
				MonoBehaviour.print("gender: " + aUser.getGender());
				MonoBehaviour.print("access token: " + aUser.getAccessToken());
			}
			else 
			{
				MonoBehaviour.print("funFbGetUserDataCB wrong, error are: " + aError.getErrorMsg() );
			}
		}
		
		public static void funVkShareCB(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("funFbShare called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));
		}

		public static void funVkSendRequestCB(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("funVkSendRequestCB called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));
		}

		//Wx callback
		public static void funWxShareCB(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("funWxShareCB called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));
		}

		public static void funWxShareImageCB(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("funWxShareImageCB called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));

		}

		public static void funWxSendRequest(bool aRet, FunplusError aError)
		{
			MonoBehaviour.print ("FunWxSendRequest called");
			MonoBehaviour.print ("ret is " + Convert.ToString(aRet));
		}

		//payment callbacks
		public static void funPaymenOrderCompleteCB(string aThroughCargo, FunplusError aError)
		{
			MonoBehaviour.print ("funPaymenOrderCompleteCB called");
			MonoBehaviour.print("aThroughCargo: " + aThroughCargo);
			MonoBehaviour.print("error is: " + aError.getErrorMsg());
		}

		public static void funPaymentGetProductsInfoCB(IList aList, FunplusError aError)
		{
			MonoBehaviour.print ("funGetProductsInfoCB called");

			if(aError.getErrorCode() == 0)
			{
				for(int i=0; i<aList.Count; ++i) 
				{
					MonoBehaviour.print("user index is : " + i);
					IDictionary tempDic = ((IDictionary)aList[i]);
					MonoBehaviour.print ("prod_title "  + tempDic["prod_title"]);
					MonoBehaviour.print ("prod_des "  + tempDic["prod_des"]);
					MonoBehaviour.print ("prod_symbol: "  + tempDic["prod_symbol"]);
					MonoBehaviour.print ("prod_code "  + tempDic["prod_code"]);
					MonoBehaviour.print ("prod_id "  + tempDic["prod_id"]);
				}
			}

		}

	}
}

#endif


