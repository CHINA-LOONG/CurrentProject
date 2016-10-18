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

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Diagnostics;

namespace Funplus.Internal
{

	public class InstallSdkInXcode
	{

		private static readonly string[] SKIP_FILES = {@".*\.meta$"};

		private static string BuildPath { get; set; }
		private static BuildOptions Options { get; set; }

		private static string ProductOutPath { get; set; }
		private static string SdkOutPath { get; set; }

		private static string FunplusBundlePath { get; set; }
		private static string FunplusBundleOutPath { get; set; }
		private static string HsThemePath { get; set; }
		private static string HsThemeOutPath { get; set; }
		private static string HsLocalizationPath { get; set; }
		private static string HsLocalizationOutPath { get; set; }
		private static string HsAssetCatlogPath { get; set; }
		private static string HsAssetCatlogOutPath { get; set; }

		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget == BuildTarget.iOS)
			{
				AfterBuild (pathToBuiltProject);
			}
		}

		public static void AfterBuild (string buildPath)
		{
			BuildPath = buildPath;

			if (BuildPath == null)
			{
				return;
			}

			// TODO
			Options = SettingUtils.GenBuildOptions (true);

			SdkOutPath = Path.Combine (BuildPath, "Libraries/FunplusSDK/Plugins/iOS/funplus-ios-sdk");
			FunplusBundlePath = Path.Combine (Application.dataPath, "WebPlayerTemplates/FunplusSdkResources.bundle");
			FunplusBundleOutPath = Path.Combine (SdkOutPath, "FunplusSdkResources.bundle");
			HsThemePath = Path.Combine (Application.dataPath, "WebPlayerTemplates/HSThemes");
			HsThemeOutPath = Path.Combine (SdkOutPath, "HSThemes");
			HsLocalizationPath = Path.Combine (Application.dataPath, "WebPlayerTemplates/HSLocalization");
			HsLocalizationOutPath = Path.Combine (SdkOutPath, "HSLocalization");
			HsAssetCatlogPath = Path.Combine (Application.dataPath, "WebPlayerTemplates/HelpshiftAssetCatalog.xcassets");
			HsAssetCatlogOutPath = Path.Combine (SdkOutPath, "HelpshiftAssetCatalog.xcassets");

			SettingUtils.CopyDirectory (FunplusBundleOutPath, FunplusBundlePath, SKIP_FILES);
			SettingUtils.CopyDirectory (HsThemeOutPath, HsThemePath, SKIP_FILES);
			SettingUtils.CopyDirectory (HsLocalizationOutPath, HsLocalizationPath, SKIP_FILES);
			SettingUtils.CopyDirectory (HsAssetCatlogOutPath, HsAssetCatlogPath, SKIP_FILES);

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

			result = SettingUtils.CheckXcodeSettings ();
			if (result.HasError ())
			{
				SettingUtils.LogError (result);
				return;
			}

			RunInstallScript ();
			ModifyAppController ();
			ModifyPlist ();
			AddFrameworks ();
			ValidateBuild ();

			UnityEngine.Debug.Log ("Build success!");
		}

		private static void RunInstallScript ()
		{
			string installScriptDir = Path.Combine (SdkOutPath, "install");
			string installScript = Path.Combine (installScriptDir, "install.sh");
			string xcodeprojFile = Path.Combine (BuildPath, "Unity-iPhone.xcodeproj");

			Process process = new Process();
			process.StartInfo.WorkingDirectory = installScriptDir;
			process.StartInfo.FileName = "bash";
			process.StartInfo.Arguments = installScript + " " + xcodeprojFile;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = false;
			process.Start ();
			process.WaitForExit ();
		}

		private static void ModifyAppController ()
		{
			string appControllerField = FunplusSettings.IosAppController;
			string appControllerClass = Path.Combine (BuildPath, appControllerField);

			if (!File.Exists (appControllerClass)) 
			{
				SettingUtils.LogError (new FieldStatus { Status = FieldStatus.StatusType.InvalidField, Message = "iOS App Controller" });
				return;
			}

			FileModifier appController = new FileModifier (appControllerClass);

			appController.InsertAfter (
				"#include \"PluginBase/AppDelegateListener.h\"",
				InsertionTmpl.XcodeTmpl.IMPORT_SDK
			);

			appController.InsertAfter (
				"::printf(\"-> applicationDidFinishLaunching()\\n\");",
				InsertionTmpl.XcodeTmpl.DID_FINISHED_LAUNCHING
			);

			appController.InsertAfter (
				"::printf(\"-> applicationDidBecomeActive()\\n\");",
				InsertionTmpl.XcodeTmpl.DID_BECOME_ACTIVE
			);

			appController.InsertAfterIfNotExist (
				"::printf(\"-> applicationWillResignActive()\\n\");",
				InsertionTmpl.XcodeTmpl.WILL_RESIGN_ACTIVE
			);

			appController.InsertAfterIfNotExist (
				"UnitySendRemoteNotification(userInfo);",
				InsertionTmpl.XcodeTmpl.NOTIFICATION
			);

			appController.InsertAfter (
				"UnitySendDeviceToken(deviceToken);",
				InsertionTmpl.XcodeTmpl.REGISTER_DEVICE_TOKEN
			);

			appController.Replace (
				"AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);\n\treturn YES;",
				InsertionTmpl.XcodeTmpl.SEND_NOTIFICATION
			);

			appController.Write ();
		}

		private static void ModifyPlist ()
		{
			string plistPath = Path.Combine (BuildPath, "Info.plist");

			if (!File.Exists (plistPath)) 
			{
				SettingUtils.LogError (new FieldStatus { Status = FieldStatus.StatusType.InvalidField, Message = "iOS Plist File" });
				return;
			}

			PlistDocument plist = new PlistDocument ();
			plist.ReadFromFile (plistPath);

			// ********** CFBundleURLTypes ********* //
			PlistElementArray bundleUrlTypes = null;
			if (plist.root.values.ContainsKey ("CFBundleURLTypes"))
			{
				bundleUrlTypes = plist.root ["CFBundleURLTypes"].AsArray ();
			}
			else
			{
				bundleUrlTypes = plist.root.CreateArray ("CFBundleURLTypes");
			}

			PlistElementDict bundleUrlSchemes = bundleUrlTypes.AddDict ();
			PlistElementArray bundleUrlSchemesArray = bundleUrlSchemes.CreateArray ("CFBundleURLSchemes");
			bundleUrlSchemesArray.AddString (string.Format ("fb{0}", FunplusSettings.FacebookAppId));

			// ********** Facebook ********** //
			plist.root.SetString("FacebookAppID", FunplusSettings.FacebookAppId);
			plist.root.SetString("FacebookDisplayName", FunplusSettings.FacebookAppName);

			// ********** NSAppTransportSecurity ********** //
			PlistElementDict appTransportSecurityDict = null;
			if (plist.root.values.ContainsKey ("NSAppTransportSecurity"))
			{
				appTransportSecurityDict = plist.root ["NSAppTransportSecurity"].AsDict ();
			}
			else
			{
				appTransportSecurityDict = plist.root.CreateDict ("NSAppTransportSecurity");
			}
				
			// ********** NSExceptionDomains ********** //
			PlistElementDict exceptionDomains = null;
			if (appTransportSecurityDict.values.ContainsKey ("NSExceptionDomains"))
			{
				exceptionDomains = appTransportSecurityDict ["NSExceptionDomains"].AsDict ();
			}
			else
			{
				exceptionDomains = appTransportSecurityDict.CreateDict ("NSExceptionDomains");
			}

			PlistElementDict facebookCom = null;
			if (exceptionDomains.values.ContainsKey ("facebook.com"))
			{
				facebookCom = exceptionDomains ["facebook.com"].AsDict ();
			}
			else
			{
				facebookCom = exceptionDomains.CreateDict ("facebook.com");
			}

			facebookCom.SetBoolean ("NSIncludesSubdomains", true);
			facebookCom.SetBoolean ("NSThirdPartyExceptionRequiresForwardSecrecy", false);

			PlistElementDict fbcdnNet = null;
			if (exceptionDomains.values.ContainsKey ("fbcdn.net"))
			{
				fbcdnNet = exceptionDomains ["fbcdn.net"].AsDict ();
			}
			else
			{
				fbcdnNet = exceptionDomains.CreateDict ("fbcdn.net");
			}

			fbcdnNet.SetBoolean ("NSIncludesSubdomains", true);
			fbcdnNet.SetBoolean ("NSThirdPartyExceptionRequiresForwardSecrecy", false);

			PlistElementDict akamaihdNet = null;
			if (exceptionDomains.values.ContainsKey ("akamaihd.net"))
			{
				akamaihdNet = exceptionDomains ["akamaihd.net"].AsDict ();
			}
			else
			{
				akamaihdNet = exceptionDomains.CreateDict ("akamaihd.net");
			}

			akamaihdNet.SetBoolean ("NSIncludesSubdomains", true);
			akamaihdNet.SetBoolean ("NSThirdPartyExceptionRequiresForwardSecrecy", false);

			// ********** LSApplicationQueriesSchemes ********** //
			PlistElementArray applicationQueriesSchemes = null;
			if (plist.root.values.ContainsKey ("LSApplicationQueriesSchemes"))
			{
				applicationQueriesSchemes = plist.root ["LSApplicationQueriesSchemes"].AsArray ();
			}
			else
			{
				applicationQueriesSchemes = plist.root.CreateArray ("LSApplicationQueriesSchemes");
			}

			applicationQueriesSchemes.AddString ("fbapi");
			applicationQueriesSchemes.AddString ("fb-messenger-api");
			applicationQueriesSchemes.AddString ("fbauth2");
			applicationQueriesSchemes.AddString ("fbshareextension");

			plist.WriteToFile (plistPath);
		}

		private static void AddFrameworks ()
		{
			string projectFile = Path.Combine (BuildPath, "Unity-iPhone.xcodeproj/project.pbxproj");

			PBXProject pbxProject = new PBXProject ();
			pbxProject.ReadFromFile (projectFile);

			string target = pbxProject.TargetGuidByName ("Unity-iPhone");

			// ********** Frameworks ********** //
			pbxProject.AddFrameworkToProject (target, "AdSupport.framework", false);
			pbxProject.AddFrameworkToProject (target, "AssetsLibrary.framework", false);
			pbxProject.AddFrameworkToProject (target, "MobileCoreServices.framework", false);
			pbxProject.AddFrameworkToProject (target, "SystemConfiguration.framework", false);
			pbxProject.AddFrameworkToProject (target, "Security.framework", false);
			pbxProject.AddFrameworkToProject (target, "CoreTelephony.framework", false);
			pbxProject.AddFrameworkToProject (target, "Social.framework", false);
			pbxProject.AddFrameworkToProject (target, "StoreKit.framework", false);

			// ********** Resources ********** //
			string name = pbxProject.AddFile (FunplusBundleOutPath, FunplusBundleOutPath, PBXSourceTree.Source);
			pbxProject.AddFileToBuild (target, name);
			name = pbxProject.AddFile (HsLocalizationOutPath, HsLocalizationOutPath, PBXSourceTree.Source);
			pbxProject.AddFileToBuild (target, name);
			name = pbxProject.AddFile (HsAssetCatlogOutPath, HsAssetCatlogOutPath, PBXSourceTree.Source);
			pbxProject.AddFileToBuild (target, name);
			name = pbxProject.AddFile (HsThemeOutPath, HsThemeOutPath, PBXSourceTree.Source);
			pbxProject.AddFileToBuild (target, name);

			// ********** Link Flags ********** //
			pbxProject.AddBuildProperty (target, "OTHER_LDFLAGS", "-ObjC -lz -lsqlite3.0");
			pbxProject.WriteToFile (projectFile);
		}

		private static void ValidateBuild ()
		{
			// TODO
		}
	}

}
