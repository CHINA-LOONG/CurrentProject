//
//  FPVkProtocol.h
//  FunplusAccount
//
//  Created by yu.zhang on 15/4/14.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//
#import <Foundation/Foundation.h>

typedef void (^CompleteBlock)(FPAResponse* response);


@protocol FPVkProtocol <NSObject>

@required
- (void) shareWithTitle:(NSString*) title
                message:(NSString *) message
                   link:(NSString *) link
              imagePath:(NSString *) imagePath
            resultBlock:(CompleteBlock) block;

- (void) sendRequsetWithUserId:(NSString*) userId
                       message:(NSString*) message
                          link:(NSString*) link
                   resultBlock:(CompleteBlock) block;
@end
