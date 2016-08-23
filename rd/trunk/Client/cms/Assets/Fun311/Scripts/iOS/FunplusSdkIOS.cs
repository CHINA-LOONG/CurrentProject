using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Funplus {

	public class FunplusSdkIOS : BaseSdkWrapper {

		private static FunplusSdkIOS instance;
		private static readonly object locker = new object ();

		public static FunplusSdkIOS GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusSdkIOS ();
				}
			}
			return instance;
		}

		public override void SetGameObject (string gameObjectName) {
			com_funplus_sdk_setGameObject (gameObjectName);
		}

		public override bool IsSdkInstalled () {
			return com_funplus_sdk_isSdkInstalled ();
		}

		public override void Install (string gameId, string gameKey, string environment) {
			bool isProduction = (environment == "production") ? true : false;
			com_funplus_sdk_setDebug(!isProduction);
			com_funplus_sdk_install (gameId, gameKey, isProduction);
		}

		public override void SetDebug (bool isDebug) {
			com_funplus_sdk_setDebug (isDebug);
		}

		public override bool IsFirstLaunch () {
			return com_funplus_sdk_isFirstLaunch ();
		}

		public string GetSdkVersion () {
			return com_funplus_sdk_getSdkVersion ();
		}

		public override void LogUserLogin (string uid) {
			com_funplus_sdk_logUserLogin (uid);
		}

		public override void LogNewUser (string uid) {
			com_funplus_sdk_logNewUser (uid);
		}

		public override void LogUserLogout () {
			com_funplus_sdk_logUserLogout ();
		}

		public override void LogUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser) {
			com_funplus_sdk_logUserInfoUpdate (serverId, userId, userName, userLevel, userVipLevel, isPaidUser);
		}

		public override void LogPayment (string productId, string throughCargo, string purchaseData) {
			com_funplus_sdk_logPayment (productId, throughCargo, purchaseData);
		}

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_setGameObject (string gameObjectName);

		[DllImport ("__Internal")]
		private static extern bool com_funplus_sdk_isSdkInstalled ();
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_install (string gameId, string gameKey, bool isProduction);
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_setDebug (bool isDebug);

		[DllImport ("__Internal")]
		private static extern bool com_funplus_sdk_isFirstLaunch ();

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_getSdkVersion ();

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_logUserLogin (string uid);

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_logNewUser (string uid);

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_logUserLogout ();

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_logUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser);

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_logPayment (string productId, string throughCargo, string purchaseData);
	}

}