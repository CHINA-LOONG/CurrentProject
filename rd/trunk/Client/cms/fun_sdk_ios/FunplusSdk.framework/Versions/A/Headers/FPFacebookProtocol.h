//
//  FPFacebookProtocol.h
//  FunplusAccount
//
//  Created by yu.zhang on 15/4/14.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//
#import <Foundation/Foundation.h>

#define FRIENDS_PERMISSION_NAME @"user_friends"
#define PUBLISH_PERMISSION_NAME @"publish_actions"

typedef void (^CompleteBlock)(FPAResponse* response);

@protocol FPFacebookProtocol <NSObject>

@required

- (BOOL) hasPublishPermission;

- (void) askPublishPermission:(CompleteBlock) block;


- (BOOL) hasFriendsPermission;

- (void) askFriendsPermission:(CompleteBlock) block;

- (BOOL) hasPermission:(NSString*) permission;

- (void) askReadPermission:(NSString*) permission
               resultBlock:(CompleteBlock) block;

- (void) sendRequestWithPlatformId:(NSString*) platformId
                           message:(NSString*) message
                       resultBlock:(CompleteBlock) block;

- (void) publishOpenGraph:(NSString *)ogNamespace
               withAction:(NSString *)ogAction
               withObject:(NSString *)ogObject
                withTitle:(NSString *)title
              withMessage:(NSString *)message
                 withLink:(NSString *)link
                  withPic:(NSString *)picUrl
                 withFlag:(BOOL)useApiOnly
              resultBlock:(CompleteBlock)block;


- (void) shareWithTitle:(NSString *)title
                message:(NSString *)message
                   link:(NSString *)link
                    pic:(NSString *)picUrl
            resultBlock:(CompleteBlock)block;

- (void) shareWithImagePath:(NSString*) imagePath
                    message:(NSString *) message
                       link:(NSString *) link
                resultBlock:(CompleteBlock) block;

- (void) loginWithoutUI:(NSString*) groupId
            resultBlock:(CompleteBlock) block;
@end
