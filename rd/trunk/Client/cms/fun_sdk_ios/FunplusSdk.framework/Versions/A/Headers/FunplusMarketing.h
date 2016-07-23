//
//  FunplusMarketing.h
//  FunplusMarketing
//
//  Created by Yuankun on 11/12/14.
//  Copyright (c) 2014 Funplus. All rights reserved.
//  Version: v1.0.2
//
#import <Foundation/Foundation.h>

typedef enum {
    FPMErrorNone                = 0,
    FPMErrorLoadConfigFailed    = 1,
    FPMErrorSetupIncompletely   = 2,
    FPMErrorSetupFailed         = 3,
    FPMErrorUnknown             = 4,
} FPMError;

typedef enum {
    FPMEventInstall             = 0,
} FPMEvent;

typedef enum {
    FPMLogLevelVerbose          = 1,
    FPMLogLevelDebug            = 2,
    FPMLogLevelInfo             = 3,
    FPMLogLevelWarn             = 4,
    FPMLogLevelError            = 5,
    FPMLogLevelAssert           = 6
} FPMLogLevel;

typedef void (^FPMarketingSetupHandler)(FPMError err);

@interface FunplusMarketing : NSObject

+ (instancetype) sharedInstance;

@property(nonatomic, strong)NSString* fpid;
@end


@protocol FunplusMarketingDelegate
@optional

// Funplus Marketing Delegate

@end
