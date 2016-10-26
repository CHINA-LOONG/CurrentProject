//
//  FunplusAccountUnityWrapper.mm
//
//  Created by 张远坤 on 11/11/15.
//  Copyright © 2015 Funplus Inc. All rights reserved.
//

#include <string.h>
#import <FunplusSdk/FunplusSdk.h>

extern void UnitySendMessage(const char *, const char *, const char *);

// Game object name no more than 255 characters.
static char gameObject[256];
static BOOL isLoggedIn;

static BOOL statusIsLoggedIn(FunplusAccountStatus status) {
    switch (status) {
        case FPAStatusLoggedInUnknow:
        case FPAStatusLoggedInExpress:
        case FPAStatusLoggedInEmail:
        case FPAStatusLoggedInFacebook:
        case FPAStatusLoggedInVk:
        case FPAStatusLoggedInWechat:
        case FPAStatusLoggedInGameCenter:
            return true;
        default:
            return false;
    };
}

static id FPObjectOrNull(id object)
{
    return object ?: [NSNull null];
}

static NSString * FPGetUnitySessionJSONString(NSString * fpid, NSString * sessionKey, FunplusAccountStatus status) {
    NSString* accountType;
    NSString* snsPlatform;
    switch (status)
    {
        case FPAStatusLoggedInExpress:
            accountType = @"express";
            break;
        case FPAStatusLoggedInEmail:
            accountType = @"email";
            break;
            //case FunplusAccountStatus.FPAccountTypeMobile:
            //return "mobile";
        case FPAStatusLoggedInFacebook:
            accountType = @"facebook";
            snsPlatform = @"facebook";
            break;
        case FPAStatusLoggedInVk:
            accountType = @"vk";
            snsPlatform = @"vk";
            break;
        case FPAStatusLoggedInWechat:
            accountType = @"wechat";
            snsPlatform = @"wechat";
            break;
        case FPAStatusLoggedInGameCenter:
            accountType = @"gamecenter";
            snsPlatform = @"gamecenter";
            break;
        default:
            accountType = @"unknown";
            snsPlatform = nil;
    }

    NSString *email = [[FunplusAccount sharedInstance] getEmail];
    NSString *snsId = [[FunplusAccount sharedInstance] getPlatformId];
    NSDictionary * data = @{@"fpid": FPObjectOrNull(fpid),
                            @"session_key": FPObjectOrNull(sessionKey),
                            @"account_type": FPObjectOrNull(accountType),
                            @"email": FPObjectOrNull(email),
                            @"sns_platform": FPObjectOrNull(snsPlatform),
                            @"sns_id": FPObjectOrNull(snsId)};
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:data
                                                       options:NSJSONWritingPrettyPrinted
                                                         error:nil];
    
    NSString * message = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return message;
}

extern "C" {

    void com_funplus_sdk_account_setGameObject(const char *gameObjectName) {
        if (gameObjectName == NULL) {
            NSLog(@"Error: com_funplus_sdk_account_setGameObject: gameObjectName must not be empty.");
            return;
        }
        strcpy(gameObject, gameObjectName);
    }

    void com_funplus_sdk_account_getAvailableAccountTypes() {
        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] getAvailableAccountTypes:^(FunplusError *error,
                                                                    NSArray *accountTypes) {
            if (error.code != ErrorNone) {  // If some error occurred.
                UnitySendMessage(gameObject,
                                 "OnAccountGetAvailableAccountTypesError",
                                 [[error toJSONString] cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {                        // Otherwise, success.
                NSMutableDictionary *dict = [[NSMutableDictionary alloc] init];
                [dict setValue:accountTypes forKey:@"types"];

                NSError *jsonError;
                NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&jsonError];
                NSString *jsonStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

                if (jsonError != nil) {
                    NSLog(@"com_funplus_sdk_account_getAvailableAccountTypes() error: %@", [jsonError description]);
                } else {
                    UnitySendMessage(gameObject,
                                     "OnAccountGetAvailableAccountTypesSuccess",
                                     [jsonStr cStringUsingEncoding:NSUTF8StringEncoding]);
                }
            }
        }];
    }

    int com_funplus_sdk_account_isUserLoggedIn() {
//        Class cls = NSClassFromString(@"FunplusAccount");
//        return [[cls sharedInstance] isUserLoggedIn];
        return isLoggedIn;
    }

    // Note: in this function, if auto login failed, only `onOpenSession()` will be called.
    // If auto login success, both `onLoginSuccess()` an `onOpenSession()` will be called.
    void com_funplus_sdk_account_openSession() {
        NSLog(@"com_funplus_sdk_account_openSession() called.");

        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] getLoginStatus:^(FunplusError *error,
                                               FunplusAccountStatus status) {
            NSString *message;

            if (statusIsLoggedIn(status)) { // Login success.
                isLoggedIn = true;
                message = @"{\"is_logged_in\": true}";

                // Retrieve fpid & sessionKey.
                NSString *fpid = [[cls sharedInstance] getFpid];
                NSString *sessionKey = [[cls sharedInstance] getSessionKey];

                NSString * loginMessage = FPGetUnitySessionJSONString(fpid, sessionKey, status);
                UnitySendMessage(gameObject,
                                 "OnAccountLoginSuccess",
                                 [loginMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {                        // Login failed.
                isLoggedIn = false;
                message = @"{\"is_logged_in\": false}";
            }

            UnitySendMessage(gameObject,
                             "OnAccountOpenSession",
                             [message cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
    }

    void com_funplus_sdk_account_login() {
        NSLog(@"com_funplus_sdk_account_login() called.");

        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] login:^(FunplusAccountStatus status,
                                                 FunplusError *error,
                                                 NSString *fpid,
                                                 NSString *sessionKey) {
            if (!statusIsLoggedIn(status)) {    // Login failed.
                isLoggedIn = false;
                NSString *errMessage = error == nil ? @"Login failed: unknown error" : [error toJSONString];

                UnitySendMessage(gameObject,
                                 "OnAccountLoginError",
                                 [errMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {                            // Login success.
                isLoggedIn = true;
                NSString *successMessage = FPGetUnitySessionJSONString(fpid, sessionKey, status);
                UnitySendMessage(gameObject,
                                 "OnAccountLoginSuccess",
                                 [successMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }

    void com_funplus_sdk_account_loginWithType(int type) {
        NSLog(@"com_funplus_sdk_account_loginWithType() called.");

        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] login:(FunplusAccountType)type
                                      callback:^(FunplusAccountStatus status,
                                                 FunplusError *error,
                                                 NSString *fpid,
                                                 NSString *sessionKey) {
            if (!statusIsLoggedIn(status)) {    // Login failed.
                isLoggedIn = false;
                NSString *errMessage = error == nil ? @"Login failed: unknown error" : [error toJSONString];

                UnitySendMessage(gameObject,
                                 "OnAccountLoginError",
                                 [errMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {                            // Login success.
                isLoggedIn = true;
                NSString *successMessage = FPGetUnitySessionJSONString(fpid, sessionKey, status);

                UnitySendMessage(gameObject,
                                 "OnAccountLoginSuccess",
                                 [successMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }
    
    void com_funplus_sdk_account_loginWithEmail(const char* email, const char* password){
        NSLog(@"com_funplus_sdk_account_loginWithEmail() called.");
        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] loginWithEmail:[NSString stringWithUTF8String:email] password:[NSString stringWithUTF8String:password] callback:^(FunplusAccountStatus status, FunplusError *error, NSString *fpid, NSString *sessionKey) {
            
            if (!statusIsLoggedIn(status)) {
                isLoggedIn = false;
                NSString *errMessage = error == nil ? @"Login failed: unknown error" : [error toJSONString];
                
                UnitySendMessage(gameObject,
                                 "OnAccountLoginError",
                                 [errMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {
                isLoggedIn = true;
                NSString *successMessage = FPGetUnitySessionJSONString(fpid, sessionKey, status);
                
                UnitySendMessage(gameObject,
                                 "OnAccountLoginSuccess",
                                 [successMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }
    
    void com_funplus_sdk_account_registerWithEmail(const char* email, const char* password){
        NSLog(@"com_funplus_sdk_account_registerWithEmail() called.");
        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] registerWithEmail:[NSString stringWithUTF8String:email] password:[NSString stringWithUTF8String:password] callback:^(FunplusAccountStatus status, FunplusError *error, NSString *fpid, NSString *sessionKey) {
            
            if (!statusIsLoggedIn(status)) {
                isLoggedIn = false;
                NSString *errMessage = error == nil ? @"Register failed: unknown error" : [error toJSONString];
                
                UnitySendMessage(gameObject,
                                 "OnAccountLoginError",
                                 [errMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {
                isLoggedIn = true;
                NSString *successMessage = FPGetUnitySessionJSONString(fpid, sessionKey, status);
                
                UnitySendMessage(gameObject,
                                 "OnAccountLoginSuccess",
                                 [successMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }
    
    void com_funplus_sdk_account_resetPassword(const char* email){
        NSLog(@"com_funplus_sdk_account_resetPassword() called.");
        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] resetPassword:[NSString stringWithUTF8String:email] callback:^(NSString *fpid, FunplusError *error) {
            
            if (error.code == 0) {
                UnitySendMessage(gameObject,
                                 "OnAccountResetPasswordSuccess",
                                 [fpid cStringUsingEncoding:NSUTF8StringEncoding]);
            }else{
                UnitySendMessage(gameObject,"OnAccountResetPasswordError",[[error toJSONString] cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }

    void com_funplus_sdk_account_logout() {
        isLoggedIn = false;
        NSLog(@"com_funplus_sdk_account_logout() called.");

        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] logout:^(FunplusAccountStatus state,
                                                  FunplusError *error,
                                                  NSString *fpid,
                                                  NSString *sessionKey) {
            UnitySendMessage(gameObject,
                             "OnAccountLogout",
                             "");
        }];
    }

    void com_funplus_sdk_account_showUserCenter() {
        NSLog(@"com_funplus_sdk_account_showUserCenter() called.");
        
        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] showUserCenter:^(FunplusAccountStatus status,
                                               FunplusError *error,
                                               NSString *fpid,
                                               NSString *sessionKey,
                                               FunplusOperation userOperation
                                               ) {
            if (userOperation == FPUserCenterClose) {
                UnitySendMessage(gameObject,
                                 "OnAccountCloseUserCenter",
                                 "");
            }
            
            if (userOperation == FPUserCenterLogout) {
                
                UnitySendMessage(gameObject,
                                 "OnAccountCloseUserCenter",
                                 "");
                
                UnitySendMessage(gameObject,
                                 "OnAccountLogout",
                                 "logout");
            }
            
            if (userOperation == FPUserCenterAccountBind) {
                UnitySendMessage(gameObject,
                                 "OnAccountCloseUserCenter",
                                 "");
                if (statusIsLoggedIn(status)) {
                    NSString *message = FPGetUnitySessionJSONString(fpid, sessionKey, status);
                    UnitySendMessage(gameObject,
                                     "OnAccountBindAccountSuccess",
                                     [message cStringUsingEncoding:NSUTF8StringEncoding]);
                } else {
                    UnitySendMessage(gameObject,
                                     "OnAccountBindAccountError",
                                     "Error");
                }
            }
            
            if (userOperation == FPUserCenterSwitchAccount) {
                if (statusIsLoggedIn(status)) {
                    NSString *message = FPGetUnitySessionJSONString(fpid, sessionKey, status);
                    UnitySendMessage(gameObject,
                                     "OnAccountLoginSuccess",
                                     [message cStringUsingEncoding:NSUTF8StringEncoding]);
                } else {
                    UnitySendMessage(gameObject,
                                     "OnAccountLoginError",
                                     "Error");
                }
            }
        }];
    }

    void com_funplus_sdk_account_bindAccount() {
        NSLog(@"com_funplus_sdk_account_bindAccount() called.");
        // Not implemented.
    }

    void com_funplus_sdk_account_bindAccountWithType(int type) {
        NSLog(@"com_funplus_sdk_account_bindAccountWithType() called.");
        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] bind:(FunplusAccountType)type
                                     callback:^(FunplusAccountStatus status,
                                                FunplusError *error,
                                                NSString *fpid,
                                                NSString *sessionKey) {
            if (error.code != ErrorNone) {  // Bind account failed.
                UnitySendMessage(gameObject,
                                 "OnAccountBindAccountError",
                                 [[error toJSONString] cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {                        // Bind account success.
                NSString *message = FPGetUnitySessionJSONString(fpid, sessionKey, status);
                UnitySendMessage(gameObject,
                                 "OnAccountBindAccountSuccess",
                                 [message cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }

}
