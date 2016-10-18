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

@optional

+ (void) getLoginStatus:(CompleteBlock) block;

- (void) loginWithoutUI:(CompleteBlock) block;

/*
 * Sign Up Funplus Account with Email
 */
- (void) signup: (NSString *)email
   withPassword: (NSString *)password
   withCallback: (CompleteBlock)block;

/*
 * Change Funplus Account(Email)'s password
 */
- (void) changePassword: (NSString *)oldPassword
        withNewPassword: (NSString *)newPassword
           withCallback: (CompleteBlock)block;

/*
 * Reset Funplsu Account(Email)'s password, used when user forget password
 */
- (void) resetPassword: (NSString *)email
          withCallback: (CompleteBlock)block;


- (void) logout;

- (void) bindWithAccount:(FPAccount*) account
             resultBlock:(CompleteBlock) block;

- (void) bind:(NSString*) fpid resultBlock:(CompleteBlock) block;

- (NSString*) getAccessToken;

- (void) getUserData:(CompleteBlock) block;

- (void) getGameFriends:(CompleteBlock) block;

- (void) bind:(NSString *)email password:(NSString *)password callback:(CompleteBlock)callback;

- (void) login:(NSString*) email password:(NSString*) password callback:(CompleteBlock) callback;
@end

































