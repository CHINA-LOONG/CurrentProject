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
using System.Collections.Generic;
using HSMiniJSON;

namespace Funplus
{

	/// <summary>
	/// This class represents an error. Each error has a unique error code which is given by <code>errorCode</code>.
	/// <code>errorMsg</code> and <code>errorLocalizedMsg</code> give a more detailed description about the error.
	/// </summary>
	public class FunplusError
	{
		public long   ErrorCode { get; set; }
		public string ErrorMsg { get; set; }
		public string ErrorLocalizedMsg { get; set; }

		public static FunplusError E (long errorCode)
		{
			return new FunplusError {
				ErrorCode = errorCode,
				ErrorMsg = errors [errorCode],
				ErrorLocalizedMsg = errors [errorCode]
			};
		}

		public static FunplusError E (long errorCode, string errorMsg)
		{
			return new FunplusError {
				ErrorCode = errorCode,
				ErrorMsg = errorMsg,
				ErrorLocalizedMsg = errorMsg
			};
		}

		public static FunplusError E (long errorCode, string errorMsg, string errorLocalizedMsg)
		{
			return new FunplusError {
				ErrorCode = errorCode,
				ErrorMsg = errorMsg,
				ErrorLocalizedMsg = errorLocalizedMsg
			};
		}

		private static Dictionary<long, string> errors = new Dictionary<long, string> {
            // No error.
            {0, "No error"},
            // SDK errors.
            {100, "Failed to install"},
            {101, "Failed to connect to config server"},
            {102, "Config server request data failed"},
            {103, "Parse config failed"},
            {104, "Invalid configuration"},
            {105, "Reflection error"},
            {106, "No local configuration found"},
            // Account server errors.
            {1101, "Wrong signature"},
            {1102, "There was a server error when creating your Funplus ID, please try again later"},
            {1103, "Incorrect game ID"},
            {1104, "Please input the correct email address"},
            {1105, "Password format is wrong"},
            {1106, "Incorrect GUID"},
            {1107, "This account already exists, please input an unique email address"},
            {1108, "There was a server error when creating your account"},
            {1109, "This Funplus ID has already been bound"},
            {1110, "There is no account with this email address"},
            {1111, "Your email or password is incorrect"},
            {1112, "Password must be 6~20 characters"},
            {1113, "Failed to reset password"},
            {1114, "Failed to reset password"},
            {1115, "Failed to reset password"},
            {1116, "The new password cannot be the same as the old password"},
            {1117, "Failed to change password"},
            {1118, "There is no account with this email address"},
            {1119, "Failed to reset password"},
            {1120, "Wrong game settings"},
            {1121, "Failed to reset temp password"},
            {1122, "Please give a valid FPID"},
            {1123, "Facebook ID is not correct"},
            {1124, "failed to bind with facebook"},
            {1125, "Reach register limit"},
            {1126, "Email address is too long"},
            // Newly added.
            {1127, "Session is empty"},
            {1128, "Failed to find session"},
            {1129, "Session has expired"},
            {1130, "Session and game do not match"},
            {1131, "Platform token is invalid"},
            {1132, "Platform type is invalid"},
            {1133, "Platform token is invalid"},
            {1134, "Platform ID and Funplus ID do not match"},
            {1135, "Platform information is not found"},
            {1136, "Mobile country code is invalid"},
            {1137, "Mobile number is invalid"},
            {1138, "This mobile number has been taken"},
            {1139, "This mobile number does not exist"},
            {1140, "Failed to generate confirmation code"},
            {1141, "Confirmation code is invalid"},
            {1142, "This mobile number has not been confirmed"},
            {1143, "Failed to reset password for this mobile number"},
            // Account client errors.
            {1200, "Unknown account type"},
            {1201, "User is not logged in"},
            {1202, "User has already logged in"},
            {1203, "Please wait until the previous login process completes"},
            {1204, "Invalid login parameters"},
            {1205, "Login failed"},
            {1206, "Bind account failed"},
            {1207, "Failed to connect funplus passport server"},
            {1208, "Failed to parse the response from funplus passport server"},
            // WeChat errors.
            {2000, "WeChat: Client error"},
            {2001, "WeChat: User cancelled action"},
            {2002, "WeChat: User login failed"},
            {2003, "WeChat: Inconsistent WeChat accounts"},
            {2004, "WeChat: Not in White List"},
            {2005, "WeChat: Funplus ID is already bound to another account"},
            {2006, "WeChat: Get user data failed"},
            {2007, "WeChat: Undefined exception"},
            {2008, "WeChat: Get friends data failed"},
            // Facebook errors.
            {2100, "Facebook user not logged in"},
            {2101, "Facebook user login failed"},
            {2102, "Facebook failed to get user data"},
            {2103, "Facebook Failed to send game request"},
            {2104, "Facebook user canceled the action"},
            {2105, "Facebook exception"},
            {2106, "Facebook has been bound to another account"},
            {2107, "Facebook failed to get game friends"},
            {2108, "Facebook need user_friends permission"},
            // VK errors.
            {2200, "VK user not logged in"},
            {2201, "VK user login failed"},
            {2202, "VK captcha error"},
            {2203, "VK access denied"},
            {2204, "VK failed to accept user token"},
            {2205, "VK failed to receive new token"},
            {2206, "VK failed to renew token"},
            {2207, "VK exception"},
            {2208, "VK user canceled the action"},
            {2209, "VK failed to get user data"},
            {2210, "VK has been bound to another account"},
            {2211, "VK failed to get game friends"},
            // Google Plus errors.
            {2300, "Google Plus current person is null"},
            {2301, "Google Plus failed to get access token"},
            {2302, "Google Plus connection failed"},
            {2303, "Google Plus failed to get game friends"},
            // Payment errors.
            {3000, "Failed to submit data to funplus payment server"},
            {3001, "xxx"},
            {3002, "xxx"},
            // Third party payment errors.
            {3900, "Non-successful get_local_pacakges response"},
            {3901, "Error parsing get_local_pacakges response"},
            {3902, "Error sending get_local_pacakges request"},
            // Google IAB errors.
            {3100, "Failed to initialize Google IAB"},
            {3101, "Failed to buy product via Google IAB"},
            {3102, "Google account is not configured"},
            {3103, "Failed to consume product via Google IAB"},
            // BI errors.
            {4000, "No such BI event"},
            {4001, "BI event properties are not valid"},
            // Unspecific errors.
            {9999, "Unspecific error"},
		};

		//\cond

		/// <summary>
		/// Constructs an error by using the given message (sent from native code).
		/// </summary>
		/// <returns>The error instance.</returns>
		/// <param name="message">Message.</param>
		public static FunplusError FromMessage (string message) 
		{
			try
			{
				var dict = Json.Deserialize (message) as Dictionary<string,object>;
				return E ((long)dict ["errorCode"], (string)dict ["errorMsg"], (string)dict ["errorLocalizedMsg"]);
			}
			catch (Exception ex)
			{
				Debug.LogError ("[funsdk] JSON parse error: " + ex.Message);
				return E (9999);
			}
		}

		//\endcond

		/// <summary>
		/// Gets the error code.
		/// </summary>
		/// <returns>The error code.</returns>
		public long GetErrorCode () 
		{
			return ErrorCode;
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <returns>The error message.</returns>
		public string GetErrorMsg () 
		{
			return ErrorMsg;
		}

		/// <summary>
		/// Gets the localized error message.
		/// </summary>
		/// <returns>The error localized message.</returns>
		public string GetErrorLocalizedMsg () 
		{
			return ErrorLocalizedMsg;
		}

		public string ToJsonString ()
		{
			return string.Format (
				@"{{""errorCode"": {0}, ""errorMsg"": ""{1}"", ""errorLocalizedMsg"": ""{2}""}}",
				ErrorCode, ErrorMsg, ErrorLocalizedMsg
			);
		}

	}

}
