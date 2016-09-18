using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class CloseMipmap : MonoBehaviour
{
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    public class AniFileInfo
    {
        public AniFileInfo(string filename)
        {
            this.filename = filename;
        }

        public string filename;
        public List<string> refrencedPrefabList = new List<string>();
        public List<string> clipinfoList = new List<string>();
    }
    //---------------------------------------------------------------------------------------------
    [MenuItem("Builder/Check Animator")]
    static void CheckAnimator()
    {
        //Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
        profabPath.Clear();
        string path = EditorUtility.OpenFolderPanel("选择要检查的资源路径", @"Assets/Prefabs", "");

        if (string.IsNullOrEmpty(path))
            return;

        string fileName = EditorUtility.SaveFilePanel("选择log保存路径", @"Assets", "ListAniInfo", "txt");
        if (string.IsNullOrEmpty(fileName))
            return;

        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
        FileStream aniInfoFile = File.Open(fileName, FileMode.OpenOrCreate);
        aniInfoFile.Close();

        Dictionary<string, AniFileInfo> animatorFileList = new Dictionary<string, AniFileInfo>();
        //List<string> prefabWithAniList = new List<string>();
        bool ignoreEnable = (path.Contains("Prefabs") == false);
        List<string> prefabs_path = new List<string>();//Recursion(path, ".prefab", ignoreEnable);
        GetAnimatorInfo(path, ignoreEnable, ref animatorFileList, ref prefabs_path);

        List<string> prefabsWithAni = new List<string>();
        for (int i = 0; i < prefabs_path.Count; i++)
        {
            prefabs_path[i] = prefabs_path[i].Replace(@"\", @"/");
            prefabs_path[i] = prefabs_path[i].Substring(prefabs_path[i].LastIndexOf("Assets/"));

            if (FindWords(prefabs_path[i], "Animator") == true)
            {
                prefabsWithAni.Add(prefabs_path[i]);
            }

        }

        for (int i = 0; i < prefabsWithAni.Count; ++i)
        {
            //if (prefabsWithAni[i].Contains("xgNvyao5"))
            //{
            //    int a = 0;
            //}
            GameObject curObj = AssetDatabase.LoadAssetAtPath(prefabsWithAni[i], typeof(GameObject)) as GameObject;
            if (curObj != null)
            {
                GameObject objInstance = Instantiate(curObj);
                Animator[] aniList = objInstance.transform.GetComponentsInChildren<Animator>();
                for (int j = 0; j < aniList.Length; ++j)
                {
                    RuntimeAnimatorController curController = aniList[j].runtimeAnimatorController;
                    if (curController != null)
                    {
                        string curAniName = curController.name + ".controller";
                        AniFileInfo curAniFile;
                        if (animatorFileList.TryGetValue(curAniName, out curAniFile) == true)
                        {
                            curAniFile.refrencedPrefabList.Add(curObj.name);
                            for (int clipIndex = 0; clipIndex < curController.animationClips.Length; ++clipIndex)
                            {
                                string aniName = curController.animationClips[clipIndex].name;
                                curAniFile.clipinfoList.Add(aniName);
                            }
                        }
                    }
                }

                DestroyImmediate(objInstance);
            }
            else
            {
                Logger.LogErrorFormat("load prefab {0} failed", prefabs_path[i]);
            }

            //DestroyImmediate(curObj, true);
        }

        StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8);
        
        //int count = animatorFileList.Count;
        var aniItr = animatorFileList.GetEnumerator();
        while (aniItr.MoveNext())
        {
            writer.WriteLine("----------------------------------------------------------------------------");
            writer.WriteLine(string.Format("AnimatorController name: {0}", aniItr.Current.Key));
            writer.WriteLine("referenced prefabs:");
            AniFileInfo aniInfo = aniItr.Current.Value;
            int aniInfoCount = aniInfo.refrencedPrefabList.Count;
            for (int aniIndex = 0; aniIndex < aniInfoCount; ++aniIndex)
            {
                writer.WriteLine("\t" + aniInfo.refrencedPrefabList[aniIndex]);
            }

            writer.WriteLine("animations:");
            aniInfoCount = aniInfo.clipinfoList.Count;
            for (int aniIndex = 0; aniIndex < aniInfoCount; ++aniIndex)
            {
                writer.WriteLine("\t" + aniInfo.clipinfoList[aniIndex]);
            }
        }

        writer.Close();

        Logger.Log("<color=#ffff00ff>check ani finish</color>");
    }
    //---------------------------------------------------------------------------------------------
    static void GetAnimatorInfo(string root, bool ignoreEnable, ref Dictionary<string, AniFileInfo> aniControllerList, ref List<string> prefabList)
    {
        //List<string> targetFiles = new List<string>();
        DirectoryInfo rootDir = new DirectoryInfo(root);
        FileInfo[] files = rootDir.GetFiles();
        DirectoryInfo[] dirs = rootDir.GetDirectories();
        foreach (var file in files)
        {
            string ext = file.Extension;
            if (ext.Contains("controller"))
            {
                if (aniControllerList.ContainsKey(file.Name) == false)
                {
                    aniControllerList.Add(file.Name, new AniFileInfo(file.Name));
                }
                else
                {
                    Logger.LogErrorFormat("find same controller {0}", file.Name);
                }
            }
            else if (ext.Contains(".prefab"))
            {
                prefabList.Add(file.FullName);
            }
        }
        foreach (var dir in dirs)
        {
            if (ignoreEnable == false ||
                dir.Name.Contains("Prefabs") ||
                dir.Name.Contains("SourceAsset")
                )
            {
                GetAnimatorInfo(dir.FullName, false, ref aniControllerList, ref prefabList);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public static bool FindWords(string filename, string worlds)
    {
        StreamReader read = new StreamReader(filename, Encoding.UTF8);
        string line = read.ReadToEnd();
        read.Close();
        return line.Contains(worlds);
    }
    //---------------------------------------------------------------------------------------------


    //---------------------------------------------------------------------------------------------
    //Close mipmap
    //---------------------------------------------------------------------------------------------
    [MenuItem("Builder/Close Mipmap")]
    static void DoCloseMipmap()
    {
        //List<Texture2D> objects = new List<Texture2D>();
        string path = EditorUtility.OpenFolderPanel("选择要检测的贴图路径", @"Assets/SourceAsset/texture/UI", "");
        if (string.IsNullOrEmpty(path))
            return;

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


    //---------------------------------------------------------------------------------------------
    // check ui font
    //---------------------------------------------------------------------------------------------
    static List<string> profabPath = new List<string>();
    [MenuItem("Builder/Check UI Font")]
    static void FindFont()
    {
        string defaultFontname = EditorUtility.OpenFilePanel("选择默认的字体", @"Assets/Font", "ttf");
        //string defaultFontname = "Arial";
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
            bool needRefresh = false;
            GameObject obj = item as GameObject;
            //GameObject go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            OutputFontRecurse(obj.transform, string.Empty, defaultFontname, defaultFont, ref needRefresh);

            if (needRefresh == true)
            {
                EditorUtility.SetDirty(obj);
                //PrefabUtility.ReplacePrefab(go, obj);
                //GameObject.DestroyImmediate(go);
                //AssetDatabase.Refresh();
            }

            //PrefabUtility.ReplacePrefab(go, obj);
            //GameObject.DestroyImmediate(go);
            //AssetDatabase.Refresh();
        }
    }
    //---------------------------------------------------------------------------------------------
    static public void OutputFontRecurse(
        Transform label,
        string parentName,
        string defaultFontname,
        Font defaultFont,
        ref bool needRefresh
        )
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
                        if (curFont.name == "Arial")
                        {
                            textComponent.font = defaultFont;
                            needRefresh = true;
                            Logger.LogFormat("<color=#ffff00ff>{0} replace Arial font to default font</color>", parentName + "/" + label.name);

                        }
                        else
                        {
                            Logger.LogFormat("<color=#ff0000ff>{0} use font {1} not default font</color>", parentName + "/" + label.name, curFont.name);
                        }
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
            OutputFontRecurse(child, parentName + "/" + label.name, defaultFontname, defaultFont, ref needRefresh);
        }
    }

    //---------------------------------------------------------------------------------------------
    // check ui font
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    public class FileUsedInfo
    {
        public FileUsedInfo(string filename)
        {
            this.filename = filename;
        }

        public string filename;
        public bool used = false;
    }

    [MenuItem("Builder/List Texture_Mat info")]
    static void OutPutTextureMatInfo()
    {
        Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
        profabPath.Clear();
        string path = EditorUtility.OpenFolderPanel("选择要检查的资源路径", @"Assets/Prefabs/effects", "");

        if (string.IsNullOrEmpty(path))
            return;

        string fileName = EditorUtility.SaveFilePanel("选择log保存路径", @"Assets", "ListTexMat", "txt");
        if (string.IsNullOrEmpty(fileName))
            return;

        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
        FileStream texMatFile = File.Open(fileName, FileMode.OpenOrCreate);
        texMatFile.Close();

        bool ignoreEnable = (path.Contains("Prefabs") == false);
        Dictionary<string, FileUsedInfo> allMatList = new Dictionary<string, FileUsedInfo>();
        Dictionary<string, FileUsedInfo> allTexList = new Dictionary<string, FileUsedInfo>();
        List<string> duplicateFilename = new List<string>();
        RecursionDictionary(path, ".mat", ref allMatList, ref duplicateFilename, ignoreEnable);
        RecursionDictionary(path, ".png|.tga", ref allTexList, ref duplicateFilename, ignoreEnable);

        for (int i = 0; i < duplicateFilename.Count; ++i)
        {
            using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8))
            {
                string s = string.Format("detect file with same name: {0}", duplicateFilename[i]);
                writer.WriteLine(s);
            }
        }

        List<string> prefabs_path = Recursion(path, ".prefab", ignoreEnable);
        for (int i = 0; i < prefabs_path.Count; i++)
        {
            prefabs_path[i] = prefabs_path[i].Replace(@"\", @"/");
            prefabs_path[i] = prefabs_path[i].Substring(prefabs_path[i].LastIndexOf("Assets/"));
            GameObject curObj = AssetDatabase.LoadAssetAtPath(prefabs_path[i], typeof(GameObject)) as GameObject;
            if (curObj != null)
            {
                int subIndex = prefabs_path[i].LastIndexOf("/");
                string filename = prefabs_path[i].Substring(subIndex + 1);
                if (objects.ContainsKey(filename) == false)
                {
                    objects.Add(filename, curObj);
                }
                else
                {
                    objects.Add(prefabs_path[i], curObj);
                    using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8))
                    {
                        string s = string.Format("prefab has same name {0}", filename);
                        writer.WriteLine(s);
                    }
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8))
                {
                    string s = string.Format("load prefab {0} failed", prefabs_path[i]);
                    writer.WriteLine(s);
                }
            }
        }

        //Dictionary<string, string> materialList = new Dictionary<string,string>();
        //Dictionary<string, string> textureList = new Dictionary<string, string>();
        var itor = objects.GetEnumerator();
        while (itor.MoveNext())
        {
            //Renderer[] renderers = Resources.FindObjectsOfTypeAll<Renderer>();
            GameObject curObj = itor.Current.Value;
            Renderer[] renderList = curObj.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer curRenderer in renderList)
            {
                foreach(Material curMat in curRenderer.sharedMaterials)
                {
                    //materialList.Add(itor.Current.Key, curMat.name);
                    if (curMat == null)
                    {
                        using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8))
                        {
                            string s = string.Format("{0}'s material is missing!", curObj.name);
                            writer.WriteLine(s);
                        }
                        continue;
                    }
                    FileUsedInfo fuInfo;
                    if (allMatList.TryGetValue(curMat.name, out fuInfo) == true)
                    {
                        fuInfo.used = true;
                    }

                    Texture curMainTex = curMat.GetTexture("_MainTex");
                    Texture distortTex = curMat.GetTexture("_NoiseTex");
                    //Texture curBumpTex = curMat.GetTexture("_BumpMap");
                    //Texture curCubeTex = curMat.GetTexture("_Cube");
                    if (curMainTex != null)
                    {
                        if (allTexList.TryGetValue(curMainTex.name, out fuInfo) == true)
                        {
                            fuInfo.used = true;
                        }
                        //textureList.Add(curMat.name, curMainTex.name);
                    }
                    if (distortTex != null)
                    {
                        if (allTexList.TryGetValue(distortTex.name, out fuInfo) == true)
                        {
                            fuInfo.used = true;
                        }
                    }
//                     if (curBumpTex != null)
//                     {
//                         textureList.Add(curMat.name, curMainTex.name);
//                     }
//                     if (curCubeTex != null)
//                     {
//                         textureList.Add(curMat.name, curMainTex.name);
//                     }
                }
            }
        }

        List<string> unUsedMatList = new List<string>();
        var itorMat = allMatList.GetEnumerator();
        while (itorMat.MoveNext())
        {
            if (itorMat.Current.Value.used == false)
            {
                unUsedMatList.Add(itorMat.Current.Value.filename);
            }
        }

        List<string> unUsedTexList = new List<string>();
        var itorTex = allTexList.GetEnumerator();
        while (itorTex.MoveNext())
        {
            if (itorTex.Current.Value.used == false)
            {
                unUsedTexList.Add(itorTex.Current.Value.filename);
            }
        }

        //string fileName = String.Format("list_tex_mat{0}-{1}-{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        //string outpath = "D:" + "/" + fileName + ".txt";
        using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8))
        {
            writer.WriteLine("---------------------------------------- Begin List Unused Material ----------------------------------------");
        }
        int count = unUsedMatList.Count;
        for (int index = 0; index < count; ++index)
        {
            using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8))
            {
                writer.WriteLine(unUsedMatList[index]+".mat");
            }
        }

        using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8))
        {
            writer.WriteLine("");
            writer.WriteLine("---------------------------------------- Begin List Unused Texture ----------------------------------------");
        }
        count = unUsedTexList.Count;
        for (int index = 0; index < count; ++index)
        {
            using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8))
            {
                writer.WriteLine(unUsedTexList[index]);
            }
        }

        using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8))
        {
            writer.WriteLine("");
            writer.WriteLine("---------------------------------------- End ----------------------------------------");
        }
        Logger.Log("<color=#ffff00ff>List texture/material finish!</color>");
    }
    //---------------------------------------------------------------------------------------------
    static List<string> Recursion(string root, string extension, bool ignoreEnable =  false)
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
            if (ignoreEnable == false ||
                dir.Name.Contains("Prefabs") ||
                dir.Name.Contains("SourceAsset")
                )
            {
                targetFiles.AddRange(Recursion(dir.FullName, extension,false));
            }
        }
        return targetFiles;
    }
    //---------------------------------------------------------------------------------------------
    static void RecursionDictionary(
        string root,
        string extension,
        ref Dictionary<string, FileUsedInfo> fileUsedinfo,
        ref List<string> duplicateFilename,
        bool ignoreEnable = false
        )
    {
        DirectoryInfo rootDir = new DirectoryInfo(root);
        FileInfo[] files = rootDir.GetFiles();
        DirectoryInfo[] dirs = rootDir.GetDirectories();
        foreach (var file in files)
        {
            string ext = file.Extension;
            if (extension.Contains(ext))
            {
                //string curName = file.Name.Replace(@"\", @"/");
                //curName = curName.Substring(curName.LastIndexOf("Assets/"));
                int index = file.Name.LastIndexOf(".");
                if (index != -1)
                {
                    string filename = file.Name.Substring(0, file.Name.LastIndexOf("."));
                    if (fileUsedinfo.ContainsKey(filename) == false)
                    {
                        fileUsedinfo.Add(filename, new FileUsedInfo(filename));
                    }
                    else
                    {
                        duplicateFilename.Add(file.Name);
                        //Logger.LogFormat("<color=#ffff00ff>detect file with same name: {0}</color>", file.Name);
                    }
                }
                else
                {
                    Logger.LogFormat("<color=#ffff00ff>file {0} without extension</color>", file.Name);
                }
            }
        }
        foreach (var dir in dirs)
        {
            if (ignoreEnable == false ||
                dir.Name.Contains("Prefabs") ||
                dir.Name.Contains("SourceAsset")
                )
            {
                RecursionDictionary(dir.FullName, extension, ref fileUsedinfo, ref duplicateFilename, false);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
