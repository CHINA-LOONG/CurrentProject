//
//  FPAccountProtocol.h
//  FunplusAccount
//
//  Created by yu.zhang on 15/3/18.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@class UIViewController;
@class FPAResponse;
@class FPAError;

#pragma mark - Block

typedef void (^CompleteBlock)(FPAResponse* response);


#pragma mark - protocol

@class FPAccount;

@protocol FPAccountProtocol <NSObject>

@required

+ (void) getLoginStatus:(CompleteBlock) block;

- (void) handleBecomeActive;

- (BOOL) handleOpenURL:(NSURL*) url sourceApplication:(NSString*) sourceApplication;

- (void) loginWithoutUI:(CompleteBlock) block;

- (void) logout;

- (void) showAccountInfoWithServerId:(NSString*) serverId
                          gameUserId:(NSString*) gameUserId
                            vipLevel:(NSString*) vipLevel
                         resultBlock:(CompleteBlock) block;

- (void) bindWithAccount:(FPAccount*) account
             resultBlock:(CompleteBlock) block;

- (void) bind:(NSString*) fpid resultBlock:(CompleteBlock) block;

- (NSString*) getAccessToken;

- (void) getUserData:(CompleteBlock) block;

- (void) getGameFriends:(CompleteBlock) block;

- (void) bind:(NSString *)email password:(NSString *)password callback:(CompleteBlock)callback;

- (void) login:(NSString*) email password:(NSString*) password callback:(CompleteBlock) callback;
@end

































