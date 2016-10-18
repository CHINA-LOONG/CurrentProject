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
	
	public class FunplusPaymentStub : BasePaymentWrapper 
	{
		
		private static FunplusPaymentStub instance;
		private static readonly object locker = new object ();

		private bool canBuy;
		
		public static FunplusPaymentStub Instance
		{
			get
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusPaymentStub ();
					}
				}
				return instance;
			}
		}
		
		public override void SetGameObject (string gameObjectName) 
		{
			Debug.Log ("[funsdk] Calling FunplusPaymentStub.SetGameObject ().");
		}
		
		public override bool CanMakePurchases () 
		{
			return canBuy;
		}
		
		public override void StartHelper () 
		{
			canBuy = true;
//			FunplusPayment.getInstance ().onInitializeSuccess (
//				"[" +
//				"{\"productId\":\"com.funplus.p1\",\"type\":\"inapp\",\"price\":\"$0.99\",\"price_amount_micros\":990000,\"price_currency_code\":\"USD\",\"product_title\":\"test product 1 (Funplus Test App)\",\"description\":\"test product 1\"}," +
//				"{\"productId\":\"com.funplus.p2\",\"type\":\"inapp\",\"price\":\"$0.99\",\"price_amount_micros\":990000,\"price_currency_code\":\"USD\",\"title\":\"test product 2 (Funplus Test App)\",\"description\":\"test product 3\"}," +
//				"{\"productId\":\"com.funplus.p3\",\"type\":\"inapp\",\"price\":\"$0.99\",\"price_amount_micros\":990000,\"price_currency_code\":\"USD\",\"title\":\"test product 3 (Funplus Test App)\",\"description\":\"test product 3\"}" +
//				"]");
			FunplusPayment.Instance.OnPaymentInitializeSuccess (
				"[" +
				"{\"product_id\":\"com.funplus.p1\",\"type\":\"inapp\",\"price\":990000,\"formatted_price\":\"$0.99\",\"locale_currency_symbol\":\"$\",\"locale_currency_code\":\"USD\",\"product_title\":\"test product 1 (Funplus Test App)\",\"product_description\":\"test product 1\"}," +
				"{\"product_id\":\"com.funplus.p2\",\"type\":\"inapp\",\"price\":990000,\"formatted_price\":\"$0.99\",\"locale_currency_symbol\":\"$\",\"locale_currency_code\":\"USD\",\"product_title\":\"test product 2 (Funplus Test App)\",\"product_description\":\"test product 3\"}," +
				"{\"product_id\":\"com.funplus.p3\",\"type\":\"inapp\",\"price\":990000,\"formatted_price\":\"$0.99\",\"locale_currency_symbol\":\"$\",\"locale_currency_code\":\"USD\",\"product_title\":\"test product 3 (Funplus Test App)\",\"product_description\":\"test product 3\"}" +
				"]");
		}

		public override void Buy (string productId, string throughCargo) 
		{
			FunplusPayment.Instance.OnPaymentPurchaseSuccess ("{\"product_id\": \"" + productId + "\", \"through_cargo\": \"" + throughCargo + "\"}");
		}

	}
	
}