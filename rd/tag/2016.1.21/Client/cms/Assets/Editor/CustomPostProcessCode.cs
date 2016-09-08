using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
using System.Xml;
using System.IO;

public static class CustomPostProcessCode
{
	[PostProcessBuild (101)]
	public static void OnPostProcessBuild (BuildTarget target, string pathToBuiltProject)
	{
#if UNITY_IOS
		string path = Path.GetFullPath (pathToBuiltProject);
		XClassHelper UnityAppController = new XClassHelper(path + "/Classes/UnityAppController.mm");
		
		UnityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"","#import <FunplusSdk/FunplusSdk.h>");
		UnityAppController.Replace("AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);\n\treturn YES;", "AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);\n\treturn [[FunplusSdk sharedInstance] appDidOpenURL: url sourceApp:sourceApplication];");
		UnityAppController.WriteBelow("_didResignActive = false;", "\t[[FunplusSdk sharedInstance] appDidBecomeActive];");
		//Add push notification handle
		UnityAppController.WriteBelow("UnitySendRemoteNotification(userInfo);", "\t[[FunplusSdk sharedInstance] handleRemoteNotification:userInfo];");
		UnityAppController.WriteBelow("UnitySendDeviceToken(deviceToken);", "\t[[FunplusSdk sharedInstance] registerDeviceToken:deviceToken];");

		
#elif UNITY_ANDROID
		string proName = PlayerSettings.productName;
		UnityEngine.Debug.Log("----CustomPostProcessCode android productName: -----" + proName);

		string curPath = path + "/" + proName + "/src";

		string bundleId = PlayerSettings.bundleIdentifier;
		UnityEngine.Debug.Log("----CustomPostProcessCode android bundleId: -----" + bundleId);
		//get paths from bundleId
		string[] paths = bundleId.Split('.');
		foreach (string s in paths)
		{
			curPath = curPath + "/" + s;
		}

		UnityEngine.Debug.Log("----CustomPostProcessCode android curPath: -----" + curPath);

		XClassHelper UnityPlayerActivity = new XClassHelper(curPath + "/UnityPlayerActivity.java" );

		//add import file and 
		UnityPlayerActivity.WriteBelow("import android.view.WindowManager;", "import android.support.v4.app.FragmentActivity;");
		UnityPlayerActivity.WriteBelow("import android.view.WindowManager;", "import com.funplus.sdk.FunplusSdk;");
		UnityPlayerActivity.WriteBelow("import android.view.WindowManager;", "import android.content.Intent;");

		UnityPlayerActivity.Replace("public class UnityPlayerActivity extends Activity", "public class UnityPlayerActivity extends FragmentActivity");

		//add onXXX callback
		UnityPlayerActivity.WriteBelow("mUnityPlayer.quit();", "\t\tFunplusSdk.onDestroy(this);");
		UnityPlayerActivity.WriteBelow("super.onPause();", "\t\tFunplusSdk.onPause(this);");
		UnityPlayerActivity.WriteBelow("super.onResume();", "\t\tFunplusSdk.onResume(this);");

		//add onActivityResult
		UnityPlayerActivity.WriteBelow("return super.dispatchKeyEvent(event);\n\t}", "\n\t@Override\n\tprotected void onActivityResult(int requestCode, int resultCode, Intent data) {\n\t\tsuper.onActivityResult(requestCode, resultCode, data);\n\t\tFunplusSdk.onActivityResult(this, requestCode, resultCode, data);\n\t}");
		//add onNewIntent
		UnityPlayerActivity.WriteBelow("return super.dispatchKeyEvent(event);\n\t}", "\n\t@Override\n\tprotected void onNewIntent(Intent intent) {\n\t\tsuper.onNewIntent(intent);\n\t\tFunplusSdk.onNewIntent(this, intent);\n\t}");
		//add onBackPressed
		UnityPlayerActivity.WriteBelow("return super.dispatchKeyEvent(event);\n\t}", "\n\t@Override\n\tpublic void onBackPressed() {\n\t\tsuper.onBackPressed();\n\t\tFunplusSdk.onBackPressed(this);\n\t}");
		
#endif

    }

}