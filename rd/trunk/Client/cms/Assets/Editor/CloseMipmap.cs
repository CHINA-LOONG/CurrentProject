using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CloseMipmap : MonoBehaviour
{
    //---------------------------------------------------------------------------------------------
    [MenuItem("Builder/Close Mipmap")]
    static void DoCloseMipmap()
    {
        List<Texture2D> objects = new List<Texture2D>();
        string path = EditorUtility.OpenFolderPanel("选择要检测的贴图路径", @"Assets/SourceAsset/texture/UI", "");

        List<string> targetFiles = Recursion(path, ".png|.tga");
        Debug.Log(targetFiles.Count);
        int index = 0;
        for (int i = 0; i < targetFiles.Count; i++)
        {
            targetFiles[i] = targetFiles[i].Replace(@"\", @"/");
            targetFiles[i] = targetFiles[i].Substring(targetFiles[i].LastIndexOf("Assets/"));
            //objects.Add(AssetDatabase.LoadAssetAtPath(targetFiles[i], typeof(Texture2D)) as Texture2D);

            AssetImporter curImport = AssetImporter.GetAtPath(targetFiles[i]);
            TextureImporter textureImport = curImport as TextureImporter;
            if (textureImport)
            {
                if (textureImport.mipmapEnabled == true)
                {
                    ++index;
                    textureImport.mipmapEnabled = false;
                    EditorUtility.SetDirty(textureImport);
                    textureImport.SaveAndReimport();
                    Logger.LogFormat("<color=#ffff00ff>close mipmap of texture {0}</color>", targetFiles[i]);
                }
            }
        }
        Logger.LogFormat("<color=#ffff00ff>close mipmap finish, changes {0} textures</color>", index);
	}
    //---------------------------------------------------------------------------------------------
    static List<string> profabPath = new List<string>();
    [MenuItem("Builder/Check UI Font")]
    static void FindFont()
    {
        string defaultFontname = EditorUtility.OpenFilePanel("选择默认的字体", @"Assets/Font", "ttf");
        defaultFontname = defaultFontname.Substring(defaultFontname.LastIndexOf("Assets/"));
        Font defaultFont = AssetDatabase.LoadAssetAtPath(defaultFontname, typeof(Font)) as Font;
        defaultFontname = defaultFont.name;

        List<GameObject> objects = new List<GameObject>();
        profabPath.Clear();
        string path = EditorUtility.OpenFolderPanel("选择要检查字体的预制路径", @"Assets/Prefabs/", "");

        if (string.IsNullOrEmpty(path))
            return;

        List<string> prefabs_path = Recursion(path, ".prefab");
        for (int i = 0; i < prefabs_path.Count; i++)
        {
            prefabs_path[i] = prefabs_path[i].Replace(@"\", @"/");
            prefabs_path[i] = prefabs_path[i].Substring(prefabs_path[i].LastIndexOf("Assets/"));
            GameObject curObj = AssetDatabase.LoadAssetAtPath(prefabs_path[i], typeof(GameObject)) as GameObject;
            if (curObj != null)
            {
                objects.Add(curObj);
            }
            else 
            {
                Logger.LogErrorFormat("load prefab {0} failed", prefabs_path[i]);
            }
        }

        //objs = Selection.gameObjects;
        foreach (var item in objects)
        {
            GameObject obj = item as GameObject;
            //GameObject go = PrefabUtility.InstantiatePrefab(obj) as GameObject;

            OutputFontRecurse(obj.transform, string.Empty, defaultFontname);

            //PrefabUtility.ReplacePrefab(go, obj);
            //GameObject.DestroyImmediate(go);
            //AssetDatabase.Refresh();
        }
    }
    //---------------------------------------------------------------------------------------------
    static public void OutputFontRecurse(Transform label, string parentName, string defaultFontname)
    {
        if (label.GetComponent<UnityEngine.UI.Text>() != null)
        {
            UnityEngine.UI.Text textComponent = label.GetComponent<UnityEngine.UI.Text>();
            if (textComponent != null)
            {
                Font curFont = textComponent.font;
                if (curFont != null)
                {
                    if (curFont.name.Equals(defaultFontname) == false)
                    {
                        Logger.LogFormat("<color=#ff0000ff>{0} use font {1} not default font</color>", parentName + "/" + label.name, curFont.name);
                    }
                    //else
                    //{
                    //    Logger.LogFormat("<color=#ffff00ff>{0} use font {1}</color>", parentName + "/" + label.name, curFont.name);
                    //}
                }
                else
                {
                    Logger.LogFormat("<color=#ff00ffff>{0} has no font</color>", parentName + "/" + label.name);
                }
            }
            else
            {
                Logger.Log("<color=#ff0000ff>error in find font</color>");
            }
        }

        for (int i = 0; i < label.childCount; ++i)
        {
            Transform child = label.GetChild(i);
            OutputFontRecurse(child, parentName + "/" + label.name, defaultFontname);
        }
    }
    //---------------------------------------------------------------------------------------------
    static List<string> Recursion(string root, string extension)
    {
        List<string> targetFiles = new List<string>();
        DirectoryInfo rootDir = new DirectoryInfo(root);
        FileInfo[] files = rootDir.GetFiles();
        DirectoryInfo[] dirs = rootDir.GetDirectories();
        foreach (var file in files)
        {
            string ext = file.Extension;
            if (extension.Contains(ext))
            {
                targetFiles.Add(file.FullName);
            }
        }
        foreach (var dir in dirs)
        {
            targetFiles.AddRange(Recursion(dir.FullName, extension));
        }
        return targetFiles;
    }
    //---------------------------------------------------------------------------------------------
}
