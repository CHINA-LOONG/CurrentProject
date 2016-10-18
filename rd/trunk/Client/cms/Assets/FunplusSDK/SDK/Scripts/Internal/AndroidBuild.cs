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

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Funplus.Internal
{

	public class AndroidBuild
	{
		private static readonly string[] SKIP_FILES = {@".*\.meta$"};

		private static string BuildPath { get; set; }
		private static List<string> EnabledScenes { get; set; }
		private static BuildOptions Options { get; set; }

		private static string ProductId { get; set; }
		private static string ProductName { get; set; }
		private static string SdkPath { get; set; }
		private static string ProductOutPath { get; set; }
		private static string SdkOutPath { get; set; }

		public static void Build(bool debuggable)
		{
			BuildPath = SettingUtils.ChooseBuildPath ();

			if (BuildPath == null)
			{
				return;
			}

			EnabledScenes = SettingUtils.GetEnabledScenes ();
			Options = SettingUtils.GenBuildOptions (debuggable);

			ProductId = PlayerSettings.bundleIdentifier;
			ProductName = PlayerSettings.productName;
			SdkPath = Path.Combine (Application.dataPath, "FunplusSDK/Plugins/Android/funplus-android-sdk");
			ProductOutPath = Path.Combine (BuildPath, ProductName);
			SdkOutPath = Path.Combine (BuildPath, "funplus");

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

			BeforeBuildPlayer ();
			RunBuildPlayer ();
			AfterBuildPlayer ();

			ValidateBuild ();

			UnityEngine.Debug.Log ("Build success!");
		}
			
		private static void BeforeBuildPlayer ()
		{
			if (!Directory.Exists (BuildPath))
			{
				Directory.CreateDirectory (BuildPath);
			}

			DirectoryInfo di = new DirectoryInfo (BuildPath);
			foreach (FileInfo file in di.GetFiles())
			{
				file.Delete(); 
			}
			foreach (DirectoryInfo dir in di.GetDirectories())
			{
				dir.Delete(true); 
			}
		}

		private static void RunBuildPlayer ()
		{
			BuildPipeline.BuildPlayer (EnabledScenes.ToArray (), BuildPath, BuildTarget.Android, Options);
		}

		private static void AfterBuildPlayer ()
		{
			foreach (string file in Directory.GetFiles(Path.Combine (ProductOutPath, "libs"), "*.aar"))
			{
				File.Delete(file);
			}

			SettingUtils.CopyDirectory (SdkOutPath, SdkPath, SKIP_FILES);

			GenRootSettingsGradle ();
			GenRootBuildGradle ();
			GenProductBuildGradle ();
			ModifyAndroidManifest ();
			ModifyLauncherActivity ();
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
			File.WriteAllText (
				Path.Combine (ProductOutPath, "build.gradle"),
				string.Format (
					InsertionTmpl.AndroidTmpl.PRODUCT_BUILD_GRADLE,
					FunplusSettings.AndroidCompileSdkVersion,
					FunplusSettings.AndroidBuildToolsVersion,
					ProductId,
					FunplusSettings.AndroidMinSdkVersion,
					FunplusSettings.AndroidTargetSdkVersion
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

			sdkAndroidManifest.Write ();
		}

		private static void ModifyLauncherActivity ()
		{
			string activityPath = "src/" + FunplusSettings.AndroidLauncherActivity.Replace ('.', '/');
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