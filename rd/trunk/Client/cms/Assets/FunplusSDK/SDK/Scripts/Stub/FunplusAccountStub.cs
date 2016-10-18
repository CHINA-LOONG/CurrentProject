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

	// TODO make this class behavior more like the native `FunplusAccount` class.
	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	public class SessionStub : ScriptableObject
	{
		private const string ASSET_NAME = "SessionStub";
		private const string ASSET_PATH = "FunplusSDK/SDK/Resources";
		private const string ASSET_EXT = ".asset";

		[SerializeField]
		private FunplusAccountType accountType;
		[SerializeField]
		private string fpid;
		[SerializeField]
		private string email;
		[SerializeField]
		private string sessionKey;
		[SerializeField]
		private long expireOn;

		private static SessionStub instance;

		public static SessionStub Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load(ASSET_NAME) as SessionStub;
					if (instance == null)
					{
						// If not found, autocreate the asset object.
						instance = ScriptableObject.CreateInstance<SessionStub>();
						#if UNITY_EDITOR
						string properPath = Path.Combine(Application.dataPath, ASSET_PATH);
						if (!Directory.Exists(properPath))
						{
							Directory.CreateDirectory(properPath);
						}

						string fullPath = Path.Combine(Path.Combine("Assets", ASSET_PATH), ASSET_NAME + ASSET_EXT);
						AssetDatabase.CreateAsset(instance, fullPath);
						#endif
					}
				}

				return instance;
			}
		}

		public static FunplusSession NewSession (FunplusAccountType accoutType, string fpid, string email, string sessionKey, long expireOn)
		{
			Instance.AccountType = accoutType;
			Instance.Fpid = fpid;
			Instance.Email = email;
			Instance.SessionKey = sessionKey;

			return Session;
		}

		public static void ClearSession ()
		{
			Instance.AccountType = FunplusAccountType.FPAccountTypeUnknown;
			Instance.Fpid = null;
			Instance.Email = null;
			Instance.SessionKey = null;
			Instance.ExpireOn = -1;
		}

		public static FunplusSession Session
		{
			get
			{
				if (string.IsNullOrEmpty (Instance.Fpid) || string.IsNullOrEmpty (Instance.SessionKey))
				{
					return null;
				}
				else
				{
					return new FunplusSession {
						AccountType = Instance.AccountType,
						Fpid = Instance.Fpid,
						Email = Instance.Email,
						SessionKey = Instance.SessionKey,
						ExpireOn = Instance.ExpireOn
					};
				}
			}
		}

		public FunplusAccountType AccountType
		{
			get
			{
				return Instance.accountType;
			}

			set
			{
				if (Instance.accountType != value)
				{
					Instance.accountType = value;
					SessionStub.DirtyEditor ();
				}
			}
		}

		public string Fpid
		{
			get
			{
				return Instance.fpid;
			}

			set
			{
				if (Instance.fpid != value)
				{
					Instance.fpid = value;
					SessionStub.DirtyEditor ();
				}
			}
		}

		public string Email
		{
			get
			{
				return Instance.email;
			}

			set
			{
				if (Instance.email != value)
				{
					Instance.email = value;
					SessionStub.DirtyEditor ();
				}
			}
		}

		public string SessionKey
		{
			get
			{
				return Instance.sessionKey;
			}

			set
			{
				if (Instance.sessionKey != value)
				{
					Instance.sessionKey = value;
					SessionStub.DirtyEditor ();
				}
			}
		}

		public long ExpireOn
		{
			get
			{
				return Instance.expireOn;
			}

			set
			{
				if (Instance.expireOn != value)
				{
					Instance.expireOn = value;
					SessionStub.DirtyEditor ();
				}
			}
		}
			
		private static void DirtyEditor ()
		{
			#if UNITY_EDITOR
			EditorUtility.SetDirty (Instance);
			#endif
		}
	}

	public class FunplusAccountStub : BaseAccountWrapper
	{
		private const string ASSET_NAME = "FunplusAccountStub";
		private const string ASSET_PATH = "FunplusSDK/SDK/Resources";
		private const string ASSET_EXT = ".asset";

		// Fake Android ID.
		private const string ANDROID_ID = "ffbc8c7432c35b4402:00:00:00:00:00";
		// Fake GUID.
		private const string GUID = "f0ad6b6300428a34dd2d7fa8f2168cad";

		private static FunplusAccountStub instance;
		private static readonly object locker = new object ();

		// Note: this field is not thread safe.
		private static string userEmail;

		private static SessionState sessionState = SessionState.Closed;

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
				
			if (SessionStub.Session == null)
			{
				sessionState = SessionState.Opened;
				FunplusAccount.Instance.OnAccountOpenSession ("{\"is_logged_in\": false}");
			}
			else
			{
				sessionState = SessionState.Actived;
				FunplusAccount.Instance.OnAccountOpenSession ("{\"is_logged_in\": true}");
				FunplusAccount.Instance.OnAccountLoginSuccess (SessionStub.Session.ToJsonString ());
			}
		}

		public override void Login () 
		{
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

			Debug.Log ("[funsdk] Calling FunplusAccountStub.Login(type).");
		}
		
		public override void Logout ()
		{
			if (!sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Session has not been activated.");
				return;
			}

			sessionState = SessionState.Opened;
			SessionStub.ClearSession ();
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

			Debug.Log ("[funsdk] Calling FunplusAccountStub.BindAccount(type).");
		}
		#endregion // override methods

		#region testing methods
		public void ExpressLogin ()
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
			
			Dictionary<string, string> data = new Dictionary<string, string> ();
			data.Add ("game_id", FunplusSdk.Instance.GameId);
			data.Add ("method", "express_signin");
			data.Add ("android_id", ANDROID_ID);
			data.Add ("guid", GUID);

			RequestSuccess onSuccess = FunplusAccountStub.OnExpressLoginRequestSuccess;
			RequestError onError = FunplusAccountStub.OnExpressLoginRequestError;
			FunplusRequest.Instance.Post (data, onSuccess, onError);
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

			userEmail = email;

			Dictionary<string, string> data = new Dictionary<string, string> ();
			data.Add ("game_id", FunplusSdk.Instance.GameId);
			data.Add ("method", "signin");
			data.Add ("email", email);
			data.Add ("password", password);
			data.Add ("android_id", ANDROID_ID);
			data.Add ("guid", GUID);

			RequestSuccess onSuccess = FunplusAccountStub.OnEmailLoginRequestSuccess;
			RequestError onError = FunplusAccountStub.OnEmailLoginRequestError;
			FunplusRequest.Instance.Post (data, onSuccess, onError);
		}

		public void Register (string email, string password)
		{
			Dictionary<string, string> data = new Dictionary<string, string> ();
			data.Add ("game_id", FunplusSdk.Instance.GameId);
			data.Add ("method", "signup");
			data.Add ("email", email);
			data.Add ("password", password);

			RequestSuccess onSuccess = FunplusAccountStub.OnEmailLoginRequestSuccess;
			RequestError onError = FunplusAccountStub.OnEmailLoginRequestError;
			FunplusRequest.Instance.Post (data, onSuccess, onError);
		}

		public void BindAccountWithEmail (string fpid, string email, string password)
		{
			if (!sessionState.Equals (SessionState.Actived))
			{
				Debug.LogError ("[funsdk] Please login first.");
			}

			userEmail = email;

			Dictionary<string, string> data = new Dictionary<string, string> ();
			data.Add ("game_id", FunplusSdk.Instance.GameId);
			data.Add ("method", "signup");
			data.Add ("fpid", fpid);
			data.Add ("email", email);
			data.Add ("password", password);

			RequestSuccess onSuccess = FunplusAccountStub.OnBindAccountSuccess;
			RequestError onError = FunplusAccountStub.OnBindAccountError;
			FunplusRequest.Instance.Post (data, onSuccess, onError);
		}
		#endregion

		#region callbacks
		private static void OnExpressLoginRequestSuccess (Dictionary<string, object> data)
		{
			sessionState = SessionState.Actived;

			string fpid = (string)data ["fpid"];
			string email = (string)data ["email"];
			string sessionKey = (string)data ["session_key"];
			long expireIn = (long)data ["session_expire_in"];

			long epochTicks = new DateTime (1970, 1, 1).Ticks;
			long timestamp = (DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond;
			long expireOn = timestamp + expireIn;

			FunplusSession session = SessionStub.NewSession (FunplusAccountType.FPAccountTypeExpress, fpid, email, sessionKey, expireOn);
			FunplusAccount.Instance.OnAccountLoginSuccess (session.ToJsonString ());
		}

		private static void OnExpressLoginRequestError (FunplusError error)
		{
			// TODO error message
			FunplusAccount.Instance.OnAccountLoginError (error.ToJsonString ());
		}

		private static void OnEmailLoginRequestSuccess (Dictionary<string, object> data)
		{
			sessionState = SessionState.Actived;

			string fpid = (string)data ["fpid"];
			string sessionKey = (string)data ["session_key"];
			long expireIn = (long)data ["session_expire_in"];

			long epochTicks = new DateTime (1970, 1, 1).Ticks;
			long timestamp = (DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond;
			long expireOn = timestamp + expireIn;

			FunplusSession session = SessionStub.NewSession (FunplusAccountType.FPAccountTypeEmail, fpid, userEmail, sessionKey, expireOn);
			FunplusAccount.Instance.OnAccountLoginSuccess (session.ToJsonString ());
		}

		private static void OnEmailLoginRequestError (FunplusError error)
		{
			// TODO error message
			FunplusAccount.Instance.OnAccountLoginError (error.ToJsonString ());
		}

		private static void OnBindAccountSuccess (Dictionary<string, object> data)
		{
			string fpid = (string)data ["fpid"];
			string sessionKey = (string)data ["session_key"];
			long expireIn = (long)data ["session_expire_in"];

			long epochTicks = new DateTime (1970, 1, 1).Ticks;
			long timestamp = (DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond;
			long expireOn = timestamp + expireIn;

			FunplusSession session = SessionStub.NewSession (FunplusAccountType.FPAccountTypeEmail, fpid, userEmail, sessionKey, expireOn);
			FunplusAccount.Instance.OnAccountBindAccountSuccess (session.ToJsonString ());
		}

		private static void OnBindAccountError (FunplusError error)
		{
			FunplusAccount.Instance.OnAccountBindAccountError (error.ToJsonString ());
		}
		#endregion // callbacks
	}

}