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

using System.Collections;

namespace Funplus
{

	/// <summary>
	/// Account types used in the account module.
	/// </summary>
	public enum FunplusAccountType 
	{

		/// <summary>
		/// Unknown accoun type, normally this means error occurred.
		/// </summary>
		FPAccountTypeUnknown		= -1,

		/// <summary>
		/// Express account type.
		/// </summary>
		FPAccountTypeExpress		= 0,

		/// <summary>
		/// Email account type.
		/// </summary>
		FPAccountTypeEmail			= 1,

		/// <summary>
		/// Facebook account type.
		/// </summary>
		FPAccountTypeFacebook		= 2,

		/// <summary>
		/// VK account type.
		/// </summary>
		FPAccountTypeVK				= 3,

		/// <summary>
		/// WeChat account type.
		/// </summary>
		FPAccountTypeWechat			= 4,

		/// <summary>
		/// Google Plus account type.
		/// </summary>
		FPAccountTypeGooglePlus		= 5,

		/// <summary>
		/// Game center account type.
		/// </summary>
		FPAccountTypeGameCenter		= 6,

		/// <summary>
		/// Unspecified account type.
		/// </summary>
		FPAccountTypeNotSpecified	= 7,

		FPAccountTypeMobile 		= 8,
	}

	/// <summary>
	/// Helper class to generate <c>FunplusAccountType</c> from string or vice verse.
	/// </summary>
	static class FunplusAccountTypeUtils
	{
		public static FunplusAccountType FromString(string type)
		{
			if (type == null)
			{
				return FunplusAccountType.FPAccountTypeUnknown;
			}

			switch (type.ToLower ())
			{
			case "express":
				return FunplusAccountType.FPAccountTypeExpress;
			case "email":
				return FunplusAccountType.FPAccountTypeEmail;
			case "mobile":
				return FunplusAccountType.FPAccountTypeMobile;
			case "facebook":
				return FunplusAccountType.FPAccountTypeFacebook;
			case "vk":
				return FunplusAccountType.FPAccountTypeVK;
			case "wechat":
				return FunplusAccountType.FPAccountTypeWechat;
			case "googleplus":
				return FunplusAccountType.FPAccountTypeGooglePlus;
			case "gamecenter":
				return FunplusAccountType.FPAccountTypeGameCenter;
			default:
				return FunplusAccountType.FPAccountTypeNotSpecified;
			}
		}

		public static string ToString(FunplusAccountType type)
		{
			switch (type)
			{
			case FunplusAccountType.FPAccountTypeExpress:
				return "express";
			case FunplusAccountType.FPAccountTypeEmail:
				return "email";
			case FunplusAccountType.FPAccountTypeMobile:
				return "mobile";
			case FunplusAccountType.FPAccountTypeFacebook:
				return "facebook";
			case FunplusAccountType.FPAccountTypeVK:
				return "vk";
			case FunplusAccountType.FPAccountTypeWechat:
				return "wechat";
			case FunplusAccountType.FPAccountTypeGooglePlus:
				return "googleplus";
			case FunplusAccountType.FPAccountTypeGameCenter:
				return "gamecenter";
			default:
				return "unknown";
			}
		}
	}

}