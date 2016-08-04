using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace UnityClientConsole
{
  
    class NetManager
    {
        private const int BUFFERSIZE = 10240;
        private static NetManager instance;
        private Socket socketClient;
        private MemoryStream sendStream;
        private MemoryStream receiveStream;
        private MemoryStream buffer;
        private App app;

        public NetManager(App app)
        {
            sendStream = new MemoryStream(BUFFERSIZE);
            receiveStream = new MemoryStream(BUFFERSIZE);
            buffer = new MemoryStream(BUFFERSIZE);
            this.app = app;
        }

        public static NetManager GetInstance()
        {
            if(instance==null)
            {
                instance=new NetManager(null);
            }
            return instance;
        }

        public bool Init(string address, int port)
        {
            IPAddress ip = IPAddress.Parse(address);
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socketClient.Connect(new IPEndPoint(ip, port)); //配置服务器IP与端口  
                Console.WriteLine("连接服务器成功");
                ReceiveData();
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return false;  
            }  
            return true;
        }

        public bool SendProtocol(int type,  ProtoBuf.IExtensible builder)
        {
            Protocol protocol = Protocol.valueOf(type, builder);
            protocol.encode(sendStream);
            try
            {
                //Console.WriteLine("{0} bytes have send",sendStream.Length);
                socketClient.BeginSend(sendStream.ToArray(), 0, (int)sendStream.Length, 0, new AsyncCallback(SendCallback), socketClient);
                sendStream.SetLength(0);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return true;
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {    
                Socket handler = (Socket)ar.AsyncState;    
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveData()
        {
            socketClient.BeginReceive(buffer.GetBuffer(),0,BUFFERSIZE,SocketFlags.None,new AsyncCallback(ReceiveCallback),socketClient);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {    
                Socket handler = (Socket)ar.AsyncState;    
                int bytesReceive = handler.EndReceive(ar);
                //Console.WriteLine("{0} bytes already received", bytesReceive);

                receiveStream.Seek(receiveStream.Length, SeekOrigin.Begin);
                receiveStream.Write(buffer.GetBuffer(), 0, bytesReceive);

                Protocol protocol = Protocol.valueOf();
                while (protocol.decode(receiveStream))
                {
                    app.OnProtocol(protocol);
                }
                //继续接收数据
                ReceiveData();       
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
            }
        }
    }




}
