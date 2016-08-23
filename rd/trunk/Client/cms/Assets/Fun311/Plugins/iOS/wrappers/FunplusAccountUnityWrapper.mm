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
                                 "OnGetAvailableAccountTypesError",
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
                                     "OnGetAvailableAccountTypesSuccess",
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
                NSString *loginMessage = [NSString stringWithFormat:@"{\"fpid\": \"%@\", \"session_key\": \"%@\"}", fpid, sessionKey];
                
                UnitySendMessage(gameObject,
                                 "OnLoginSuccess",
                                 [loginMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {                        // Login failed.
                isLoggedIn = false;
                message = @"{\"is_logged_in\": false}";
            }
            
            UnitySendMessage(gameObject,
                             "OnOpenSession",
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
                                 "OnLoginError",
                                 [errMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {                            // Login success.
                isLoggedIn = true;
                NSString *successMessage = [NSString stringWithFormat:@"{\"fpid\": \"%@\", \"session_key\": \"%@\"}", fpid, sessionKey];
                
                UnitySendMessage(gameObject,
                                 "OnLoginSuccess",
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
                                 "OnLoginError",
                                 [errMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {                            // Login success.
                isLoggedIn = true;
                NSString *successMessage = [NSString stringWithFormat:@"{\"fpid\": \"%@\", \"session_key\": \"%@\"}", fpid, sessionKey];
                
                UnitySendMessage(gameObject,
                                 "OnLoginSuccess",
                                 [successMessage cStringUsingEncoding:NSUTF8StringEncoding]);
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
                             "OnLogout",
                             "");
        }];
    }

    void com_funplus_sdk_account_showUserCenter() {
        NSLog(@"com_funplus_sdk_account_showUserCenter() called.");
        
        Class cls = NSClassFromString(@"FunplusAccount");
        [[cls sharedInstance] showUserCenter:^(FunplusAccountStatus status,
                                                          FunplusError *error,
                                                          NSString *fpid,
                                                          NSString *sessionKey) {
            
            if (status == FPAStatusLogout || status == FPAStatusNotLoggedIn) {        // Logout.
                UnitySendMessage(gameObject,
                                 "OnLogout",
                                 "");
            } else if (statusIsLoggedIn(status)) {  // Bind account.
                NSString *message = [NSString stringWithFormat:@"{\"fpid\": \"%@\", \"session_key\": \"%@\"}", fpid, sessionKey];
                UnitySendMessage(gameObject,
                                 "OnBindAccountSuccess",
                                 [message cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {                                // Close user center.
                UnitySendMessage(gameObject,
                                 "OnCloseUserCenter",
                                 "");
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
                                     callback:^(FunplusAccountStatus state,
                                                FunplusError *error,
                                                NSString *fpid,
                                                NSString *sessionKey) {
            if (error.code != ErrorNone) {  // Bind account failed.
                UnitySendMessage(gameObject,
                                 "OnBindAccountError",
                                 [[error toJSONString] cStringUsingEncoding:NSUTF8StringEncoding]);
            } else {                        // Bind account success.
                NSString *message = [NSString stringWithFormat:@"{\"fpid\": \"%@\", \"session_key\": \"%@\"}", fpid, sessionKey];
                UnitySendMessage(gameObject,
                                 "OnBindAccountSuccess",
                                 [message cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }

}
