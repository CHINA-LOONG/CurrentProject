/**
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015-Present Funplus
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Funplus.Abstract;
using Funplus.Internal;
using HSMiniJSON;

namespace Funplus.Stub
{

	public class FunplusAccountStub : BaseAccountWrapper
	{
		private static FunplusAccountStub instance;
		private static readonly object locker = new object ();
		private string deviceId;
		private string guid;

		private static SessionState sessionState = SessionState.Closed;
		private static FunplusSession session = new FunplusSession ();

		private enum SessionState
		{
			Closed,
			Opened,
			Activating,
			Actived,
		}

		public static FunplusAccountStub Instance
		{
			get 
			{
				if (instance == null)
				{
					lock (locker) 
					{
						instance = new FunplusAccountStub ();
					}
				}
				return instance;
			}
		}

		public string DeviceId
		{
			get
			{
				#if UNITY_EDITOR
				if (string.IsNullOrEmpty (deviceId))
				{
					deviceId = FunplusSettings.FakeDeviceId;
				}
				#endif
				return deviceId;
			}
		}

		public string Guid
		{
			get
			{
				if (string.IsNullOrEmpty (guid))
				{
					guid = GetMd5Hash (DeviceId);
				}
				return guid;
			}
		}

		#region override methods
		public override void SetGameObject (string gameObjectName) 
		{
		}
		
		public override bool IsUserLoggedIn () 
		{
			return sessionState.Equals (SessionState.Actived);
		}
		
		public override void GetAvailableAccountTypes () 
		{
		}
		
		public override void OpenSession () 
		{
			if (!sessionState.Equals (SessionState.Closed))
			{
				Debug.LogError ("[funsdk] Session has been opened, no need to open it again.");
				return;
			}

			#if UNITY_EDITOR
			FacebookManager.Init (OnFacebookInit);
			#else
			sessionState = SessionState.Opened;
			FunplusAccount.Instance.OnAccountOpenSession ("{\"is_logged_in\": false}");
			#endif
		}

		public override void Login () 
		{
			if (sessionState.Equals (SessionState.Closed))
			{
				Debug.LogError ("[funsdk] Session has not been opened, please open session first.");
				return;
			}

			if (sessionState.Equals (SessionState.Activating))
			{
				Debug.LogError ("[funsdk] Another authenticating process is going on.");
				return;
			}

			if (sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Session has already been activated.");
				return;
			}

			ExpressLogin ();
		}
		
		public override void Login (FunplusAccountType type) 
		{
			if (sessionState.Equals (SessionState.Closed))
			{
				Debug.LogError ("[funsdk] Session has not been opened, please open session first.");
				return;
			}

			if (sessionState.Equals (SessionState.Activating))
			{
				Debug.LogError ("[funsdk] Another authenticating process is going on.");
				return;
			}

			if (sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Session has already been activated.");
				return;
			}

			switch (type)
			{
			case FunplusAccountType.FPAccountTypeExpress:
				ExpressLogin ();
				break;
			case FunplusAccountType.FPAccountTypeFacebook:
				#if UNITY_EDITOR
				FacebookManager.Login (OnFacebookLoginSuccess, OnFacebookLoginFailed);
				#endif
				break;
			default:
				// TODO
				break;
			}
		}

		public override void LoginWithEmail (string email, string password)
		{
			
		}

		public override void RegisterWithEmail (string email, string password)
		{
			
		}

		public override void ResetPassword (string email)
		{
			
		}
		
		public override void Logout ()
		{
			if (!sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Session has not been activated.");
				return;
			}

			sessionState = SessionState.Opened;
			session.Clear ();
			FunplusAccount.Instance.OnAccountLogout (null);
		}
		
		public override void ShowUserCenter () 
		{
			if (!sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Please login first.");
			}

			Debug.Log ("[funsdk] Calling FunplusAccountStub.ShowUserCenter().");
		}
		
		public override void BindAccount () 
		{
			if (!sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Please login first.");
			}
			
			Debug.Log ("[funsdk] Calling FunplusAccountStub.BindAccount().");
		}
		
		public override void BindAccount (FunplusAccountType type) 
		{
			if (!sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Please login first.");
			}

			switch (type)
			{
			case FunplusAccountType.FPAccountTypeExpress:
				ExpressLogin ();
				break;
			case FunplusAccountType.FPAccountTypeFacebook:
				#if UNITY_EDITOR
				FacebookManager.Login (OnFacebookLoginSuccess, OnFacebookLoginFailed);
				#endif
				break;
			default:
				// TODO
				break;
			}
		}
		#endregion // override methods

		#region testing methods
		public void ExpressLogin ()
		{			
			Dictionary<string, string> data = new Dictionary<string, string> ();
			data.Add ("game_id", FunplusSdk.Instance.GameId);
			data.Add ("method", "express_signin");
			data.Add ("android_id", deviceId);
			data.Add ("guid", Guid);

			session.AccountType = FunplusAccountType.FPAccountTypeExpress;

			FpRequestSuccess onSuccess = FunplusAccountStub.OnLoginSuccess;
			FpRequestError onError = FunplusAccountStub.OnLoginError;
			FunplusRequest.Instance.FpPost (data, onSuccess, onError);
		}

		public void EmailLogin (string email, string password)
		{
			if (sessionState.Equals (SessionState.Closed))
			{
				Debug.LogError ("[funsdk] Session has not been opened, please open session first.");
				return;
			}

			if (sessionState.Equals (SessionState.Activating))
			{
				Debug.LogError ("[funsdk] Another authenticating process is going on.");
				return;
			}

			if (sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Session has already been activated.");
				return;
			}

			session.AccountType = FunplusAccountType.FPAccountTypeExpress;
			session.Email = email;

			Dictionary<string, string> data = new Dictionary<string, string> ();
			data.Add ("game_id", FunplusSdk.Instance.GameId);
			data.Add ("method", "signin");
			data.Add ("email", email);
			data.Add ("password", password);
			data.Add ("android_id", DeviceId);
			data.Add ("guid", Guid);

			FunplusRequest.Instance.FpPost (data, OnLoginSuccess, OnLoginError);
		}

		public void FacebookLogin (string accessToken, string platformId)
		{
			Dictionary<string, string> data = new Dictionary<string, string> ();
			data.Add ("game_id", FunplusSdk.Instance.GameId);
			data.Add ("method", "login_with_sns");
			data.Add ("platform_id", platformId);
			data.Add ("access_token", accessToken);

			session.AccountType = FunplusAccountType.FPAccountTypeFacebook;

			FunplusRequest.Instance.FpPost (data, OnLoginSuccess, OnLoginError);
		}

		public void FacebookBind (string accessToken, string fpid, string platformId)
		{
			if (!sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Please login first.");
			}

			Dictionary<string, string> data = new Dictionary<string, string> ();
			data.Add ("game_id", FunplusSdk.Instance.GameId);
			data.Add ("method", "login_with_sns");
			data.Add ("fpid", fpid);
			data.Add ("platform_id", platformId);
			data.Add ("access_token", accessToken);

			session.AccountType = FunplusAccountType.FPAccountTypeFacebook;

			FunplusRequest.Instance.FpPost (data, OnLoginSuccess, OnLoginError);
		}

		public void Register (string email, string password)
		{
			Dictionary<string, string> data = new Dictionary<string, string> ();
			data.Add ("game_id", FunplusSdk.Instance.GameId);
			data.Add ("method", "signup");
			data.Add ("email", email);
			data.Add ("password", password);

			session.AccountType = FunplusAccountType.FPAccountTypeEmail;
			session.Email = email;

			FunplusRequest.Instance.FpPost (data, OnLoginSuccess, OnLoginError);
		}

		public void BindAccountWithEmail (string fpid, string email, string password)
		{
			if (!sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Please login first.");
			}

			session.AccountType = FunplusAccountType.FPAccountTypeEmail;
			session.Email = email;

			Dictionary<string, string> data = new Dictionary<string, string> ();
			data.Add ("game_id", FunplusSdk.Instance.GameId);
			data.Add ("method", "signup");
			data.Add ("fpid", fpid);
			data.Add ("email", email);
			data.Add ("password", password);

			FunplusRequest.Instance.FpPost (data, OnLoginSuccess, OnLoginError);
		}
		#endregion

		#region callbacks
		private static void OnLoginSuccess (Dictionary<string, object> data)
		{
			sessionState = SessionState.Actived;

			string fpid = (string)data ["fpid"];
			string sessionKey = (string)data ["session_key"];
			long expireIn = (long)data ["session_expire_in"];

			long epochTicks = new DateTime (1970, 1, 1).Ticks;
			long timestamp = (DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond;
			long expireOn = timestamp + expireIn;

			session.SessionKey = sessionKey;
			session.ExpireOn = expireOn;

			if (string.IsNullOrEmpty (session.Fpid))
			{
				session.Fpid = fpid;
				FunplusAccount.Instance.OnAccountLoginSuccess (session.ToJsonString ());
			}
			else
			{
				FunplusAccount.Instance.OnAccountBindAccountSuccess (session.ToJsonString ());
			}
		}

		private static void OnLoginError (FunplusError error)
		{
			// TODO error message
			if (string.IsNullOrEmpty (session.Fpid)) {
				FunplusAccount.Instance.OnAccountLoginError (error.ToJsonString ());
			} else {
				FunplusAccount.Instance.OnAccountBindAccountError (error.ToJsonString ());
			}
		}
		#endregion // callbacks

		private void OnFacebookInit ()
		{
			sessionState = SessionState.Opened;
			FunplusAccount.Instance.OnAccountOpenSession ("{\"is_logged_in\": false}");
		}

		private void OnFacebookLoginSuccess (string accessToken, string uid)
		{
			session.SnsPlatform = "fb";
			session.SnsId = uid;

			string platformId = string.Format ("fb:{0}", uid);
			if (string.IsNullOrEmpty(session.Fpid))
			{
				Instance.FacebookLogin (accessToken, platformId);
			}
			else
			{
				string fpid = session.Fpid;
				Instance.FacebookBind (accessToken, fpid, platformId);
			}
		}

		private void OnFacebookLoginFailed ()
		{
			if (string.IsNullOrEmpty(session.Fpid))
			{
				FunplusAccount.Instance.OnAccountLoginError (FunplusError.E (2101).ToJsonString ());
			}
			else
			{
				FunplusAccount.Instance.OnAccountBindAccountError (FunplusError.E (2106).ToJsonString ());
			}
		}

		private static string GetMd5Hash (string s)
		{
			MD5 md5 = MD5.Create ();
			byte[] bytes = Encoding.ASCII.GetBytes (s);
			byte[] hash = md5.ComputeHash (bytes);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("x2"));
			}
			return sb.ToString();
		}
	}

}