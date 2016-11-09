using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;


namespace UnityClientConsole
{
    class SocketClient
    {
        public class Param
        {
            public TcpClient client;
        }

        public enum DisType
        {
            Exception,
            Disconnect,
        }

        private TcpClient client = null;
        private NetworkStream outStream = null;
        private MemoryStream memStream = null;
        private const int MAX_READ = 8192;
        private byte[] byteBuffer = new byte[MAX_READ];
        public bool isConnected = false;
        public bool isClosing = false;
        public bool hasClosed = false;
        public Param paramClient = new Param();
        private Session session;

        public SocketClient(Session session)
        {
            this.session = session;
        }

        public bool Init(string address, int port)
        {
            memStream = new MemoryStream(MAX_READ);
            IPAddress ip = IPAddress.Parse(address);
            client = new TcpClient();
            client.SendTimeout = 600000;
            client.ReceiveTimeout = 600000;
            client.NoDelay = true;

            paramClient.client = client; 

            try
            {
                client.BeginConnect(address, port, new AsyncCallback(OnConnect), null);
            }
            catch (Exception e)
            {
                Close();
                Console.WriteLine(e.Message);

                return false;
            }

            return true;
        }

        void OnConnect(IAsyncResult asr)
        {
            try
            {
                isConnected = true;
                outStream = client.GetStream();
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), paramClient);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void WriteMessage(byte[] message)
        {
            try
            {
                lock (this)
                {
                    MemoryStream ms = null;
                    using (ms = new MemoryStream())
                    {
                        ms.Position = 0;
                        BinaryWriter writer = new BinaryWriter(ms);
                        //ushort msglen = (ushort)message.Length;
                        // writer.Write(msglen);
                        writer.Write(message);
                        writer.Flush();
                        if (client != null && client.Connected && !isClosing)
                        {
                            //NetworkStream stream = client.GetStream(); 
                            byte[] payload = ms.ToArray();
                            outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), paramClient);
                        }
                        else
                        {
                            Console.WriteLine("client.connected----->>false");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Close();
                Console.WriteLine(e.Message);
            }
        }

        void OnRead(IAsyncResult asr)
        {
            int bytesRead = 0;
            try
            {
                lock (this)
                {
                    Param parma = (Param)asr.AsyncState;
                    if (parma.client == null || !parma.client.Connected)
                    {
                        OnDisconnected(DisType.Disconnect, "close for time out");
                        return;
                    }

                    if (client.Connected)
                    {
                        //读取字节流到缓冲区
                        bytesRead = client.GetStream().EndRead(asr);
                    }

                    if (bytesRead < 1)
                    {
                        //包尺寸有问题，断线处理
                        OnDisconnected(DisType.Disconnect, " bytesRead < 1");
                        return;
                    }

                    //分析数据包内容，抛给逻辑层
                    OnReceive(byteBuffer, bytesRead);

                    //分析完，再次监听服务器发过来的新消息
                    Array.Clear(byteBuffer, 0, byteBuffer.Length);

                    client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), paramClient);
                }
            }
            catch (Exception ex)
            {
                //PrintBytes();
                OnDisconnected(DisType.Exception, ex.Message);
            }
        }

        void OnDisconnected(DisType dis, string msg)
        {
            Close();
            isConnected = false;
            hasClosed = true;
            Console.WriteLine("OnDisconnected " + msg);
        }

        void PrintBytes()
        {
            string returnStr = string.Empty;
            for (int i = 0; i < byteBuffer.Length; i++)
            {
                returnStr += byteBuffer[i].ToString("X2");
            }
            Console.WriteLine(returnStr);
        }

        void OnWrite(IAsyncResult asr)
        {
            try
            {
                lock(this){
                    Param parma = (Param)asr.AsyncState;
                    if (parma.client != null && parma.client.Connected)
                    {
                        outStream.EndWrite(asr);
                    }
                    else
                    {
                        OnDisconnected(DisType.Disconnect, "close for time out");
                    }
                }      
            }
            catch (Exception ex)
            {
                Console.WriteLine("OnWrite--->>>" + ex.Message);
            }
        }

        void OnReceive(byte[] bytes, int length)
        {
            memStream.Seek(memStream.Length, SeekOrigin.Begin);
            memStream.Write(bytes, 0, length);

            Protocol receivMsg = Protocol.valueOf();
            while (receivMsg.decode(memStream))
            {
                session.OnProtocol(receivMsg);
                receivMsg = Protocol.valueOf();
            }
        }

        void SessionSend(Protocol msg)
        {
            MemoryStream ms = new MemoryStream();
            msg.encode(ms);
            WriteMessage(ms.ToArray());
        }

        public void SendProtocol(int type, ProtoBuf.IExtensible builder)
        {
            Protocol protocol = Protocol.valueOf(type, builder);
            SessionSend(protocol);
        }

        public void Close()
        {
            lock (this)
            {
                isClosing = true;
                if (client != null)
                {
                    if (client.Connected)
                    {
                        outStream.Close();
                        client.Close();
                        client = null;
                    }
                }
            }
        }

    }

}


