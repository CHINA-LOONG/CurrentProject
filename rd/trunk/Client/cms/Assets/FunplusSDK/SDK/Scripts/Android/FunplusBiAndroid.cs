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

	public class FunplusBiAndroid : BaseBiWrapper
	{
		private AndroidJavaClass funplusBiWrapper;
		
		private static FunplusBiAndroid instance;
		private static readonly object locker = new object ();
		
		private FunplusBiAndroid ()
		{
			funplusBiWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusBiWrapper");
		}
		
		public static FunplusBiAndroid Instance
		{
			get
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusBiAndroid ();
					}
				}
				return instance;
			}
		}

		public override void TraceEvent (string eventName, string eventData)
		{
			funplusBiWrapper.CallStatic ("traceEvent", eventName, eventData);
		}

	}

}