//
//  FPAResponse.h
//  FunplusAccount
//
//  Created by yu.zhang on 15/3/26.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#import "FPAObject.h"
#import "FunplusErrorCode.h"
#import "FunplusDefine.h"
#import "FPAUser.h"
#import "FunplusError.h"

@class FPAUser;

@interface FPAResponse : FPAObject


@property (nonatomic, strong) NSMutableArray* accountTypes;
@property (nonatomic, strong) NSString* fpid;
@property (nonatomic, strong) NSString* sessionKey;
@property (nonatomic, assign) double sessionExpireIn;
@property (nonatomic, strong) NSString* email;
@property (nonatomic, strong) NSString* platformId;

@property (nonatomic, assign) BOOL bSuccess;

@property (nonatomic, strong) NSMutableDictionary* params;

@property (nonatomic, strong) FPAUser* user;
@property (nonatomic, strong) NSArray* friends;

@property (nonatomic, strong) NSString* fpidInRequest;
@property (nonatomic, strong) NSString* fpidInResponse;

@property (nonatomic, strong) FunplusError* error;
@property (nonatomic, assign) FunplusAccountStatus accountStatus;

@end
