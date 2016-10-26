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

	public class FunplusAccountAndroid : BaseAccountWrapper
	{

		private AndroidJavaClass funplusAccountWrapper;

		private static FunplusAccountAndroid instance;
		private static readonly object locker = new object ();

		private FunplusAccountAndroid ()
		{
			funplusAccountWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusAccountWrapper");
		}

		public static FunplusAccountAndroid Instance
		{
			get {
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusAccountAndroid ();
					}
				}
				return instance;
			}
		}

		public override void SetGameObject (string gameObjectName)
		{
			funplusAccountWrapper.CallStatic ("setGameObject", gameObjectName);
		}

		public override bool IsUserLoggedIn ()
		{
			return funplusAccountWrapper.CallStatic<bool> ("isUserLoggedIn");
		}

		public override void GetAvailableAccountTypes ()
		{
			funplusAccountWrapper.CallStatic ("getAvailableAccountTypes");
		}

		public override void OpenSession ()
		{
			funplusAccountWrapper.CallStatic ("openSession");
		}

		public override void Login ()
		{
			funplusAccountWrapper.CallStatic ("login");
		}

		public override void Login (FunplusAccountType type)
		{
			funplusAccountWrapper.CallStatic ("login", (int)type);
		}

		public override void LoginWithEmail (string email, string password)
		{
			funplusAccountWrapper.CallStatic ("loginWithEmail", email, password);
		}

		public override void RegisterWithEmail (string email, string password)
		{
			funplusAccountWrapper.CallStatic ("registerWithEmail", email, password);
		}

		public override void ResetPassword (string email)
		{
			funplusAccountWrapper.CallStatic ("resetPassword", email);
		}
		
		public override void Logout ()
		{
			funplusAccountWrapper.CallStatic ("logout");
		}

		public override void ShowUserCenter ()
		{
			funplusAccountWrapper.CallStatic ("showUserCenter");
		}

		public override void BindAccount ()
		{
			funplusAccountWrapper.CallStatic ("bind");
		}

		public override void BindAccount (FunplusAccountType type)
		{
			funplusAccountWrapper.CallStatic ("bind", (int)type);
		}
	}
	
}