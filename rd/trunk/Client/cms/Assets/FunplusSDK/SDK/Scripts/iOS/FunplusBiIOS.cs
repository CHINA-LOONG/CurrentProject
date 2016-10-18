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

	public class FunplusBiIOS : BaseBiWrapper 
	{

		private static FunplusBiIOS instance;
		private static readonly object locker = new object ();
		
		public static FunplusBiIOS Instance 
		{
			get
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusBiIOS ();
					}
				}
				return instance;
			}
		}

		public override void TraceEvent (string eventName, string eventData) 
		{
			com_funplus_sdk_bi_traceEvent (eventName, eventData);
		}
		
		public void LogUserLogin (string uid) 
		{
			com_funplus_sdk_bi_logUserLogin (uid);
		}
		
		public void LogNewUser (string uid) 
		{
			com_funplus_sdk_bi_logNewUser (uid);
		}
		
		public void LogUserLogout () 
		{
			com_funplus_sdk_bi_logUserLogout ();
		}

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_bi_traceEvent (string eventName, string eventData);

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_bi_logUserLogin (string uid);
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_bi_logNewUser (string uid);
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_bi_logUserLogout ();
	}

}