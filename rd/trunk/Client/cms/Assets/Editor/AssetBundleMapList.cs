using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class AssetBundleMapList
{
    public static string assetBundleRoot=@"Assets/StreamingAssets/assetbundle/";
    public static string readFilsExtension = ".manifest";   //There is a "."
    public static string saveFilePath = @"Assets/StreamingAssets/staticData/";
    public static string saveFileDefaultName = "assetMap";
    public static string saveFileExtension = ".csv";         //There is no "."

    public static Dictionary<string, string> map = new Dictionary<string, string>();

    public static string selectPath;

    public static bool isError = false;
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
                isError = true;
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

    [MenuItem("Builder/UTF-8 +BOM Selected")]
    public static void SetCSVEncodingToUTF8_BOM()
    {
        string folder = new DirectoryInfo(saveFilePath).FullName;
        string[] files = Directory.GetFiles(folder, "*.csv", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            Debug.Log(file);
            SetFileFormatToUTF8_BOM(file);
        }

        Debug.Log("<color=#00ff00ff>\t success \t</color>");
    }

    /// <summary>
    /// 将指定文件编码转换为：UTF-8 +BOM
    /// </summary>
    public static void SetFileFormatToUTF8_BOM(string file)
    {
        if (!File.Exists(file))
        {
            Debug.LogWarning(string.Format("不存在文件：{0}", file));
            return;
        }

        string extension = Path.GetExtension(file);
        if (extension == ".csv")
        {
            //先检测文件编码格式，防止无用刷新
            //先判断BOM模式，防止无用检测
            bool isUTF8_BOM = FileEncoding.isUTF8_BOM(file);
            Encoding fileEncoding = null;
            if (extension != ".shader")
            {
                if (isUTF8_BOM)
                    return;
            }
            //shader脚本不添加签名，因为内置shader编译器暂不支持带签名的UTF8脚本
            else if (!isUTF8_BOM)
            {
                fileEncoding = FileEncoding.GetType(file);
                if (fileEncoding == Encoding.UTF8)
                    return;
            }

            //根据具体编码格式读出内容，再设置对象编码，防止出现乱码
            if (fileEncoding == null)
                fileEncoding = FileEncoding.GetType(file);
            UTF8Encoding utf8 = new UTF8Encoding((extension != ".shader"));
            File.WriteAllText(file, File.ReadAllText(file, fileEncoding), utf8);
        }
    }


    /// <summary>
    /// 获取文件的编码格式
    /// </summary>
    public class FileEncoding
    {
        /// <summary>
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// </summary>
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>是否是带签名的UTF8编码</returns>
        public static bool isUTF8_BOM(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            r.Close();
            fs.Close();

            return IsUTF8_BOMBytes(ss);
        }

        /// <summary>
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// </summary>
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>
        public static Encoding GetType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static Encoding GetType(FileStream fs)
        {
            //		byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            //		byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            //		byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || IsUTF8_BOMBytes(ss))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();

            return reVal;
        }

        /// <summary>
        /// 将文件格式转换为UTF-8-BOM
        /// </summary>
        /// <param name="FILE_NAME">文件路径</param>
        public static void CovertToUTF8_BOM(string FILE_NAME)
        {
            byte[] BomHeader = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.ReadWrite);

            //按默认编码获取文件内容
            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            r.Close();

            bool isBom = false;
            if (ss.Length >= 3)
            {
                if (ss[0] == BomHeader[0] && ss[1] == BomHeader[1] && ss[2] == BomHeader[2])
                {
                    isBom = true;
                }
            }

            //将内容转换为UTF8格式，并添加Bom头
            if (!isBom)
            {
                string content = Encoding.Default.GetString(ss);
                byte[] newSS = Encoding.UTF8.GetBytes(content);

                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(BomHeader, 0, BomHeader.Length);
                fs.Write(newSS, 0, i);
            }

            fs.Close();
        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;	//计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }

            if (charByteCounter > 1)
            {
                Debug.LogError("非预期的byte格式");
            }

            return true;
        }

        /// <summary>
        /// 判断是否是带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8_BOMBytes(byte[] data)
        {
            if (data.Length < 3)
                return false;

            return ((data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF));
        }
    }

    static List<string> profabPath=new List<string>();
    [MenuItem(("Builder/setFont"))]
    static public void setProfabFont()
    {
        Font font = Resources.Load("Font/FZLTCXHJW") as Font;
        System.Action<Transform> setFont = (label) =>
        {
            label.GetComponent<UnityEngine.UI.Text>().font = font;
        };


        List<GameObject> objects=new List<GameObject>();

        profabPath.Clear();
        string path = EditorUtility.OpenFolderPanel("选择要修改字体的预制路径", @"Assets/Prefabs/ui/", "");

        List<string> prefabs_path = Recursion(path, ".prefab");
        Debug.Log(prefabs_path.Count);
        for (int i = 0; i < prefabs_path.Count; i++)
        {
            prefabs_path[i] = prefabs_path[i].Replace(@"\", @"/");
            Debug.Log(prefabs_path[i] + "\n" + prefabs_path[i].Length + "\t" + prefabs_path[i].LastIndexOf("Assets/"));
            prefabs_path[i] = prefabs_path[i].Substring(prefabs_path[i].LastIndexOf("Assets/"));
            if (prefabs_path[i].Contains("VitalChange")) { Debug.Log("包含"); continue; }
            objects.Add(AssetDatabase.LoadAssetAtPath(prefabs_path[i], typeof(GameObject)) as GameObject);
        }

        //objs = Selection.gameObjects;
        foreach (var item in objects)
        {
            GameObject obj = item as GameObject;
            GameObject go = PrefabUtility.InstantiatePrefab(obj) as GameObject;

            Util.SetChild_UsedT<UnityEngine.UI.Text>(go.transform, setFont);

            PrefabUtility.ReplacePrefab(go, obj);
            GameObject.DestroyImmediate(go);
            AssetDatabase.Refresh();
        }
    }


    static List<string> Recursion(string root,string extension)
    {
        List<string> prefabs = new List<string>();
        DirectoryInfo rootDir = new DirectoryInfo(root);
        FileInfo[] files = rootDir.GetFiles();
        DirectoryInfo[] dirs = rootDir.GetDirectories();
        foreach (var file in files)
        {
            string ext = file.Extension;
            if (ext.Equals(extension))
            {
                prefabs.Add(file.FullName);
            }
        }
        foreach (var dir in dirs)
        {
            prefabs.AddRange(Recursion(dir.FullName,extension));
        }
        return prefabs;
    }
}
