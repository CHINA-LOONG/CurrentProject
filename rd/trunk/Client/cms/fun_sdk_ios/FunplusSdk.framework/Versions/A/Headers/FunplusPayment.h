//
//  FunplusPayment.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/6/9.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

#import "FunplusDefine.h"
#import "FunplusError.h"


typedef enum {
    FPILogLevelVerbose          = 1,
    FPILogLevelDebug            = 2,
    FPILogLevelInfo             = 3,
    FPILogLevelWarn             = 4,
    FPILogLevelError            = 5,
    FPILogLevelAssert           = 6
} FPILogLevel;



typedef void (^FunplusProductsInfoCallback)(FunplusError* error, NSArray * products);
typedef void (^FunplusOrderCompleteCallback)(FunplusError* error, NSString* productId, NSString* throughCargo);

@interface FunplusPayment : NSObject

+ (instancetype) sharedInstance;

- (void) getProductsInfo:(FunplusProductsInfoCallback) callback;
- (void) setOrderCompleteCallback:(FunplusOrderCompleteCallback) callback;
- (void) buy:(NSString*) productId withThroughCargo:(NSString *)throughCargo;

- (BOOL) canMakePurchases;
- (void) setLogLevel:(FPILogLevel)logLevel;

@property (nonatomic, strong) NSDictionary *configSet;
@property (nonatomic, strong) NSSet* productsIdSet;

@property (nonatomic, strong) NSString *userId;
@property (nonatomic, strong) NSString *paymentAppId;
@property (nonatomic, strong) NSString *paymentServerUrl;

@property (nonatomic, strong) NSString *throughCargo;
@property (nonatomic, strong) SKPaymentTransaction *currentTransaction;

@property (nonatomic, strong) NSArray* appProducts;
@property (nonatomic, strong) FunplusProductsInfoCallback productsInfoCallback;
@property (nonatomic, strong) FunplusOrderCompleteCallback orderCompleteCallback;

@end
