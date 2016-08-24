//
//  FunplusBi.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/9/14.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//
#import "FunplusError.h"
#import "FunplusObject.h"
#import <Foundation/Foundation.h>

@interface FunplusBi : FunplusObject

+ (instancetype) sharedInstance;

/**
 * Trace KPI event: session_start
 */
- (void) traceSessionStart;

/**
 * Trace KPI event: session_end
 */

- (void) traceSessionEnd;

/**
 * Trace KPI event: new_user
 */

- (void) traceNewUser;

/**
 * Trace KPI event: payment
 *
 * @param amount
 * @param currency
 * @param iapProductId
 * @param iapProductName
 * @param iapProductType
 * @param transactionId
 * @param paymentProcessor
 * @param dCurrencyReceivedType
 * @param mCurrencyReceived
 * @param cItemsReceived
 */
- (void) tracePayment:(NSString*) amount
             currency:(NSString*) currency
            productId:(NSString*) iapProductId
          productName:(NSString*) iapProductName
          productType:(NSString*) iapProudcutType
        transactionId:(NSString*) transactionId
     paymentProcessor:(NSString*) paymentProcessor
 currencyReceivedType:(NSString*) currencyReceivedType
     currencyReceived:(NSString*) currencyReceived
        itemsReceived:(NSString*) itemsReceived;

/**
 * Trace finance event: payment
 *
 * @param amount
 * @param currency
 * @param iapProductId
 * @param iapProductName
 * @param iapProductType
 * @param transactionId
 * @param paymentProcessor
 * @param dCurrencyReceivedType
 * @param mCurrencyReceived
 * @param cItemsReceived
 */
- (void) traceFinancePayment:(NSString*) amount
                    currency:(NSString*) currency
                  productId:(NSString*) iapProductId
                 productName:(NSString*) iapProductName
                 productType:(NSString*) iapProductType
               transactionId:(NSString*) transactionId
            paymentProcessor:(NSString*) paymentProcessor
        currencyReceivedType:(NSString*) currencyReceivedType
            currencyReceived:(NSString*) currencyReceived
               itemsReceived:(NSString*) itemsReceived;

/**
 * Trace finance event: transaction
 *
 * @param dCurrencyReceivedType
 * @param mCurrencyReceived
 * @param dCurrencySpentType
 * @param mCurrencySpent
 * @param dTransactionType
 * @param dAction
 * @param cItemsReceived
 * @param cItemsUsed
 */
- (void) traceTransaction:(NSString*) amount
                 currency:(NSString*) currency
                productId:(NSString*) iapProductId
              productName:(NSString*) iapProductName
              productType:(NSString*) iapProductType
            transactionId:(NSString*) transactionId
         paymentProcessor:(NSString*) paymentProcessor
     currencyReceivedType:(NSString*) currencyReceivedType
         currencyReceived:(NSString*) currencyReceived
            itemsReceived:(NSString*) itemsReceived;

/**
 * Trace a custom event.
 *
 * @param eventName
 * @param properties
 */
- (void) trace:(NSString*) eventName properties:(NSDictionary*) properties;

- (void) onUserLoggedIn;

- (void) onUserLoggedOut;

- (void) onNewUser;

- (void) onPayment;

- (void) onStop;

@property(nonatomic, assign)BOOL            isDebug;
@property(nonatomic, strong)NSString*       appTag;
@property(nonatomic, strong)NSString*       dataVersion;
@property(nonatomic, strong)NSString*       installSource;
@property(nonatomic, strong)NSString*       gameId;
@property(nonatomic, strong)NSString*       gameKey;


@end
