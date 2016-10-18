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
#endif
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Funplus;

namespace Funplus.Internal
{

	#if UNITY_EDITOR
    [InitializeOnLoad]
	#endif
    public class FunplusSettings : ScriptableObject
	{
		private const string ASSET_NAME = "FunplusSettings";
		private const string ASSET_PATH = "FunplusSDK/SDK/Resources";
		private const string ASSET_EXT = ".asset";

		private static FunplusSettings instance;

		[SerializeField]
		private string funplusGameId;
		[SerializeField]
		private string funplusGameKey;
		[SerializeField]
		private string environment;

		[SerializeField]
		private string androidLauncherActivity;
		[SerializeField]
		private string androidCompileSdkVersion = "23";
		[SerializeField]
		private string androidBuildToolsVersion = "23.0.1";
		[SerializeField]
		private string androidMinSdkVersion = "15";
		[SerializeField]
		private string androidTargetSdkVersion = "23";

		[SerializeField]
		private string iosAppController;

		[SerializeField]
		private bool facebookEnabled = true;
        [SerializeField]
		private string facebookAppId;
		[SerializeField]
		private string facebookAppName;

		// TODO
		[SerializeField]
		private string sdkVersion = "3.0.19";

		[SerializeField]
		private string buildPath;

	    private static FunplusSettings Instance
	    {
	        get
	        {
	            if (instance == null)
	            {
					instance = Resources.Load(ASSET_NAME) as FunplusSettings;
	                if (instance == null)
	                {
	                    // If not found, autocreate the asset object.
	                    instance = ScriptableObject.CreateInstance<FunplusSettings>();
	                    #if UNITY_EDITOR
	                    string properPath = Path.Combine(Application.dataPath, ASSET_PATH);
	                    if (!Directory.Exists(properPath))
	                    {
	                        Directory.CreateDirectory(properPath);
	                    }

	                    string fullPath = Path.Combine(Path.Combine("Assets", ASSET_PATH), ASSET_NAME + ASSET_EXT);
	                    AssetDatabase.CreateAsset(instance, fullPath);
	                    #endif
	                }
				}

	            return instance;
	        }
	    }

		public static string FunplusGameId
		{
			get
			{
				return Instance.funplusGameId;
			}

			set
			{
				if (Instance.funplusGameId != value)
				{
					Instance.funplusGameId = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static string FunplusGameKey
		{
			get
			{
				return Instance.funplusGameKey;
			}

			set
			{
				if (Instance.funplusGameKey != value)
				{
					Instance.funplusGameKey = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static string Environment
		{
			get
			{
				if (Instance.environment == null)
				{
					Instance.environment = "sandbox";
				}
				return Instance.environment;
			}

			set
			{
				if (Instance.environment != value)
				{
					Instance.environment = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static bool FacebookEnabled
		{
			get
			{
				return Instance.facebookEnabled;
			}

			set
			{
				if (Instance.facebookEnabled != value)
				{
					Instance.facebookEnabled = value;
					FunplusSettings.DirtyEditor ();
				}
			}
		}

	    public static string FacebookAppId
	    {
	        get
	        {
	            return Instance.facebookAppId;
	        }

	        set
	        {
	            if (Instance.facebookAppId != value)
	            {
					Instance.facebookAppId = value;
	                FunplusSettings.DirtyEditor();
	            }
	        }
	    }

		public static string FacebookAppName
		{
			get
			{
				return Instance.facebookAppName;
			}

			set
			{
				if (Instance.facebookAppName != value)
				{
					Instance.facebookAppName = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static string AndroidLauncherActivity
		{
			get
			{
				#if UNITY_EDITOR
				if (Instance.androidLauncherActivity == null
					|| Instance.androidLauncherActivity.Trim ().Length == 0)
				{
					Instance.androidLauncherActivity = string.Format("{0}.UnityPlayerActivity", PlayerSettings.bundleIdentifier);
				}
				#endif
				return Instance.androidLauncherActivity;
			}

			set
			{
				if (Instance.androidLauncherActivity != value)
				{
					Instance.androidLauncherActivity = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static string AndroidCompileSdkVersion
		{
			get
			{
				return Instance.androidCompileSdkVersion;
			}

			set
			{
				if (Instance.androidCompileSdkVersion != value)
				{
					Instance.androidCompileSdkVersion = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static string AndroidBuildToolsVersion
		{
			get
			{
				return Instance.androidBuildToolsVersion;
			}

			set
			{
				if (Instance.androidBuildToolsVersion != value)
				{
					Instance.androidBuildToolsVersion = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static string AndroidMinSdkVersion
		{
			get
			{
				return Instance.androidMinSdkVersion;
			}

			set
			{
				if (Instance.androidMinSdkVersion != value)
				{
					Instance.androidMinSdkVersion = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static string AndroidTargetSdkVersion
		{
			get
			{
				return Instance.androidTargetSdkVersion;
			}

			set
			{
				if (Instance.androidTargetSdkVersion != value)
				{
					Instance.androidTargetSdkVersion = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static string IosAppController
		{
			get
			{
				#if UNITY_EDITOR
				if (Instance.iosAppController == null
					|| Instance.iosAppController.Trim ().Length == 0)
				{
					Instance.iosAppController = "Classes/UnityAppController.mm";
				}
				#endif
				return Instance.iosAppController;
			}

			set
			{
				if (Instance.iosAppController != value)
				{
					Instance.iosAppController = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static string SdkVersion
		{
			get
			{
				return Instance.sdkVersion;
			}

			set
			{
				if (Instance.sdkVersion != value)
				{
					Instance.sdkVersion = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static string BuildPath
		{
			get
			{
				return Instance.buildPath;
			}

			set
			{
				if (Instance.buildPath != value)
				{
					Instance.buildPath = value;
					FunplusSettings.DirtyEditor();
				}
			}
		}

		public static void SettingsChanged ()
		{
			FunplusSettings.DirtyEditor ();
		}

		#if UNITY_EDITOR
		[MenuItem("FunPlusSDK/Edit Settings", false, 0)]
	    public static void EditSettings ()
	    {
	        Selection.activeObject = Instance;
	    }
		#endif

		#if UNITY_EDITOR
		[MenuItem("FunPlusSDK/Export Android Project/Debug", false, 1)]
		public static void ExportAndroidDebugProject ()
		{
			AndroidBuild.Build (true);
		}
		#endif

		#if UNITY_EDITOR
		[MenuItem("FunPlusSDK/Export Android Project/Release", false, 2)]
		public static void ExportAndroidReleaseProject ()
		{
			AndroidBuild.Build (false);
		}
		#endif

		#if UNITY_EDITOR
		[MenuItem("FunPlusSDK/About", false, 4)]
		public static void AboutSDK ()
		{
			string about = @"Version: 3.0.18

Made by:
Siyuan Zhang <siyuan.zhang@funplus.com>
Yu Zhang <yu.zhang@funplus.com>
Yuankun Zhang <yuankun.zhang@funplus.com>
Yantao Shi <yantao.shi@funplus.com>";
			EditorUtility.DisplayDialog ("Funplus SDK for Unity", about, "OK");
		}
		#endif

	    private static void DirtyEditor ()
	    {
			#if UNITY_EDITOR
	        EditorUtility.SetDirty (Instance);
			#endif
	    }
	}

}
