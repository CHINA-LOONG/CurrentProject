//
//  FunplusErrorCode.h
//  FunplusAccount
//
//  Created by yu.zhang on 15/3/24.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#ifndef FunplusAccount_FunplusErrorCode_h
#define FunplusAccount_FunplusErrorCode_h

typedef enum{
    ErrorNone                             = 0,
    ErrorUnspecific                       = 9999,
    
    FPErrorNotInstall                       = 100, // Not Install
    FPErrorFailedToConnectToConfigServer    = 101, // NetWork 错误
    FPErrorInvalidConfigRequestData         = 102, //
    FPErrorFailedToParseConfig              = 103, // 指从config服务器拿到的config无法正常解析
    FPErrorInvliadConfigData                = 104, //
    
    // server error codes, could only show user the errorCode if occurs
    FPErrorWrongSignature                   = 1101, //Wrong signature
    FPErrorFailedCreateID                   = 1102, //There was a server error when creating your Funplus ID, please try again later
    FPErrorWrongGameID                      = 1103, //Incorrect game ID
    FPErrorWrongEmail                       = 1104, //Please input the correct email address
    FPErrorWrongPassword                    = 1105, //Password format is wrong
    FPErrorWrongGUID                        = 1106, //Incorrect GUID
    FPErrorEmailExisted                     = 1107, //This account already exists, please input an unique email address
    FPErrorFailedCreateAccount              = 1108, //There was a server error when creating your account, please try again later
    FPErrorAccountBound                     = 1109, //This Funplus ID has already been bound
    FPErrorEmailNotFound                    = 1110, //There is no account with this email address
    FPErrorLoginFailed                      = 1111, //Your email or password is incorrect
    FPErrorPasswordLength                   = 1112, //Password must be 6~20 characters
    FPErrorFailedResetPassword              = 1113, //Failed to reset password
    FPErrorWrongResetPasswordToken          = 1114, //Failed to reset password
    FPErrorEmptyResetPasswordToken          = 1115, //Failed to reset password
    FPErrorPasswordNotChanged               = 1116, //The new password cannot be the same as the old password
    FPErrorFailedChangePassword             = 1117, //Failed to change password
    FPErrorFailedAccountNotFound            = 1118, //There is no account with this email address
    FPErrorFailedResetPasswordEmail         = 1119, //
    FPErrorFailedWrongGameSetting           = 1120,
    FPErrorFailedResetPasswordTemp          = 1121,
    FPErrorEmptyFPID                        = 1122,
    FPErrorWrongPlatformId                  = 1123, //Facebook ID is not correct
    FPErrorFailedToBindWithSocialID         = 1124, //failed to bind with facebook
    FPErrorReachLoginLimit                  = 1125, //Reach register limit
    FPErrorEmailTooLong                     = 1126, //Email address is too long
    
    // session related, could only show user the errorCode if occurs
    FPErrorEmptySession                     = 1127,
    FPErrorSessionNotFound                  = 1128,
    FPErrorSessionExpired                   = 1129,
    FPErrorSessionAppMismatch               = 1130,
    FPErrorWrongPlatformToken               = 1131,
    FPErrorWrongPlatformType                = 1132,
    FPErrorInvalidPlatformToken             = 1133,
    
    // 帐号-客户端错误（12xx）
    FPErrorUnknownAccountType               = 1200,
    FPErrorNotLoggedIn                      = 1201,
    FPErrorAlreadyLoggedIn                  = 1202, //need to modify
    FPErrorIoginInProgress                  = 1203, //need to modify
    FPErrorWrongLoginParams                 = 1204, //option
//    FPErrorFailedLoginAccount               = 1205,
//    FPErrorAccountBindedToOther             = 1206,
    FPErrorFailedToConnectServer            = 1207, //change name
    FPErrorFailedToParseResponse            = 1208, //change name
    
    // 社交平台错误（2xxx）
    
    // 微信错误（20xx）
    FPErrorWechatClientIssue                = 2000, //Wechat not install or version issue
    FPErrorWechatUserCancel                 = 2001, //User cancel or deny auth via Wechat
    FPErrorWechatLoginFailed                = 2002, //Login failed via Wecha
    FPErrorWechatDiffId                     = 2003, //Different wechat account
    FPErrorWechatNotInWhiteList             = 2004, //Only for andriod
    FPErrorWechatAccountBoundToOther        = 2005, //Wechat id has bind to another fpid
    
    // Facebook错误（21xx）
    //FPErrorFacebookNotLogin                 = 2100,
    FPErrorFacebookLoginFailed              = 2101,
    FPErrorFacebookGetUserDataFailed        = 2102,
    //FPErrorFacebookSendRequestError         = 2103,
    FPErrorFacebookUserCanceledAction       = 2104,
    FPErrorFacebookException                = 2105,
    FPErrorFacebookAccountBoundToOther      = 2106,
    FPErrorFacebookGetFriendsDataFailed     = 2107,
    FPErrorFacebookNeedUserFriendsPermission = 2108,
    FPErrorFacebookNotInTestGroup           = 2109, //user not in test group, forbid to play
    FPErrorFacebookAntherOpenGraphPublishingIsOnGoing = 2110,

    // VK错误（22xx）
    //FPErrorVkNotLoggedIn                    = 2200,
    FPErrorVkLoginFailed                    = 2201,
    FPErrorVkCaptchaError                   = 2202,
    FPErrorVkAccessDenied                   = 2203,
    FPErrorVkAcceptUserTokenFailed          = 2204,
    FPErrorVkReceiveNewTokenFailed          = 2205,
    FPErrorVkRenewTokenFailed               = 2206,
    FPErrorVkException                      = 2207,
    FPErrorVkUserCanceledAction             = 2208,
    FPErrorVkGetUserDataFailed              = 2209,
    FPErrorVkAccountBoundToOther            = 2210,
    FPErrorVkGetFriendsDataFailed           = 2211,
    
    //GameCenter错误(24xx)
    FPErrorGameCenterAccountBoundToOther    = 2410,
    
    // Funplus支付服务器错误（30xx）
    FPErrorFailedToConnectToPaymentServer   = 3000, //用户取消购买或者没有获取到receipt
    FPErrorInvalidPaymentRequestData        = 3001, //Funplus Payment 服务器错误
    FPErrorFailedToParsePaymentResponse     = 3002,
    FPErrorPaymentResponseError             = 3003,
    
    // Google IAB错误（31xx）
    FPErrorInitGoogleIab                    = 3100, //Only for andriod
    FPErrorGoogleIabFailedToBuy             = 3101, //Only for andriod
    FPErrorGoogleAccountNotConfigured       = 3102, //Only for andriod
    FPErrorGoogleIabFailedToConsume         = 3103, //Only for andriod
    
    
    // Apple IAP错误（32xx）
    FPErrorPurchaseFailed                   = 3200,
    FPErrorProductsInfoInvalid              = 3201,
    FPErrorBuyProductIdInvalid              = 3202,
    FPErrorNoLocalReceipt                   = 3203,
} FunplusErrorCode;

#endif
