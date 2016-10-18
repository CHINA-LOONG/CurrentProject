//
//  FunplusAccount.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/6/5.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "FunplusDefine.h"
#import "FPAccountProtocol.h"
#import "FunplusObject.h"

@class FunplusError;
@class FPAccount;

typedef void (^FunplusAccountCallback)(FunplusAccountStatus status, FunplusError* error, NSString * fpid, NSString * sessionKey);
typedef void (^FunplusUserCenterCallback)(FunplusAccountStatus status, FunplusError* error, NSString* fpid, NSString* sessionKey, FunplusOperation userOperation);
typedef void (^FunplusAccountStatusCallback)(FunplusError* error, FunplusAccountStatus status);
typedef void (^FunplusGetAvailableAccountTypesCallback)(FunplusError* error, NSArray * availableTypes);
typedef void (^FunplusAccountEmailCallback)(FunplusError* error);

@interface FunplusAccount : FunplusObject

+ (instancetype) sharedInstance;

- (void) login:(FunplusAccountType) type callback:(FunplusAccountCallback) callback;

- (void) login:(FunplusAccountCallback) callback;

- (void) logout:(FunplusAccountCallback) callback;

- (void) bind:(FunplusAccountType) type callback:(FunplusAccountCallback) callback;

- (void) showUserCenter:(FunplusUserCenterCallback) callback;

- (void) getAvailableAccountTypes:(FunplusGetAvailableAccountTypesCallback) callback;

- (void) getLoginStatus:(FunplusAccountStatusCallback) callback;

- (NSString *) getFpid;

- (NSString *) getSessionKey;

- (BOOL) isUserLoggedIn;

@property(nonatomic, strong) FPAccount*             account;
@property(nonatomic, strong) NSDictionary*          config;
@property(nonatomic, strong) NSArray*               availableAccountTypes;
@property(nonatomic, strong) FPAResponse*           response;

@end
