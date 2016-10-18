//
//  FunplusBiUnityWrapper.mm
//
//  Created by 张远坤 on 11/11/15.
//  Copyright © 2015 Funplus Inc. All rights reserved.
//

#include <string.h>
#include <FunplusSdk/FunplusSdk.h>

extern void UnitySendMessage(const char *, const char *, const char *);


extern "C" {

    void com_funplus_sdk_bi_traceEvent(const char *eventName, const char *eventData) {
        NSLog(@"com_funplus_sdk_bi_traceEvent() called.");
        
        NSString *event = [NSString stringWithCString:eventName encoding:NSUTF8StringEncoding];
        
        NSString *jsonStr = [NSString stringWithCString:eventData encoding:NSUTF8StringEncoding];
        NSError *jsonError;
        NSData *jsonData = [jsonStr dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *properties = [NSJSONSerialization JSONObjectWithData:jsonData
                                                                   options:NSJSONReadingMutableContainers
                                                                     error:&jsonError];
        
        if (jsonError != nil) {
            NSLog(@"com_funplus_sdk_bi_traceEvent() error: %@", [jsonError description]);
        } else {
            Class cls = NSClassFromString(@"FunplusBi");
            [[cls sharedInstance] trace:event properties:properties];
        }
    }
    
    void com_funplus_sdk_bi_logUserLogin(const char *uid) {
        Class cls = NSClassFromString(@"FunplusBi");
        [[cls sharedInstance] onUserLoggedIn];
    }
    
    void com_funplus_sdk_bi_logNewUser(const char *uid) {
        Class cls = NSClassFromString(@"FunplusBi");
        [[cls sharedInstance] onNewUser];
    }
    
    void com_funplus_sdk_bi_logUserLogout() {
        Class cls = NSClassFromString(@"FunplusBi");
        [[cls sharedInstance] onUserLoggedIn];
    }

}
