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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Funplus.Internal
{

	public class SettingUtils
	{

		public static string ChooseBuildPath ()
		{
			// Choose output location.
			string outputLocation = EditorUtility.SaveFolderPanel ("Choose Output Location: ", FunplusSettings.BuildPath, "");

			if (string.IsNullOrEmpty(outputLocation))
			{
				return null;
			}

			FunplusSettings.BuildPath = outputLocation;
			return outputLocation;
		}

		public static FieldStatus CheckFunplusSettings ()
		{
			if (string.IsNullOrEmpty (FunplusSettings.FunplusGameId))
			{
				return new FieldStatus { Status = FieldStatus.StatusType.MissingField, Message = "Funplus Game Id" };
			}

			if (string.IsNullOrEmpty (FunplusSettings.FunplusGameKey))
			{
				return new FieldStatus { Status = FieldStatus.StatusType.MissingField, Message = "Funplus Game Key" };
			}

			if (string.IsNullOrEmpty (FunplusSettings.Environment))
			{
				return new FieldStatus { Status = FieldStatus.StatusType.MissingField, Message = "running environment" };
			}

			return new FieldStatus { Status = FieldStatus.StatusType.OK };
		}

		public static FieldStatus CheckFacebookSettings ()
		{
			if (FunplusSettings.FacebookEnabled) 
			{
				if (string.IsNullOrEmpty (FunplusSettings.FacebookAppId))
				{
					return new FieldStatus { Status = FieldStatus.StatusType.MissingField, Message = "Facebook App Id" };
				}

				if (string.IsNullOrEmpty (FunplusSettings.FacebookAppName))
				{
					return new FieldStatus { Status = FieldStatus.StatusType.MissingField, Message = "Facebook App Name" };
				}
			}

			return new FieldStatus { Status = FieldStatus.StatusType.OK };
		}

		public static FieldStatus CheckAndroidSettings ()
		{
			if (string.IsNullOrEmpty(FunplusSettings.AndroidCompileSdkVersion))
			{
				return new FieldStatus { Status = FieldStatus.StatusType.MissingField, Message = "Android Compile SDK Version" };
			}

			if (string.IsNullOrEmpty (FunplusSettings.AndroidBuildToolsVersion))
			{
				return new FieldStatus { Status = FieldStatus.StatusType.MissingField, Message = "Android Build Tools Version" };
			}

			if (string.IsNullOrEmpty (FunplusSettings.AndroidMinSdkVersion))
			{
				return new FieldStatus { Status = FieldStatus.StatusType.MissingField, Message = "Android Min SDK Version" };
			}

			if (string.IsNullOrEmpty (FunplusSettings.AndroidTargetSdkVersion))
			{
				return new FieldStatus { Status = FieldStatus.StatusType.MissingField, Message = "Android Target SDK Version" };
			}

			if (string.IsNullOrEmpty (FunplusSettings.AndroidLauncherActivity))
			{
				return new FieldStatus { Status = FieldStatus.StatusType.MissingField, Message = "Android Launcher Activity" };
			}

			return new FieldStatus { Status = FieldStatus.StatusType.OK };
		}

		public static FieldStatus CheckXcodeSettings ()
		{
			return new FieldStatus { Status = FieldStatus.StatusType.OK };
		}

		public static List<string> GetEnabledScenes ()
		{
			List<string> enabledScenes = new List<string>();
			foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes) 
			{
				if (S.enabled) 
				{
					enabledScenes.Add (S.path);
				}
			}
			return enabledScenes;
		}

		public static BuildOptions GenBuildOptions (bool debuggable)
		{
			if (debuggable)
			{
				return BuildOptions.AcceptExternalModificationsToPlayer | BuildOptions.AllowDebugging | BuildOptions.Development;
			}
			else
			{
				return BuildOptions.AcceptExternalModificationsToPlayer;
			}
		}

		public static void LogError (FieldStatus result)
		{
			string dialogTitle = null;
			string dialogContent = null;
			string dialogDismiss = "OK, Got It!";

			switch (result.Status)
			{
			case FieldStatus.StatusType.OK:
				return;
			case FieldStatus.StatusType.MissingField:
				dialogTitle = "Missing Settings";
				dialogContent = string.Format ("You didn't specify the {0}. Please add one using the Funplus menu in the main Unity Editor.", result.Message);
				break;
			case FieldStatus.StatusType.InvalidField:
				dialogTitle = "Invalid Settings";
				dialogContent = string.Format ("The {0} you've specified is invalid. Please change to a valid one using the Funplus menu in the main Unity Editor.", result.Message);
				break;
			}

			UnityEngine.Debug.LogError (dialogContent);
			EditorUtility.DisplayDialog (dialogTitle, dialogContent, dialogDismiss);
		}

		public static void CopyDirectory (string dstDirName, string srcDirName, string[] skipList)
		{
			DirectoryInfo dir = new DirectoryInfo (srcDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException (
					"Source directory does not exist or could not be found: "
					+ srcDirName);
			}

			if (!Directory.Exists (dstDirName)) 
			{
				Directory.CreateDirectory (dstDirName);
			}

			FileInfo[] files = dir.GetFiles ();
			foreach (FileInfo file in files) 
			{
				if (skipList != null) 
				{
					bool skip = false;

					foreach (string pattern in skipList) 
					{
						if (Regex.IsMatch (file.FullName, pattern, RegexOptions.IgnoreCase)) 
						{
							skip = true;
							break;
						}
					}

					if (skip) 
					{
						continue;
					}
				}

				file.CopyTo (Path.Combine (dstDirName, file.Name));
			}

			DirectoryInfo[] subdirs = dir.GetDirectories ();

			foreach (DirectoryInfo subdir in subdirs) 
			{
				if (skipList != null) 
				{
					bool skip = false;

					foreach (var pattern in skipList) 
					{
						if (Regex.IsMatch (subdir.FullName, pattern, RegexOptions.IgnoreCase)) 
						{
							skip = true;
							break;
						}
					}

					if (skip) 
					{
						continue;
					}
				}

				CopyDirectory (Path.Combine (dstDirName, subdir.Name), subdir.FullName, skipList);
			}
		}
	}

}
#endif