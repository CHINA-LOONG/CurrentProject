using UnityEngine;
using System.Collections;

namespace Funplus {
	
	public class FunplusSdkAndroid : BaseSdkWrapper {

		private AndroidJavaClass unityPlayer;
		private AndroidJavaObject currentActivity;
		private AndroidJavaClass funplusSdkWrapper;

		private static FunplusSdkAndroid instance;
		private static readonly object locker = new object ();

		private FunplusSdkAndroid () {
			unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			currentActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			funplusSdkWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusSdkWrapper");
		}

		public static FunplusSdkAndroid GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusSdkAndroid ();
				}
			}
			return instance;
		}

		public override void SetGameObject (string gameObjectName) {
			funplusSdkWrapper.CallStatic ("setGameObject", gameObjectName);
		}

		public override bool IsSdkInstalled () {
			return funplusSdkWrapper.CallStatic<bool> ("isSdkInstalled");
		}

		public override void Install (string gameId, string gameKey, string environment) {
			funplusSdkWrapper.CallStatic ("install", new object[] {
				currentActivity,
				gameId,
				gameKey,
				environment
			});
		}

		public override bool IsFirstLaunch () {
			return funplusSdkWrapper.CallStatic<bool> ("isFirstLaunch");
		}

		public string GetSdkVersion () {
			return funplusSdkWrapper.CallStatic<string> ("getSdkVersion");
		}

		public override void SetDebug (bool isDebug) {

		}

		public override void LogUserLogin (string uid) {
			funplusSdkWrapper.CallStatic ("logUserLogin", uid);
		}

		public override void LogNewUser (string uid) {
			funplusSdkWrapper.CallStatic ("logNewUser", uid);
		}

		public override void LogUserLogout () {
			funplusSdkWrapper.CallStatic ("logUserLogout");
		}

		public override void LogUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser) {
			funplusSdkWrapper.CallStatic ("logUserInfoUpdate", new object[] {
				serverId,
				userId,
				userLevel,
				userVipLevel,
				isPaidUser
			});
		}

		public override void LogPayment (string productId, string throughCargo, string purchaseData) {
			funplusSdkWrapper.CallStatic ("logPayment", productId, throughCargo, purchaseData);
		}
	}

}