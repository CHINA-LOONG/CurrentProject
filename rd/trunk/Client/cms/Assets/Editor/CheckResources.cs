using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
//public class ShowCheckResult : EditorWindow 
//{
//    public List<string> fileName = new List<string>();
//    CheckResources window;
//    static ShowCheckResult mInst = null;
//    public static ShowCheckResult Instance
//    {
//        get
//        {
//            return mInst;
//        }
//    }
//    void Start()
//    {
//        mInst = this;
//    }
//    void Update()
//    { 
        
//    }
//    public void ShowStart()
//    {
//        Rect wr = new Rect(0, 0, 500, 500);
//        window = (CheckResources)EditorWindow.GetWindowWithRect(typeof(CheckResources), wr, true, "资源检查");
//        window.Show();
//        //StartCoroutine(RenovateMsg(""));
//    }
//    IEnumerator RenovateMsg(string text)
//    {
//        yield return new WaitForEndOfFrame();
//        this.ShowNotification(new GUIContent(text));
//        yield return new WaitForSeconds(1f);
//        this.RemoveNotification();
//    }
//}

public class CheckResources : EditorWindow
{
    //---------------------------------------------------------------------------------------------
    static string resourcesName;
    static string metaGuid;
    static string objectPath;
    static List<string> fileName = new List<string>();
    static CheckResources window;
    [MenuItem("Assets/Check Resources")]
    static void das()
    {
        fileName.Clear();
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            resourcesName = o.name;
            objectPath = AssetDatabase.GetAssetPath(o) + ".";
        }
        Recursive("Assets");
    }
    //---------------------------------------------------------------------------------------------
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        bool isOk = false;
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (filename.Contains("Editor") ||filename.Contains("Font") ||
                filename.Contains("Fun311") || filename.Contains("HockeyAppUnityIOS") || filename.Contains("Resources") ||
                filename.Contains("Materials") || filename.Contains("Plugins") || filename.Contains("script")
                || filename.Contains("Shader") || filename.Contains("StreamingAssets")) continue;
            if (filename.Contains(resourcesName) == true )
            {
                if (!filename.Replace('\\', '/').Contains(objectPath)) continue;
                if (ext.Equals(".meta"))
                {
                    string nexLine;
                    StreamReader sr = File.OpenText(filename.Replace('\\', '/'));
                    while ((nexLine = sr.ReadLine()) != null)
                    {
                        if (nexLine.Contains("guid"))
                        {
                            metaGuid = nexLine;
                            sr.Close();
                            isOk = true;
                            ReadMeta("Assets");
                            return;
                        }
                    }
                }
            }
        }
        foreach (string dir in dirs)
        {
            if (!isOk)
                Recursive(dir);
            else
                return;
        }
    }
    //---------------------------------------------------------------------------------------------
    static void ReadMeta(string path)
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
            if (filename.Contains(".prefab") || ext.Equals(".unity") || ext.Equals(".controller") || ext.Equals(".anim") || ext.Equals(".mat"))
            {
                string nexLine;
                StreamReader sr = File.OpenText(filename.Replace('\\', '/'));
                while ((nexLine = sr.ReadLine()) != null)
                {
                    if (nexLine.Contains(metaGuid))
                    {
                        fileName.Add(filename.Replace('\\', '/'));
                        continue;
                    }
                }
                sr.Close();
            }
        }
        foreach (string dir in dirs)
        {
            ReadMeta(dir);
        }
        if (window == null)
        {
            Rect wr = new Rect(0, 0, 500, 500);
            window = (CheckResources)EditorWindow.GetWindowWithRect(typeof(CheckResources), wr, true, "资源检查");
            window.Show();
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnGUI()
    {
        if (fileName.Count > 0)
        {
            for (int i = 0; i < fileName.Count; i++)
            {
                EditorGUILayout.LabelField("文件路径: " + fileName[i]);
            }
        }
        else
        {
            this.ShowNotification(new GUIContent("没有地方用到此资源,也有可能程序出错了"));
        }
    }
    void OnInspectorUpdate()
    {
        //这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }
}
