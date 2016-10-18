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
	/// The RUM module helps to integrate your game to Funplus RUM System.
	/// </summary>
	public class FunplusRum
	{
		
		private static FunplusRum instance;
		private static readonly object locker = new object ();
		
		private BaseRumWrapper Wrapper
		{
			get
			{
				#if UNITY_ANDROID
				return FunplusRumAndroid.Instance;
				#elif UNITY_IOS
				return FunplusRumIOS.Instance;
				#else
				return FunplusRumStub.Instance;
				#endif
			}
		}
		
		/// <summary>
		/// Gets the intance.
		/// </summary>
		/// <returns>The intance.</returns>
		[Obsolete("This method is deprecated, please use FunplusRum.Instance instead.")]
		public static FunplusRum GetIntance ()
		{
			return Instance;
		}

		public static FunplusRum Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusRum();
					}
				}
				return instance;
			}
		}
		
		/// <summary>
		/// Traces the service_monitoring event.
		/// </summary>
		/// <param name="eventName">The event name.</param>
		/// <param name="properties">Properties of the event.</param>
		public void TraceServiceMonitoring (string serviceName, string httpUrl, string httpStatus, int httpLatency, int requestSize, int responseSize)
		{
			if (string.IsNullOrEmpty (serviceName))
			{
				Debug.LogError ("[funsdk] `serviceName` must not be empty.");
				return;
			}
			if (string.IsNullOrEmpty (httpUrl))
			{
				Debug.LogError ("[funsdk] `httpUrl` must not be empty.");
				return;
			}
			Wrapper.TraceServiceMonitoring (serviceName, httpUrl, httpStatus, httpLatency, requestSize, responseSize);
		}
	}
	
}