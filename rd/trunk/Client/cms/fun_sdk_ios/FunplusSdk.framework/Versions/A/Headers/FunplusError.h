//
//  FunplusError.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/6/19.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "FunplusErrorCode.h"

@interface FunplusError : NSObject 

- (instancetype) initWithCode:(FunplusErrorCode) code;

@property(nonatomic, strong) NSError* error;
@property(nonatomic) FunplusErrorCode code;
@property(nonatomic, strong)NSString* localizedDescription;
@end
