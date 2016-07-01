using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    private int count;
    private TimerInfo timer;
    private static Queue<KeyValuePair<int, ByteBuffer>> sEvents = new Queue<KeyValuePair<int, ByteBuffer>>();

    void Start()
    {
    }

    public void OnInit()
    {
    }

    ///------------------------------------------------------------------------------------
    public static void AddEvent(int _event, ByteBuffer data)
    {
        sEvents.Enqueue(new KeyValuePair<int, ByteBuffer>(_event, data));
    }

    void Update()
    {
        if (sEvents.Count > 0)
        {
            while (sEvents.Count > 0)
            {
                KeyValuePair<int, ByteBuffer> _event = sEvents.Dequeue();
                switch (_event.Key)
                {
                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 发送链接请求
    /// </summary>
    public void SendConnect()
    {
        SocketClient.SendConnect();
    }

    /// <summary>
    /// 发送SOCKET消息
    /// </summary>
    public void SendMessage(ByteBuffer buffer)
    {
        SocketClient.SendMessage(buffer);
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    void OnDestroy()
    {
        Debug.Log("~NetworkManager was destroy");
    }
}
