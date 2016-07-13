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

		Logger.ConsoleEnable = Const.DebugConsoleEnable;
		if (Const.DebugConsoleEnable) 
		{
			gameObject.AddComponent<OutLog>();
		}

        //释放资源，将包中的压缩资源解压出来
      //  CheckExtractResource();
    }


    /// <summary>
    /// 初始化场景
    /// </summary>
    public void OnInitScene()
    {
        Logger.Log("OnInitScene-->>" + Application.loadedLevelName);
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    void OnDestroy()
    {
        Logger.Log("~GameManager was destroyed");
    }
}

