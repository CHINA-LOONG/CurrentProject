//
//  FunplusSdk.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/5/13.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//  Version: v3.0.17

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "FunplusDefine.h"
#import "FunplusObject.h"

@class FunplusError;

typedef void (^FunplusSdkCallback)(FunplusError* _Nonnull  error);

@interface FunplusSdk : FunplusObject

+ (nonnull instancetype) sharedInstance;

- (void) install:(nonnull NSString*) gameId gameKey:(nonnull NSString*) gameKey callback:(nullable FunplusSdkCallback) callback;

- (void) install:(nonnull NSString*) gameId gameKey:(nonnull NSString*) gameKey environment:(nonnull NSString*)environment callback:(nullable FunplusSdkCallback) callback;

- (void) install:(nonnull NSString*) gameId gameKey:(nonnull NSString*) gameKey isProduct:(BOOL) isProduct callback:(nullable FunplusSdkCallback) callback __deprecated;

- (BOOL) isSdkInstalled;

- (void) setDebug:(BOOL) isDebugMode;

- (void) setLogLevel:(FunplusLogLevel) logLevel;

- (void) application:(nonnull UIApplication*) application didFinishLaunchingWithOptions:(nullable NSDictionary*)launchOptions;

- (BOOL) appDidOpenURL:(nonnull NSURL*) url application:(nullable UIApplication*) application sourceApp:(nullable NSString*)app annotation:(nullable id)annotation;

- (void) appDidBecomeActive;

- (void) appDidEnterBackground;

- (void) appWillEnterForeground;

- (void) appWillTerminate;

- (void) appWillResignActive;

- (void) registerPushNotification:(NSDictionary*) launchOptions;

- (void) handleRemoteNotification:(nonnull NSDictionary*) userInfo;

- (void) registerDeviceToken:(nonnull NSData*) deviceToken;

- (nonnull NSString*) getVersion;

- (void) logUserLogin:(nonnull NSString*) uid;

- (void) logUserLogout;

- (void) logNewUser:(nonnull NSString*) uid;

- (void) logUserInfoUpdate:(nonnull NSDictionary*) userInfo;

- (void) setUserInfo:(nonnull NSDictionary*) userInfo __deprecated;

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

#import "FunplusHelpShift.h"
