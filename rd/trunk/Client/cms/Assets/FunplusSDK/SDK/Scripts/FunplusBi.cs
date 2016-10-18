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
using System;
using System.Collections;
using Funplus.Abstract;
using Funplus.Android;
using Funplus.IOS;
using Funplus.Stub;

namespace Funplus
{

	/// <summary>
	/// The BI module helps to integrate your game to Funplus Data System.
	/// </summary>
	public class FunplusBi
	{

		private static FunplusBi instance;
		private static readonly object locker = new object ();

		private BaseBiWrapper Wrapper
		{
			get
			{
				#if UNITY_ANDROID
				return FunplusBiAndroid.Instance;
				#elif UNITY_IOS
				return FunplusBiIOS.Instance;
				#else
				return FunplusBiStub.Instance;
				#endif
			}
		}

		/// <summary>
		/// Gets the intance.
		/// </summary>
		/// <returns>The intance.</returns>
		[Obsolete("This method is deprecated, please use FunplusBi.Instance instead.")]
		public static FunplusBi GetIntance ()
		{
			return Instance;
		}

		public static FunplusBi Instance
		{
			get
			{
				if (instance == null)
				{
					lock (locker) 
					{
						instance = new FunplusBi();
					}
				}
				return instance;
			}
		}

		/// <summary>
		/// Traces an event.
		/// </summary>
		/// <param name="eventName">The event name.</param>
		/// <param name="properties">Properties of the event.</param>
		public void TraceEvent (string eventName, string properties)
		{
			if (string.IsNullOrEmpty(eventName))
			{
				Debug.LogError ("[funsdk] `eventName` should not be empty.");
				return;
			}
			if (string.IsNullOrEmpty(properties))
			{
				Debug.LogError ("[funsdk] `properties` should not be empty.");
				return;
			}

			Wrapper.TraceEvent (eventName, properties);
		}
	}

}