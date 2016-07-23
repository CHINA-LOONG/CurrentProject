//
//  FPWechatProtocol.h
//  FunplusAccount
//
//  Created by yu.zhang on 15/4/14.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//
#import <Foundation/Foundation.h>

typedef void (^CompleteBlock)(FPAResponse* response);

@protocol FPWechatProtocol <NSObject>

@required
- (void) shareWithTitle:(NSString*)title
                message:(NSString*)message
                 picUrl:(NSString*)picUrl
             messageExt:(NSString*)messageExt
               jumpType:(NSString*)jumpType
            resultBlock:(CompleteBlock)block;

- (void) shareWithImagePath:(NSString *)imgPath
                    message:(NSString *)message
                   jumpType:(NSString *)jumpType
                resultBlock:(CompleteBlock)block;

- (void) sendRequestWithPlatformId:(NSString*)platformId
                             title:(NSString*)title
                           message:(NSString*)message
                         imagePath:(NSString*)imgPath
                           mediaId:(NSString*)mediaId
                        messageExt:(NSString*)messageExt
                       resultBlock:(CompleteBlock)block;
@end