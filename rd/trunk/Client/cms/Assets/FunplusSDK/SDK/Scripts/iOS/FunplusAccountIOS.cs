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

	public class FunplusAccountIOS : BaseAccountWrapper 
	{

		private static FunplusAccountIOS instance;
		private static readonly object locker = new object ();
		
		public static FunplusAccountIOS Instance
		{
			get
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusAccountIOS ();
					}
				}
				return instance;
			}
		}

		public override void SetGameObject (string gameObjectName)
		{
			com_funplus_sdk_account_setGameObject (gameObjectName);
		}

		public override void GetAvailableAccountTypes() 
		{
			// TODO
		}
		
		public override bool IsUserLoggedIn () 
		{
			return com_funplus_sdk_account_isUserLoggedIn ();
		}

		public override void OpenSession () 
		{
			com_funplus_sdk_account_openSession ();
		}

		public override void Login () 
		{
			com_funplus_sdk_account_login ();
		}

		public override void Login (FunplusAccountType type) 
		{
			com_funplus_sdk_account_loginWithType ((int)type);
		}

		public override void Logout () 
		{
			com_funplus_sdk_account_logout ();
		}

		public override void ShowUserCenter () 
		{
			com_funplus_sdk_account_showUserCenter ();
		}

		public override void BindAccount () 
		{
			com_funplus_sdk_account_bindAccount ();
		}

		public override void BindAccount (FunplusAccountType type) 
		{
			com_funplus_sdk_account_bindAccountWithType ((int)type);
		}

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_setGameObject (string gameObjectName);
		
		[DllImport ("__Internal")]
		private static extern bool com_funplus_sdk_account_isUserLoggedIn ();
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_openSession ();
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_login ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_loginWithType (int type);

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_logout ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_showUserCenter ();
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_bindAccount ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_bindAccountWithType (int type);

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_account_getAvailableAccountTypes ();
	}

}