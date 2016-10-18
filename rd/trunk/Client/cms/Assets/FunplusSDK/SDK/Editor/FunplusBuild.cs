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
using System.IO;

namespace Funplus.Internal
{

	internal class FunplusBuild {

		private const string sdkVersion = "3.0.19";

		private const string funplusPath = "Assets/FunplusSDK/SDK/";
		private const string samplePath = "Assets/FunplusSDK/Sample/";
		private const string pluginsPath = "Assets/FunplusSDK/Plugins/";
		private const string webpagesPath = "Assets/WebPlayerTemplates/";

		public enum Target
		{
			DEBUG,
			RELEASE
		}

		private static string PackageName
		{
			get
			{
				return "funplus-unity-sdk.unitypackage";
			}
		}

		private static string OutputPath
		{
			get
			{
				string projectRoot = Directory.GetCurrentDirectory();
				string outdir = Path.Combine (projectRoot, Path.Combine("release", "funplus-unity-sdk-" + sdkVersion));
				if (Directory.Exists (outdir)) {
					Directory.Delete (outdir, true);
				}
					
				var outputDirectory = new DirectoryInfo(outdir);
				outputDirectory.Create();

				return Path.Combine(outputDirectory.FullName, FunplusBuild.PackageName);
			}
		}

		// Exporting the *.unityPackage for Asset store
		public static string ExportPackage()
		{
			Debug.Log("Exporting FunplusSDK Unity Package...");
			string path = OutputPath;

			// hook: before
			BeforeExport ();

			try
			{
				if (!File.Exists(Path.Combine(Application.dataPath, "Temp")))
				{
					AssetDatabase.CreateFolder("Assets", "Temp");
				}

				AssetDatabase.MoveAsset(funplusPath + "Resources/FunplusSettings.asset", "Assets/Temp/FunplusSettings.asset");

				string[] funplusFiles = (string[])Directory.GetFiles(funplusPath, "*.*", SearchOption.AllDirectories);
				string[] sampleFiles = (string[])Directory.GetFiles(samplePath, "*.*", SearchOption.AllDirectories);
				string[] pluginsFiles = (string[])Directory.GetFiles(pluginsPath, "*.*", SearchOption.AllDirectories);
				string[] webpagesFiles = (string[])Directory.GetFiles(webpagesPath, "*.*", SearchOption.AllDirectories);
				string[] files = new string[funplusFiles.Length + sampleFiles.Length + pluginsFiles.Length + webpagesFiles.Length];

				funplusFiles.CopyTo(files, 0);
				sampleFiles.CopyTo(files, funplusFiles.Length);
				pluginsFiles.CopyTo(files, funplusFiles.Length + sampleFiles.Length);
				webpagesFiles.CopyTo(files, funplusFiles.Length + sampleFiles.Length + pluginsFiles.Length);

				AssetDatabase.ExportPackage(
					files,
					path,
					ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);
			}
			finally
			{
				// Move files back no matter what
				AssetDatabase.MoveAsset("Assets/Temp/FunplusSettings.asset", funplusPath + "Resources/FunplusSettings.asset");
				AssetDatabase.DeleteAsset("Assets/Temp");
			}

			// hook: after
			AfterExport ();

			Debug.Log("Exporting finished: " + Path.Combine("release", "funplus-unity-sdk-" + sdkVersion));

			return path;
		}

		private static void BeforeExport()
		{
			string versionFile = Path.Combine (Directory.GetCurrentDirectory (), "VERSION");
			string versionStr = "Version: " + sdkVersion;
			File.WriteAllText (versionFile, versionStr);
		}

		private static void AfterExport()
		{
			string projectRoot = Directory.GetCurrentDirectory();
			string outdir = Path.Combine (projectRoot, Path.Combine("release", "funplus-unity-sdk-" + sdkVersion));

			string srcPath = Directory.GetCurrentDirectory ();

			File.Copy (Path.Combine (srcPath, "VERSION"), Path.Combine(outdir, "VERSION"), true);
			File.Copy (Path.Combine (srcPath, "README.md"), Path.Combine(outdir, "README.md"), true);
			File.Copy (Path.Combine (srcPath, "CHANGELOG.md"), Path.Combine(outdir, "CHANGELOG.md"), true);
		}
	}

}