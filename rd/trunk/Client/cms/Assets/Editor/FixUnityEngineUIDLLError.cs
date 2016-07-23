/*
 * You will meet error like:
 *  'UnityEngine.UI.dll is in timestamps but is not known in assetdatabase'
 * Run this script will fix this: 
 * 	 run the menu item "Assets/Reimport UI Assemblies" 
 *  from: http://forum.unity3d.com/threads/unityengine-ui-dll-is-in-timestamps-but-is-not-known-in-assetdatabase.274492/
 */

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

public class ReimportUnityEngineUI
{
	[MenuItem( "Assets/Reimport UI Assemblies", false, 100 )]
	public static void ReimportUI()
	{
		#if UNITY_4_6
		var path = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/GUISystem/{0}/{1}";
		var version = Regex.Match( Application.unityVersion,@"^[0-9]+\.[0-9]+\.[0-9]+").Value;
		#else
		var path = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/GUISystem/{1}";
		var version = string.Empty;
		#endif
		string engineDll = string.Format( path, version, "UnityEngine.UI.dll");
		string editorDll = string.Format( path, version, "Editor/UnityEditor.UI.dll");
		ReimportDll( engineDll );
		ReimportDll( editorDll );
		
	}
	static void ReimportDll(string path )
	{
		if ( File.Exists( path ) )
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate| ImportAssetOptions.DontDownloadFromCacheServer );
		else
			Debug.LogError( string.Format( "DLL not found {0}", path ) );
	}
}