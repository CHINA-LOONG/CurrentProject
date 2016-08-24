//
//  FunplusVKHelper.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/6/8.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "FunplusVkUser.h"
#import "FunplusError.h"

@interface FunplusVKHelper : NSObject

typedef void (^FPVkUserDataHandler)(FunplusError* error,FunplusVkUser* user);
typedef void (^FPVkFriendsDataHandler)(FunplusError* error, NSArray * friends);
typedef void (^FPVkShareResultHandler)(FunplusError* error);
typedef void (^FPVkRequestResultHandler)(FunplusError* error);

+ (instancetype) sharedInstance;

- (NSString*) getAccessToken;

- (void) getUserData:(FPVkUserDataHandler)handler;

- (void) getGameFriends:(FPVkFriendsDataHandler)handler;

- (void) shareWithTitle:(NSString*)title
                message:(NSString *)message
                   link:(NSString *)link
                  image:(NSString *)imagePath
                handler:(FPVkShareResultHandler)handler;

//These methods are available only for the apps that use our mobile platform
//messages.send instead of it.
- (void) sendRequsetWithUserId:(NSString*)userId
                       message:(NSString*)message
                          link:(NSString*)link
                       handler:(FPVkRequestResultHandler)handler;

@end
