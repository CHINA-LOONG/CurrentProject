using UnityEngine;
using System.Collections;

namespace Funplus {

	abstract public class BaseSdkWrapper {
		abstract public void SetGameObject (string gameObjectName);

		abstract public void Install (string gameId, string gameKey, string environment);
		abstract public void SetDebug (bool isDebug);
		abstract public bool IsSdkInstalled ();
		abstract public bool IsFirstLaunch ();

		abstract public void LogUserLogin (string uid);
		abstract public void LogNewUser (string uid);
		abstract public void LogUserLogout ();
		abstract public void LogUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser);
		abstract public void LogPayment (string productId, string throughCargo, string purchaseData);
	}

}