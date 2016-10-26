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

﻿using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Funplus.Internal
{
	[CustomEditor(typeof(FunplusSettings))]
	public class FunplusSettingsEditor : Editor
	{
		private bool showAndroidSettings = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
		private bool showIOSSettings = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
		private bool showFacebookSettings = true;

		private GUIContent funplusGameIdLabel = new GUIContent ("Funplus Game Id [?]:", "Funplus Game Id can be found at http://developer.funplus.com");
		private GUIContent funplusGameKeyLabel = new GUIContent ("Funplus Game Key [?]:", "Funplus Game Key can be found at http://developer.funplus.com");
		private GUIContent funplusEnvironmentLabel = new GUIContent ("Environment [?]:", "production or sandbox, default is sandbox");
		private GUIContent fakeDeviceIdLabel = new GUIContent ("Fake Device Id [?]:", "provide a unique device ID which can be used for testing, like this: \"ffbc8c7432c35b4402:00:00:00:00:00\"");

		private GUIContent facebookEnabledLabel = new GUIContent ("Enable Facebook?");
		private GUIContent facebookAppNameLabel = new GUIContent ("Facebook App Name [?]:", "Facebook App Name can be found at https://developers.facebook.com/apps");
		private GUIContent facebookAppIdLabel = new GUIContent ("Facebook App Id [?]:", "Facebook App Id can be found at https://developers.facebook.com/apps");
		private GUIContent facebookDebugAccessTokenLabel = new GUIContent("Debug Access Token [?]:", "A testing acess token");

		private GUIContent androidLauncherActivityLabel = new GUIContent ("Launcher Activity [?]:", "Identifier of the Android launcher activity (without package name, e.g. UnityPlayerActivity");
		private GUIContent androidCompileSdkVersionLabel = new GUIContent ("Compile SDK Version [?]:", "Android Compile SDK version");
		private GUIContent androidBuildToolsVersionLabel = new GUIContent ("Build Tools Version [?]:", "Android Build Tools version");
		private GUIContent androidMinSdkVersionLabel = new GUIContent ("Min SDK Version [?]:", "Android Min SDK version");
		private GUIContent androidTargetSdkVersionLabel = new GUIContent ("Target SDK Version [?]:", "Android Target SDK version");
		private GUIContent androidKeystorePathLabel = new GUIContent ("Keystore Path [?]:", "For release version");
		private GUIContent androidKeystorePasswordLabel = new GUIContent ("Keystore Password [?]:", "The password for the keystore file");
		private GUIContent androidKeystoreAliasLabel = new GUIContent ("Keystore alias [?]:", "The alias for the keystore file");
		private GUIContent androidKeystoreAliasPasswordLabel = new GUIContent ("Keystore alias password [?]:", "The password for this alias");
		private GUIContent androidIabEnabledLabel = new GUIContent ("Enable IAB?");
		private GUIContent androidHelpshiftEnabledLabel = new GUIContent ("Enable HelpShift?");
		private GUIContent androidMarketingEnabledLabel = new GUIContent ("Enable Marketing?");
		private GUIContent androidGcmEnabledLabel = new GUIContent ("Enable GCM?");
		private GUIContent androidRumEnabledLabel = new GUIContent ("Enable Rum?");

		private GUIContent iosAppControllerLabel = new GUIContent ("Application Controller [?]:", "iOS application controller");

//		private GUIContent sdkVersionLabel = new GUIContent ("Funplus SDK Version:");

		public override void OnInspectorGUI ()
		{
			this.FunplusGameIdGUI ();
			EditorGUILayout.Separator ();

			this.AndroidSettingsGUI ();
			EditorGUILayout.Separator ();

			this.IOSSettingsGUI ();
			EditorGUILayout.Separator ();

			this.FacebookSettingsGUI ();
			EditorGUILayout.Separator ();

			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			this.BuildGUI ();
		}

		private void FunplusGameIdGUI ()
		{
			EditorGUILayout.Foldout (true, "Funplus Settings");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (this.funplusGameIdLabel);
			FunplusSettings.FunplusGameId = EditorGUILayout.TextField (FunplusSettings.FunplusGameId);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (this.funplusGameKeyLabel);
			FunplusSettings.FunplusGameKey = EditorGUILayout.TextField (FunplusSettings.FunplusGameKey);
			EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (this.funplusEnvironmentLabel);
			FunplusSettings.Environment = EditorGUILayout.TextField (FunplusSettings.Environment);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (this.fakeDeviceIdLabel);
			FunplusSettings.FakeDeviceId = EditorGUILayout.TextField (FunplusSettings.FakeDeviceId);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
		}

		private void FacebookSettingsGUI ()
		{
			this.showFacebookSettings = EditorGUILayout.Foldout (this.showFacebookSettings, "Facebook Settings");
			if (this.showFacebookSettings) {
				EditorGUILayout.BeginHorizontal ();
				FunplusSettings.FacebookEnabled = EditorGUILayout.Toggle (this.facebookEnabledLabel, FunplusSettings.FacebookEnabled);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.facebookAppNameLabel);
				FunplusSettings.FacebookAppName = EditorGUILayout.TextField (FunplusSettings.FacebookAppName);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.facebookAppIdLabel);
				FunplusSettings.FacebookAppId = EditorGUILayout.TextField (FunplusSettings.FacebookAppId);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.facebookDebugAccessTokenLabel);
				FunplusSettings.FacebookDebugAccessToken = EditorGUILayout.TextField (FunplusSettings.FacebookDebugAccessToken);
				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();
		}

		private void AndroidSettingsGUI ()
		{
			this.showAndroidSettings = EditorGUILayout.Foldout (this.showAndroidSettings, "Android Settings");
			if (this.showAndroidSettings)
			{
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.androidLauncherActivityLabel);
				FunplusSettings.AndroidLauncherActivity = EditorGUILayout.TextField (FunplusSettings.AndroidLauncherActivity);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.androidCompileSdkVersionLabel);
				FunplusSettings.AndroidCompileSdkVersion = EditorGUILayout.TextField (FunplusSettings.AndroidCompileSdkVersion);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.androidBuildToolsVersionLabel);
				FunplusSettings.AndroidBuildToolsVersion = EditorGUILayout.TextField (FunplusSettings.AndroidBuildToolsVersion);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.androidMinSdkVersionLabel);
				FunplusSettings.AndroidMinSdkVersion = EditorGUILayout.TextField (FunplusSettings.AndroidMinSdkVersion);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.androidTargetSdkVersionLabel);
				FunplusSettings.AndroidTargetSdkVersion = EditorGUILayout.TextField (FunplusSettings.AndroidTargetSdkVersion);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.androidKeystorePathLabel);
				FunplusSettings.AndroidKeystorePath = EditorGUILayout.TextField (FunplusSettings.AndroidKeystorePath);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.androidKeystorePasswordLabel);
				FunplusSettings.AndroidKeystorePassword = EditorGUILayout.TextField (FunplusSettings.AndroidKeystorePassword);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.androidKeystoreAliasLabel);
				FunplusSettings.AndroidKeystoreAlias = EditorGUILayout.TextField (FunplusSettings.AndroidKeystoreAlias);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.androidKeystoreAliasPasswordLabel);
				FunplusSettings.AndroidKeystoreAliasPassword = EditorGUILayout.TextField (FunplusSettings.AndroidKeystoreAliasPassword);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				FunplusSettings.AndroidIabEnabled = EditorGUILayout.Toggle (this.androidIabEnabledLabel, FunplusSettings.AndroidIabEnabled);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				FunplusSettings.AndroidHelpshiftEnabled = EditorGUILayout.Toggle (this.androidHelpshiftEnabledLabel, FunplusSettings.AndroidHelpshiftEnabled);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				FunplusSettings.AndroidMarketingEnabled = EditorGUILayout.Toggle (this.androidMarketingEnabledLabel, FunplusSettings.AndroidMarketingEnabled);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				FunplusSettings.AndroidGcmEnabled = EditorGUILayout.Toggle (this.androidGcmEnabledLabel, FunplusSettings.AndroidGcmEnabled);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				FunplusSettings.AndroidRumEnabled = EditorGUILayout.Toggle (this.androidRumEnabledLabel, FunplusSettings.AndroidRumEnabled);
				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();
		}

		private void IOSSettingsGUI ()
		{
			this.showIOSSettings = EditorGUILayout.Foldout (this.showIOSSettings, "iOS Settings");
			if (this.showIOSSettings) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (this.iosAppControllerLabel);
				FunplusSettings.IosAppController = EditorGUILayout.TextField (FunplusSettings.IosAppController);
				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();
		}

		private void BuildGUI ()
		{	
//			EditorGUILayout.BeginHorizontal ();
//			EditorGUILayout.LabelField (this.sdkVersionLabel);
//			FunplusSettings.SdkVersion = EditorGUILayout.TextField (FunplusSettings.SdkVersion);
//			EditorGUILayout.EndHorizontal ();

			if (GUILayout.Button ("Export Android Project (Debug)"))
			{
				try
				{
					EditorApplication.delayCall += ExportAndroidDebugProject;
				}
				catch (System.Exception e)
				{
					Debug.LogError ("Error exporting Android project (Debug): " + e.Message);
				}
			}

			if (GUILayout.Button ("Export Android Project (Release)"))
			{
				try
				{
					EditorApplication.delayCall += ExportAndroidReleaseProject;
				}
				catch (System.Exception e)
				{
					Debug.LogError ("Error exporting Android project (Release): " + e.Message);
				}
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Export SDK Package"))
			{
				try
				{
					EditorApplication.delayCall += ExportPackage;
				}
				catch (System.Exception e)
				{
					Debug.LogError ("Error exporting unity package: " + e.Message);
				}
			}
		}

		private void ExportPackage ()
		{
			FunplusBuild.ExportPackage ();
		}

		private void ExportAndroidDebugProject ()
		{
			#if UNITY_EDITOR
			FunplusSettings.ExportAndroidDebugProject ();
			#endif
		}

		private void ExportAndroidReleaseProject ()
		{
			#if UNITY_EDITOR
			FunplusSettings.ExportAndroidReleaseProject ();
			#endif
		}
	}

}
