//
//  FunAccount4Unity.mm
//  Unity-iPhone
//
//  Created by Lei Gu on 15/6/8.
//
//

#include <stdio.h>
#include <string>

#import <FunplusSdk/FunplusSdk.h>
#import <FunplusSdk/FunplusAccount.h>

using namespace std;

static string g_fp_fun_account_obj_name = "";

NSString* processRetStr(FunplusAccountStatus aState, FunplusError* aError, NSString *aFpid, NSString * aSessionKey)
{
    NSString* retStr = nil;
    
    NSString* fpid = aFpid.length ? aFpid : @"";
    NSString* sessionKey = aFpid.length ? aSessionKey : @"";

    NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
    NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
    NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
    
    NSError *error = nil;
    NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: [[NSNumber numberWithInt: aState] stringValue],@"state", errorCode, @"errorcode",errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg", fpid, @"fpid", sessionKey, @"sessionkey", nil];
    
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&error];
    if([jsonData length] > 0 && error == nil)
    {
        retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        
    }
    return retStr;
}

void onLoginFinished(FunplusAccountStatus state, FunplusError* errorCode, NSString *fpid, NSString *sessionKey)
{
    NSString* retStr = processRetStr(state, errorCode, fpid, sessionKey);
    if(retStr && [retStr isKindOfClass:[NSString class]])
    {
        UnitySendMessage(g_fp_fun_account_obj_name.c_str(), "onFunSdkLoginFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

void onShowUserCenterFinished(FunplusAccountStatus aState, FunplusError* aError, NSString *aFpid, NSString *aSessionKey)
{
    NSString* retStr = processRetStr(aState, aError, aFpid, aSessionKey);
    if(retStr && [retStr isKindOfClass:[NSString class]])
    {
        UnitySendMessage(g_fp_fun_account_obj_name.c_str(), "onFunSdkShowUserCenterFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

void onBindAccountFinished(FunplusAccountStatus state, FunplusError* errorCode, NSString *fpid, NSString *sessionKey)
{
    NSString* retStr = processRetStr(state, errorCode, fpid, sessionKey);
    if(retStr && [retStr isKindOfClass:[NSString class]])
    {
        UnitySendMessage(g_fp_fun_account_obj_name.c_str(), "onFunSdkBindAccountFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

void onLogoutFinished(FunplusAccountStatus state, FunplusError* errorCode, NSString *fpid, NSString *sessionKey)
{
    NSString* retStr = processRetStr(state, errorCode, fpid, sessionKey);
    if(retStr && [retStr isKindOfClass:[NSString class]])
    {
        UnitySendMessage(g_fp_fun_account_obj_name.c_str(), "onFunSdkLogoutFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

extern "C" {
    void obj_fp_fun_account_setObjName(const char* aName)
    {
        if(g_fp_fun_account_obj_name != aName)
        {
            g_fp_fun_account_obj_name = aName;
        }
    }
    
    void obj_fp_account_login_withUI()
    {
        NSLog(@"funsdk, obj_fp_account_login_withUI called");
        [[FunplusAccount sharedInstance] login:^(FunplusAccountStatus state, FunplusError *errorCode, NSString *fpid, NSString *sessionKey) {
            NSLog(@"********** login callback");
            NSLog(@"Login state: %d", state);
            NSLog(@"Error Code: %d", errorCode.code);
            NSLog(@"Funplus ID: %@", fpid);
            NSLog(@"Session Key: %@", sessionKey);
            
            onLoginFinished(state, errorCode, fpid, sessionKey);
        }];
    }

    void obj_fp_account_login(FunplusAccountType aType)
    {
        NSLog(@"funsdk, obj_fp_account_login called");
        
        [[FunplusAccount sharedInstance] login:aType callback:^(FunplusAccountStatus state, FunplusError *errorCode, NSString *fpid, NSString *sessionKey) {
            NSLog(@"********** login callback");
            
            NSLog(@"Login state: %d", state);
            NSLog(@"Error Code: %d", errorCode.code);
            NSLog(@"Funplus ID: %@", fpid);
            NSLog(@"Session Key: %@", sessionKey);
            onLoginFinished(state, errorCode, fpid, sessionKey);
        }];
    }
    
    void obj_fp_account_show_user_center()
    {
        NSLog(@"funsdk, obj_fp_account_show_user_center_view called");
        
        [[FunplusAccount sharedInstance] showUserCenter:^(FunplusAccountStatus state, FunplusError *errorCode, NSString *fpid, NSString *sessionKey) {
            NSLog(@"********** showUserCenter callback");
            
            NSLog(@"Login state: %d", state);
            NSLog(@"Error Code: %d", errorCode.code);
            NSLog(@"Funplus ID: %@", fpid);
            NSLog(@"Session Key: %@", sessionKey);
            
            onShowUserCenterFinished(state, errorCode, fpid, sessionKey);
        }];
    }
    
    void obj_fp_account_logout()
    {
        NSLog(@"funsdk, obj_fp_account_logout called");
        
        [[FunplusAccount sharedInstance] logout:^(FunplusAccountStatus state, FunplusError *errorCode, NSString *fpid, NSString *sessionKey) {
            NSLog(@"********** logout callback");
            
            NSLog(@"Login state: %d", state);
            NSLog(@"Error Code: %d", errorCode.code);
            NSLog(@"Funplus ID: %@", fpid);
            NSLog(@"Session Key: %@", sessionKey);
            onLogoutFinished(state, errorCode, fpid, sessionKey);
        }];
    }
    
    void obj_fp_account_get_available_account_types()
    {
        NSLog(@"fun account, getAvailableAccountTypes called");
        [[FunplusAccount sharedInstance] getAvailableAccountTypes:^(FunplusError *aError, NSArray *aAavailableTypes) {
            NSLog(@"********** getAvailableAccountTypes callback");
            
            NSString* retStr = nil;
            NSError* error = nil;
            
            NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
            NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
            NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
            
            NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: errorCode, @"errorcode", aAavailableTypes,@"types", errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg", nil];
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&error];
            
            if([jsonData length] > 0 && error == nil)
            {
                retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            }
            
            UnitySendMessage(g_fp_fun_account_obj_name.c_str(), "onFunSdkGetAvailableAccountTypesFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
    }
    
    void obj_fp_account_get_login_status()
    {
        NSLog(@"fun account, obj_fp_sdk_get_login_state called");
        [[FunplusAccount sharedInstance] getLoginStatus:^(FunplusError *error, FunplusAccountStatus state) {
            NSLog(@"********** Get Login Status callback");
            UnitySendMessage(g_fp_fun_account_obj_name.c_str(), "onFunSdkGetLoginStatusFinished", [[NSString stringWithFormat:@"%d", state] cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
    }
    
    void obj_fp_account_bind_account(FunplusAccountType aType)
    {
        NSLog(@"fun account, obj_fp_sdk_bind_account called");
        
        [[FunplusAccount sharedInstance] bind: aType callback:^(FunplusAccountStatus state, FunplusError *errorCode, NSString *fpid, NSString *sessionKey) {
                NSLog(@"********** bind account");
                NSLog(@"Login state: %d", state);
                NSLog(@"Error Code: %d", errorCode.code);
                NSLog(@"Funplus ID: %@", fpid);
                NSLog(@"Session Key: %@", sessionKey);
                
                onBindAccountFinished(state, errorCode, fpid, sessionKey);
        }];
  
    }
    
}