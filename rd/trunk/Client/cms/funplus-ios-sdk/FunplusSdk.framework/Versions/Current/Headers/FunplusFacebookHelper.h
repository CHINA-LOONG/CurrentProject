//
//  FunplusFacebookHelper.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/6/8.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "FunplusFacebookUser.h"
#import "FunplusError.h"

@interface FunplusFacebookHelper : NSObject

typedef void (^FPFacebookShareResultHandler)(FunplusError* error);

typedef void (^FPFacebookUserDataHandler)(FunplusError* error, FunplusFacebookUser * user);

typedef void (^FPFacebookAskPermissionResultHandler)(FunplusError* error);

// friends is an array of FunplusFacebookUser
typedef void (^FPFacebookFriendsDataHandler)(FunplusError* error, NSArray * friends);

typedef void (^FPFacebookRequestResultHandler)(FunplusError* error);

typedef void (^FPFacebookPublishOpenGraphResultHandler)(FunplusError* error);

+ (instancetype) sharedInstance;

- (NSString *) getAccessToken;

- (void) getUserData:(FPFacebookUserDataHandler)handler;

- (void) getGameFriends:(FPFacebookFriendsDataHandler)handler;

/*
 * check if the user granted the app "userfriends" permission
 */
- (BOOL) hasFriendsPermission;

- (BOOL) hasPublishPermission;

/*
 * re-ask the user for granting the app "userfriends" permission
 * this will jump to facebook app/facebook web
 */
- (void) askFriendsPermission:(FPFacebookAskPermissionResultHandler)handler;

- (void) askPublishPermission:(FPFacebookAskPermissionResultHandler)handler;

- (void) shareWithTitle:(NSString *)title
                message:(NSString *)message
                   link:(NSString *)link
                    pic:(NSString *)picUrl
                handler:(FPFacebookShareResultHandler)handler;

- (void) shareWithImage:(NSString *)imgPath
                message:(NSString *)message
                   link:(NSString *)link
                handler:(FPFacebookShareResultHandler)handler;

- (void) sendRequestWithPlatformId:(NSString *)platformId
                           message:(NSString *)message
                           handler:(FPFacebookRequestResultHandler)handler;

- (void) publishOpenGraphWithNamespace:(NSString *)ogNamespace
                                action:(NSString *)ogAction
                                object:(NSString *)ogObject
                                 title:(NSString *)title
                               message:(NSString *)message
                                  link:(NSString *)link
                                   pic:(NSString *)picUrl
                                  flag:(BOOL)useApiOnly
                               handler:(FPFacebookPublishOpenGraphResultHandler)handler;
@end
