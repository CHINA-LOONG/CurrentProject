using UnityEngine;
using System.Collections;

namespace Funplus {

	public class FunplusAccountStub : BaseAccountWrapper {

		private static FunplusAccountStub instance;
		private static readonly object locker = new object();

		private bool loginFlag;
		
		private FunplusAccountStub () {
		}
		
		public static FunplusAccountStub GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusAccountStub ();
				}
			}
			return instance;
		}

		public override void SetGameObject (string gameObjectName) {
			Logger.Log ("Calling FunplusAccountStub.SetGameObject ().");
		}
		
		public override bool IsUserLoggedIn () {
			return loginFlag;
		}
		
		public override void GetAvailableAccountTypes () {
			// TODO
		}
		
		public override void OpenSession () {
			FunplusAccount.GetInstance ().OnOpenSession ("{\"is_logged_in\": false}");
		}
		
		public override void Login () {
			loginFlag = true;
			FunplusAccount.GetInstance ().OnLoginSuccess ("{\"fpid\": \"111111\", \"session_key\": \"test_login_session_key\"}");
		}
		
		public override void Login (FunplusAccountType type) {
			loginFlag = true;
			FunplusAccount.GetInstance ().OnLoginSuccess ("{\"fpid\": \"111111\", \"session_key\": \"test_login_session_key\"}");
		}
		
		public override void Logout () {
			FunplusAccount.GetInstance ().OnLogout (null);
		}
		
		public override void ShowUserCenter () {
			Logger.Log ("Calling FunplusAccountStub.ShowUserCenter().");
		}
		
		public override void BindAccount () {
			FunplusAccount.GetInstance ().OnBindAccountSuccess ("{\"fpid\": \"222222\", \"session_key\": \"test_bind_session_key\"}");
		}
		
		public override void BindAccount (FunplusAccountType type) {
			FunplusAccount.GetInstance ().OnBindAccountSuccess ("{\"fpid\": \"222222\", \"session_key\": \"test_bind_session_key\"}");
		}
	}

}