using UnityEngine;
using System.Collections;

namespace Funplus {

	/// <summary>
	/// Account types used in the account module.
	/// </summary>
	public enum FunplusAccountType {

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
	}

}