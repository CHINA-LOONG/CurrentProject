using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Packager
{
    public static string platform = string.Empty;
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    /// <summary>
    /// 生成assetbundle以及file list文件
    /// </summary>
    [MenuItem("Builder/Build AssetBundles and filelist")]
    public static void BuildAssetResource()
    {
        //build assetbundle: Assets/StreamingAssets/assetbundle
        string abPath = Path.Combine(Util.BuildPath, Const.AssetDirname);
        if (!Directory.Exists(abPath))
            Directory.CreateDirectory(abPath);
        BuildPipeline.BuildAssetBundles(abPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        //build lua file list: Assets/StreamingAssets/files.txt
        ///----------------------创建文件列表-----------------------
        string newFilePath = Util.BuildPath + "/files.txt";

        if (File.Exists(newFilePath))
            File.Delete(newFilePath);
        paths.Clear();
        files.Clear();
        Recursive(Util.BuildPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (ext.Equals(".meta")) continue;
            if (ext.Equals(".exe")) continue;
            if (file.Contains(".DS_Store")) continue;
            if (file.Contains(".svn")) continue;

            string value = file.Replace(Util.BuildPath, string.Empty);
            sw.WriteLine(value);
        }
        sw.Close(); fs.Close();
        AssetDatabase.Refresh();
        Debug.Log("Generated files.txt at:" + newFilePath);
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }
}