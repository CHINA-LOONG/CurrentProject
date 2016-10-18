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
using System.Collections.Generic;
using HSMiniJSON;

namespace Funplus
{

	/// <summary>
	/// This class represents a product used in the payment module.
	/// </summary>
	public class FunplusProduct 
	{
		/// <summary>
		/// The product ID for the product.
		/// </summary>
		public string ProductId { get; set; }

		/// <summary>
		/// Title of the product.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// The product's detailed description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// ISO 4217 currency code for price.
		/// </summary>
		public string PriceCurrencyCode { get; set; }

		/// <summary>
		/// Formatted price of the item, including its currency sign.
		/// </summary>
		public string FormattedPrice { get; set; }

		/// <summary>
		/// Price in micro-units, where 1,000,000 micro-units equal one unit of the currency.
		/// </summary>
		public long   Price { get; set; }

		//\cond

		/// <summary>
		/// Constructs a list of products by using the given message (sent from Google IAB).
		/// </summary>
		/// <returns>The product list.</returns>
		/// <param name="message">The Google IAB message.</param>
		public static List<FunplusProduct> FromGoogleIabMessage (string message) 
		{
			List<FunplusProduct> productList = new List<FunplusProduct> ();

			try
			{
				var list = Json.Deserialize (message) as List<object>;
				foreach (object item in list) {
					var dict = item as Dictionary<string, object>;
					productList.Add(new FunplusProduct {
						ProductId = (string)dict ["productId"],
						Title = (string)dict ["title"],
						Description = (string)dict ["description"],
						PriceCurrencyCode = (string)dict ["price_currency_code"],
						FormattedPrice = (string)dict ["price"],
						Price = (long)  dict ["price_amount_micros"]
					});
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError ("[funsdk] Failed to parse products info: " + e.Message);
			}

			return productList;
		}

		/// <summary>
		/// Constructs a list of products by using the given message (sent from Apple IAP).
		/// </summary>
		/// <returns>The product list.</returns>
		/// <param name="message">The Apple IAP message.</param>
		public static List<FunplusProduct> FromAppleIapMessage (string message) 
		{
			List<FunplusProduct> productList = new List<FunplusProduct> ();

			try
			{
				var list = Json.Deserialize (message) as List<object>;
				foreach (object item in list)
				{
					var dict = item as Dictionary<string, object>;

					productList.Add(new FunplusProduct {
						ProductId = (string)dict ["product_id"],
						Title = (string)dict ["product_title"],
						Description = (string)dict ["product_description"],
						PriceCurrencyCode = (string)dict ["locale_currency_code"],
						FormattedPrice = (string)dict ["formatted_price"],
						Price = (long)dict ["price"]
					});
	            }
			}
			catch (System.Exception e)
			{
				Debug.LogError ("[funsdk] Failed to parse products info: " + e.Message);
			}

			return productList;
		}

		//\endcond

		/// <summary>
		/// Gets the product ID.
		/// </summary>
		/// <returns>The product ID.</returns>
		public string GetProductId () 
		{
			return ProductId;
		}

		/// <summary>
		/// Gets the product title.
		/// </summary>
		/// <returns>The product title.</returns>
		public string GetTitle () 
		{
			return Title;
		}

		/// <summary>
		/// Gets the product description.
		/// </summary>
		/// <returns>The product description.</returns>
		public string GetDescription () 
		{
			return Description;
		}

		/// <summary>
		/// Gets the currency code of the price.
		/// </summary>
		/// <returns>The currency code.</returns>
		public string GetPriceCurrencyCode () 
		{
			return PriceCurrencyCode;
		}

		/// <summary>
		/// Gets the well-formatted price.
		/// </summary>
		/// <returns>The formatted price.</returns>
		public string GetFormattedPrice () 
		{
			return FormattedPrice;
		}

		/// <summary>
		/// Gets the product price.
		/// </summary>
		/// <returns>The product price.</returns>
		public long GetPrice () 
		{
			return Price;
		}
	}

}
