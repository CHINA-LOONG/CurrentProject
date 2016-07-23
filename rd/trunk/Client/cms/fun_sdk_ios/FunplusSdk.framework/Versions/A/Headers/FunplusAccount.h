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

@class FunplusError;
@class FPAccount;

typedef void (^FunplusAccountCallback)(FunplusAccountStatus status, FunplusError* error, NSString * fpid, NSString * sessionKey);
typedef void (^FunplusAccountStatusCallback)(FunplusError* error, FunplusAccountStatus status);
typedef void (^FunplusGetAvailableAccountTypesCallback)(FunplusError* error, NSArray * availableTypes);

@interface FunplusAccount : NSObject

+ (instancetype) sharedInstance;

- (void) login:(FunplusAccountType) type callback:(FunplusAccountCallback) callback;

- (void) login:(FunplusAccountCallback) callback;

- (void) logout:(FunplusAccountCallback) callback;

- (void) bind:(FunplusAccountType) type callback:(FunplusAccountCallback) callback;

- (void) showUserCenter:(FunplusAccountCallback) callback;

- (void) getAvailableAccountTypes:(FunplusGetAvailableAccountTypesCallback) callback;

- (void) getLoginStatus:(FunplusAccountStatusCallback) callback;

- (void) setUserInfo:(NSDictionary*) userInfo;

@property(nonatomic, strong) NSString* gameId;
@property(nonatomic, strong) NSString* gameKey;
@property(nonatomic, strong) NSDictionary* configSet;
@property(nonatomic, strong) FPAccount* account;
@property(nonatomic, strong) NSArray* availableAccountTypes;
@property(nonatomic, strong) FPAResponse* response;
@property(nonatomic, strong) NSString* email;
@property(nonatomic, strong) NSString* password;
@end
