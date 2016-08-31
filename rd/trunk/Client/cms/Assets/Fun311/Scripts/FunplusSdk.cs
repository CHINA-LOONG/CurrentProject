using UnityEngine;
using System.Collections;

namespace Funplus {

	/// <summary>
	/// First, you need to call <c>setGameObjectAndDelegate()</c> to set a game object
	/// and a delegate class to the Funplus SDK. Then, you call <c>install()</c> to
	/// install the SDK. The SDK will send notifications back to your game at certain
	/// moments by using the delegate class you've passed in. Example:
	/// 
	/// <example>
	/// <code>
	/// FunplusSdk.getInstance().setGameObjectAndDelegate("NAME-OF-YOUR-GAME-OBJECT",
	/// 												  new YourDelegate());
	/// FunplusSdk.getInstance().install("YOUR-GAME-ID", "YOUR-GAME-KEY", "production");
	/// </code>
	/// </example>
	/// </summary>
	public class FunplusSdk : MonoBehaviour {

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

		private static FunplusSdk instance;
		private static IDelegate _delegate;

		private BaseSdkWrapper GetWrapper () {
			if (Application.platform == RuntimePlatform.Android) {
				return FunplusSdkAndroid.GetInstance ();
			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				return FunplusSdkIOS.GetInstance ();
			} else {
				return FunplusSdkStub.GetInstance ();
			}
		}

		void Awake () {
			instance = this;
		}

		/// <summary>
		/// Gets the instance of the SDK.
		/// </summary>
		/// <returns>The instance.</returns>
		public static FunplusSdk GetInstance () {
			return instance;
		}

		/// <summary>
		/// Sets the game object and delegate to the SDK.
		/// </summary>
		/// <param name="gameObjectName">Name of the game object.</param>
		/// <param name="sdkDelegate">Sdk delegate.</param>
		public void SetGameObjectAndDelegate (string gameObjectName, IDelegate sdkDelegate) {
			if (_delegate == null) {
				_delegate = sdkDelegate;
				GetWrapper ().SetGameObject (gameObjectName);
			} else {
				Logger.LogWarning ("Delegate has already been set");
			}
		}

		/// <summary>
		/// This method installs the Funplus SDK. You need to call
		/// <code>SetGameObjectAndDelegate</code> before calling this method.
		/// </summary>
		/// <param name="gameId">Game ID.</param>
		/// <param name="gameKey">Game key.</param>
		/// <param name="environment">Environment, either "sandbox" or "production".</param>
		public void Install (string gameId, string gameKey, string environment) {
			if (_delegate == null) {
				// set the game object and delegate first!
				Logger.LogError ("Delegate not set, please call SetGameObjectAndDelegate() first");
				return;
			}
			GetWrapper ().Install (gameId, gameKey, environment);
		}
		
		/// <summary>
		/// Tests if the sdk is installed.
		/// </summary>
		/// <returns><c>true</c>, if sdk has been installed, <c>false</c> otherwise.</returns>
		public bool IsSdkInstalled () {
			return (_delegate != null) && GetWrapper ().IsSdkInstalled ();
		}


		/// <summary>
		/// Gets current SDK's version.
		/// </summary>
		/// <returns>The SDK version.</returns>
		public string GetSdkVersion () {
			return "3.0.10";
		}

		/// <summary>
		/// Test if this is game's very first launch.
		/// </summary>
		/// <returns><c>true</c>, if this is game's first launch, <c>false</c> otherwise.</returns>
		public bool IsFirstLaunch () {
			return GetWrapper ().IsFirstLaunch ();
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
		public void LogUserLogin (string uid) {
			GetWrapper ().LogUserLogin (uid);
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
		public void LogNewUser (string uid) {
			GetWrapper ().LogNewUser (uid);
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
		public void LogUserLogout () {
			GetWrapper ().LogUserLogout ();
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
		public void LogUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser) {
			GetWrapper ().LogUserInfoUpdate (serverId, userId, userName, userLevel, userVipLevel, isPaidUser);
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
		public void LogPayment (string productId, string throughCargo, string purchaseData) {
			if (Application.platform == RuntimePlatform.Android) {
				GetWrapper ().LogPayment (productId, throughCargo, purchaseData);
			} else {
				// TODO
			}
		}

		//\cond
		#region callbacks
		public void OnFunplusSdkInstallSuccess (string message) {
			_delegate.OnSdkInstallSuccess (message);
		}
		
		public void OnFunplusSdkInstallError (string message) {
			_delegate.OnSdkInstallError (FunplusError.FromMessage(message));
		}
		#endregion //callbacks
		//\endcond
	}

}