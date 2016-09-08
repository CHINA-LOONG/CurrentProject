using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Funplus {

	public class FunplusAccountIOS : BaseAccountWrapper {

		private static FunplusAccountIOS instance;
		private static readonly object locker = new object ();
		
		public static FunplusAccountIOS GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusAccountIOS ();
				}
			}
			return instance;
		}

		public override void SetGameObject (string gameObjectName) {
			com_funplus_sdk_account_setGameObject (gameObjectName);
		}

		public override void GetAvailableAccountTypes() {
		}
		
		public override bool IsUserLoggedIn () {
			return com_funplus_sdk_account_isUserLoggedIn ();
		}

		public override void OpenSession () {
			com_funplus_sdk_account_openSession ();
		}

		public override void Login () {
			com_funplus_sdk_account_login ();
		}

		public override void Login (FunplusAccountType type) {
			com_funplus_sdk_account_loginWithType ((int)type);
		}

		public override void Logout () {
			com_funplus_sdk_account_logout ();
		}

		public override void ShowUserCenter () {
			com_funplus_sdk_account_showUserCenter ();
		}

		public override void BindAccount () {
			com_funplus_sdk_account_bindAccount ();
		}

		public override void BindAccount (FunplusAccountType type) {
			com_funplus_sdk_account_bindAccountWithType ((int)type);
		}

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_setGameObject (string gameObjectName);
		
		[DllImport ("__Internal")]
		private static extern bool com_funplus_sdk_account_isUserLoggedIn ();
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_openSession ();
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_login ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_loginWithType (int type);

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_logout ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_showUserCenter ();
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_bindAccount ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_bindAccountWithType (int type);

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_getAvailableAccountTypes ();
	}

}