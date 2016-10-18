/**
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015-Present Funplus
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

namespace Funplus.Internal
{

	public class InsertionTmpl
	{

		public class AndroidTmpl
		{

			public static readonly string SETTINGS_GRADLE = @"include ':{0}'";

			public static readonly string BUILD_GRADLE = @"buildscript {{
    repositories {{
        jcenter()
    }}
    dependencies {{
        classpath 'com.android.tools.build:gradle:1.5.0'
    }}
}}

allprojects {{
    repositories {{
        jcenter()
    }}
}}";

			public static readonly string PRODUCT_BUILD_GRADLE = @"apply plugin: 'com.android.application'

android {{
    compileSdkVersion {0}
    buildToolsVersion ""{1}""

    defaultConfig {{
        applicationId ""{2}""
        minSdkVersion {3}
        targetSdkVersion {4}
    }}

    buildTypes {{
        release {{
            minifyEnabled false
        }}
    }}

    sourceSets {{
        main {{
            manifest.srcFile 'AndroidManifest.xml'
            java.srcDirs = ['src']
            res.srcDirs = ['res']
            assets.srcDirs = ['assets']
            jniLibs.srcDir 'libs'
        }}
    }}
}}
repositories {{
    flatDir {{
        dirs '../funplus/aars'
    }}
}}

dependencies {{
    compile fileTree(dir: 'libs', include: ['*.jar'])
    compile(name:'funplus-sdk-account', ext:'aar')
    compile(name:'funplus-sdk-bi', ext:'aar')
    compile(name:'funplus-sdk-caffeine', ext:'aar')
    compile(name:'funplus-sdk-caffeine-gcm', ext:'aar')
    compile(name:'funplus-sdk-core', ext:'aar')
    compile(name:'funplus-sdk-helpshift', ext:'aar')
    compile(name:'funplus-sdk-marketing', ext:'aar')
    compile(name:'funplus-sdk-marketing-adjust', ext:'aar')
    compile(name:'funplus-sdk-marketing-chartboost', ext:'aar')
    compile(name:'funplus-sdk-marketing-localytics', ext:'aar')
    compile(name:'funplus-sdk-payment', ext:'aar')
    compile(name:'funplus-sdk-payment-googleiab', ext:'aar')
    compile(name:'funplus-sdk-rum', ext:'aar')
    compile(name:'funplus-sdk-social', ext:'aar')
    compile(name:'funplus-sdk-social-facebook', ext:'aar')
    compile(name:'funplus-sdk-unity-wrapper', ext:'aar')
    compile(name:'support-v4-23.1.1', ext:'aar')
    compile(name:'appcompat-v7-23.1.1', ext:'aar')
    compile(name:'cardview-v7-23.1.1', ext:'aar')
    compile(name:'recyclerview-v7-23.1.1', ext:'aar')
    compile(name:'design-23.1.1', ext:'aar')
    compile(name:'facebook-android-sdk-4.8.2', ext:'aar')
    compile(name:'helpshift-4.2.0-support', ext:'aar')
    compile(name:'play-services-base-8.4.0', ext:'aar')
    compile(name:'play-services-basement-8.4.0', ext:'aar')
    compile(name:'play-services-gcm-8.4.0', ext:'aar')
}}";

			public static readonly string FACEBOOK = @"
	<activity
		android:name=""com.facebook.FacebookActivity""
		android:configChanges=""keyboard|keyboardHidden|screenLayout|screenSize|orientation""
		android:label=""{0}""
		android:theme=""@android:style/Theme.Translucent.NoTitleBar"" />
	<provider
		android:name=""com.facebook.FacebookContentProvider""
		android:authorities=""com.facebook.app.FacebookContentProvider{1}""
		android:exported=""true"" />";

			public static readonly string GCM = @"
    <receiver
        android:name=""com.google.android.gms.gcm.GcmReceiver""
        android:exported=""true""
        android:permission=""com.google.android.c2dm.permission.SEND"" >
        <intent-filter>
            <action android:name=""com.google.android.c2dm.intent.RECEIVE"" />
            <category android:name=""{0}"" />
        </intent-filter>
    </receiver>

    <service
        android:name=""com.funplus.sdk.caffeine.gcm.FunplusGcmListenerService""
        android:exported=""false"" >
        <intent-filter>
            <action android:name=""com.google.android.c2dm.intent.RECEIVE"" />
        </intent-filter>
    </service>

    <service
        android:name=""com.funplus.sdk.caffeine.gcm.FunplusInstanceIDListenerService""
        android:exported=""false"">
        <intent-filter>
            <action android:name=""com.google.android.gms.iid.InstanceID""/>
        </intent-filter>
    </service>

    <service
        android:name=""com.funplus.sdk.caffeine.gcm.FunplusRegisterIntentService""
        android:exported=""false"">
    </service>";

			public static readonly string INSTALL_REFERER = @"
    <receiver
        android:name=""com.funplus.sdk.FunplusInstallReferrerReceiver""
        android:exported=""true"" >
        <intent-filter>
            <action android:name=""com.android.vending.INSTALL_REFERRER"" />
        </intent-filter>
    </receiver>";

			public static readonly string RUM = @"
	<receiver
        android:name=""com.funplus.sdk.rum.NetworkChangeReceiver""
        android:exported=""true"">
        <intent-filter>
            <action android:name=""android.net.conn.CONNECTIVITY_CHANGE"" />
        </intent-filter>
    </receiver>";

			public static readonly string PERMISSION_BILLING = @"<uses-permission android:name=""com.android.vending.BILLING"" />";
			public static readonly string PERMISSION_RECEIVE = @"<uses-permission android:name=""com.google.android.c2dm.permission.RECEIVE"" />";
			public static readonly string PERMISSION_WEAK_LOCK = @"<uses-permission android:name=""android.permission.WAKE_LOCK"" />";

			public static readonly string IMPORT_SDK = @"import com.funplus.sdk.FunplusSdk;";
			public static readonly string IMPORT_INTENT = @"import android.content.Intent;";

			public static readonly string ON_DESCROY = @"FunplusSdk.onDestroy(this);";
			
			public static readonly string ON_PAUSE = @"FunplusSdk.onPause(this);";
			
			public static readonly string ON_RESUME = @"FunplusSdk.onResume(this);";
			
			public static readonly string ON_ACTIVITY_RESULT = @"
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {{
		super.onActivityResult(requestCode, resultCode, data);
		FunplusSdk.onActivityResult(this, requestCode, resultCode, data);
	}}

	@Override
	protected void onStart() {{
		super.onStart();
		FunplusSdk.onStart(this);
	}}

	@Override
	public void onStop() {{
		super.onStop();
		FunplusSdk.onStop(this);
	}}";
			
		}

		public class XcodeTmpl
		{
			public static readonly string IMPORT_SDK = @"#import <FunplusSdk/FunplusSdk.h>";

			public static readonly string DID_FINISHED_LAUNCHING = @"
	[[FunplusSdk sharedInstance] application:application didFinishLaunchingWithOptions:launchOptions];";

			public static readonly string DID_BECOME_ACTIVE = @"
	[[FunplusSdk sharedInstance] appDidBecomeActive];";

			public static readonly string WILL_RESIGN_ACTIVE = @"
	[[FunplusSdk sharedInstance] appWillResignActive];";

			public static readonly string NOTIFICATION = @"
	[[FunplusSdk sharedInstance] handleRemoteNotification:userInfo];";

			public static readonly string REGISTER_DEVICE_TOKEN = @"
	[[FunplusSdk sharedInstance] registerDeviceToken:deviceToken];";

			public static readonly string SEND_NOTIFICATION = @"
	AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);
	return [[FunplusSdk sharedInstance] appDidOpenURL: url application:application sourceApp:sourceApplication annotation:annotation];";
		}
	}

}