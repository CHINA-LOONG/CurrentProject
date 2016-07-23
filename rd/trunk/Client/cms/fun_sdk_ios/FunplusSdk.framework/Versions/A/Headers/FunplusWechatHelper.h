//
//  FunplusWechatHelper.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/6/8.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "FunplusWechatUser.h"
#import "FunplusError.h"

@interface FunplusWechatHelper : NSObject

typedef void (^FPWechatGameFriendsHandler)(FunplusError* error, NSArray * friends);
typedef void (^FPWechatShareResultHandler)(FunplusError* error);
typedef void (^FPWechatUserDataHandler)(FunplusError* error, FunplusWechatUser* user);

+ (instancetype) sharedInstance;

- (NSString *) getAccessToken;

- (void) getUserData: (FPWechatUserDataHandler)handler;

- (void) getGameFriends: (FPWechatGameFriendsHandler)handler;

/**
 * @Param jumpType
 *   Should be the following value:
 *   @"WECHAT_SNS_JUMP_SHOWRANK"       Jump tp Ranking page
 *   @"WECHAT_SNS_JUMP_URL"            Jump to URL
 *   @"WECHAT_SNS_JUMP_APP"            Jump to APP.
 *                                           If no App Installed,
 *                                           will jump to App introduce page.
 * @Param messageExt
 *   When share finished, app will wake up.
 *   messageExt will pass the callback function.
 *
 */
- (void) share:(NSString *)title
   withMessage:(NSString *)message
       withPic:(NSString *)picUrl
withMessageExt:(NSString *)messageExt
  withJumpType:(NSString *)jumpType
   withHandler:(FPWechatShareResultHandler)handler;

- (void) shareImage:(NSString *)imgPath
        withMessage:(NSString *)message
       withJumpType:(NSString *)jumpType
        withHandler:(FPWechatShareResultHandler)handler;

/**
 * @Param messageExt
 *   When share finished, app will wake up.
 *   messageExt will pass the callback function.
 *
 */
- (void) sendRequest:(NSString *)platformId
           withTitle:(NSString *)title
         withMessage:(NSString *)message
       withImagePath:(NSString *)imgPath
         withMediaId:(NSString *)mediaId
      withMessageExt:(NSString *)messageExt
         withHandler:(FPWechatShareResultHandler)handler;
@end
