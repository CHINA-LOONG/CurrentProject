//
//  FunFbHelper4Unity.mm
//  Unity-iPhone
//
//  Created by Lei Gu on 15/6/9.
//
//

#include <stdio.h>

#include <FunplusSdk/FunplusSdk.h>
#include <string>
#import <FunplusSdk/FunplusFacebookHelper.h>

using namespace std;

static string g_fp_sdk_fb_obj_name = "";

extern "C" {
    void obj_fp_sdk_fb_setObjName(const char* aName)
    {
        if(g_fp_sdk_fb_obj_name != aName)
        {
            g_fp_sdk_fb_obj_name = aName;
        }
    }
    
    void obj_fp_sdk_fb_share(const char* aTitle, const char* aMessage, const char* aLink, const char* aPic)
    {
        NSLog(@"FunFbHelper4Unity, obj_fp_funsdk_install called");
        NSString* title = [NSString stringWithUTF8String: aTitle];
        NSString* message = [NSString stringWithUTF8String: aMessage];
        NSString* link = [NSString stringWithUTF8String: aLink];
        NSString* pic = [NSString stringWithUTF8String: aPic];

        [[FunplusFacebookHelper sharedInstance] share:title withMessage:message withLink:link withPic:pic withHandler:^(FunplusError *aError) {
            NSLog(@"FunFbHelper4Unity share result %@",aError.description);

            NSString* success = aError.code == ErrorNone ? @"true" : @"false";
            NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
            NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
            NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
            
            NSError *error = nil;
            NSString* retStr = @"";
            NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: success, @"success", errorCode, @"errorcode",errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg",nil];
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&error];
            if([jsonData length] > 0 && error == nil)
            {
                retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            }
            
            UnitySendMessage(g_fp_sdk_fb_obj_name.c_str(), "onFbShareFinished",[retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
    }
    
    void obj_fp_sdk_fb_share_image(const char* aPicPath, const char* aMessage, const char* aLink)
    {
        //NSString* imgPath = [[NSBundle mainBundle] pathForResource:@"vk" ofType:@"jpg"];
        
        NSString* imgPath = [NSString stringWithUTF8String: aPicPath];
        NSString* message = [NSString stringWithUTF8String: aMessage];
        NSString* link = [NSString stringWithUTF8String: aLink];
        
        [[FunplusFacebookHelper sharedInstance] shareImage:imgPath withMessage:message withLink:link withHandler:^(FunplusError *aError) {
            NSLog(@"FunFbHelper4Unity share result %@",aError.description);
            
            NSString* success = aError.code == ErrorNone ? @"true" : @"false";
            NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
            NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
            NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
            
            NSError *error = nil;
            NSString* retStr = @"";
            NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: success, @"success", errorCode, @"errorcode",errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg",nil];
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&error];
            if([jsonData length] > 0 && error == nil)
            {
                retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            }
            
            UnitySendMessage(g_fp_sdk_fb_obj_name.c_str(), "onFbShareImageFinished",[retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
    }
    
    const char* obj_fp_sdk_fb_get_access_token()
    {
        NSString* ret =  [[FunplusFacebookHelper sharedInstance] getAccessToken];
        //need alloc memory, or unity will free the ret val.
        char* cRet = (char*)malloc(strlen([ret cStringUsingEncoding:NSUTF8StringEncoding]) + 1);
        strcpy(cRet, [ret cStringUsingEncoding:NSUTF8StringEncoding]);
        return cRet;
    }
    
    void obj_fp_sdk_fb_get_user_data()
    {
        [[FunplusFacebookHelper sharedInstance] getUserData:^(FunplusError *aError, FunplusFacebookUser *aUser) {
            
            NSString* retStr = @"";
            NSString* user_uid = [aUser.uid length] ? aUser.uid : @"";
            NSString* user_name = [aUser.name length] ? aUser.name : @"";
            NSString* user_email = [aUser.email length] ? aUser.email : @"";
            NSString* user_pic = [aUser.pic length] ? aUser.pic : @"";
            NSString* user_gender = [aUser.gender length] ? aUser.gender : @"";
            //NSString* accesstoken = [aUser.accessToken length] ? aUser.accessToken : @"";
            NSString* accessToken = @"";
            NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
            NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
            NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
            
            NSError *jsonError = nil;
            NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: user_uid,@"uid", user_name, @"name",user_email, @"email", user_pic, @"pic",user_gender, @"gender", accessToken, @"accesstoken", errorCode, @"errorcode", errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg", nil];
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&jsonError];
            if([jsonData length] > 0 && jsonError == nil)
            {
                retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            }
            
            UnitySendMessage(g_fp_sdk_fb_obj_name.c_str(), "onFbGetUserDataFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
    }
    
    void obj_fp_sdk_fb_get_game_friends()
    {
        [[FunplusFacebookHelper sharedInstance] getGameFriends:^(FunplusError *aError, NSArray *aFriends) {
            NSString* retStr = @"";
            
            if(0 != aFriends.count)
            {
                FunplusFacebookUser* tempUesr;
                NSMutableArray * arr = [[NSMutableArray alloc] init];
                for (tempUesr in aFriends)
                {
                    NSString* user_uid = [tempUesr.uid length] ? tempUesr.uid : @"";
                    NSString* user_name = [tempUesr.name length] ? tempUesr.name : @"";
                    NSString* user_email = [tempUesr.email length] ? tempUesr.email : @"";
                    NSString* user_pic = [tempUesr.pic length] ? tempUesr.pic : @"";
                    NSString* user_gender = [tempUesr.gender length] ? tempUesr.gender : @"";
                    
                    NSDictionary* tempDic = [NSDictionary dictionaryWithObjectsAndKeys: user_uid,@"uid", user_name, @"name",user_email, @"email", user_pic, @"pic",user_gender, @"gender", nil];
                    
                    [arr addObject: tempDic];
                }
                
                NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
                NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
                NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
                
                NSError *jsonError = nil;
                NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: errorCode, @"errorcode", errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg", arr, @"friends", nil];
                
                NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&jsonError];
                if([jsonData length] > 0 && jsonError == nil)
                {
                    retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                }

            }
            UnitySendMessage(g_fp_sdk_fb_obj_name.c_str(), "onFbGetGameFriendsFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];

    }
    
    bool obj_fp_sdk_fb_has_friends_permission()
    {
        bool ret = [[FunplusFacebookHelper sharedInstance] hasFriendsPermission];
        return ret;
    }

    void obj_fp_sdk_fb_ask_friends_permission()
    {
        [[FunplusFacebookHelper sharedInstance] askFriendsPermission:^(FunplusError *aError) {
            NSLog(@"obj_fp_askFriendsPermission, success %@", aError.description);
            
            NSString* retStr = @"";
            
            NSString* success = aError.code == ErrorNone ? @"true" : @"false";
            NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
            NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
            NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;

            NSError *error = nil;
            NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: success, @"success", errorCode, @"errorcode",errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg", nil];
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&error];
            if([jsonData length] > 0 && error == nil)
            {
                retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            }
            
            UnitySendMessage(g_fp_sdk_fb_obj_name.c_str(), "onFbAskFriendsPermissionFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
        

        
    }
    
    void obj_fp_sdk_fb_send_request(const char* aPlatformId, const char* aMessage)
    {
        NSString * nsToPlatformId = [NSString stringWithUTF8String:aPlatformId];
        NSString * nsMessage = [NSString stringWithUTF8String:aMessage];
        
        [[FunplusFacebookHelper sharedInstance] sendRequest:nsToPlatformId  withMessage:nsMessage withHandler:^(FunplusError *aError) {
            
            NSString* retStr = @"";
            
            NSString* success = aError.code == ErrorNone ? @"true" : @"false";
            NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
            NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
            NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
            
            NSError *error = nil;
            NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: success, @"success", errorCode, @"errorcode",errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg", nil];
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&error];
            if([jsonData length] > 0 && error == nil)
            {
                retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            }
            
            UnitySendMessage(g_fp_sdk_fb_obj_name.c_str(), "onFbSendRequestFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
    }
    
    bool obj_fp_sdk_fb_has_publish_permission()
    {
        bool ret = [[FunplusFacebookHelper sharedInstance] hasPublishPermission];
        return ret;
    }

    void obj_fp_sdk_fb_ask_publish_permission()
    {
        [[FunplusFacebookHelper sharedInstance] askPublishPermission:^(FunplusError *aError) {
            NSString* retStr = @"";
            
            NSString* success = aError.code == ErrorNone ? @"true" : @"false";
            NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
            NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
            NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
            
            NSError *error = nil;
            NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: success, @"success", errorCode, @"errorcode",errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg", nil];
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&error];
            if([jsonData length] > 0 && error == nil)
            {
                retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            }
            
            UnitySendMessage(g_fp_sdk_fb_obj_name.c_str(), "onFbAskPublishPermissionFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
        
    }
    
    void obj_fp_sdk_fb_publish_open_graph(const char* aNameSpace, const char* aAction, const char* aObj, const char* aTitle, const char* aMessage, const char* aLink, const char* aPicUrl, bool aUseApiOnly)
    {
        NSString* nameSpace = [NSString stringWithUTF8String: aNameSpace];
        NSString* action = [NSString stringWithUTF8String: aAction];
        NSString* obj = [NSString stringWithUTF8String: aObj];
        NSString* title = [NSString stringWithUTF8String: aTitle];
        NSString* message = [NSString stringWithUTF8String: aMessage];
        NSString* link = [NSString stringWithUTF8String: aLink];
        NSString* picUrl = [NSString stringWithUTF8String: aPicUrl];
        
        [[FunplusFacebookHelper sharedInstance] publishOpenGraph:nameSpace withAction:action withObject:obj withTitle:title withMessage:message withLink:link withPic:picUrl withFlag:aUseApiOnly withHandler:^(FunplusError* aError) {
            NSString* retStr = @"";
            
            NSString* success = aError.code == ErrorNone ? @"true" : @"false";
            NSString* errorCode = [NSString stringWithFormat:@"%d", aError.code];
            NSString* errorMsg = aError.code == ErrorNone ? @"" : aError.description;
            NSString* errorLocMsg = aError.code == ErrorNone ? @"" : aError.localizedDescription;
            
            NSError *error = nil;
            NSDictionary* retJson = [NSDictionary dictionaryWithObjectsAndKeys: success, @"success", errorCode, @"errorcode",errorMsg, @"errormsg", errorLocMsg, @"errorlocmsg", nil];
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:retJson options:NSJSONWritingPrettyPrinted error:&error];
            if([jsonData length] > 0 && error == nil)
            {
                retStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            }
            
            UnitySendMessage(g_fp_sdk_fb_obj_name.c_str(), "onPublishOpenGraphFinished", [retStr cStringUsingEncoding:NSUTF8StringEncoding]);
        }];
    }
    
}