//
//  FunplusSdkUtils.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/9/23.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface FunplusSdkUtils : NSObject

+ (instancetype) sharedInstance;

- (NSString*) getTotalMemory;

- (NSString*) getAvailableMemory;

@end
