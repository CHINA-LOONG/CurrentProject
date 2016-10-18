//
//  FPAUser.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/6/2.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

@interface FPAUser : NSObject

@property (nonatomic, strong) NSString* uid;
@property (nonatomic, strong) NSString* name;
@property (nonatomic, strong) NSString* pic;
@property (nonatomic, strong) NSString* gender;
@property (nonatomic, strong) NSString* email;

@property (nonatomic, strong) NSString* token;

@property (nonatomic, strong) NSMutableDictionary* userInfo;
@end
