#if UNITY_IOS || UNITY_ANDROID

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Funplus{

	public class FunplusUtils {

		public static FunplusError makeFunplusError(Dictionary<string, object> aDic) 
		{
			string errorCode = (string)aDic ["errorcode"];
			string errorMsg = (string)aDic["errormsg"];
			string errorLocMsg = (string)aDic["errorlocmsg"]; 
			
			FunplusError retError = new FunplusError(
				(FunplusError.EFunplusErrorCode)Convert.ToInt32(errorCode),
				errorMsg, errorLocMsg);
			
			return retError;
		}

		public static FunplusUser makeFunplusUser(Dictionary<string, object> aDic)
		{
			string uid = (String)aDic["uid"];
			string name = (String)aDic["name"];
			string email = (String)aDic["email"];
			string gender = (String)aDic["gender"];
			string accessToken = (String)aDic["accesstoken"];

			FunplusUser retUser = new FunplusUser(uid, name, email, gender, accessToken);

			return retUser;
		}

	}
}



#endif