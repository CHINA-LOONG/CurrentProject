//
//  FunplusRumUnityWrapper.mm
//
//  Created by 张远坤 on 11/11/15.
//  Copyright © 2015 Funplus Inc. All rights reserved.
//

#include <string.h>
#include <FunplusSdk/FunplusSdk.h>


extern "C" {
    
    void com_funplus_sdk_rum_traceServiceMonitoring (const char *serviceName, const char *httpUrl, const char *httpStatus, int httpLatency, int requestSize, int responseSize) {
        NSLog(@"com_funplus_sdk_rum_traceServiceMonitoring() called.");
        
        NSString *nsServiceName = [NSString stringWithCString:serviceName encoding:NSUTF8StringEncoding];
        NSString *nsHttpUrl = [NSString stringWithCString:httpUrl encoding:NSUTF8StringEncoding];
        NSString *nsHttpStatus = [NSString stringWithCString:httpStatus encoding:NSUTF8StringEncoding];
        
        Class cls = NSClassFromString(@"FunplusRum");
        [[cls sharedInstance] service_monitoring:nsServiceName
                                 urlString:nsHttpUrl
                                httpStatus:nsHttpStatus
                                   latency:httpLatency
                               requestSize:requestSize
                              responseSize:responseSize];
    }

}
