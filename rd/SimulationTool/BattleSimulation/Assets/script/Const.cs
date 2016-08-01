using UnityEngine;
using System.Collections;


public class Const
{
    /// <summary>
    /// 调试模式
    /// 非mobile平台（editor，windows等）会直接读取streamingassets中的文件
    /// mobile平台（android，ios）：将streamingassets下面的文件复制到persistentDataPath中，并从persistentDataPath中读取数据
    /// </summary>
    public static bool DebugMode
    {
        get
        {
			//return false;
           return !Application.isMobilePlatform;
        }
    }

    /// <summary>
    /// 更新模式（开始此模式请确保DebugMode为false）
    /// 更新模式下会从web server上获取version list，然后下载更新包更新版本
    /// ios初审时请关闭
    /// </summary>
    public static bool UpdateMode = false;

	public static bool DebugConsoleEnable = true;

    public static int TimerInterval = 1;
    //游戏帧频
    public static int GameFrameRate = 30;

    //更新地址
	public static string WebUrl = "http://192.168.199.122:9000/resource_control";
    public static string UpdateUrl
    {
        get
        {
            string url = WebUrl;
#if UNITY_IOS
          //  url += "ios/";
#else
          //  url += "android/";
#endif
            return url;
        }
    }

    public static string CollectorUrl = "http://123.59.45.55:9001/fetch_accountServer";
    //应用程序名称
    public static string AppName = "hawk"; 
    //应用程序前缀
    public static string AppPrefix = AppName + "_";
    //素材扩展名
    public static string ExtName = "";
    //素材目录
    public static string AssetDirname = "assetbundle";
    //游戏管理器object名字
    public static string GameAppName = "GameApp";

    public enum SERVERTYPE
    {
        LOCAL_SERVER_NORMAL,
        LOCAL_SERVER_TEST,
        REMOTE_SERVER_NORMAL,

        NUM_SERVER_TYPE
    }

	//关于打包的一些配置 todo:move to config file for auto package
	public	static	int		versionCode = 1;//程序版本号
	public	static	string	versionName = "1.0.0";
	private	static	int		resouceCode = 1;//资源版本号 

	public	static	string	channel = "test";//渠道号
	public	static	string	platform = "ios";//平台 ios  android

	public	static	int	ResouceCodeAttr
	{
		get
		{
			int savecode = PlayerPrefs.GetInt("resouceCode");
			if(savecode < resouceCode)
				return resouceCode;
			return savecode;
		}
	}

	public static void SetResouceCode(int resCode)
	{
		PlayerPrefs.SetInt ("resouceCode", resCode);
	}

}
