//
//  FunplusInAppPurchase.h
//  FunplusInAppPurchase
//
//  Created by Siyuan Zhang on 12/30/14.
//  Copyright (c) 2014 com.funplus. All rights reserved.
//  Version: v1.3.1
//

#import <Foundation/Foundation.h>

typedef enum {
    FPILogLevelVerbose          = 1,
    FPILogLevelDebug            = 2,
    FPILogLevelInfo             = 3,
    FPILogLevelWarn             = 4,
    FPILogLevelError            = 5,
    FPILogLevelAssert           = 6
} FPILogLevel;

typedef enum {
    FPIErrorNone                    = 0, // 没有错误
    FPIErrorPurchaseFailed          = 1, // 用户取消购买或者没有获取到receipt
    FPIErrorFunplusPaymentFailed    = 2, // Funplus Payment 服务器错误
    FPIErrorUnknown                 = 3, // 未知错误
    FPIErrorNetWork                 = 4, // NetWork 错误
} FPIError;

typedef void (^FPIAPSetupCompleteHandler)(BOOL isSuccessful, NSArray * products);

/**
 * @param productId: 购买成功或失败的物品的productId
 * @param err: 此次行为的错误码
 **/
typedef void (^FPIAPBuyCompleteHandler)(FPIError err, NSString * productId, NSString * throughCargo);

@interface FunplusInAppPurchase : NSObject

+ (void) setupWithGameId:(NSString *)gameId
              withUserId:(NSString *)userId
         withEnvironment:(NSString *)environment
          withProductIds:(NSArray *)productIds
             withHandler:(FPIAPSetupCompleteHandler)handler;

// Call this method once startup.
+ (void) checkOngoingPurchasesWithHandler:(FPIAPBuyCompleteHandler)handler;

+ (void) checkOngoingPurchasesWithThroughCargo:(NSString *)throughCargo
                                   withHandler:(FPIAPBuyCompleteHandler)handler __deprecated;



+ (BOOL) canMakePurchases;

+ (void) buy: (NSString *) productId withHandler:(FPIAPBuyCompleteHandler)handler;

+ (void) buy: (NSString *) productId withThroughCargo:(NSString *)throughCargo withHandler:(FPIAPBuyCompleteHandler)handler;

+ (void) setLogLevel:(FPILogLevel)logLevel;

@end
