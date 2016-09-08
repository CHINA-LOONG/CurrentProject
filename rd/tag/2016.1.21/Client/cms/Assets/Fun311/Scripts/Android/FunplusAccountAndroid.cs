using UnityEngine;
using System.Collections;

namespace Funplus {

	public class FunplusAccountAndroid : BaseAccountWrapper {

		private AndroidJavaClass funplusAccountWrapper;

		private static FunplusAccountAndroid instance;
		private static readonly object locker = new object ();

		private FunplusAccountAndroid () {
			funplusAccountWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusAccountWrapper");
		}

		public static FunplusAccountAndroid GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusAccountAndroid ();
				}
			}
			return instance;
		}

		public override void SetGameObject (string gameObjectName) {
			funplusAccountWrapper.CallStatic ("setGameObject", gameObjectName);
		}

		public override bool IsUserLoggedIn () {
			return funplusAccountWrapper.CallStatic<bool> ("isUserLoggedIn");
		}

		public override void GetAvailableAccountTypes () {
			funplusAccountWrapper.CallStatic ("getAvailableAccountTypes");
		}

		public override void OpenSession () {
			funplusAccountWrapper.CallStatic ("openSession");
		}

		public override void Login () {
			funplusAccountWrapper.CallStatic ("login");
		}

		public override void Login (FunplusAccountType type) {
			funplusAccountWrapper.CallStatic ("login", (int)type);
		}
		
		public override void Logout () {
			funplusAccountWrapper.CallStatic ("logout");
		}

		public override void ShowUserCenter () {
			funplusAccountWrapper.CallStatic ("showUserCenter");
		}

		public override void BindAccount () {
			funplusAccountWrapper.CallStatic ("bind");
		}

		public override void BindAccount (FunplusAccountType type) {
			funplusAccountWrapper.CallStatic ("bind", (int)type);
		}
	}
	
}