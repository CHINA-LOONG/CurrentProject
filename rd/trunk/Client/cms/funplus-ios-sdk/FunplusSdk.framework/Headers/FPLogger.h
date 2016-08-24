//
//  FPALogger.h
//  FunplusAccount
//
//  Created by Siyuan Zhang on 7/22/14.
//  Copyright (c) 2014 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "FunplusDefine.h"

// A simple logger with multiple log levels.
@protocol FPLoggerInterface

- (void)setLogLevel:(FunplusLogLevel)logLevel;

- (void)verbose:(NSString *)message, ...;
- (void)debug:  (NSString *)message, ...;
- (void)info:   (NSString *)message, ...;
- (void)warn:   (NSString *)message, ...;
- (void)error:  (NSString *)message, ...;
- (void)assert: (NSString *)message, ...;

@end

@interface FPLogger : NSObject <FPLoggerInterface>

@property (nonatomic, assign) FunplusLogLevel loglevel;

- (id)initWithTag:(NSString*) tag;
@end
