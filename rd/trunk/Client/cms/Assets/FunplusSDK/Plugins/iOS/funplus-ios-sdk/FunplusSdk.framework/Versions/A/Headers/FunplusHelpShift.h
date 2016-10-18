//
//  FunplusHelpShift.h
//  FunplusSdk
//
//  Created by yu.zhang on 15/7/22.
//  Copyright (c) 2015年 Funplus Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "FunplusObject.h"

@interface FunplusHelpShift : FunplusObject

+ (id) sharedInstance;

- (void) showFAQs;

- (void) showConversation;

- (void) setUserIdentifier:(NSString *)userIdentifier;
@end
