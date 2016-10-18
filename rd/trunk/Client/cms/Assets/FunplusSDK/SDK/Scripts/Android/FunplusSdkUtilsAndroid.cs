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

	public class FunplusSdkUtilsAndroid : BaseSdkUtilsWrapper 
	{

		private AndroidJavaClass funplusSdkUtilsWrapper;

		private static FunplusSdkUtilsAndroid instance;
		private static readonly object locker = new object ();

		private FunplusSdkUtilsAndroid () 
		{
			funplusSdkUtilsWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusSdkUtilsWrapper");
		}

		public static FunplusSdkUtilsAndroid Instance
		{
			get
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusSdkUtilsAndroid ();
					}
				}
				return instance;
			}
		}

		public override string GetTotalMemory ()
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getTotalMemory");
		}

		public override string GetAvailableMemory () 
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getAvailableMemory");
		}

		public override string GetDeviceName() 
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getDeviceName");
		}

		public override string GetOsName()
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getOsName");
		}

		public override string GetOsVersion()
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getOsVersion");
		}

		public override string GetCountry() 
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getCountry");
		}

		public override string GetDeviceType() 
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getDeviceType");
		}

		public override string GetScreenSize() 
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getScreenSize");
		}
		
		public override string GetScreenOrientation() 
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getScreenOrientation");
		}
		
		public override string GetScreenDensity() 
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getScreenDensity");
		}

		public override string GetDisplayWidth() 
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getDisplayWidth");
		}

		public override string GetDisplayHeight() 
		{
			return funplusSdkUtilsWrapper.CallStatic<string> ("getDisplayHeight");
		}
	}

}