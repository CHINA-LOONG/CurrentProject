//
//  FunplusAccountUnityWrapper.mm
//
//  Created by 张远坤 on 11/11/15.
//  Copyright © 2015 Funplus Inc. All rights reserved.
//

#include <string.h>
#import <FunplusSdk/FunplusSdk.h>

extern void UnitySendMessage(const char *, const char *, const char *);

// Game object name no more than 255 characters.
static char gameObject[256];

extern "C" {

    void com_funplus_sdk_social_facebook_setGameObject(const char *gameObjectName) {
        if (gameObjectName == NULL) {
            NSLog(@"Error: com_funplus_sdk_social_facebook_setGameObject: gameObjectName must not be empty.");
            return;
        }
        strcpy(gameObject, gameObjectName);
    }
    
    int com_funplus_sdk_social_facebook_hasFriendsPermission (){
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_hasFriendsPermission");
        return [[FunplusFacebookHelper sharedInstance] hasFriendsPermission];
    }
    
    void com_funplus_sdk_social_facebook_askFriendsPermission (){
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_askFriendsPermission");
        [[FunplusFacebookHelper sharedInstance] askFriendsPermission:^(FunplusError *error) {
            NSLog(@">>>>>>askFriendsPermission %@",[error toJSONString]);
            if (error.code == 0) {
                UnitySendMessage(gameObject, "OnFacebookAskFriendsPermission", "true");
            }else{
                UnitySendMessage(gameObject, "OnFacebookAskFriendsPermission", "false");
            }
        }];
    }
    
    int com_funplus_sdk_social_facebook_hasPublishPermission (){
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_hasPublishPermission");
        return [[FunplusFacebookHelper sharedInstance] hasPublishPermission];
    }
    
    void com_funplus_sdk_social_facebook_askPublishPermission(){
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_askPublishPermission");
        [[FunplusFacebookHelper sharedInstance] askPublishPermission:^(FunplusError *error) {
            NSLog(@">>>>>>askPublishPermission %@",[error toJSONString]);
            if (error.code == 0) {
                UnitySendMessage(gameObject, "OnFacebookAskPublishPermission", "true");
            }else{
                UnitySendMessage(gameObject, "OnFacebookAskPublishPermission", "false");
            }
        }];
    }

    void com_funplus_sdk_social_facebook_getUserData() {
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_getUserData");
        [[FunplusFacebookHelper sharedInstance] getUserData:^(FunplusError *error, FunplusFacebookUser *user) {
            if (user != nil) {
                NSArray *userIDArray = [user.uid componentsSeparatedByString:@":"];
                if ([userIDArray count] > 1) {
                    NSString *message = [NSString stringWithFormat:@"{\"uid\": \"%@\", \"pic\": \"%@\", \"name\":\"%@\", \"email\":\"%@\", \"access_token\":\"%@\"}", [userIDArray objectAtIndex:1], user.pic,user.name,user.email,[[FunplusFacebookHelper sharedInstance] getAccessToken]];
                    UnitySendMessage(gameObject, "OnFacebookGetUserDataSuccess", [message cStringUsingEncoding:NSUTF8StringEncoding]);
                } else {
                    NSString *message = [NSString stringWithFormat:@"{\"uid\": \"%@\", \"pic\": \"%@\", \"name\":\"%@\", \"email\":\"%@\", \"access_token\":\"%@\"}", user.uid, user.pic,user.name,user.email,[[FunplusFacebookHelper sharedInstance] getAccessToken]];
                    UnitySendMessage(gameObject, "OnFacebookGetUserDataSuccess", [message cStringUsingEncoding:NSUTF8StringEncoding]);
                }
            }else{
                NSString *errMessage = error == nil ? @"Login failed: unknown error" : [error toJSONString];
                UnitySendMessage(gameObject, "OnFacebookGetUserDataError", [errMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }
    
    void com_funplus_sdk_social_facebook_getGameFriends() {
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_getGameFriends");
        [[FunplusFacebookHelper sharedInstance] getGameFriends:^(FunplusError *error, NSArray *friends) {
            if (friends != nil && [friends count] > 0) {
                NSMutableString *friendsMessageString = [[NSMutableString alloc] initWithFormat:@"["];
                for (int i = 0; i < [friends count]; i++) {
                    FunplusFacebookUser *oneFriend = [friends objectAtIndex:i];
                    NSLog(@">>>>>>FunplusFacebookUnityWrapper oneFriend uid = %@, pic = %@, name = %@, gender = %@",oneFriend.uid,oneFriend.pic,oneFriend.name,oneFriend.gender);
                    
                    NSArray *userIDArray = [oneFriend.uid componentsSeparatedByString:@":"];
                    if ([userIDArray count] > 1) {
                        if (i == ([friends count]-1)) {
                            [friendsMessageString appendString:[NSString stringWithFormat:@"{\"id\": \"%@\", \"name\":\"%@\", \"pic\":\"%@\", \"gender\":\"%@\"}",[userIDArray objectAtIndex:1],oneFriend.name,oneFriend.pic,oneFriend.gender]];
                        }
                        else{
                            [friendsMessageString appendString:[NSString stringWithFormat:@"{\"id\": \"%@\", \"name\":\"%@\", \"pic\":\"%@\", \"gender\":\"%@\"},",[userIDArray objectAtIndex:1],oneFriend.name,oneFriend.pic,oneFriend.gender]];
                        }
                    } else {
                        if (i == ([friends count]-1)) {
                            [friendsMessageString appendString:[NSString stringWithFormat:@"{\"id\": \"%@\", \"name\":\"%@\", \"pic\":\"%@\", \"gender\":\"%@\"}",oneFriend.uid,oneFriend.name,oneFriend.pic,oneFriend.gender]];
                        }
                        else{
                            [friendsMessageString appendString:[NSString stringWithFormat:@"{\"id\": \"%@\", \"name\":\"%@\", \"pic\":\"%@\", \"gender\":\"%@\"},",oneFriend.uid,oneFriend.name,oneFriend.pic,oneFriend.gender]];
                        }
                    }
                }
                [friendsMessageString appendString:@"]"];
                NSLog(@">>>>>>FunplusFacebookUnityWrapper friendsMessageString = %@",friendsMessageString);
                UnitySendMessage(gameObject, "OnFacebookGetGameFriendsSuccess", [friendsMessageString cStringUsingEncoding:NSUTF8StringEncoding]);
            }else{
                NSString *errMessage = error == nil ? @"Get game friends failed: unknown error" : [error toJSONString];
                UnitySendMessage(gameObject, "OnFacebookGetGameFriendsError", [errMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }
    
    void com_funplus_sdk_social_facebook_getGameInvitableFriends() {
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_getGameInvitableFriends");
        [[FunplusFacebookHelper sharedInstance] getInvitableFriends:^(FunplusError *error, NSArray *friends) {
            if (friends != nil && [friends count] > 0) {
                NSMutableString *friendsMessageString = [[NSMutableString alloc] initWithFormat:@"["];
                for (int i = 0; i < [friends count]; i++) {
                    FunplusFacebookUser *oneFriend = [friends objectAtIndex:i];
                    NSLog(@">>>>>>FunplusFacebookUnityWrapper oneFriend uid = %@, pic = %@, name = %@, gender = %@",oneFriend.uid,oneFriend.pic,oneFriend.name,oneFriend.gender);
                    NSArray *userIDArray = [oneFriend.uid componentsSeparatedByString:@":"];
                    if ([userIDArray count] > 1) {
                        if (i == ([friends count]-1)) {
                            [friendsMessageString appendString:[NSString stringWithFormat:@"{\"id\":\"%@\", \"name\":\"%@\", \"pic\":\"%@\", \"gender\":\"%@\"}",[userIDArray objectAtIndex:1],oneFriend.name,oneFriend.pic,oneFriend.gender]];
                        }
                        else{
                            [friendsMessageString appendString:[NSString stringWithFormat:@"{\"id\":\"%@\", \"name\":\"%@\", \"pic\":\"%@\", \"gender\":\"%@\"},",[userIDArray objectAtIndex:1],oneFriend.name,oneFriend.pic,oneFriend.gender]];
                        }
                    } else {
                        if (i == ([friends count]-1)) {
                            [friendsMessageString appendString:[NSString stringWithFormat:@"{\"id\":\"%@\", \"name\":\"%@\", \"pic\":\"%@\", \"gender\":\"%@\"}",oneFriend.uid,oneFriend.name,oneFriend.pic,oneFriend.gender]];
                        }
                        else{
                            [friendsMessageString appendString:[NSString stringWithFormat:@"{\"id\":\"%@\", \"name\":\"%@\", \"pic\":\"%@\", \"gender\":\"%@\"},",oneFriend.uid,oneFriend.name,oneFriend.pic,oneFriend.gender]];
                        }
                    }
                }
                [friendsMessageString appendString:@"]"];
                NSLog(@">>>>>>FunplusFacebookUnityWrapper friendsMessageString = %@",friendsMessageString);
                UnitySendMessage(gameObject, "OnFacebookGetGameInvitableFriendsSuccess", [friendsMessageString cStringUsingEncoding:NSUTF8StringEncoding]);
            }else{
                NSString *errMessage = error == nil ? @"Get game invitable friends failed: unknown error" : [error toJSONString];
                UnitySendMessage(gameObject, "OnFacebookGetGameInvitableFriendsError", [errMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
        
    }
    
    void com_funplus_sdk_social_facebook_sendGameRequest(const char * message){
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_sendGameRequest message = %@",[NSString stringWithUTF8String:message]);
        [[FunplusFacebookHelper sharedInstance] sendRequestWithPlatformId:@"" message:[NSString stringWithUTF8String:message] handler:^(NSString*  requestId,FunplusError *error) {
            NSLog(@">>>>>>send game request result = %@ %u %@",[error toJSONString],error.code,error.localizedDescription);
            if (error.code == 0) {
                NSString * message = [NSString stringWithFormat:@"{\"request_id\": \"%@\"}",requestId];
                NSLog(@">>>>>>com_funplus_sdk_social_facebook_sendGameRequest result message = %@",message);
                UnitySendMessage(gameObject, "OnFacebookSendGameRequestSuccess", [message cStringUsingEncoding:NSUTF8StringEncoding]);
            }else{
                UnitySendMessage(gameObject, "OnFacebookSendGameRequestError", [[error toJSONString] cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }

    void com_funplus_sdk_social_facebook_sendGameRequestWithPlatformId(const char * platformId,const char * message) {
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_sendGameRequestWithPlatformId id = %@, message = %@",[NSString stringWithUTF8String:platformId],[NSString stringWithUTF8String:message]);
        
        NSString *requestFBUserIDs = [NSString stringWithUTF8String:platformId];
        [[FunplusFacebookHelper sharedInstance] sendRequestWithPlatformId:[NSString stringWithUTF8String:platformId] message:[NSString stringWithUTF8String:message] handler:^(NSString* requestId,FunplusError *error) {
            NSLog(@">>>>>>send game request result = %@ %u %@",[error toJSONString],error.code,error.localizedDescription);
            if (error.code == 0) {
                NSString * message = [NSString stringWithFormat:@"{\"request_id\": \"%@\",\"platformId\":\"%@\"}",requestId,requestFBUserIDs];
                NSLog(@">>>>>>com_funplus_sdk_social_facebook_sendGameRequestWithPlatformId result message = %@",message);
                UnitySendMessage(gameObject, "OnFacebookSendGameRequestSuccess", [message cStringUsingEncoding:NSUTF8StringEncoding]);
            }else{
                UnitySendMessage(gameObject, "OnFacebookSendGameRequestError", [[error toJSONString] cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }
    
    void com_funplus_sdk_social_facebook_shareLink(const char * title, const char *description, const char *url, const char *imageUrl) {
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_shareLink %@ %@ %@ %@",[NSString stringWithUTF8String:title],[NSString stringWithUTF8String:description],[NSString stringWithUTF8String:url],[NSString stringWithUTF8String:imageUrl]);
        [[FunplusFacebookHelper sharedInstance] shareWithTitle:[NSString stringWithUTF8String:title] message:[NSString stringWithUTF8String:description] link:[NSString stringWithUTF8String:url] pic:[NSString stringWithUTF8String:imageUrl] handler:^(FunplusError *error) {
            if (error.code == 0) {
                UnitySendMessage(gameObject, "OnFacebookShareSuccess", "success");
            }else{
                UnitySendMessage(gameObject, "OnFacebookShareError", "error");
            }
        }];
    }
    
    void com_funplus_sdk_social_facebook_shareImage(const char * title, const char *description, const char *url, const char *imagePath) {
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_shareImage image path %@",[NSString stringWithUTF8String:imagePath]);
        [[FunplusFacebookHelper sharedInstance] shareWithImage:[NSString stringWithUTF8String:imagePath] message:[NSString stringWithUTF8String:description] link:[NSString stringWithUTF8String:url] handler:^(FunplusError *error) {
            NSLog(@"share image result %@",[error toJSONString]);
            if (error.code == 0) {
                UnitySendMessage(gameObject, "OnFacebookShareSuccess", "success");
            }else{
                UnitySendMessage(gameObject, "OnFacebookShareSuccess", "error");
            }
        }];
    }
    
    void com_funplus_sdk_social_facebook_shareOpenGraphStory() {
        NSLog(@">>>>>>com_funplus_sdk_social_facebook_shareOpenGraphStory");
    }

}
