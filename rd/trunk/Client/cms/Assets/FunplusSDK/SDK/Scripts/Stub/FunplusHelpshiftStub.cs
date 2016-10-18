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
using Funplus.Abstract;

namespace Funplus.Stub
{

	public class FunplusHelpshiftStub : BaseHelpshiftWrapper 
	{

		private static FunplusHelpshiftStub instance;
		private static readonly object locker = new object ();
		
		private FunplusHelpshiftStub () 
		{
		}
		
		public static FunplusHelpshiftStub Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusHelpshiftStub ();
					}
				}
				return instance;
			}
		}

		public override void ShowConversation () 
		{
			Debug.Log ("[funsdk] Calling FunplusHelpshiftStub.ShowConversation ().");
		}

		public override void ShowFAQs () 
		{
			Debug.Log ("[funsdk] Calling FunplusHelpshiftStub.ShowFAQs ().");
		}

	}

}