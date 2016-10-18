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
using System.Collections;
using Funplus.Abstract;

namespace Funplus.Android
{
	
	public class FunplusSdkAndroid : BaseSdkWrapper 
	{

		private AndroidJavaClass unityPlayer;
		private AndroidJavaObject currentActivity;
		private AndroidJavaClass funplusSdkWrapper;

		private static FunplusSdkAndroid instance;
		private static readonly object locker = new object ();

		private FunplusSdkAndroid () 
		{
			unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			currentActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			funplusSdkWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusSdkWrapper");

			if (unityPlayer == null)
			{
				UnityEngine.Debug.LogError ("[funsdk] Unable to find com.unity3d.player.UnityPlayer");
			}

			if (currentActivity == null) 
			{
				UnityEngine.Debug.LogError ("[funsdk] Unable to find currentActivity");
			}

			if (funplusSdkWrapper == null) 
			{
				UnityEngine.Debug.LogError ("[funsdk] Unable to find com.funplus.sdk.unity3d.FunplusSdkWrapper");
			}
		}

		public static FunplusSdkAndroid Instance
		{
			get
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusSdkAndroid ();
					}
				}
				return instance;
			}
		}

		public override void SetGameObject (string gameObjectName) 
		{
			funplusSdkWrapper.CallStatic ("setGameObject", gameObjectName);
		}

		public override bool IsSdkInstalled () 
		{
			return funplusSdkWrapper.CallStatic<bool> ("isSdkInstalled");
		}

		public override void Install (string gameId, string gameKey, string environment) 
		{
			environment = (environment == "production") ? "production" : "sandbox";
			funplusSdkWrapper.CallStatic ("install", new object[] {
				currentActivity,
				gameId,
				gameKey,
				environment
			});
		}

		public override bool IsFirstLaunch () 
		{
			return funplusSdkWrapper.CallStatic<bool> ("isFirstLaunch");
		}

		public string GetSdkVersion () 
		{
			return funplusSdkWrapper.CallStatic<string> ("getSdkVersion");
		}

		public override void SetDebug (bool isDebug) 
		{

		}

		public override void LogUserLogin (string uid) 
		{
			funplusSdkWrapper.CallStatic ("logUserLogin", uid);
		}

		public override void LogNewUser (string uid) 
		{
			funplusSdkWrapper.CallStatic ("logNewUser", uid);
		}

		public override void LogUserLogout () 
		{
			funplusSdkWrapper.CallStatic ("logUserLogout");
		}

		public override void LogUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser) 
		{
			funplusSdkWrapper.CallStatic ("logUserInfoUpdate", new object[] {
				serverId,
				userId,
				userLevel,
				userVipLevel,
				isPaidUser
			});
		}

		public override void LogPayment (string productId, string throughCargo, string purchaseData) 
		{
			funplusSdkWrapper.CallStatic ("logPayment", productId, throughCargo, purchaseData);
		}
	}

}