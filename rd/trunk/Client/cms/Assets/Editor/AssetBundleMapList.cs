using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AssetBundleMapList
{
    public static string assetBundleRoot=@"Assets/StreamingAssets/assetbundle/";
    public static string readFilsExtension = ".manifest";   //There is a "."
    public static string saveFilePath = @"Assets/StreamingAssets/staticData/";
    public static string saveFileDefaultName = "assetMap";
    public static string saveFileExtension = ".csv";         //There is no "."

    public static Dictionary<string, string> map = new Dictionary<string, string>();

    public static string selectPath;

    //[MenuItem("Builder/CreateMapList")]
    public static void GetMapData()
    {
        selectPath = assetBundleRoot;//EditorUtility.OpenFolderPanel("Select the resource source", assetBundleRoot, "");

        map.Clear();
        map.Add("#TEXT","TEXT");
        map.Add("asset","bundle");
        map.Add("资源名","包名");

		selectPath = selectPath.Replace ("\\", "/");

        Recursion(new DirectoryInfo(selectPath));
        CreateCsv();
    }
    //递归获取manifest文件
    static void Recursion(DirectoryInfo root)
    {
        FileInfo[] files = root.GetFiles();
        DirectoryInfo[] dirs = root.GetDirectories();
        foreach (var file in files)
        {
            string ext = file.Extension;
            if (ext.Equals(readFilsExtension))
            {
                GetResource(file);
            }
        }
        foreach (var dir in dirs)
        {
            Recursion(dir);
        }

    }
    //从一个manifest文件获取资源映射
    static void GetResource(FileInfo file)
    {
        List<string> lines = new List<string>(File.ReadAllLines(file.FullName));
        int start = lines.IndexOf("Assets:");
        int end = -1;
        if (start == -1) return;

        if (lines.Contains("Dependencies: []"))
            end = lines.IndexOf("Dependencies: []");
        else if (lines.Contains("Dependencies:"))
            end = lines.IndexOf("Dependencies:");
        else
        {
            Logger.Log(file.Name + " not found end tag");
            return;
        }
        List<string> assets = new List<string>(lines.GetRange(start + 1, end - start-1));
        string bundleName = file.FullName.Replace("\\","/");
        int bundleStart=bundleName.LastIndexOf(assetBundleRoot)+assetBundleRoot.Length;
        int bundleEnd = bundleName.LastIndexOf(readFilsExtension);
        bundleName = bundleName.Substring(bundleStart, bundleEnd-bundleStart);
        foreach (var item in assets)
        {
            int assetStart = item.LastIndexOf("/");
            int assetEnd = item.LastIndexOf(".");
            string assetName = item.Substring(assetStart + 1, assetEnd - assetStart - 1);
            if (map.ContainsKey(assetName))
            {
                Debug.LogError("error: found same files <color=#ff0000ff>" + assetName + "</color> in <color=#00ff00ff>" + file.Name + "</color> and <color=#00ff00ff>" + map[assetName]+readFilsExtension + "</color>");
                continue;
            }
            map.Add(assetName, bundleName);
        }
    }

    static bool CreateCsv()
    {
        string savepath=saveFilePath+saveFileDefaultName+saveFileExtension;//EditorUtility.SaveFilePanel("保存映射表",saveFilePath,saveFileDefaultName,saveFileExtension);
        FileStream fs = new FileStream(savepath, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        foreach (var line in map)
        {
            sw.WriteLine(line.Key + "," + line.Value);
        }
        sw.Close();
        fs.Close();
        return true;
    }

}
