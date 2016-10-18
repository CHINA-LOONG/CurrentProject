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
	/// The Helpshift module.
	/// </summary>
	public class FunplusHelpshift
	{

		private static FunplusHelpshift instance;
		private static readonly object locker = new object ();

		private BaseHelpshiftWrapper Wrapper
		{
			get
			{
				#if UNITY_ANDROID
				return FunplusHelpshiftAndroid.Instance;
				#elif UNITY_IOS
				return FunplusHelpshiftIOS.Instance;
				#else
				return FunplusHelpshiftStub.Instance;
				#endif
			}
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		[Obsolete("This method is deprecated, please use FunplusHelpshift.Instance instead.")]
		public static FunplusHelpshift GetInstance ()
		{
			return Instance;
		}

		public static FunplusHelpshift Instance
		{
			get
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusHelpshift();
					}
				}
				return instance;
			}
		}

		/// <summary>
		/// Shows the conversation window.
		/// </summary>
		public void ShowConversation ()
		{
			Wrapper.ShowConversation ();
		}

		/// <summary>
		/// Shows the FAQ window.
		/// </summary>
		public void ShowFAQs()
		{
			Wrapper.ShowFAQs ();
		}
	}

}