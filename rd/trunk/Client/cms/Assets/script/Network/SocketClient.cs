using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

public enum DisType { 
    Exception,
    Disconnect,
}

public enum NetActionType {
    Connect,
    Message,
    Logout
}

public class SocketClient : MonoBehaviour {
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];

    public static bool loggedIn = false;
	private static Queue<KeyValuePair<NetActionType, ProtocolMessage>> _events = new Queue<KeyValuePair<NetActionType, ProtocolMessage>>();

    // Use this for initialization
    void Awake() {
        memStream = new MemoryStream(MAX_READ);
    }

    /// <summary>
    /// 消息循环
    /// </summary>
    void Update() {
        while (_events.Count > 0) 
		{
			KeyValuePair<NetActionType, ProtocolMessage> _event = _events.Dequeue();

            switch (_event.Key) 
			{
                case NetActionType.Connect:
                    ConnectServer(GameApp.Instance.netManager.GameServerAdd, GameApp.Instance.netManager.GameServerPort);                 
				break;
                case NetActionType.Message: 
					SessionSend( _event.Value );
                break;
                case NetActionType.Logout:
					Close();
				break;
            }
            //if (_event.Value != null) _event.Value.Close();
        }
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void ConnectServer(string host, int port) {
       // Debug.LogError("begin connect to server");
        client = null;
        client = new TcpClient();
        client.SendTimeout = 1000;
        client.ReceiveTimeout = 1000;
        client.NoDelay = true;
        try
		{
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        } 
		catch (Exception e) {
            Close(); 
			Logger.LogError(e.Message);
        }
    }

    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect(IAsyncResult asr) {
		try
		{
        	outStream = client.GetStream();
        	client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
       	 	NetworkManager.AddEvent(ResponseState.Connect, null);
		}
		catch(Exception e)
		{
			Logger.LogException(e);
			NetworkManager.AddEvent(ResponseState.UnConnect, null);
		}
    }

    /// <summary>
    /// 写数据
    /// </summary>
    void WriteMessage(byte[] message) {
        MemoryStream ms = null;
        using (ms = new MemoryStream()) 
		{
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            //ushort msglen = (ushort)message.Length;
           // writer.Write(msglen);
            writer.Write(message);
            writer.Flush();
            if (client != null && client.Connected) 
			{
                //NetworkStream stream = client.GetStream(); 
                byte[] payload = ms.ToArray(); 
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
			else 
			{
				Logger.LogError("client.connected----->>false");
            }
        }
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead(IAsyncResult asr)
	{
        int bytesRead = 0;
        try 
		{
            lock (client.GetStream()) 
			{         //读取字节流到缓冲区
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1) 
			{                //包尺寸有问题，断线处理
                OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                return;
            }

            OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层

            lock (client.GetStream())
			{         //分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
        } 
		catch (Exception ex) 
		{
            //PrintBytes();
            OnDisconnected(DisType.Exception, ex.Message);
        }
    }

    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected(DisType dis, string msg) {
        Close();   //关掉客户端链接
        int protocal = dis == DisType.Exception ? 
			ResponseState.Exception : ResponseState.Disconnect;

        NetworkManager.AddEvent(protocal, null);
		Logger.LogError("Connection was closed by the server:>" + msg + " Distype:>" + dis);
    }

    /// <summary>
    /// 打印字节
    /// </summary>
    /// <param name="bytes"></param>
    void PrintBytes() { 
        string returnStr = string.Empty; 
        for (int i = 0; i < byteBuffer.Length; i++) {
            returnStr += byteBuffer[i].ToString("X2"); 
        }
		Logger.LogError(returnStr);
    }

    /// <summary>
    /// 向链接写入数据流
    /// </summary>
    void OnWrite(IAsyncResult r) {
        try {
            outStream.EndWrite(r);
        } catch (Exception ex) {
			Logger.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceive(byte[] bytes, int length) 
	{
        memStream.Seek(memStream.Length, SeekOrigin.Begin);
        memStream.Write(bytes, 0, length);

        ProtocolMessage receivMsg = ProtocolMessage.Create();
        while (receivMsg.decode(memStream))
        {
            NetworkManager.AddEvent(ResponseState.Message, receivMsg);
            receivMsg = ProtocolMessage.Create();
        }
    }

    /// <summary>
    /// 剩余的字节
    /// </summary>
    private long RemainingBytes() {
        return memStream.Length - memStream.Position;
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    /// <param name="ms"></param>
    void OnReceivedMessage(MemoryStream ms) 
	{
		ProtocolMessage receivMsg = ProtocolMessage.Create ();
		receivMsg.decode (ms);

		NetworkManager.AddEvent(ResponseState.Message, receivMsg);
    }


    /// <summary>
    /// 会话发送
    /// </summary>
    void SessionSend(ProtocolMessage msg) 
	{
		MemoryStream ms = new MemoryStream ();
		msg.encode (ms);
        WriteMessage(ms.ToArray());
    }

    /// <summary>
    /// 关闭链接
    /// </summary>
    void Close() { 
        if (client != null) {
            if (client.Connected) client.Close();
            client = null;
        }
        loggedIn = false;
    }

    /// <summary>
    /// 登出
    /// </summary>
    public static void Logout() { 
		_events.Enqueue(new KeyValuePair<NetActionType, ProtocolMessage>(NetActionType.Logout, null));
    }

    /// <summary>
    /// 发送连接请求
    /// </summary>
    public static void SendConnect() {
		_events.Enqueue(new KeyValuePair<NetActionType, ProtocolMessage>(NetActionType.Connect, null));
    }

    /// <summary>
    /// 发送消息
    /// </summary>
	public static void SendMessage(ProtocolMessage buffer) {
		_events.Enqueue(new KeyValuePair<NetActionType, ProtocolMessage>(NetActionType.Message, buffer));
    }
}
