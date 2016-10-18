//
//  FunplusSdkUnityWrapper.mm
//
//  Created by 张远坤 on 11/11/15.
//  Copyright © 2015 Funplus Inc. All rights reserved.
//

#include <string.h>
#include <stdlib.h>
#import <FunplusSdk/FunplusSdk.h>

extern void UnitySendMessage(const char *, const char *, const char *);

// Game object name no more than 255 characters.
static char gameObject[256];

static void onSdkInstallFinished(FunplusError *error) {
    if (error.code == ErrorNone) {
        UnitySendMessage(gameObject, "OnFunplusSdkInstallSuccess", "");
    } else {
        UnitySendMessage(gameObject, "OnFunplusSdkInstallError", [[error toJSONString] cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

static char *reteinStr(const char *str) {
    if (str == NULL) {
        return NULL;
    }

    char *ret = (char *)malloc(strlen(str) + 1);
    strcpy(ret, str);
    return ret;
}


extern "C" {

    void com_funplus_sdk_setGameObject(const char *gameObjectName) {
        if (gameObjectName == NULL) {
            NSLog(@"Error: com_funplus_sdk_setGameObject: gameObjectName must not be empty.");
            return;
        }
        strcpy(gameObject, gameObjectName);
    }

    void com_funplus_sdk_install(const char *gameId,
                                 const char *gameKey,
                                 int isProduction) {
        NSLog(@"com_funplus_sdk_install() called.");
        
        [[FunplusSdk sharedInstance] install:[NSString stringWithUTF8String:gameId]
                                     gameKey:[NSString stringWithUTF8String:gameKey]
                                   isProduct:isProduction
                                    callback:^(FunplusError *error) {
            onSdkInstallFinished(error);
         }];
    }

    void com_funplus_sdk_setDebug(int isDebug) {
        [[FunplusSdk sharedInstance] setDebug:isDebug];
    }

    int com_funplus_sdk_isSdkInstalled() {
        return [[FunplusSdk sharedInstance] isSdkInstalled];
    }
    
    int com_funplus_sdk_isFirstLaunch() {
        // TODO
        return false;
    }

    char * com_funplus_sdk_getSdkVersion() {
        NSString *version = [[FunplusSdk sharedInstance] getVersion];
        return reteinStr([version cStringUsingEncoding:NSUTF8StringEncoding]);
    }

    void com_funplus_sdk_logUserLogin(const char *uid) {
        [[FunplusSdk sharedInstance] logUserLogin:[NSString stringWithUTF8String:uid]];
    }

    void com_funplus_sdk_logNewUser(const char *uid) {
        [[FunplusSdk sharedInstance] logNewUser:[NSString stringWithUTF8String:uid]];
    }

    void com_funplus_sdk_logUserLogout() {
        [[FunplusSdk sharedInstance] logUserLogout];
    }

    void com_funplus_sdk_logUserInfoUpdate(const char *serverId,
                                           const char *userId,
                                           const char *userName,
                                           const char *userLevel,
                                           const char *userVipLevel,
                                           int isPaidUser) {
        NSString *nsServerId = [NSString stringWithUTF8String:serverId];
        NSString *nsUserId = [NSString stringWithUTF8String:userId];
        NSString *nsUserName = [NSString stringWithUTF8String:userName];
        NSString *nsUserLevel = [NSString stringWithUTF8String:userLevel];
        NSString *nsUserVipLevel = [NSString stringWithUTF8String:userVipLevel];
        NSString *nsIsPaidUser = [NSString stringWithFormat:@"%d", isPaidUser];
        
        NSDictionary *userInfo = [NSDictionary dictionaryWithObjectsAndKeys:
                                  nsServerId, @"serverId",
                                  nsUserId, @"userId",
                                  nsUserName, @"userName",
                                  nsUserLevel, @"userLevel",
                                  nsUserVipLevel, @"userVipLevel",
                                  nsIsPaidUser, @"isPaidUser", nil];
        [[FunplusSdk sharedInstance] logUserInfoUpdate:userInfo];
    }

    void com_funplus_sdk_logPayment(const char *sku,
                                    const char *throughCargo,
                                    const char *purchaseData) {
        // TODO
    }

};
