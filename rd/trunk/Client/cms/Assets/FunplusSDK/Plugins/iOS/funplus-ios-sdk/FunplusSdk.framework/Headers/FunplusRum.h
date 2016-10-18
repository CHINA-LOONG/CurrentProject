//
//  FunplusRum.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/11/30.
//  Copyright © 2015年 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "FunplusObject.h"

@interface FunplusRum : FunplusObject

+ (instancetype) sharedInstance;

/**
 * Logging Network Requests
 * @param serviceName:service being called, eg: payment/backend/neighbor
 * @param urlString:representing the contacted endpoint
 * @param httpStatus:status code, 200/500 ...
 * @param latency:milliseconds
 * @param requestSize:size of request body, in bytes
 * @param responseSize:size of response body, in bytes
*/
- (void) service_monitoring:(NSString*) serviceName
                 urlString:(NSString*) urlString
                httpStatus:(NSString*) httpStatus
                   latency:(int) latency
               requestSize:(int) requestSize
              responseSize:(int) responseSize;
@end
