#if UNITY_ANDROID || UNITY_IPHONE

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using HSMiniJSON;

public static class BuildPostProcessor {
	
	static readonly string[] SKIP_FILES = {@".*\.meta$"};
	
	[PostProcessBuild(101)]
	public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject) {
		UnityEngine.Debug.Log ("Running post build script, product path: " + pathToBuiltProject);

		string buildPath = Path.GetFullPath (pathToBuiltProject);
		
#if UNITY_ANDROID
		string productName = PlayerSettings.productName;
		string bundleId = PlayerSettings.bundleIdentifier;
		
		string productPath = Path.Combine(buildPath, productName);

		string sdkPath = Path.Combine (productPath, "../funplus-android-sdk");

		if (Directory.Exists (sdkPath)) {
			Directory.Delete (sdkPath, true);
		}
		
		CopyDirectory (sdkPath, "Assets/Plugins/Android/funplus-android-sdk", SKIP_FILES);
		
		string packagePath = "src/" + bundleId.Replace ('.', '/');
		string javaPath = Path.Combine (productPath, packagePath);
		
		FileModifier unityPlayerActivity = new FileModifier (Path.Combine (javaPath, "UnityPlayerActivity.java"));
		unityPlayerActivity.InsertAfter ("import android.view.WindowManager;", "import com.funplus.sdk.FunplusSdk;");
		unityPlayerActivity.InsertAfter ("import android.app.Activity;", "import android.content.Intent;");
		unityPlayerActivity.InsertAfter ("super.onDestroy();", "\t\tFunplusSdk.onDestroy(this);");
		unityPlayerActivity.InsertAfter ("super.onPause();", "\t\tFunplusSdk.onPause(this);");
		unityPlayerActivity.InsertAfter ("super.onResume();", "\t\tFunplusSdk.onResume(this);");
		
		unityPlayerActivity.InsertAfter ("mUnityPlayer.requestFocus();\n\t}",
		                                 "\t@Override\n" +
		                                 "\tprotected void onActivityResult(int requestCode, int resultCode, Intent data) {\n" +
		                                 "\t\tsuper.onActivityResult(requestCode, resultCode, data);\n" +
		                                 "\t\tFunplusSdk.onActivityResult(this, requestCode, resultCode, data);\n" +
		                                 "\t}\n\n" +
		                                 "\t@Override\n" +
		                                 "\tprotected void onStart() {\n" +
		                                 "\t\tsuper.onStart();\n" +
		                                 "\t\tFunplusSdk.onStart(this);\n" +
		                                 "\t}\n\n" +
		                                 "\n\t@Override\n" +
		                                 "\tpublic void onStop() {\n" +
		                                 "\t\tsuper.onStop();\n" +
		                                 "\t\tFunplusSdk.onStop(this);\n" +
		                                 "\t}");
		unityPlayerActivity.Write ();
		
		FileModifier projectProperties = new FileModifier(Path.Combine (productPath, "project.properties"));
		projectProperties.Append ("manifestmerger.enabled=true");
		projectProperties.Append ("android.library.reference.1=../funplus-android-sdk/funplus");
		projectProperties.Write ();

#elif UNITY_IPHONE
		string ocPath = Path.Combine (buildPath, "Classes");
		FileModifier unityAppController = new FileModifier(Path.Combine (ocPath, "UnityAppController.mm"));

		UnityEngine.Debug.Log ("Insert necessary codes to UnityAppController.mm");
		unityAppController.InsertAfter ("#include \"PluginBase/AppDelegateListener.h\"", "#import <FunplusSdk/FunplusSdk.h>");
		unityAppController.InsertAfter ("_didResignActive = false;", "\t[[FunplusSdk sharedInstance] appDidBecomeActive];");
		unityAppController.InsertAfter ("UnitySendRemoteNotification(userInfo);", "\t[[FunplusSdk sharedInstance] handleRemoteNotification:userInfo];");
		unityAppController.InsertAfter ("UnitySendDeviceToken(deviceToken);", "\t[[FunplusSdk sharedInstance] registerDeviceToken:deviceToken];");

		UnityEngine.Debug.Log ("Replace codes in UnityAppController.mm");
		unityAppController.Replace ("AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);\n\treturn YES;",
		                            "AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);\n" +
		                            "\treturn [[FunplusSdk sharedInstance] appDidOpenURL: url sourceApp:sourceApplication];");
		unityAppController.Write ();

		string sdkPath = Path.Combine (buildPath, "../funplus-ios-sdk");

		if (Directory.Exists (sdkPath)) {
			Directory.Delete (sdkPath, true);
		}

		UnityEngine.Debug.Log ("Copy funplus-ios-sdk");
		CopyDirectory (sdkPath, "funplus-ios-sdk", SKIP_FILES);

		UnityEngine.Debug.Log ("Install funplus-ios-sdk");
		string installScript = Path.Combine (sdkPath, "install/install.sh");
		string xcodeprojFile = Path.Combine (buildPath, "Unity-iPhone.xcodeproj");
		string installScriptDir = Path.Combine (sdkPath, "install");

		Process process = new Process();
		process.StartInfo.WorkingDirectory = installScriptDir;
		process.StartInfo.FileName = "bash";
		process.StartInfo.Arguments = installScript + " " + xcodeprojFile;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = false;
		process.Start ();
		process.WaitForExit ();

		string plistPath = Path.Combine (buildPath, "Info.plist");

		string settingsText = string.Empty;
		using (StreamReader streamReader = new StreamReader("funsdk-settings.json", Encoding.UTF8))
		{            
			settingsText = streamReader.ReadToEnd();
		}

		var settings = Json.Deserialize (settingsText) as Dictionary<string,object>;
		String facebookAppId = (string) settings["facebook_app_id"];
		String facebookAppName = (string) settings["facebook_app_name"];

		UnityEngine.Debug.Log ("Insert necessary settings to Info.plist");
		XmlDocument plist = new XmlDocument ();
		plist.Load (plistPath);

		if (facebookAppId != null) {
			XmlElement doc = (XmlElement) plist.DocumentElement.SelectSingleNode ("dict");

			XmlElement urlTypesKey = plist.CreateElement ("key");
			XmlElement urlTypesArray = plist.CreateElement ("array");
			XmlElement urlTypes = plist.CreateElement ("dict");
			XmlElement urlSchemesKey = plist.CreateElement ("key");
			XmlElement urlSchemesArray = plist.CreateElement ("array");
			XmlElement urlSchemes = plist.CreateElement ("string");
			XmlElement appIdKey = plist.CreateElement ("key");
			XmlElement appIdValue = plist.CreateElement ("string");
			XmlElement appNameKey = plist.CreateElement ("key");
			XmlElement appNameValue = plist.CreateElement ("string");

			urlTypesKey.InnerText = "CFBundleURLTypes";
			urlTypesArray.AppendChild (urlTypes);
			urlTypes.AppendChild (urlSchemesKey);
			urlTypes.AppendChild (urlSchemesArray);
			urlSchemesKey.InnerText = "CFBundleURLSchemes";
			urlSchemesArray.AppendChild (urlSchemes);
			urlSchemes.InnerText = "fb" + facebookAppId;
			appIdKey.InnerText = "FacebookAppID";
			appIdValue.InnerText = facebookAppId;
			appNameKey.InnerText = "FacebookDisplayName";
			appNameValue.InnerText = facebookAppName;

			doc.AppendChild (urlTypesKey);
			doc.AppendChild (urlTypesArray);
			doc.AppendChild (appIdKey);
			doc.AppendChild (appIdValue);
			doc.AppendChild (appNameKey);
			doc.AppendChild (appNameValue);

			XmlElement securityKey = plist.CreateElement ("key");
			XmlElement securityValue = plist.CreateElement ("dict");
			XmlElement domainsKey = plist.CreateElement ("key");
			XmlElement domainsValue = plist.CreateElement ("dict");
			XmlElement facebookComKey = plist.CreateElement ("key");
			XmlElement facebookComValue = plist.CreateElement ("dict");
			XmlElement facebookComSubdomainsKey = plist.CreateElement ("key");
			XmlElement facebookComSubdomainsValue = plist.CreateElement ("true");
			XmlElement facebookComForwardSecrecyKey = plist.CreateElement ("key");
			XmlElement facebookForwardSecrecyValue = plist.CreateElement ("false");
			XmlElement fbcdnNetKey = plist.CreateElement ("key");
			XmlElement fbcdnNetValue = plist.CreateElement ("dict");
			XmlElement fbcdnNetSubdomainsKey = plist.CreateElement ("key");
			XmlElement fbcdnNetSubdomainsValue = plist.CreateElement ("true");
			XmlElement fbcdnNetForwardSecrecyKey = plist.CreateElement ("key");
			XmlElement fbcdnNetForwardSecrecyValue = plist.CreateElement ("false");
			XmlElement akamaihdNetKey = plist.CreateElement ("key");
			XmlElement akamaihdNetValue = plist.CreateElement ("dict");
			XmlElement akamaihdNetSubdomainsKey = plist.CreateElement ("key");
			XmlElement akamaihdNetSubdomainsValue = plist.CreateElement ("true");
			XmlElement akamaihdNetForwardSecrecyKey = plist.CreateElement ("key");
			XmlElement akamaihdNetForwardSecrecyValue = plist.CreateElement ("false");
			XmlElement funplusgameComKey = plist.CreateElement ("key");
			XmlElement funplusgameComValue = plist.CreateElement ("dict");
			XmlElement funplusgameComHttpLoadsKey = plist.CreateElement ("key");
			XmlElement funplusgameComHttpLoadsValue = plist.CreateElement ("true");
			XmlElement funplusgameComSubdomainsKey = plist.CreateElement ("key");
			XmlElement funplusgameComSubdomainsValue = plist.CreateElement ("true");
			XmlElement funplusgameComForwardSecrecyKey = plist.CreateElement ("key");
			XmlElement funplusgameComForwardSecrecyValue = plist.CreateElement ("false");

			securityKey.InnerText = "NSAppTransportSecurity";
			domainsKey.InnerText = "NSExceptionDomains";
			facebookComKey.InnerText = "facebook.com";
			facebookComSubdomainsKey.InnerText = "NSIncludesSubdomains";
			facebookComForwardSecrecyKey.InnerText = "NSThirdPartyExceptionRequiresForwardSecrecy";
			fbcdnNetKey.InnerText = "fbcdn.net";
			fbcdnNetSubdomainsKey.InnerText = "NSIncludesSubdomains";
			fbcdnNetForwardSecrecyKey.InnerText = "NSThirdPartyExceptionRequiresForwardSecrecy";
			akamaihdNetKey.InnerText = "akamaihd.net";
			akamaihdNetSubdomainsKey.InnerText = "NSIncludesSubdomains";
			akamaihdNetForwardSecrecyKey.InnerText = "NSThirdPartyExceptionRequiresForwardSecrecy";
			funplusgameComKey.InnerText = "funplusgame.com";
			funplusgameComHttpLoadsKey.InnerText = "NSExceptionAllowsInsecureHTTPLoads";
			funplusgameComSubdomainsKey.InnerText = "NSIncludesSubdomains";
			funplusgameComForwardSecrecyKey.InnerText = "NSThirdPartyExceptionRequiresForwardSecrecy";

			securityValue.AppendChild (domainsKey);
			securityValue.AppendChild (domainsValue);
			domainsValue.AppendChild (facebookComKey);
			domainsValue.AppendChild (facebookComValue);
			facebookComValue.AppendChild (facebookComSubdomainsKey);
			facebookComValue.AppendChild (facebookComSubdomainsValue);
			facebookComValue.AppendChild (facebookComForwardSecrecyKey);
			facebookComValue.AppendChild (facebookForwardSecrecyValue);
			domainsValue.AppendChild (fbcdnNetKey);
			domainsValue.AppendChild (fbcdnNetValue);
			fbcdnNetValue.AppendChild (fbcdnNetSubdomainsKey);
			fbcdnNetValue.AppendChild (fbcdnNetSubdomainsValue);
			fbcdnNetValue.AppendChild (fbcdnNetForwardSecrecyKey);
			fbcdnNetValue.AppendChild (fbcdnNetForwardSecrecyValue);
			domainsValue.AppendChild (akamaihdNetKey);
			domainsValue.AppendChild (akamaihdNetValue);
			akamaihdNetValue.AppendChild (akamaihdNetSubdomainsKey);
			akamaihdNetValue.AppendChild (akamaihdNetSubdomainsValue);
			akamaihdNetValue.AppendChild (akamaihdNetForwardSecrecyKey);
			akamaihdNetValue.AppendChild (akamaihdNetForwardSecrecyValue);
			domainsValue.AppendChild (funplusgameComKey);
			domainsValue.AppendChild (funplusgameComValue);
			funplusgameComValue.AppendChild (funplusgameComHttpLoadsKey);
			funplusgameComValue.AppendChild (funplusgameComHttpLoadsValue);
			funplusgameComValue.AppendChild (funplusgameComSubdomainsKey);
			funplusgameComValue.AppendChild (funplusgameComSubdomainsValue);
			funplusgameComValue.AppendChild (funplusgameComForwardSecrecyKey);
			funplusgameComValue.AppendChild (funplusgameComForwardSecrecyValue);

			doc.AppendChild (securityKey);
			doc.AppendChild (securityValue);

			XmlElement queriesSchemesKey = plist.CreateElement ("key");
			XmlElement queriesSchemesArray = plist.CreateElement ("array");
			XmlElement item1 = plist.CreateElement ("string");
			XmlElement item2 = plist.CreateElement ("string");
			XmlElement item3 = plist.CreateElement ("string");
			XmlElement item4 = plist.CreateElement ("string");
			XmlElement item5 = plist.CreateElement ("string");
			XmlElement item6 = plist.CreateElement ("string");
			XmlElement item7 = plist.CreateElement ("string");
			XmlElement item8 = plist.CreateElement ("string");
			XmlElement item9 = plist.CreateElement ("string");
			XmlElement item10 = plist.CreateElement ("string");
			XmlElement item11 = plist.CreateElement ("string");
			XmlElement item12 = plist.CreateElement ("string");
			XmlElement item13 = plist.CreateElement ("string");

			queriesSchemesKey.InnerText = "LSApplicationQueriesSchemes";
			item1.InnerText = "fbapi";
			item2.InnerText = "fbapi20130214";
			item3.InnerText = "fbapi20130410";
			item4.InnerText = "fbapi20130702";
			item5.InnerText = "fbapi20131010";
			item6.InnerText = "fbapi20131219";
			item7.InnerText = "fbapi20140410";
			item8.InnerText = "fbapi20140116";
			item9.InnerText = "fbapi20150313";
			item10.InnerText = "fbapi20150629";
			item11.InnerText = "fbauth";
			item12.InnerText = "fbauth2";
			item13.InnerText = "fb-messenger-api20140430";
			queriesSchemesArray.AppendChild (item1);
			queriesSchemesArray.AppendChild (item2);
			queriesSchemesArray.AppendChild (item3);
			queriesSchemesArray.AppendChild (item4);
			queriesSchemesArray.AppendChild (item5);
			queriesSchemesArray.AppendChild (item6);
			queriesSchemesArray.AppendChild (item7);
			queriesSchemesArray.AppendChild (item8);
			queriesSchemesArray.AppendChild (item9);
			queriesSchemesArray.AppendChild (item10);
			queriesSchemesArray.AppendChild (item11);
			queriesSchemesArray.AppendChild (item12);
			queriesSchemesArray.AppendChild (item13);

			doc.AppendChild (queriesSchemesKey);
			doc.AppendChild (queriesSchemesArray);
		}

		plist.Save (plistPath);

		FileModifier plistModifier = new FileModifier(plistPath);
		UnityEngine.Debug.Log (plistModifier.ToString());
		plistModifier.Replace("[]","");
		UnityEngine.Debug.Log (plistModifier.ToString());
		
		plistModifier.Write ();

#endif
		
		UnityEngine.Debug.Log ("Finish running post build script");

	}

	/**
	 * Copy a directory to another path, recursively.
	 */
	private static void CopyDirectory (string dstDirName, string srcDirName, string[] skipList) {
		DirectoryInfo dir = new DirectoryInfo (srcDirName);
		
		if (!dir.Exists) {
			throw new DirectoryNotFoundException(
				"Source directory does not exist or could not be found: "
				+ srcDirName);
		}
		
		if (!Directory.Exists (dstDirName)) {
			Directory.CreateDirectory (dstDirName);
		}
		
		FileInfo[] files = dir.GetFiles ();
		foreach (FileInfo file in files) {
			if (skipList != null) {
				bool skip = false;
				
				foreach (string pattern in skipList) {
					if (Regex.IsMatch (file.FullName, pattern, RegexOptions.IgnoreCase)) {
						skip = true;
						break;
					}
				}
				
				if (skip) {
					continue;
				}
			}
			
			file.CopyTo(Path.Combine(dstDirName, file.Name));
		}
		
		DirectoryInfo[] subdirs = dir.GetDirectories ();
		
		foreach (DirectoryInfo subdir in subdirs) {
			if (skipList != null) {
				bool skip = false;
				
				foreach (var pattern in skipList) {
					if (Regex.IsMatch (subdir.FullName, pattern, RegexOptions.IgnoreCase)) {
						skip = true;
						break;
					}
				}
				
				if (skip) {
					continue;
				}
			}
			
			CopyDirectory (Path.Combine (dstDirName, subdir.Name), subdir.FullName, skipList);
		}
	}
}

#endif