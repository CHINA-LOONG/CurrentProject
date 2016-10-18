//
//  FunplusDefine.h
//  FunplusAccount
//
//  Created by yu.zhang on 15/3/24.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#ifndef FunplusAccount_FunplusDefine_h
#define FunplusAccount_FunplusDefine_h
#include "FunplusErrorCode.h"

typedef enum {
    FPAStatusLoggedInUnknow                   = -1,
    /*! this state indicates that the user logged in with Qucik Play */
    FPAStatusLoggedInExpress                  = 0,
    
    /*! this state indicates that the user logged in with Facebook(bound with Facebook) */
    FPAStatusLoggedInFacebook                 = 1,
    
    /*! this state indicates that the user logged in with Email(bound with Email) */
    FPAStatusLoggedInEmail                    = 2,
    
    /*! this state indicates that user has been logged out, game may
     need to reload the game and recall [FunplusAccount login] to get
     the user logged in*/
    FPAStatusLogout                           = 3,
    
    /*! this state indicates that user did nothing in the account info
     UI, in this senario, the FPID will be nil */
    FPAStatusNoChange                         = 4,
    
    /*! this state indicates that user did not logged in */
    FPAStatusNotLoggedIn                      = 5,
    
    /*! this state indicates that error occured when trying to login or bind without UI */
    FPAStatusError                            = 6,
    
    /*! this state indicates that the user logged in with Email(bound with Wechat) */
    FPAStatusLoggedInWechat                   = 7,
    
    /*! this state indicates that the user logged in with Vk(bound with Vk) */
    FPAStatusLoggedInVk                       = 8,
    
    /*! this state indicates that the user logged in with GameCenter(bound with GameCenter) */
    FPAStatusLoggedInGameCenter               = 9,
} FunplusAccountStatus;

/*!
 @typedef FPALoginType enum
 
 @abstract login types, eg. express login, facebook login
 
 @discussion
 */
typedef enum {
    /*! this state indicates a unkown account type, it means error was occurred*/
    FPATypeUnknow                            = -1,
    
    /*! this state indicates express login without showing any permission dialog*/
    FPATypeExpress                           = 0,
    
    /*! this state indicates login with email without showing any permission dialog*/
    FPATypeEmail                             = 1,
    
    /*! this state indicates login with facebook, may showing facebook permission
     dialog or redirect the user to mobile safari */
    FPATypeFacebook                          = 2,
    
    /*! this state indicates login with VK, may showing VK permission dialog or redirect the user to mobile safari */
    FPATypeVK                                = 3,
    
    /*! this state indicates login with wechat, may redirect the user to wechat for authorize*/
    FPATypeWechat                            = 4,
    
    /*! this state indicates login with GameCenter, may showing GameCenter permission dialog */
    
    FPATypeGameCenter                        = 6,
    
} FunplusAccountType;

typedef enum {
    FPLogLevelVerbose = 1,
    FPLogLevelDebug   = 2,
    FPLogLevelInfo    = 3,
    FPLogLevelWarn    = 4,
    FPLogLevelError   = 5,
    FPLogLevelAssert  = 6
} FunplusLogLevel;

typedef void (^FPStatusCallback)(FunplusAccountStatus status);

typedef enum {
    FPUserCenterClose = 0,
    FPUserCenterLogout = 1,
    FPUserCenterAccountBind = 2,
    FPUserCenterSwitchAccount = 3,
    FPUserCenterAccountLogin = 4,
    FPUserCenterOtherOperation = 5,
} FunplusOperation;
#endif
