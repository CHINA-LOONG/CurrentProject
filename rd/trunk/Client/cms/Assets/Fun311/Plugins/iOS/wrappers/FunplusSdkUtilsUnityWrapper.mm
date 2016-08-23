//
//  FunplusSdkUtilsUnityWrapper.mm
//
//  Created by 张远坤 on 11/11/15.
//  Copyright © 2015 Funplus Inc. All rights reserved.
//

#include <string.h>
#include <FunplusSdk/FunplusSdk.h>

extern void UnitySendMessage(const char *, const char *, const char *);

static char *reteinStr(const char *str) {
    if (str == NULL) {
        return NULL;
    }

    char *ret = (char *)malloc(strlen(str) + 1);
    strcpy(ret, str);
    return ret;
}


extern "C" {

    char * com_funplus_sdk_getTotalMemory() {
        NSLog(@"com_funplus_sdk_getTotalMemory() called.");
        
        NSString *totalMemory = [[FunplusSdkUtils sharedInstance] getTotalMemory];
        return reteinStr([totalMemory cStringUsingEncoding:NSUTF8StringEncoding]);
    }

    char * com_funplus_sdk_getAvailableMemory() {
        NSLog(@"com_funplus_sdk_getAvailableMemory() called.");
        
        NSString *availableMemory = [[FunplusSdkUtils sharedInstance] getAvailableMemory];
        return reteinStr([availableMemory cStringUsingEncoding:NSUTF8StringEncoding]);
    }

}
