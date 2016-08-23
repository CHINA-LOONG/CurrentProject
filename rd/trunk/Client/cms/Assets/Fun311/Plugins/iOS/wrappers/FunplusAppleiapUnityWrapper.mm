//
//  FunplusAppleiapUnityWrapper.mm
//
//  Created by 张远坤 on 11/11/15.
//  Copyright © 2015 Funplus Inc. All rights reserved.
//

#include <cstring>
#import <FunplusSdk/FunplusSdk.h>

extern void UnitySendMessage(const char *, const char *, const char *);

// Game object name no more than 255 characters.
static char gameObject[256];

@interface AppleIapDelegate : NSObject <FunplusPaymentDelegate>

- (void) onInitializeSuccess:(NSArray*) products;
- (void) onInitializeError:(FunplusError*) error;
- (void) onPurchaseSuccess:(NSString*) productId throughCargo:(NSString*) throughCargo;
- (void) onPurchaseError:(FunplusError*) error;

@end

@implementation AppleIapDelegate

- (void) onInitializeSuccess:(NSArray*) products
{
    NSMutableArray *prods = [NSMutableArray new];
    NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
    [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
    [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
    
    for (SKProduct *product in products) {
        [numberFormatter setLocale:product.priceLocale];
    
        NSString *item = [NSString stringWithFormat:@"{\"product_description\": \"%@\", \"product_title\": \"%@\", \"price\": %lld, \"formatted_price\": \"%@\", \"locale_currency_symbol\": \"%@\", \"locale_currency_code\": \"%@\", \"product_id\": \"%@\"}",
                           product.localizedDescription,
                           product.localizedTitle,
                           (long long)([product.price doubleValue] * 1000000),
                           [numberFormatter stringFromNumber:product.price],
                           [product.priceLocale objectForKey:NSLocaleCurrencySymbol],
                           [product.priceLocale objectForKey:NSLocaleCurrencyCode],
                           product.productIdentifier];
        [prods addObject:item];
    }
    
    NSString *jsonStr = [NSString stringWithFormat:@"[%@]", [prods componentsJoinedByString:@", "]];
    
    UnitySendMessage(gameObject,
                     "OnInitializeSuccess",
                     [jsonStr cStringUsingEncoding:NSUTF8StringEncoding]);
}

- (void) onInitializeError:(FunplusError*) error
{
    UnitySendMessage(gameObject,
                     "OnInitializeError",
                     [[error toJSONString] cStringUsingEncoding:NSUTF8StringEncoding]);
}

- (void) onPurchaseSuccess:(NSString*) productId throughCargo:(NSString*) throughCargo
{
    NSString *message = [NSString stringWithFormat:@"{\"product_id\": \"%@\", \"through_cargo\": \"%@\"}",
                         productId,
                         throughCargo];

    UnitySendMessage(gameObject,
                     "OnPurchaseSuccess",
                     [message cStringUsingEncoding:NSUTF8StringEncoding]);
}

- (void) onPurchaseError:(FunplusError*) error
{
    UnitySendMessage(gameObject,
                     "OnPurchaseError",
                     [[error toJSONString] cStringUsingEncoding:NSUTF8StringEncoding]);
}

@end

static AppleIapDelegate *delegate = [[AppleIapDelegate alloc] init];


extern "C" {

    void com_funplus_sdk_payment_appleiap_setGameObject(const char *gameObjectName) {
        if (gameObjectName == NULL) {
            NSLog(@"Error: com_funplus_sdk_appleiap_setGameObject: gameObjectName must not be empty.");
            return;
        }
        strcpy(gameObject, gameObjectName);
    }

    int com_funplus_sdk_payment_appleiap_canMakePurchases() {
        Class cls = NSClassFromString(@"FunplusPayment");
        return [[cls sharedInstance] canMakePurchases];
    }

    void com_funplus_sdk_payment_appleiap_startHelper() {
        NSLog(@"com_funplus_sdk_payment_appleiap_startHelper() called.");
        Class cls = NSClassFromString(@"FunplusPayment");
        [[cls sharedInstance] setDelegate:delegate];
        [[cls sharedInstance] startHelper];
    }

    void com_funplus_sdk_payment_appleiap_buy(const char *sku, const char *payload) {
        NSLog(@"com_funplus_sdk_payment_appleiap_buy() called.");
        
        NSString *productId = [NSString stringWithCString:sku encoding:NSUTF8StringEncoding];
        NSString *throughCargo = [NSString stringWithCString:payload encoding:NSUTF8StringEncoding];
        
        Class cls = NSClassFromString(@"FunplusPayment");
        [[cls sharedInstance] buy:productId withThroughCargo:throughCargo];
    }

}
