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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HSMiniJSON;
using Funplus.Abstract;
using Funplus.Android;
using Funplus.IOS;
using Funplus.Stub;

namespace Funplus
{

	/// <summary>
	/// The account module enables players to sign into your game with Funplus login.
	/// Basic patterns when using Funplus login are described here:
	/// <list type="number">
	/// <item>When game is launching for the very first time, a manual login is required.</item>
	/// <item>An automatic login is performed everytime after that when game is starting.</item>
	/// <item>If the automatic login in step 2 fails, a manual login is required.</item>
	/// <item>Player will be required to bind his/her account to email or SNS account
	/// if the player keeps using express login, based on security considerations.</item>
	/// <item>A user center can be shown at any time after player has logged in,
	/// to provide additional functions like 'logout' or 'open support center'.</item>
	/// </list>
	/// </summary>
	public class FunplusAccount : MonoBehaviour
	{
		/// <summary>
		/// This interface defines methods which will be called
		/// on specific moment in the life circle of the session.
		/// </summary>
		public interface IDelegate
		{

			/// <summary>
			/// This method will be called after session is opened. An argument named
			/// <c>isLoggedIn</c> is passed back to indicate whether player has automatically
			/// login or not.
			/// </summary>
			/// <param name="isLoggedIn"><c>true</c> if player has logged in, <c>false</c> otherwise.</param>
			void OnOpenSession (bool isLoggedIn);

			/// <summary>
			/// This method will be called after player logs in.
			/// </summary>
			/// <param name="session">The active session.</param>
			void OnLoginSuccess (FunplusSession session);

			/// <summary>
			/// This method will be called after player fails to login.
			/// </summary>
			/// <param name="error">Error.</param>
			void OnLoginError (FunplusError error);

			/// <summary>
			/// This method will be called after player logs out.
			/// </summary>
			void OnLogout ();

			/// <summary>
			/// This method will be called after player binds his/her account.
			/// </summary>
			/// <param name="session">The active session.</param>
			void OnBindAccountSuccess (FunplusSession session);

			/// <summary>
			/// This method will be called after player fails to bind account.
			/// </summary>
			/// <param name="error">Error.</param>
			void OnBindAccountError (FunplusError error);

			/// <summary>
			/// This method will be called after user center closes without
			/// any operation is performed.
			/// </summary>
			void OnCloseUserCenter ();
		}

		private static IDelegate _delegate;
		public static FunplusAccount Instance { get; set; }
		public FunplusSession Session { get; set; }

		private BaseAccountWrapper Wrapper
		{
			get
			{
				#if UNITY_ANDROID
			return FunplusAccountAndroid.Instance;
				#elif UNITY_IOS
			return FunplusAccountIOS.Instance;
				#else
				return FunplusAccountStub.Instance;
				#endif
			}
		}

		void Awake ()
		{
			Instance = this;
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		[Obsolete("This method is deprecated, please use FunplusAccount.Instance instead.")]
		public static FunplusAccount GetInstance ()
		{
			return Instance;
		}

		public FunplusAccount SetDelegate (IDelegate sdkDelegate)
		{
			if (sdkDelegate == null)
			{
				Debug.LogError ("[funsdk] `sdkDelegate` must not be null.");
				return Instance;
			}
			SetGameObjectAndDelegate (this.gameObject.name, sdkDelegate);

			return Instance;
		}

		/// <summary>
		/// Sets the game object and delegate to the account module.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		/// <param name="accountDelegate">The delegate.</param>
		public void SetGameObjectAndDelegate (string gameObjectName, IDelegate accountDelegate)
		{
			if (!FunplusSdk.Instance.IsSdkInstalled ()) 
			{
				Debug.LogError ("[funsdk] Please call FunplusSdk.Instance.Install() first.");
				return;
			}

			if (_delegate == null) 
			{
				_delegate = accountDelegate;
				Wrapper.SetGameObject (gameObjectName);
			} 
			else 
			{
				Debug.LogWarning ("[funsdk] Delegate has already been set.");
			}
		}

		/// <summary>
		/// Test if player has logged in or not.
		/// </summary>
		/// <returns><c>true</c>, if player has already logged in, <c>false</c> otherwise.</returns>
		public bool IsUserLoggedIn ()
		{
			return (_delegate != null) && Wrapper.IsUserLoggedIn ();
		}

		/// <summary>
		/// Opens the session.
		/// </summary>
		public void OpenSession ()
		{
			if (_delegate == null) 
			{
				// set the game object and delegate first!
				Debug.LogError ("[funsdk] Please call FunplusAccount.SetDelegate() first.");
			} 
			else 
			{
				Debug.Log ("[funsdk] Trying to open session.");
				Wrapper.OpenSession ();
			}
		}

		/// <summary>
		/// Shows a login window, from which player can choose an account type to login.
		/// </summary>
		public void Login ()
		{
			Wrapper.Login ();
		}

		/// <summary>
		/// Signs in the player by using the given account type.
		/// </summary>
		/// <param name="type">The account type.</param>
		public void Login (FunplusAccountType type)
		{
			Wrapper.Login (type);
		}

		/// <summary>
		/// Note: this method should only be called in Unity Editor.
		/// </summary>
		/// <param name="email">Email.</param>
		/// <param name="password">Password.</param>
		public void LoginWithEmail (string email, string password)
		{
			#if UNITY_EDITOR
			FunplusAccountStub.Instance.EmailLogin (email, password);
			#endif
		}

		/// <summary>
		/// Signs out the player.
		/// </summary>
		public void Logout ()
		{
			Wrapper.Logout ();
		}

		/// <summary>
		/// Shows the user center.
		/// </summary>
		public void ShowUserCenter ()
		{
			Wrapper.ShowUserCenter ();
		}

		/// <summary>
		/// Shows an account binding window, from which players can choose an account type to bind his/her account.
		/// </summary>
		public void BindAccount ()
		{
			Wrapper.BindAccount ();
		}

		/// <summary>
		/// Binds player's account to the given account type.
		/// </summary>
		/// <param name="type">The account type.</param>
		public void BindAccount (FunplusAccountType type)
		{
			Wrapper.BindAccount (type);
		}

		/// <summary>
		/// Note: this method should only be called in Unity Editor.
		/// </summary>
		/// <param name="fpid">Funplus ID.</param>
		/// <param name="email">Email.</param>
		/// <param name="password">Password.</param>
		public void BindWithEmail (string fpid, string email, string password)
		{
			#if UNITY_EDITOR
			FunplusAccountStub.Instance.BindAccountWithEmail (fpid, email, password);
			#endif
		}

		/// <summary>
		/// Gets current active session, in which you can retrieve player's account data.
		/// </summary>
		/// <returns>The current session.</returns>
		public FunplusSession GetSession ()
		{
			return Session;
		}

		//\cond
		#region callbacks
		public void OnAccountGetAvailableAccountTypesSuccess (string message)
		{
			Debug.LogFormat ("[funsdk] Got account types: {0}.", message);
		}

		public void OnAccountGetAvailableAccountTypesError (string message)
		{
			Debug.LogErrorFormat ("[funsdk] Failed to get account types: {0}.", message);
		}
		
		public void OnAccountOpenSession (string message)
		{
			var dict = Json.Deserialize (message) as Dictionary<string,object>;
			try {
				bool isLoggedIn = (bool)dict ["is_logged_in"];
				_delegate.OnOpenSession (isLoggedIn);
			} catch (Exception e) {
				Debug.LogErrorFormat ("[funsdk] Failed to open session: {0}.", e.Message);
			}
		}

		public void OnAccountLoginSuccess (string message)
		{
			Debug.LogFormat ("[funsdk] Account Login Success: {0}.", message);

			Session = FunplusSession.FromMessage (message);
			_delegate.OnLoginSuccess (Session);
		}

		public void OnAccountLoginError (string message)
		{
			Debug.LogErrorFormat ("[funsdk] Account Login Error: {0}.", message);

			_delegate.OnLoginError (FunplusError.FromMessage (message));
		}

		public void OnAccountLogout (string message)
		{
			Debug.LogFormat ("[funsdk] Account Logout: {0}.", message);

			_delegate.OnLogout ();
		}

		public void OnAccountBindAccountSuccess (string message)
		{
			Debug.LogFormat ("[funsdk] Bind Account Success: {0}.", message);

			Session = FunplusSession.FromMessage (message);
			_delegate.OnBindAccountSuccess (Session);
		}

		public void OnAccountBindAccountError (string message)
		{
			Debug.LogErrorFormat ("[funsdk] Bind Account Error: {0}.", message);

			_delegate.OnBindAccountError (FunplusError.FromMessage (message));
		}

		public void OnAccountCloseUserCenter (string message)
		{
			Debug.LogFormat ("[funsdk] On Close User Center: {0}.", message);

			_delegate.OnCloseUserCenter ();
		}
		#endregion //callbacks
		//\endcond
	}

}