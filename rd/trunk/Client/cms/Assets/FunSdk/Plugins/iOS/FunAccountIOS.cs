/*
 * FunAccountIOS.cs
 * 
 * Create by Lei Gu 12/11/14
 */

#if UNITY_IPHONE

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Funplus
{	
	public class FunAccountIOS {
		
		[DllImport ("__Internal")]
		private static extern void obj_fp_fun_account_setObjName (string aGameObjName);
		[DllImport ("__Internal")]
		private static extern void obj_fp_account_login (FunplusAccountType aType);
		[DllImport ("__Internal")]
		private static extern void obj_fp_account_login_withUI ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_account_show_user_center ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_account_logout ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_account_get_available_account_types ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_account_get_login_status ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_account_bind_account (FunplusAccountType aType);
		
		public FunAccountIOS() {
			
		}
		
		public void reset (string aGameObjName) 
		{
			MonoBehaviour.print ("------FunSdkIOS reset------");
			obj_fp_fun_account_setObjName (aGameObjName);
		}
		
		public void login() {
			obj_fp_account_login_withUI ();
		}
		
		public void login(FunplusAccountType aType) {
			obj_fp_account_login (aType);
		}
		
		public void showUserCenter() {
			obj_fp_account_show_user_center ();
		}
		
		public void logout() {
			obj_fp_account_logout ();
		}
		
		public void getAvailableAccountTypes() {
			obj_fp_account_get_available_account_types ();
		}
		
		public void getLoginStatus() {
			obj_fp_account_get_login_status();
		}
		
		public void bindAccount(FunplusAccountType aType) {
			obj_fp_account_bind_account (aType);
		}
		
	};
}

#endif


