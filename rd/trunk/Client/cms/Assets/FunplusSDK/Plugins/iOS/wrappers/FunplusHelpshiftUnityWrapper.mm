//
//  FunplusHelpshiftUnityWrapper.c
//  Unity-iPhone
//
//  Created by 张远坤 on 11/24/15.
//
//

#include <string>
#include <FunplusSdk/FunplusSdk.h>

extern "C" {
    
    void com_funplus_sdk_helpshift_showConversation() {
        Class cls = NSClassFromString(@"FunplusHelpShift");
        [[cls sharedInstance] showConversation];
    }
    
    void com_funplus_sdk_helpshift_showFAQs() {
        Class cls = NSClassFromString(@"FunplusHelpShift");
        [[cls sharedInstance] showFAQs];
    }
    
}
