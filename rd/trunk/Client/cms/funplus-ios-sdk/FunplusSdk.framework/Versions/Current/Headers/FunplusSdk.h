//
//  FunplusSdk.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/5/13.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//  Version: v3.0.8

#import <Foundation/Foundation.h>
#import "FunplusDefine.h"
#import "FunplusObject.h"

@class FunplusError;

typedef void (^FunplusSdkCallback)(FunplusError* error);

@interface FunplusSdk : FunplusObject

+ (instancetype) sharedInstance;

- (void) install:(NSString*) gameId gameKey:(NSString*) gameKey callback:(FunplusSdkCallback) callback;

- (void) install:(NSString*) gameId gameKey:(NSString*) gameKey environment:(NSString*)environment callback:(FunplusSdkCallback) callback;

- (void) install:(NSString*) gameId gameKey:(NSString*) gameKey isProduct:(BOOL) isProduct callback:(FunplusSdkCallback) callback;

- (BOOL) isSdkInstalled;

- (void) setDebug:(BOOL)isDebugMode;

- (void) setLogLevel:(FunplusLogLevel)logLevel;

- (void) registerPushNotification:(NSDictionary *)launchOptions;

- (void) handleRemoteNotification:(NSDictionary *)launchOptions;

- (void) registerDeviceToken:(NSData *) deviceToken;

- (void) appDidBecomeActive;

- (void) appDidEnterBackground;

- (void) appWillEnterForeground;

- (void) appWillTerminate;

- (void) appWillResignActive;

- (BOOL) appDidOpenURL:(NSURL *)url sourceApp:(NSString *)app;

- (void) setUserInfo:(NSDictionary*) userInfo;

- (NSString*) getVersion;

- (void) logUserLogin:(NSString*) uid;
- (void) logUserLogout;
- (void) logNewUser:(NSString*) uid;
- (void) logUserInfoUpdate:(NSDictionary*) userInfo;

@end

#import "FunplusError.h"
#import "FunplusAccount.h"

#import "FunplusVKHelper.h"
#import "FunplusVkUser.h"
#import "FunplusFacebookHelper.h"
#import "FunplusFacebookUser.h"
#import "FunplusWechatHelper.h"
#import "FunplusWechatUser.h"

#import "FunplusSdkUtils.h"

#import "FunplusMarketing.h"
#import "FunplusPayment.h"

#import "FunplusBi.h"
#import "FunplusRum.h"