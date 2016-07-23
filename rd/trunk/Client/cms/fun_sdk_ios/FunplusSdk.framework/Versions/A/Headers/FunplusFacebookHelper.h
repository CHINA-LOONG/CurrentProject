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

- (void) share:(NSString *)title
   withMessage:(NSString *)message
      withLink:(NSString *)link
       withPic:(NSString *)picUrl
   withHandler:(FPFacebookShareResultHandler)handler;

- (void) shareImage:(NSString *)imgPath
        withMessage:(NSString *)message
           withLink:(NSString *)link
        withHandler:(FPFacebookShareResultHandler)handler;

- (void) sendRequest:(NSString *)platformId
         withMessage:(NSString *)message
         withHandler:(FPFacebookRequestResultHandler)handler;

- (void) publishOpenGraph:(NSString *)ogNamespace
               withAction:(NSString *)ogAction
               withObject:(NSString *)ogObject
                withTitle:(NSString *)title
              withMessage:(NSString *)message
                 withLink:(NSString *)link
                  withPic:(NSString *)picUrl
                 withFlag:(BOOL)useApiOnly
              withHandler:(FPFacebookPublishOpenGraphResultHandler)handler;


@end
