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
using Funplus.Abstract;

namespace Funplus.Stub
{

	public class FunplusSdkStub : BaseSdkWrapper 
	{

		private static FunplusSdkStub instance;
		private static readonly object locker = new object ();

		private bool installed;

		public static FunplusSdkStub Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusSdkStub ();
					}
				}
				return instance;
			}
		}

		public override void SetGameObject (string gameObjectName) 
		{
//			Debug.Log ("[funsdk] Calling FunplusSdkStub.SetGameObject ().");
		}

		public override bool IsSdkInstalled () 
		{
			return installed;
		}
		
		public override void Install (string gameId, string gameKey, string environment) 
		{
			Debug.Log ("[funsdk] Funplus SDK installed.");
			Debug.LogFormat ("[funsdk] Game ID: {0}", gameId);
			Debug.LogFormat ("Environment: {0}", environment);

			installed = true;
			FunplusSdk.Instance.OnFunplusSdkInstallSuccess (null);
		}
		
		public override bool IsFirstLaunch () 
		{
			return false;
		}

		public override void SetDebug (bool isDebug) 
		{
		}
		
		public override void LogUserLogin (string uid) 
		{
			Debug.Log ("[funsdk] Calling FunplusSdkStub.LogUserLogin ().");
		}
		
		public override void LogNewUser (string uid) 
		{
			Debug.Log ("[funsdk] Calling FunplusSdkStub.LogNewUser ().");
		}
		
		public override void LogUserLogout () 
		{
			Debug.Log ("[funsdk] Calling FunplusSdkStub.LogUserLogout ().");
		}
		
		public override void LogUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser) 
		{
			Debug.Log ("[funsdk] Calling FunplusSdkStub.LogUserInfoUpdate ().");
		}
		
		public override void LogPayment (string productId, string throughCargo, string purchaseData) 
		{
			Debug.Log ("[funsdk] Calling FunplusSdkStub.LogPayment ().");
		}
	}

}