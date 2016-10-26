#if UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

namespace Funplus.Internal
{
	public class InstallSdkInAndroid
	{
		private static string BuildPath { get; set; }
		private static BuildOptions Options { get; set; }

		private static string ProductId { get; set; }
		private static string ProductName { get; set; }
		private static string SdkPath { get; set; }
		private static string ProductOutPath { get; set; }
		private static string SdkOutPath { get; set; }

		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget == BuildTarget.Android)
			{
				AfterBuild (pathToBuiltProject);
			}
		}

		public static void AfterBuild (string buildPath)
		{
			BuildPath = buildPath;

			ProductId = PlayerSettings.bundleIdentifier;
			ProductName = PlayerSettings.productName;
			SdkPath = Path.Combine (Application.dataPath, "FunplusSDK/Plugins/Android/funplus-android-sdk");
			ProductOutPath = Path.Combine (BuildPath, ProductName);
			SdkOutPath = Path.Combine (BuildPath, "funplus");

			if (BuildPath == null)
			{
				return;
			}

			FieldStatus result = SettingUtils.CheckFunplusSettings ();
			if (result.HasError ())
			{
				SettingUtils.LogError (result);
				return;
			}

			result = SettingUtils.CheckFacebookSettings ();
			if (result.HasError ())
			{
				SettingUtils.LogError (result);
				return;
			}

			result = SettingUtils.CheckAndroidSettings ();
			if (result.HasError ())
			{
				SettingUtils.LogError (result);
				return;
			}

			GenRootSettingsGradle ();
			GenRootBuildGradle ();
			GenProductBuildGradle ();
			ModifyAndroidManifest ();
			ModifyLauncherActivity ();
			ValidateBuild ();

			UnityEngine.Debug.Log ("Build success!");
		}

		private static void GenRootSettingsGradle ()
		{
			File.WriteAllText (
				Path.Combine (BuildPath, "settings.gradle"),
				string.Format (InsertionTmpl.AndroidTmpl.SETTINGS_GRADLE, ProductName)
			);
		}

		private static void GenRootBuildGradle ()
		{
			File.WriteAllText (
				Path.Combine (BuildPath, "build.gradle"),
				string.Format(InsertionTmpl.AndroidTmpl.BUILD_GRADLE)
			);
		}

		private static void GenProductBuildGradle ()
		{
			string releaseKeystoreConfig = "";

			if (!string.IsNullOrEmpty (FunplusSettings.AndroidKeystorePath))
			{
				releaseKeystoreConfig = string.Format (
					InsertionTmpl.AndroidTmpl.RELEASE_KEYSTORE_CONFIG,
					FunplusSettings.AndroidKeystorePath,
					FunplusSettings.AndroidKeystorePassword,
					FunplusSettings.AndroidKeystoreAlias,
					FunplusSettings.AndroidKeystoreAliasPassword
				);
			}

			File.WriteAllText (
				Path.Combine (ProductOutPath, "build.gradle"),
				string.Format (
					InsertionTmpl.AndroidTmpl.PRODUCT_BUILD_GRADLE,
					FunplusSettings.AndroidCompileSdkVersion,
					FunplusSettings.AndroidBuildToolsVersion,
					ProductId,
					FunplusSettings.AndroidMinSdkVersion,
					FunplusSettings.AndroidTargetSdkVersion,
					releaseKeystoreConfig
				)
			);
		}

		private static void ModifyAndroidManifest ()
		{
			FileModifier sdkAndroidManifest = new FileModifier (Path.Combine (ProductOutPath, "AndroidManifest.xml"));

			if (FunplusSettings.FacebookEnabled)
			{
				sdkAndroidManifest.InsertAfter (
					"</activity>",
					string.Format (InsertionTmpl.AndroidTmpl.FACEBOOK, FunplusSettings.FacebookAppName, FunplusSettings.FacebookAppId)
				);
			}

			sdkAndroidManifest.InsertAfter (
				"</activity>",
				string.Format (InsertionTmpl.AndroidTmpl.GCM, ProductId)
			);

			sdkAndroidManifest.InsertAfter (
				"</activity>",
				string.Format (InsertionTmpl.AndroidTmpl.INSTALL_REFERER)
			);

			sdkAndroidManifest.InsertAfter (
				"</activity>",
				string.Format (InsertionTmpl.AndroidTmpl.RUM)
			);

			sdkAndroidManifest.InsertAfterIfNotExist (
				"</application>",
				InsertionTmpl.AndroidTmpl.PERMISSION_BILLING
			);

			sdkAndroidManifest.InsertAfterIfNotExist (
				"</application>",
				InsertionTmpl.AndroidTmpl.PERMISSION_RECEIVE
			);

			sdkAndroidManifest.InsertAfterIfNotExist (
				"</application>",
				InsertionTmpl.AndroidTmpl.PERMISSION_WEAK_LOCK
			);

//			if (debuggable)
//			{
//				sdkAndroidManifest.Replace ("android:debuggable=\"true\"", "");
//			}
//			else
//			{
//				sdkAndroidManifest.Replace ("android:debuggable=\"true\"", "");
//				sdkAndroidManifest.Replace ("android:debuggable=\"false\"", "");
//			}
			sdkAndroidManifest.Write ();
		}

		private static void ModifyLauncherActivity ()
		{
			string launcherActivity = FunplusSettings.AndroidLauncherActivity;
			if (!launcherActivity.Contains (ProductId))
			{
				launcherActivity = string.Format("{0}.{1}", ProductId, launcherActivity);
			}

			string activityPath = "src/" + launcherActivity.Replace ('.', '/');
			string activityClass = Path.Combine (ProductOutPath, activityPath + ".java");

			if (!File.Exists (activityClass)) 
			{
				SettingUtils.LogError (new FieldStatus { Status = FieldStatus.StatusType.InvalidField, Message = "Android Launcher Activity" });
				return;
			}

			FileModifier unityPlayerActivity = new FileModifier (activityClass);

			unityPlayerActivity.InsertAfterIfNotExist (
				"import android.view.WindowManager;",
				InsertionTmpl.AndroidTmpl.IMPORT_SDK
			);

			unityPlayerActivity.InsertAfterIfNotExist (
				"import android.view.WindowManager;",
				InsertionTmpl.AndroidTmpl.IMPORT_INTENT
			);

			unityPlayerActivity.InsertAfter (
				"super.onDestroy();",
				InsertionTmpl.AndroidTmpl.ON_DESCROY
			);

			unityPlayerActivity.InsertAfter (
				"super.onPause();",
				InsertionTmpl.AndroidTmpl.ON_PAUSE
			);

			unityPlayerActivity.InsertAfter (
				"super.onResume();",
				InsertionTmpl.AndroidTmpl.ON_RESUME
			);

			unityPlayerActivity.InsertAfter (
				"mUnityPlayer.requestFocus();\n\t}",
				InsertionTmpl.AndroidTmpl.ON_ACTIVITY_RESULT
			);

			unityPlayerActivity.Write ();
		}

		private static void ValidateBuild ()
		{
			// TODO
		}
	}
}

#endif