#if UNITY_IOS || UNITY_ANDROID

/*
 * GUIInit.cs
 * 
 * Create by Lei Gu 06/10/15
 */

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using Funplus;
using System.Collections.Generic;
using HSMiniJSON;

public class FunTest : MonoBehaviour {

	public static ESHOWGROUPID mShowFlag = ESHOWGROUPID.eShowLogin;

	int screenWidth = 0;
	int screenHeight = 0;
	int btnWidth = 0;
	int btnHeight = 0;
	int btnSpace = 0;

	public enum ESHOWGROUPID {
		eShowLogin = 0,
		eShowWXService = 1,
		eShowFBService = 2,
		eShowVKService = 3,
		eshowAccountMore = 4, // express login will goto here
		eShowBuySth = 5
	}

	private FunSdk mSdkIns = null;
	private FunFacebookHelper mFbHelper = null;
//	private FunVkHelper mVkHelper = null;
//	private FunWechatHelper mWxHelper = null;
	private FunPayment mPayment = null;
	private FunAccount mAccountInc = null;

	private const string mObjName = "GameObject";

	

	void Start() {
		print ("------FunSDK init------ here in FunTest start");
		screenWidth = Screen.width * 3 /5;
		screenHeight = Screen.height * 3/5;

		
		btnWidth = screenWidth / 2;
		btnHeight = screenHeight / 15;
		btnSpace = btnHeight + 20 ;

#if UNITY_IPHONE
		mSdkIns = FunSdk.getInstance();
#else
		mSdkIns = FunSdk.instance;
#endif

		mSdkIns.install("1007", "FUNPLUS_GAME_KEY", false, FunTestCallbacks.funsdkInstallCB);
		mSdkIns.reset (mObjName);

		mAccountInc = FunAccount.instance;
		mAccountInc.reset(mObjName);

		//#if UNITY_IOS

		//#elif UNITY_ANDROID
		//screenWidth = Screen.width;
		//screenHeight = Screen.height;
		//#endif

#if UNITY_IOS
		mSdkIns.setDebug(true);
#endif

		mFbHelper = FunFacebookHelper.instance;
		mFbHelper.reset (mObjName);

//		mVkHelper = FunVkHelper.instance;
//		mVkHelper.reset (mObjName);
//
//		mWxHelper = FunWechatHelper.instance;
//		mWxHelper.reset (mObjName);

		mPayment = FunPayment.instance;
		mPayment.reset (mObjName);
	}

	//init ui
	void OnGUI() 
	{

		int leftTopX = screenWidth/5;
		int leftTopY = 50;

		if(ESHOWGROUPID.eShowLogin == mShowFlag) {

			GUI.Box (new Rect (leftTopX, leftTopY, screenWidth, screenHeight), "Please login");
			
			if(GUI.Button(new Rect(leftTopX + 50, leftTopY + 50, btnWidth, btnHeight), "Express Login"))
			{
				print("Unity Express Login");
				mAccountInc.login(FunplusAccountType.FPAccountTypeExpress, FunTestCallbacks.funsdkLoginCBEX);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + btnSpace + 50, btnWidth, btnHeight), "FB Login"))
			{
				print("Unity FB Login");
				mAccountInc.login(FunplusAccountType.FPAccountTypeFacebook, FunTestCallbacks.funsdkLoginCBFB);
			}

			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 2*btnSpace + 50, btnWidth, btnHeight), "VK Login"))
			{
				print("Unity VK Login");
				mAccountInc.login(FunplusAccountType.FPAccountTypeVK, FunTestCallbacks.funsdkLoginCBVK);

			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 3*btnSpace + 50, btnWidth, btnHeight), "Wechat Login"))
			{
				print("Unity Wechat Login");
				mAccountInc.login(FunplusAccountType.FPAccountTypeWechat, FunTestCallbacks.funsdkLoginCBWX);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 4*btnSpace + 50, btnWidth, btnHeight), "Logout local"))
			{
				print("logout called");
				mAccountInc.logout(FunTestCallbacks.funsdkLogoutCB);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 5 * btnSpace + 50, btnWidth, btnHeight), "login with ui"))
			{
				print("Unity login withui called");
				mAccountInc.login(FunTestCallbacks.funsdkLoginCBUI);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 6 * btnSpace + 50, btnWidth, btnHeight), "get available account types"))
			{
				print("Unity getAvailableAccountTypes called");

				mAccountInc.getAvailableAccountTypes(FunTestCallbacks.funsdkGetAvailableAccountTypesCB);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 7 * btnSpace + 50, btnWidth, btnHeight), "Test buy something"))
			{
				print("Unity, go to buy something");
				mShowFlag = ESHOWGROUPID.eShowBuySth;
			}

			if(GUI.Button(new Rect(leftTopX + 50, leftTopY + 8 * btnSpace + 50, btnWidth, btnHeight), "show user center"))
			{
				mAccountInc.showUserCenter(FunTestCallbacks.funsdkShowUserCenterCB);
			}

			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 9*btnSpace + 50, btnWidth, btnHeight), "Back to login"))
			{
				mShowFlag = ESHOWGROUPID.eShowLogin;
			}
		}

		if(ESHOWGROUPID.eShowFBService == mShowFlag) {
			GUI.Box (new Rect (leftTopX, leftTopY, screenWidth,screenHeight), "Test with FB");
			
			if(GUI.Button(new Rect(leftTopX + 50,leftTopY, btnWidth, btnHeight), "Get login status"))
			{
				mAccountInc.getLoginStatus(FunTestCallbacks.funsdkGetLoginStatusCB);
			}
			
			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + btnSpace + 50, btnWidth, btnHeight), "get user data FB "))
			{
				mFbHelper.getUserData(FunTestCallbacks.funFbGetUserDataCB);
			}
			
			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 2*btnSpace + 50, btnWidth, btnHeight), "get friends FB "))
			{
				mFbHelper.getGameFriends(FunTestCallbacks.funFbGetGameFriendsCB);
			}
			
			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 3*btnSpace + 50 , btnWidth, btnHeight), "share FB "))
			{
				mFbHelper.share("Title, come on, save the earth", 
				                "Message, come on, baby", "developer.funplus.com", 
				                "http://mt2.funplus.com/static/img/mt2-qr.png", FunTestCallbacks.funFbShareCB);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 5*btnSpace + 50, btnWidth, btnHeight), "send request FB"))
			{
				string id = "100005614755416"; //Zhang Yuankun
				mFbHelper.sendRequest(id, "Come on, let's save the world", FunTestCallbacks.funFbSendRequestCB);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 6*btnSpace + 50, btnWidth, btnHeight), "logout FB"))
			{
				print("logout FB ");
				mAccountInc.logout(FunTestCallbacks.funsdkLogoutCB);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 7*btnSpace + 50, btnWidth, btnHeight), "publish open graph"))
			{
				mFbHelper.pushlishOpenGraph("this is namespace", "bite", "dog",
				                            "this is title", "this is messge", "this is link", 
				                            "this is pic url", false,FunTestCallbacks.funFbPublishOpenGraphCB);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 8*btnSpace + 50, btnWidth, btnHeight), "Ask friends permission"))
			{
				mFbHelper.askFriendsPermission(FunTestCallbacks.funFbAskFriendsPermissionCB);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 9*btnSpace + 50, btnWidth, btnHeight), "Ask for publish permission"))
			{
				mFbHelper.askPublishPermission(FunTestCallbacks.funFbAskPublishPermissionCB);
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 10*btnSpace + 50, btnWidth, btnHeight), "Show account more"))
			{
				//print("Show account more");
				mShowFlag = ESHOWGROUPID.eshowAccountMore;
			}

			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 11*btnSpace + 50, btnWidth, btnHeight), "Back to login"))
			{
				mShowFlag = ESHOWGROUPID.eShowLogin;
			}
		}
				
		if(ESHOWGROUPID.eShowWXService == mShowFlag) {
			GUI.Box (new Rect (leftTopX, leftTopY, screenWidth,screenHeight), "Test with WX");
			
			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 50, btnWidth, btnHeight), "Get login stateWX"))
			{
				mAccountInc.getLoginStatus(FunTestCallbacks.funsdkGetLoginStatusCB);
			}

#if UNITY_IOS
			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + btnSpace + 50, btnWidth, btnHeight), "get access token WX"))
			{
				//mWxHelper.getAccessToken();
			}
#endif
			
//			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 2*btnSpace+50,btnWidth, btnHeight), "share image WX "))
//			{
//				mWxHelper.share("this is title", "this is msg", "this is aPic", 
//				                "this is msgExt", "this is jumpType", FunTestCallbacks.funWxShareCB);
//			}
//
//			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 3*btnSpace + 50,btnWidth, btnHeight), "share WX "))
//			{
//				mWxHelper.shareImage("this aPicPath", "this is aMsg", 
//				                     "this is jumptype", FunTestCallbacks.funWxShareImageCB);
//			}
//			
//			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 4*btnSpace + 50,btnWidth, btnHeight), "send request WX"))
//			{
//				mWxHelper.sendRequest("this is aPlatformId", "this is title", 
//				                      "this is aMsg", "this is aImagePath", 
//				                      "bG-mZJhOeYnLUIF-0N2p2VhirnI1_b_BZx9_kBGEo2GdqTwNmM-cwNyvNUwrW2l-", 
//				                      "aMsgExt", FunTestCallbacks.funWxSendRequest);
//			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 6*btnSpace + 50,btnWidth, btnHeight), "logout WX"))
			{
				mAccountInc.logout(FunTestCallbacks.funsdkLogoutCB);

			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 7*btnSpace + 50, btnWidth, btnHeight), "Show account more"))
			{
				mShowFlag = ESHOWGROUPID.eshowAccountMore;
			}

			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 8*btnSpace + 50, btnWidth, btnHeight), "Back to login"))
			{
				mShowFlag = ESHOWGROUPID.eShowLogin;
			}
		}

		if(ESHOWGROUPID.eShowVKService == mShowFlag) {
			GUI.Box (new Rect (leftTopX, leftTopY, screenWidth,screenHeight), "Test with VK");
			
			if(GUI.Button(new Rect(leftTopX + 50,leftTopY,btnWidth, btnHeight), "Get login state VK"))
			{
				mAccountInc.getLoginStatus(FunTestCallbacks.funsdkGetLoginStatusCB);
			}
			
//			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + btnSpace + 50,btnWidth, btnHeight), "get user data VK"))
//			{
//				mVkHelper.getUserData(FunTestCallbacks.funVkGetUserDataCB);
//			}
//			
//			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 2*btnSpace+50,btnWidth, btnHeight), "get friends VK"))
//			{
//				mVkHelper.getGameFriends(FunTestCallbacks.funVkGetGameFriendsCB);
//			}
//			
//			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 3*btnSpace + 50 ,btnWidth, btnHeight), "share VK"))
//			{
//
//#if UNITY_ANDROID
//				string path = "/storage/emulated/0/Android/Fight-club.png";
//#else
//				string path = "";
//#endif
//				mVkHelper.share("this is title", "this is message" , "http://developer.funplus.com", 
//					                path, FunTestCallbacks.funVkShareCB);
//			}
			
			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 4*btnSpace + 50 ,btnWidth, btnHeight), "share image VK"))
			{
				//print("share image VK");
			}
			
//			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 5*btnSpace + 50 ,btnWidth, btnHeight), "send request VK"))
//			{
//				string id = "296879180"; //zhang yu
//				mVkHelper.sendRequest(id, "this is msg", "this is link", FunTestCallbacks.funVkSendRequestCB);
//			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 7*btnSpace + 50 ,btnWidth, btnHeight), "logout VK"))
			{
				mAccountInc.logout(FunTestCallbacks.funsdkLogoutCB);
				mShowFlag = ESHOWGROUPID.eShowLogin;
			}

			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + 8*btnSpace + 50, btnWidth, btnHeight), "Show account more"))
			{
				mShowFlag = ESHOWGROUPID.eshowAccountMore;
			}

			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 9*btnSpace + 50, btnWidth, btnHeight), "Back to login"))
			{
				mShowFlag = ESHOWGROUPID.eShowLogin;
			}
		}

		if(ESHOWGROUPID.eshowAccountMore == mShowFlag)
		{
			GUI.Box (new Rect (leftTopX, leftTopY, screenWidth,screenHeight), "Accout More");

			if(GUI.Button(new Rect(leftTopX + 50, leftTopY + 50, btnWidth, btnHeight), "show user center"))
			{
				mAccountInc.showUserCenter(FunTestCallbacks.funsdkShowUserCenterCB);
			}
			
			if(GUI.Button(new Rect(leftTopX + 50,leftTopY + btnSpace + 50, btnWidth, btnHeight), "get login status"))
			{
				mAccountInc.getLoginStatus(FunTestCallbacks.funsdkGetLoginStatusCB);
			}
			
			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 2*btnSpace + 50, btnWidth, btnHeight), "logout"))
			{
				mAccountInc.logout(FunTestCallbacks.funsdkLogoutCB);
			}

			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 3*btnSpace + 50, btnWidth, btnHeight), "Back to login"))
			{
				mShowFlag = ESHOWGROUPID.eShowLogin;
			}
		}

		if(ESHOWGROUPID.eShowBuySth == mShowFlag)
		{
			GUI.Box (new Rect (leftTopX, leftTopY, screenWidth,screenHeight), "Buy Buy Buy");
			
			if(GUI.Button(new Rect(leftTopX + 50, leftTopY + 50, btnWidth, btnHeight), "init env"))
			{
				mPayment.initPaymentEnv(FunTestCallbacks.funPaymentGetProductsInfoCB, FunTestCallbacks.funPaymenOrderCompleteCB);
			}
			
			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 2*btnSpace + 50, btnWidth, btnHeight), "buy"))
			{
				mPayment.buy("com.funplus.p2", "this is through cargo");
			}

			if(GUI.Button(new Rect(leftTopX + 50 ,leftTopY + 3*btnSpace + 50, btnWidth, btnHeight), "Back to login"))
			{
				mShowFlag = ESHOWGROUPID.eShowLogin;
			}
			
		}
	}

}

#endif
