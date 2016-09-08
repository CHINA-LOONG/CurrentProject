using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HSMiniJSON;

namespace Funplus {

	/// <summary>
	/// This class represents an error. Each error has a unique error code which is given by <code>errorCode</code>.
	/// <code>errorMsg</code> and <code>errorLocalizedMsg</code> give a more detailed description about the error.
	/// </summary>
	public class FunplusError {

		private long errorCode;
		private string errorMsg;
		private string errorLocalizedMsg;

		//\cond

		/// <summary>
		/// Constructs an error by using the given message (sent from native code).
		/// </summary>
		/// <returns>The error instance.</returns>
		/// <param name="message">Message.</param>
		public static FunplusError FromMessage (string message) {
			FunplusError error = new FunplusError ();

			try {
				var dict = Json.Deserialize (message) as Dictionary<string,object>;

				error.errorCode = (long)dict ["errorCode"];
				error.errorMsg = (string)dict ["errorMsg"];
				error.errorLocalizedMsg = (string)dict ["errorLocalizedMsg"];
			} catch (Exception) {
				error.errorCode = 9999;
				error.errorMsg = "Unknown error";
				error.errorLocalizedMsg = "Unknown error";
			}

			return error;
		}

		//\endcond

		/// <summary>
		/// Gets the error code.
		/// </summary>
		/// <returns>The error code.</returns>
		public long GetErrorCode () {
			return errorCode;
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <returns>The error message.</returns>
		public string GetErrorMsg () {
			return errorMsg;
		}

		/// <summary>
		/// Gets the localized error message.
		/// </summary>
		/// <returns>The error localized message.</returns>
		public string GetErrorLocalizedMsg () {
			return errorLocalizedMsg;
		}
	}

}