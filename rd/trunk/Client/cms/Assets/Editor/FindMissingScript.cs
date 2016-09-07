using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
public class FindMissingScript : EditorWindow
{
    static int go_count = 0, components_count = 0, missing_count = 0;
    static List<string> objList = new List<string>();
    [MenuItem("Builder/FindMissingScripts")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindMissingScript));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("在选定的对象找到失踪的脚本"))
        {
            FindInSelected();
        }
        if (GUILayout.Button("查找所有预制丢失的脚本"))
        {
            FindAllSelect("Assets");
        }
    }
    private static void FindAllSelect(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (filename.Contains("Editor") || filename.Contains("Font") ||
                filename.Contains("Fun311") || filename.Contains("HockeyAppUnityIOS") || filename.Contains("Resources") ||
                filename.Contains("Materials") || filename.Contains("Plugins") || filename.Contains("script")
                || filename.Contains("Shader") || filename.Contains("StreamingAssets")
                || filename.Contains("importModels") || filename.Contains("sound") || filename.Contains("texture")) continue;
            if (filename.Contains(".prefab") && !filename.Contains(".meta"))
            {
                string name = filename.Replace('\\', '/');
                GameObject curObj = AssetDatabase.LoadAssetAtPath(name, typeof(GameObject)) as GameObject;
                if (curObj != null)
                {
                    GameObject objInstance = Instantiate(curObj);
                    //Component[] components = objInstance.GetComponents<Component>();
                    //for (int i = 0; i < components.Length; i++)
                    //{
                    //    if (components[i] == null)
                    //    {
                    //        Debug.Log("此预制缺少脚本: " + objInstance.name + " 路径是: " + filename.Replace('\\', '/'));
                    //    }
                    //}
                    FindInGO(objInstance, filename.Replace('\\', '/'));
                    DestroyImmediate(objInstance);
                }
            }
        }
        foreach (string dir in dirs)
        {
            FindAllSelect(dir);
        }
    }

    private static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g);
        }
        Debug.Log(string.Format("搜索 {0} 个游戏对象, {1} 个组件, 发现 {2} 丢失", go_count, components_count, missing_count));
    }

    private static void FindInGO(GameObject g, string path = "")
    {
        go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }
                Debug.Log("名字: " + g + " 路径: " + path + "空脚本数: " + i);
            }
        }

        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );  
            FindInGO(childT.gameObject, path);
        }
    }
}
