//
//  FunplusWechatUser.h
//  FunplusAccount
//
//  Created by yu.zhang on 15/4/15.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//
#import <Foundation/Foundation.h>
#import "FPAUser.h"

@interface FunplusWechatUser : FPAUser
@property(nonatomic, strong)NSString* province;
@property(nonatomic, strong)NSString* city;;
@property(nonatomic, strong)NSString* country;
@end
