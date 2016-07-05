using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class GameApp : MonoBehaviour
{
    private static GameApp _manager = null;
	public NetworkManager netManager =null;
    public static GameApp Instance
    {
        get
        {
            if (_manager == null)
                _manager = GameObject.Find("/GameApp").GetComponent<GameApp>();
            return _manager;
        }
    }

    /// <summary>
    /// 初始化游戏管理器
    /// </summary>
    void Awake()
    {
        if (name != Const.GameAppName)
        {
            Logger.LogWarning("Game App Object's name is not " + Const.GameAppName);
            Logger.LogWarning("Changing GameApp GameObject's name to " + Const.GameAppName);
            name = Const.GameAppName;
        }
        _manager = this;
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Init()
    {
        DontDestroyOnLoad(gameObject);  //防止销毁自己

		netManager = gameObject.GetComponent<NetworkManager> ();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = Const.GameFrameRate;

		gameObject.AddComponent<GameConfig> ();

        //释放资源，将包中的压缩资源解压出来
        CheckExtractResource();
    }

    void InitMgr()
    {
		GameEventMgr.Instance.Init();

        StaticDataMgr.Instance.Init();
        ObjectDataMgr.Instance.Init();
        GameDataMgr.Instance.Init();
		LayerConst.Init ();
        //NetMgr.Instance.Init();
       
        UIMgr.Instance.Init();
        GameMain.Instance.Init();
        SpellService.Instance.Init();
        ActorEventService.Instance.Init();
        GameSpeedService.Instance.Init();
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void CheckExtractResource()
    {
        bool isExists = Directory.Exists(Util.ResPath) && File.Exists(Path.Combine(Util.ResPath, "files.txt"));
        //
        if (Const.DebugMode || isExists)
        {
            StartCoroutine(OnUpdateResource());
            return;   //文件已经解压过了，自己可添加检查文件列表逻辑
        }
        StartCoroutine(OnExtractResource());    //启动释放协成 
    }

    IEnumerator OnExtractResource()
    {
        string dataPath = Util.ResPath;  //数据目录
        string resPath = Util.AppContentPath(); //游戏包资源目录
        Debug.Log(Util.ResPath);

        if (Directory.Exists(dataPath))
            Directory.Delete(dataPath, true);
        Directory.CreateDirectory(dataPath);

        string infile = Path.Combine(resPath, "files.txt");
        string outfile = Path.Combine(dataPath, "files.txt");
        if (File.Exists(outfile))
            File.Delete(outfile);

        Logger.Log("正在解包文件:>files.txt\n");
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;

            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
            yield return 0;
        }
        else File.Copy(infile, outfile, true);
        yield return new WaitForEndOfFrame();

        //释放所有文件到数据目录
        string[] files = File.ReadAllLines(outfile);
        foreach (var file in files)
        {
            infile = resPath + file;
            outfile = dataPath + file;
            Logger.Log("正在解包文件:>" + file);

            string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone)
                {
                    Debug.LogError(infile + " ## " + www.bytes.Length + " ## " + outfile);
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return null;
            }
            else
                File.Copy(infile, outfile, true);

            yield return new WaitForEndOfFrame();
        }
        Logger.Log("解包完成!!!");
        yield return new WaitForSeconds(0.1f);

        //释放完成，开始启动更新资源
        StartCoroutine(OnUpdateResource());
    }

    /// <summary>
    /// 启动更新下载
    /// </summary>
    IEnumerator OnUpdateResource()
    {
        //不更新模式下直接添加ResourceManager
        if (!Const.UpdateMode)
        {
            ResourceMgr.Instance.Init();
            yield break;
        }

        WWW www = null;
        string dataPath = Util.ResPath;  //数据目录
        string url = Const.UpdateUrl;

        //获取本地版本
        string versionFilePath = Path.Combine(dataPath, "version");
        string localVersion = string.Empty;
        if (File.Exists(versionFilePath))
            localVersion = File.ReadAllText(versionFilePath).Trim();

        //获取server的version list，random防止服务器缓存
        string random = DateTime.Now.ToString("yyyymmddhhmmss");
        string listUrl = url + "version.txt?v=" + random;

        if (Debug.isDebugBuild)
            Debug.LogWarning("LoadUpdate---->>>" + listUrl);

        www = new WWW(listUrl);
        Debug.Log(listUrl);
        yield return www;
        if (www.error != null)
        {
            OnUpdateFailed(string.Empty);
            yield break;
        }
        //将version list文件解析为列表
        string remoteVersion = www.text;
        string[] versions = remoteVersion.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        int versionIndex = -1;
        if (localVersion.Length != 0)
            versionIndex = Array.IndexOf<string>(versions, localVersion);

        //从下一个index开始更新
        for (int i = versionIndex+1; i < versions.Length; ++i )
        {
            Debug.Log(url + versions[i]);
            www = new WWW(url + versions[i]);
            yield return www;
            if (www.error != null)
            {
                OnUpdateFailed("version"+versions[i]);
                yield break;
            }

            //解压zip，写入本地目录
            Util.UnZipFromBytes(www.bytes, dataPath);

            //更新成功后写入客户端的version文件
            File.WriteAllText(versionFilePath, versions[i]);
        }

        yield return new WaitForEndOfFrame();
        Logger.Log("更新完成!!");
        ResourceMgr.Instance.Init();
    }

    void OnUpdateFailed(string file)
    {
        Logger.LogError("更新失败!>" + file);
    }

    /// <summary>
    /// 资源初始化结束
    /// </summary>
    public void OnResourceInited()
    {
        //初始化完成
        Debug.Log("OnResourceInited");

        InitMgr();
    }

    /// <summary>
    /// 初始化场景
    /// </summary>
    public void OnInitScene()
    {
        Debug.Log("OnInitScene-->>" + Application.loadedLevelName);
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    void OnDestroy()
    {
        Debug.Log("~GameManager was destroyed");
    }
}

