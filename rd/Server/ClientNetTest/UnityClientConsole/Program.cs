using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net;
using System.Net.Sockets;
using System.Threading;

using ProtoBuf;
using ProtoBuf.Meta;
using PB;

namespace UnityClientConsole
{
    class Program
    {
        private static byte[] result = new byte[1024];


        static void Main(string[] args)
        {
            //设定服务器IP地址  
            if (App.GetInstance().Init("127.0.0.1", 9595) == false)
                return;

            App.GetInstance().Run();

            HPLogin login = new HPLogin();
            login.puid = "2";

            NetManager.GetInstance().SendProtocol(code.LOGIN_C.GetHashCode(), login);


            //for (int i = 0; i < 10000; ++i)
            //{
            //    HPHeartBeat heartBeat = new HPHeartBeat();
            //    heartBeat.timeStamp = 123123123;
            //    NetManager.GetInstance().SendProtocol(sys.HEART_BEAT.GetHashCode(), heartBeat);

            //}

            Console.ReadLine();

            return;

        }
    }
}
