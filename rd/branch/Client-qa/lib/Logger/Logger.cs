using UnityEngine;
using System.Collections;

/// <summary>
/// Logger类
/// </summary>
public class Logger
{
    static bool consoleEnable = true;
    public static bool ConsoleEnable
    {
        get { return consoleEnable; }
        set { consoleEnable = value; }
    }

    public static void Log(object message)
    {
        if (consoleEnable)
            Debug.Log(message);
    }

    public static void Log(object message, Object context)
    {
        if (consoleEnable)
            Debug.Log(message, context);
    }

    public static void LogFormat(string format, params object[] args)
    {
        if (consoleEnable)
            Debug.LogFormat(format, args);
    }

    public static void LogFormat(Object context, string format, params object[] args)
    {
        if (consoleEnable)
            Debug.LogFormat(context, format, args);
    }

    public static void LogError(object message)
    {
        if (consoleEnable)
            Debug.LogError(message);
    }

    public static void LogError(object message, Object context)
    {
        if (consoleEnable)
            Debug.LogError(message, context);
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        if (consoleEnable)
            Debug.LogErrorFormat(format, args);
    }

    public static void LogErrorFormat(Object context, string format, params object[] args)
    {
        if (consoleEnable)
            Debug.LogErrorFormat(context, format, args);
    }

    public static void LogException(System.Exception exception)
    {
        if (consoleEnable)
            Debug.LogException(exception);
    }

    public static void LogException(System.Exception exception, Object context)
    {
        if (consoleEnable)
            Debug.LogException(exception, context);
    }

    public static void LogWarning(object message)
    {
        if (consoleEnable)
            Debug.LogWarning(message);
    }

    public static void LogWarning(object message, Object context)
    {
        if (consoleEnable)
            Debug.Log(message, context);
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
        if (consoleEnable)
            Debug.LogWarningFormat(format, args);
    }

    public static void LogWarningFormat(Object context, string format, params object[] args)
    {
        if (consoleEnable)
            Debug.LogWarningFormat(context, format, args);
    }
}
