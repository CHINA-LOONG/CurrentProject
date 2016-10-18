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
#import "FunplusObject.h"

@protocol FunplusPaymentDelegate <NSObject>

- (void) onInitializeSuccess:(NSArray*) products;

- (void) onInitializeError:(FunplusError*) error;

- (void) onPurchaseSuccess:(NSString*) productId throughCargo:(NSString*) throughCargo;

- (void) onPurchaseError:(FunplusError*) error;

@end

@interface FunplusPayment : FunplusObject

+ (instancetype) sharedInstance;

- (void) setDelegate:(id <FunplusPaymentDelegate>) delegate;
- (void) startHelper;
- (void) buy:(NSString*) productId withThroughCargo:(NSString *)throughCargo;
- (BOOL) canMakePurchases;
- (void) setLogLevel:(FunplusLogLevel)logLevel;
- (void) setDebug:(BOOL)isDebugMode;

@property (nonatomic, strong) NSDictionary *configSet;
@property (nonatomic, strong) NSSet* productsIdSet;

@property (nonatomic, strong) NSString *userId;
@property (nonatomic, strong) NSString *paymentAppId;
@property (nonatomic, strong) NSString *paymentServerUrl;

@property (nonatomic, strong) SKPaymentTransaction *currentTransaction;

@property (nonatomic, strong) NSArray* appProducts;

@property (nonatomic, strong) NSString* gameId;
@property (nonatomic, strong) NSString* gameKey;

@property (atomic, strong) NSMutableDictionary* requestedTransactionsDict;
@property (atomic, strong) NSMutableDictionary* transactionRequestTimesDict;
@property (atomic, strong) NSMutableDictionary* paymentConnectionDict;
@end
