using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    private int count;
    private TimerInfo timer;
	private static Queue<KeyValuePair<int, ProtocolMessage>> sEvents = new Queue<KeyValuePair<int, ProtocolMessage>>();

    void Start()
    {

    }

    public void OnInit()
    {
    }

    ///------------------------------------------------------------------------------------
	public static void AddEvent(int _event, ProtocolMessage data)
    {
		sEvents.Enqueue(new KeyValuePair<int, ProtocolMessage>(_event, data));
    }

    void Update()
    {
        if (sEvents.Count > 0)
        {
            while (sEvents.Count > 0)
            {
				KeyValuePair<int, ProtocolMessage> _event = sEvents.Dequeue();
				//
                switch (_event.Key)
                {
				case ResponseState.Connect:
					GameEventMgr.Instance.FireEvent<int>(NetEventList.NetConnectFinished,1);
					UINetRequest.Close();
					break;
				case ResponseState.UnConnect:
					GameEventMgr.Instance.FireEvent<int>(NetEventList.NetConnectFinished,0);
					break;
				case ResponseState.Disconnect:
					Debug.LogError("disconnect msg");
					break;
				case ResponseState.Exception:
					Debug.LogError("net Exception");
					break;
				case ResponseState.Message:
				{ 
					ProtocolMessage pmsg = _event.Value;
					Debug.Log("receiv msg : " + pmsg.GetMessageType());
					GameEventMgr.Instance.FireEvent<ProtocolMessage>(pmsg.GetMessageType().ToString(),pmsg);
					UINetRequest.Close();
				}
					break;
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
		UINetRequest.Open ();
        SocketClient.SendConnect();
    }

    /// <summary>
    /// 发送SOCKET消息
    /// </summary>
	public void SendMessage(ProtocolMessage buffer)
    {
        SocketClient.SendMessage(buffer);
		UINetRequest.Open ();
    }

	public void SendMessage(int messageType, ProtoBuf.IExtensible builder)
	{
		ProtocolMessage pbmsg = ProtocolMessage.Create (messageType, builder);
		SendMessage (pbmsg);
	}

    /// <summary>
    /// 析构函数
    /// </summary>
    void OnDestroy()
    {
        Debug.Log("~NetworkManager was destroy");
    }
}
