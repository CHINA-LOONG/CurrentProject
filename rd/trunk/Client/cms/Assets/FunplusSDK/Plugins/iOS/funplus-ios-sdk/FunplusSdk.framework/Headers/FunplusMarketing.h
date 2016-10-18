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
    FPMEventInstall             = 0,
} FPMEvent;

@interface FunplusMarketing : NSObject

+ (instancetype) sharedInstance;

- (void) tagEvent:(NSString*) eventTag attributes:(NSDictionary*) attributes;

@property(nonatomic, strong)NSString* fpid;
@property(nonatomic, strong)NSString* gameId;
@property(nonatomic, strong)NSString* gameKey;

- (void) checkStatus;
@end


@protocol FunplusMarketingDelegate
@optional

// Funplus Marketing Delegate

@end
