#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

namespace Funplus.Stub
{

	public delegate void OnFacebookInit ();
	public delegate void OnFacebookLoginSuccess (string accessToken, string uid);
	public delegate void OnFacebookLoginFailed ();

	public class FacebookManager
	{

		private static OnFacebookInit onInit;
		private static OnFacebookLoginSuccess onLoginSuccess;
		private static OnFacebookLoginFailed onLoginFailed;

		public static void Init (OnFacebookInit onFacebookInit)
		{
			onInit = onFacebookInit;

			if (!FB.IsInitialized)
			{
				FB.Init (InitCallback, OnHideUnity);
			}
			else
			{
				FB.ActivateApp ();
			}
		}

		public static void Login (OnFacebookLoginSuccess onFacebookLoginSuccess, OnFacebookLoginFailed onFacebookLoginFailed)
		{
			onLoginSuccess = onFacebookLoginSuccess;
			onLoginFailed = onFacebookLoginFailed;

			var perms = new List<string>(){"public_profile", "email", "user_friends"};
			FB.LogInWithReadPermissions(perms, AuthCallback);
		}

		private static void InitCallback ()
		{
			if (FB.IsInitialized)
			{
				// Signal an app activation App Event
				FB.ActivateApp ();
				// Continue with Facebook SDK
				// ...

				if (onInit != null)
				{
					onInit ();
				}
			}
			else
			{
				Debug.Log("Failed to Initialize the Facebook SDK");
			}
		}

		private static void OnHideUnity (bool isGameShown)
		{
			if (!isGameShown)
			{
				// Pause the game - we will need to hide
				Time.timeScale = 0;
			}
			else
			{
				// Resume the game - we're getting focus again
				Time.timeScale = 1;
			}
		}

		private static void AuthCallback (ILoginResult result)
		{
			if (FB.IsLoggedIn)
			{
				var accessToken = Facebook.Unity.AccessToken.CurrentAccessToken;

				if (onLoginSuccess != null)
				{
					onLoginSuccess (accessToken.TokenString, accessToken.UserId);
				}
			}
			else
			{
				Debug.Log("User cancelled login");

				if (onLoginFailed != null)
				{
					onLoginFailed ();
				}
			}
		}
	}

}
#endif