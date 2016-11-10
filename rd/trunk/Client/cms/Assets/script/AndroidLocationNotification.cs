//================================
//  Create by xuelong.
//  Mail:[xuelong@way4games.com]
//================================

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class AndroidLocationNotification
{

#if UNITY_ANDROID&&!UNITY_EDITOR
    private static string fullClassName = "com.gamegarifee.hs.UnityNotificationManager";
    private static string unityClass = "com.unity3d.player.UnityPlayerNativeActivty";

    /// <summary>
    /// 通知一次
    /// </summary>
    /// <param name="id">推送ID</param>
    /// <param name="delay">延时</param>
    /// <param name="title">标题</param>
    /// <param name="message">内容</param>
    public static void SendNotification(int id, long delay, string title, string message)
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetNotification", unityClass, id, delay * 1000, title, message);
        }
    }
    /// <summary>
    /// 通知多次
    /// </summary>
    /// <param name="id">推送ID</param>
    /// <param name="delay">延时时间</param>
    /// <param name="timeout">循环间隔</param>
    /// <param name="title">标题</param>
    /// <param name="message">内容</param>
    public static void SendRepeatingNotification(int id, long delay, long timeout, string title, string message)
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetRepeatingNotification", unityClass, id, delay * 1000, timeout * 1000, title, message);
        }
    }
    /// <summary>
    /// 取消通知
    /// </summary>
    /// <param name="id">通知ID</param>
    public static void CancelNotification(int id)
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass(unityClass);
        if (pluginClass!=null)
        {
            pluginClass.CallStatic("CancelNotification", id);
        }
    }

#endif
}
