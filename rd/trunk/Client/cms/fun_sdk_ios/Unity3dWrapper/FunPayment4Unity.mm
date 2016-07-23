//
//  FunPayment4Unity.mm
//  Unity-iPhone
//
//  Created by Lei Gu on 15/6/15.
//
//

#include <stdio.h>

#include <FunplusSdk/FunplusSdk.h>
#import <FunplusSdk/FunplusPayment.h>
#include <string>

using namespace std;

static string g_fp_payment_obj_name = "";

extern "C" {
    void obj_fp_payment_setObjName(const char* aName)
    {
        if(g_fp_payment_obj_name != aName)
        {
            g_fp_payment_obj_name = aName;
        }
    }
    
    void obj_fp_payment_get_products_info()
    {
        NSLog(@"obj_fp_payment_get_products_info called");

        [[FunplusPayment sharedInstance] getProductsInfo:^(FunplusError *aError, NSArray *aProducts) {
            NSLog(@"obj_fp_payment_get_products_info callback called");

            NSString* retStr = @"";
            NSString* prodsStr = @"";
            if(0 != aProducts.count)
            {
                NSMutableArray * arr = [[NSMutableArray alloc] init];
                
                for (SKProduct *product in aProducts)
                {
                    NSString* localizedTitle = product.localizedTitle;
                    NSString* localizedDescription = product.localizedDescription;
                    NSString* localeCurrentSymbol = [product.priceLocale objectForKey:NSLocaleCurrencySymbol];
                    NSString* localeCurrentCode = [product.priceLocale objectForKey: NSLocaleCurrencyCode];
                    NSString* prodId = product.productIdentifier;
                    
                    NSDictionary* tempDic = [NSDictionary dictionaryWithObjectsAndKeys: localizedTitle, @"prod_title", localizedDescription, @"prod_des", localeCurrentSymbol, @"prod_symbol", localeCurrentCode, @"prod_code", prodId, @"prod_id", nil];
                    
                    [arr addObject:tempDic];
                }
                
                NSError* error = nil;
                
                NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
                NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
                NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
                
                NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: arr,@"prods",errorCode, @"errorcode", errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg", nil];
                
                NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&error];
                
                if([jsonData length] > 0 && error == nil)
                {
                    retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                    
                }
            }
            
            UnitySendMessage(g_fp_payment_obj_name.c_str(), "onGetProductsIdsFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
    }
    
    void obj_fp_payment_set_order_complete_callback()
    {
        NSLog(@"obj_fp_payment_set_order_complete_callback called");
        
        [[FunplusPayment sharedInstance] setOrderCompleteCallback:^(FunplusError* aError, NSString *aProductId, NSString *aThroughCargo) {
            NSLog(@"payment4Unity, setOrderCompleteCallback, productId is %@, throughCargo %@", aProductId, aThroughCargo);
            
            NSString* retStr = nil;
            NSError *err = nil;
            
            NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
            NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
            NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
            
            NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: errorCode, @"errorcode", errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg",aThroughCargo, @"throughcargo" , nil];
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&err];
            
            if([jsonData length] > 0 && err == nil)
            {
                retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            }
            
            UnitySendMessage(g_fp_payment_obj_name.c_str(), "onOrderFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
    }
    
    void obj_fp_payment_buy(const char* aProId, const char* aThroughCargo)
    {
        NSString* proId = [NSString stringWithUTF8String: aProId];
        NSString* cargo = [NSString stringWithUTF8String: aThroughCargo];

        [[FunplusPayment sharedInstance] buy:proId withThroughCargo:cargo];
    }
    
}