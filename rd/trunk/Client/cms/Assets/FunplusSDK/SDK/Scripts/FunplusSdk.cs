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
using Funplus.Abstract;
using Funplus.Android;
using Funplus.IOS;
using Funplus.Stub;
using Funplus.Internal;

namespace Funplus
{
	/// <summary>
	/// First, you need to call <c>setGameObjectAndDelegate()</c> to set a game object
	/// and a delegate class to the Funplus SDK. Then, you call <c>install()</c> to
	/// install the SDK. The SDK will send notifications back to your game at certain
	/// moments by using the delegate class you've passed in. Example:
	/// 
	/// <example>
	/// <code>
	/// FunplusSdk.Instance.SetDelegate (new YourDelegate());
	/// FunplusSdk.Instance.Install ();
	/// </code>
	/// </example>
	/// </summary>
	public class FunplusSdk : MonoBehaviour
	{

		/// <summary>
		/// This interface defines methods which will be called
		/// when Funplus SDK finishes to install.
		/// </summary>
		public interface IDelegate
		{
			/// <summary>
			/// This method will be called when the installation succeeds.
			/// </summary>
			/// <param name="config">The game configuration.</param>
			void OnSdkInstallSuccess (string config);

			/// <summary>
			/// This method will be called when the installation fails.
			/// </summary>
			/// <param name="error">Error.</param>
			void OnSdkInstallError (FunplusError error);
		}

		private static readonly string SDK_VERSION = "3.0.19";

		public static FunplusSdk Instance { get; set; }
		private static IDelegate _delegate;

		public string GameId { get; set; }
		public string GameKey { get; set; }
		public string Environment { get; set; }

		private BaseSdkWrapper Wrapper
		{
			get
			{
				#if UNITY_ANDROID
				return FunplusSdkAndroid.Instance;
				#elif UNITY_IOS
				return FunplusSdkIOS.Instance;
				#else
				return FunplusSdkStub.Instance;
				#endif
			}
		}

		void Awake ()
		{
			DontDestroyOnLoad (this.gameObject);
			Instance = this;
		}

		/// <summary>
		/// Gets the instance of the SDK.
		/// </summary>
		/// <returns>The instance.</returns>
		[Obsolete("This method is deprecated, please use FunplusSdk.Instance instead.")]
		public static FunplusSdk GetInstance ()
		{
			return Instance;
		}

		public FunplusSdk SetDelegate (IDelegate sdkDelegate)
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
		/// Sets the game object and delegate to the SDK.
		/// </summary>
		/// <param name="gameObjectName">Name of the game object.</param>
		/// <param name="sdkDelegate">Sdk delegate.</param>
		public void SetGameObjectAndDelegate (string gameObjectName, IDelegate sdkDelegate)
		{
			if (_delegate == null) 
			{
				_delegate = sdkDelegate;
				Wrapper.SetGameObject (gameObjectName);
			}
			else
			{
				Debug.LogWarning ("[funsdk] Delegate has already been set.");
			}
		}

		/// <summary>
		/// This method installs the Funplus SDK. You need to call
		/// <code>SetGameObjectAndDelegate</code> before calling this method.
		/// </summary>
		public void Install ()
		{
			this.gameObject.AddComponent<FunplusAccount> ();
			this.gameObject.AddComponent<FunplusPayment> ();
			this.gameObject.AddComponent<FunplusFacebook> ();
			this.gameObject.AddComponent<FunplusRequest> ();

			GameId = FunplusSettings.FunplusGameId;
			GameKey = FunplusSettings.FunplusGameKey;
			Environment = FunplusSettings.Environment;
			Install (GameId, GameKey, Environment);
		}

		/// <summary>
		/// This method installs the Funplus SDK. You need to call
		/// <code>SetGameObjectAndDelegate</code> before calling this method.
		/// </summary>
		/// <param name="gameId">Game ID.</param>
		/// <param name="gameKey">Game key.</param>
		/// <param name="environment">Environment, either "sandbox" or "production".</param>
		public void Install (string gameId, string gameKey, string environment)
		{
			if (_delegate == null) 
			{
				// set the game object and delegate first!
				Debug.LogError ("[funsdk] Delegate not set, please call FunplusSdk.SetDelegate() first.");
				return;
			}
			if (string.IsNullOrEmpty (gameId))
			{
				Debug.LogError ("[funsdk] `gameId` must not be empty.");
				return;
			}
			if (string.IsNullOrEmpty (gameKey))
			{
				Debug.LogError ("[funsdk] `gameKey` must not be empty.");
				return;
			}
			if (string.IsNullOrEmpty (environment))
			{
				Debug.LogError ("[funsdk] `environment` must not be empty.");
				return;
			}
			Wrapper.Install (gameId, gameKey, environment);
		}
		
		/// <summary>
		/// Tests if the sdk is installed.
		/// </summary>
		/// <returns><c>true</c>, if sdk has been installed, <c>false</c> otherwise.</returns>
		public bool IsSdkInstalled ()
		{
			return (_delegate != null) && Wrapper.IsSdkInstalled ();
		}


		/// <summary>
		/// Gets current SDK's version.
		/// </summary>
		/// <returns>The SDK version.</returns>
		public static string GetSdkVersion ()
		{
			return SDK_VERSION;
		}

		/// <summary>
		/// Test if this is game's very first launch.
		/// </summary>
		/// <returns><c>true</c>, if this is game's first launch, <c>false</c> otherwise.</returns>
		public bool IsFirstLaunch ()
		{
			return Wrapper.IsFirstLaunch ();
		}

		/// <summary>
		/// <para>
		/// If you are using the Funplus account module, this method will be called automatically.
		/// Just ignore it.
		/// </para>
		/// <para>
		/// Otherwise, you have to call this method after player logs in.
		/// </para>
		/// </summary>
		/// <param name="uid">Player's ID.</param>
		public void LogUserLogin (string uid)
		{
			Wrapper.LogUserLogin (uid);
		}

		/// <summary>
		/// <para>
		/// If you are using the Funplus account module, this method will be called automatically.
		/// Just ignore it.
		/// </para>
		/// <para>
		/// Otherwise, you have to call this method after player creates a new account.
		/// </para>
		/// </summary>
		/// <param name="uid">Player's ID.</param>
		public void LogNewUser (string uid)
		{
			Wrapper.LogNewUser (uid);
		}

		/// <summary>
		/// <para>
		/// If you are using the Funplus account module, this method will be called automatically.
		/// Just ignore it.
		/// </para>
		/// <para>
		/// Otherwise, you have to call this method after player logs out.
		/// </para>
		/// </summary>
		public void LogUserLogout ()
		{
			Wrapper.LogUserLogout ();
		}

		/// <summary>
		/// <para>
		/// If you are using the Funplus account module, this method will be called automatically.
		/// Just ignore it.
		/// </para>
		/// <para>
		/// Otherwise, you have to call this method after player updates his/her profile.
		/// </para>.
		/// </summary>
		/// <param name="serverId">Server ID.</param>
		/// <param name="userId">Player's game ID, not Funplus ID.</param>
		/// <param name="userName">Player's name.</param>
		/// <param name="userLevel">Player's level.</param>
		/// <param name="userVipLevel">Player's VIP level.</param>
		/// <param name="isPaidUser">If set to <c>true</c> is paid user.</param>
		public void LogUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser)
		{
			Wrapper.LogUserInfoUpdate (serverId, userId, userName, userLevel, userVipLevel, isPaidUser);
		}

		/// <summary>
		/// <para>
		/// If you are using the Funplus payment module, this method will be called automatically.
		/// Just ignore it.
		/// </para>
		/// <para>
		/// Otherwise, you have to call this method after a purchase process finishes.
		/// </para>
		/// </summary>
		/// <param name="productId">Product identifier.</param>
		/// <param name="throughCargo">Through cargo.</param>
		/// <param name="purchaseData">Purchase data.</param>
		public void LogPayment (string productId, string throughCargo, string purchaseData)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				Wrapper.LogPayment (productId, throughCargo, purchaseData);
			} else {
				// TODO
			}
		}

		//\cond
		#region callbacks
		public void OnFunplusSdkInstallSuccess (string message)
		{
			_delegate.OnSdkInstallSuccess (message);
		}
		
		public void OnFunplusSdkInstallError (string message)
		{
			_delegate.OnSdkInstallError (FunplusError.FromMessage(message));
		}
		#endregion //callbacks
		//\endcond
	}

}