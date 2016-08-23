using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HSMiniJSON;

namespace Funplus {

	/// <summary>
	/// This class represents a product used in the payment module.
	/// </summary>
	public class FunplusProduct {

		private string productId;
		private string title;
		private string description;
		private string priceCurrencyCode;
		private string formattedPrice;
		private long price;

		private FunplusProduct (string productId,
		                        string title,
		                        string description,
		                        string priceCurrencyCode,
		                        string formattedPrice,
		                        long price) {
			this.productId = productId;
			this.title = title;
			this.description = description;
			this.priceCurrencyCode = priceCurrencyCode;
			this.formattedPrice = formattedPrice;
			this.price = price;
		}

		//\cond

		/// <summary>
		/// Constructs a list of products by using the given message (sent from Google IAB).
		/// </summary>
		/// <returns>The product list.</returns>
		/// <param name="message">The Google IAB message.</param>
		public static List<FunplusProduct> FromGoogleIabMessage (string message) {
			List<FunplusProduct> productList = new List<FunplusProduct> ();

			var list = Json.Deserialize (message) as List<object>;
			foreach (object item in list) {
				var dict = item as Dictionary<string, object>;
				productList.Add(new FunplusProduct ((string)dict ["productId"],
				                                    (string)dict ["title"],
				                                    (string)dict ["description"],
				                                    (string)dict ["price_currency_code"],
				                                    (string)dict ["price"],
				                                    (long)  dict ["price_amount_micros"]));
			}

			return productList;
		}

		/// <summary>
		/// Constructs a list of products by using the given message (sent from Apple IAP).
		/// </summary>
		/// <returns>The product list.</returns>
		/// <param name="message">The Apple IAP message.</param>
		public static List<FunplusProduct> FromAppleIapMessage (string message) {
			List<FunplusProduct> productList = new List<FunplusProduct> ();

			var list = Json.Deserialize (message) as List<object>;
			foreach (object item in list) {
				var dict = item as Dictionary<string, object>;
				long price = (long)dict ["price"];

				productList.Add(new FunplusProduct ((string)dict ["product_id"],
				                                    (string)dict ["product_title"],
				                                    (string)dict ["product_description"],
				                                    (string)dict ["locale_currency_code"],
				                                    (string)dict ["formatted_price"],
				                                    price));
            }

			return productList;
		}

		//\endcond

		/// <summary>
		/// Gets the product ID.
		/// </summary>
		/// <returns>The product ID.</returns>
		public string GetProductId () {
			return productId;
		}

		/// <summary>
		/// Gets the product title.
		/// </summary>
		/// <returns>The product title.</returns>
		public string GetTitle () {
			return title;
		}

		/// <summary>
		/// Gets the product description.
		/// </summary>
		/// <returns>The product description.</returns>
		public string GetDescription () {
			return description;
		}

		/// <summary>
		/// Gets the currency code of the price.
		/// </summary>
		/// <returns>The currency code.</returns>
		public string GetPriceCurrencyCode () {
			return priceCurrencyCode;
		}

		/// <summary>
		/// Gets the well-formatted price.
		/// </summary>
		/// <returns>The formatted price.</returns>
		public string GetFormattedPrice () {
			return formattedPrice;
		}

		/// <summary>
		/// Gets the product price.
		/// </summary>
		/// <returns>The product price.</returns>
		public long GetPrice () {
			return price;
		}
	}

}
