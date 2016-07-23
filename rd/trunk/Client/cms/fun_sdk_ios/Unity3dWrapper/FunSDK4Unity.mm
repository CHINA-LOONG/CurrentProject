//
//  FunSDK4Unity.mm
//  Unity-iPhone
//
//  Created by Lei Gu on 15/6/8.
//
//

#include <stdio.h>
#include <string>

#import <FunplusSdk/FunplusSdk.h>

using namespace std;

static string g_fp_funsdk_obj_name = "";

void onInstallFinished(FunplusError* aError)
{
    NSString* retStr = nil;
    NSString* success = aError.code == ErrorNone ? @"true" : @"false";
    NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
    NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
    NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
    
    NSError *error = nil;
    NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: errorCode, @"errorcode", success, @"success", errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg", nil];
    
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&error];
    if([jsonData length] > 0 && error == nil)
    {
        retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    
    UnitySendMessage(g_fp_funsdk_obj_name.c_str(), "onFunSdkInstallFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
}

extern "C" {
    void obj_fp_funsdk_setObjName(const char* aName)
    {
        if(g_fp_funsdk_obj_name != aName)
        {
            g_fp_funsdk_obj_name = aName;
        }
    }
    
    void obj_fp_funsdk_install(const char* aId, const char* aKey)
    {
        NSLog(@"funsdk, obj_fp_funsdk_install called");
        
        NSString* gId = [NSString stringWithUTF8String: aId];
        NSString* gKey = [NSString stringWithUTF8String: aKey];
        
        [[FunplusSdk sharedInstance] install:gId gameKey:gKey callback:^(FunplusError *errorCode) {
            NSLog(@"********** install callback");
            onInstallFinished(errorCode);
        }];
    }
    
    void obj_fp_sdk_set_debug(bool aIsDebug)
    {
        NSLog(@"funsdk, install called");
        [[FunplusSdk sharedInstance] setDebug:aIsDebug];
    }
}