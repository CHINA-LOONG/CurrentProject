using UnityEngine;
using System.Collections;

namespace Funplus {

	public enum FunplusAccountStatus{
		FPAStateLoggedInUnknow                   = -1,
		/*! this state indicates that the user logged in with Qucik Play */
		FPAStateLoggedInExpress                  = 0,
		
		/*! this state indicates that the user logged in with Facebook(bound with Facebook) */
		FPAStateLoggedInFacebook                 = 1,
		
		/*! this state indicates that the user logged in with Email(bound with Email) */
		FPAStateLoggedInEmail                    = 2,
		
		/*! this state indicates that user has been logged out, game may
   need to reload the game and recall [FunplusAccount login] to get
   the user logged in*/
		FPAStateLogout                           = 3,
		
		/*! this state indicates that user did nothing in the account info
   UI, in this senario, the FPID will be nil */
		FPAStateNoChange                         = 4,
		
		/*! this state indicates that user did not logged in */
		FPAStateNotLoggedIn                      = 5,
		
		/*! this state indicates that error occured when trying to login or bind without UI */
		FPAStateError                            = 6,
		
		/*! this state indicates that the user logged in with Email(bound with Wechat) */
		FPAStateLoggedInWechat                   = 7,
		
		/*! this state indicates that the user logged in with Vk(bound with Vk) */
		FPAStateLoggedInVk                       = 8,
		
		/*! this state indicates that the user logged in with GameCenter(bound with GameCenter) */
		FPAStateLoggedInGameCenter               = 9,
	};
}

