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

using System.Runtime.InteropServices;
using Funplus.Abstract;

namespace Funplus.IOS
{

	public class FunplusSdkIOS : BaseSdkWrapper 
	{

		private static FunplusSdkIOS instance;
		private static readonly object locker = new object ();

		public static FunplusSdkIOS Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusSdkIOS ();
					}
				}
				return instance;
			}
		}

		public override void SetGameObject (string gameObjectName) 
		{
			com_funplus_sdk_setGameObject (gameObjectName);
		}

		public override bool IsSdkInstalled () 
		{
			return com_funplus_sdk_isSdkInstalled ();
		}

		public override void Install (string gameId, string gameKey, string environment) 
		{
			bool isProduction = (environment == "production") ? true : false;
			com_funplus_sdk_setDebug(!isProduction);
			com_funplus_sdk_install (gameId, gameKey, isProduction);
		}

		public override void SetDebug (bool isDebug) 
		{
			com_funplus_sdk_setDebug (isDebug);
		}

		public override bool IsFirstLaunch () 
		{
			return com_funplus_sdk_isFirstLaunch ();
		}

		public string GetSdkVersion () 
		{
			return com_funplus_sdk_getSdkVersion ();
		}

		public override void LogUserLogin (string uid) 
		{
			com_funplus_sdk_logUserLogin (uid);
		}

		public override void LogNewUser (string uid) 
		{
			com_funplus_sdk_logNewUser (uid);
		}

		public override void LogUserLogout () 
		{
			com_funplus_sdk_logUserLogout ();
		}

		public override void LogUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser) 
		{
			com_funplus_sdk_logUserInfoUpdate (serverId, userId, userName, userLevel, userVipLevel, isPaidUser);
		}

		public override void LogPayment (string productId, string throughCargo, string purchaseData) 
		{
			com_funplus_sdk_logPayment (productId, throughCargo, purchaseData);
		}

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_setGameObject (string gameObjectName);

		[DllImport ("__Internal")]
		private static extern bool com_funplus_sdk_isSdkInstalled ();
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_install (string gameId, string gameKey, bool isProduction);
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_setDebug (bool isDebug);

		[DllImport ("__Internal")]
		private static extern bool com_funplus_sdk_isFirstLaunch ();

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_getSdkVersion ();

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_logUserLogin (string uid);

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_logNewUser (string uid);

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_logUserLogout ();

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_logUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser);

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_logPayment (string productId, string throughCargo, string purchaseData);
	}

}