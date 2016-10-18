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

	public class FunplusSdkUtilsIOS : BaseSdkUtilsWrapper 
	{

		private static FunplusSdkUtilsIOS instance;
		private static readonly object locker = new object ();

		public static FunplusSdkUtilsIOS Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusSdkUtilsIOS ();
					}
				}
				return instance;
			}
		}
		
		public override string GetTotalMemory () 
		{
			return com_funplus_sdk_getTotalMemory ();
		}
		
		public override string GetAvailableMemory () {
			return com_funplus_sdk_getAvailableMemory ();
		}

		public override string GetDeviceName () 
		{
			return null;
		}
		
		public override string GetOsName () 
		{
			return null;
		}
		
		public override string GetOsVersion () 
		{
			return null;
		}
		
		public override string GetCountry () 
		{
			return null;
		}
		
		public override string GetDeviceType () 
		{
			return null;
		}

		public override string GetScreenSize () 
		{
			return null;
		}
		
		public override string GetScreenOrientation () 
		{
			return null;
		}
		
		public override string GetScreenDensity () 
		{
			return null;
		}
		
		public override string GetDisplayWidth () 
		{
			return null;
		}
		
		public override string GetDisplayHeight () 
		{
			return null;
		}

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_getTotalMemory ();

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_getAvailableMemory ();
	}
}