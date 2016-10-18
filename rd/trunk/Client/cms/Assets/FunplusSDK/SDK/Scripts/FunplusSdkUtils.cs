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

using System;
using System.Collections;
using Funplus.Abstract;
using Funplus.Android;
using Funplus.IOS;
using Funplus.Stub;

namespace Funplus
{

	/// <summary>
	/// This class provides some helper methods for Funplus SDK.
	/// </summary>
	public class FunplusSdkUtils
	{
		private static FunplusSdkUtils instance;
		private static readonly object locker = new object ();

		private BaseSdkUtilsWrapper Wrapper
		{
			get
			{
				#if UNITY_ANDROID
				return FunplusSdkUtilsAndroid.Instance;
				#elif UNITY_IOS
				return FunplusSdkUtilsIOS.Instance;
				#else
				return FunplusSdkUtilsStub.Instance;
				#endif
			}
		}

		[Obsolete("This method is deprecated, please use FunplusSdkUtils.Instance instead.")]
		public static FunplusSdkUtils GetIntance ()
		{
			return Instance;
		}

		public static FunplusSdkUtils Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusSdkUtils();
					}
				}
				return instance;
			}
		}

		/// <summary>
		/// Gets the total memory.
		/// </summary>
		/// <returns>The total memory.</returns>
		public string GetTotalMemory ()
		{
			return Wrapper.GetTotalMemory ();
		}

		/// <summary>
		/// Gets the available memory.
		/// </summary>
		/// <returns>The available memory.</returns>
		public string GetAvailableMemory()
		{
			return Wrapper.GetAvailableMemory ();
		}

		/// <summary>
		/// Gets the name of the device.
		/// </summary>
		/// <returns>The device name.</returns>
		public string GetDeviceName()
		{
			return Wrapper.GetDeviceName ();
		}

		/// <summary>
		/// Gets the name of the OS.
		/// </summary>
		/// <returns>The OS name.</returns>
		public string GetOsName()
		{
			return Wrapper.GetOsName ();
		}

		/// <summary>
		/// Gets the OS version.
		/// </summary>
		/// <returns>The OS version.</returns>
		public string GetOsVersion()
		{
			return Wrapper.GetOsVersion ();
		}

		/// <summary>
		/// Gets country.
		/// </summary>
		/// <returns>The country code.</returns>
		public string GetCountry()
		{
			return Wrapper.GetCountry ();
		}

		/// <summary>
		/// Gets the type of the device.
		/// </summary>
		/// <returns>The device type.</returns>
		public string GetDeviceType()
		{
			return Wrapper.GetDeviceType ();
		}

		/// <summary>
		/// Gets the size of the screen.
		/// </summary>
		/// <returns>The screen size.</returns>
		public string GetScreenSize()
		{
			return Wrapper.GetScreenSize ();
		}

		/// <summary>
		/// Gets the screen orientation.
		/// </summary>
		/// <returns>The screen orientation.</returns>
		public string GetScreenOrientation()
		{
			return Wrapper.GetScreenOrientation ();
		}

		/// <summary>
		/// Gets the screen density.
		/// </summary>
		/// <returns>The screen density.</returns>
		public string GetScreenDensity()
		{
			return Wrapper.GetScreenDensity ();
		}

		/// <summary>
		/// Gets the display width.
		/// </summary>
		/// <returns>The display width.</returns>
		public string GetDisplayWidth()
		{
			return Wrapper.GetDisplayWidth ();
		}

		/// <summary>
		/// Gets the display height.
		/// </summary>
		/// <returns>The display height.</returns>
		public string GetDisplayHeight()
		{
			return Wrapper.GetDisplayHeight ();
		}

		public string GetHashKey()
		{
			// TODO
			return null;
		}
	}

}