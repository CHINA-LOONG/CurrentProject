using UnityEngine;
using System.Collections;

namespace Funplus {
	public enum FunplusAccountType{
		/*! this state indicates a unkown account type, it means error was occurred*/
		FPAccountTypeUnknow                            = -1,
		
		/*! this state indicates express login without showing any permission dialog*/
		FPAccountTypeExpress                           = 0,
		
		/*! this state indicates login with facebook, may showing facebook permission
         dialog or redirect the user to mobile safari */
		FPAccountTypeFacebook                          = 1,
		
		/*! this state indicates login with email without showing any permission dialog*/
		FPAccountTypeEmail                             = 2,
		
		/*! this state indicates login with wechat, may redirect the user to wechat for authorize*/
		FPAccountTypeWechat                            = 3,
		
		/*! this state indicates login with VK, may showing VK permission dialog or redirect the user to mobile safari */
		FPAccountTypeVK                                = 4,
		
		/*! this state indicates login with GameCenter, may showing GameCenter permission dialog */
		
		FPAccountTypeGameCenter                        = 5,
		/*! this state indicates login with UI, may showing Funplus UI permission dialog */
		FPAccountTypeNotSpecified                      = 6,
	};
}
