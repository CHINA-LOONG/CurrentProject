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


            App.GetInstance().SendLoginProtocol("zhengshuai");

            App.GetInstance().SendDeleteRoleProtocol(2);

            Console.ReadLine();

            return;

        }
    }
}
